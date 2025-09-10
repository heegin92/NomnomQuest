// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using UnityEditor;

using UnityEngine;

namespace Ironcow.Synapse.Common
{
    public static class ScriptTemplateExecutor
    {
        private static string CachePath = Application.dataPath.Replace("Assets", "Library/PendingComponentAttach.json");

        [Serializable]
        private class PendingAttach
        {
            public string className;
            public string gameObjectPath;
        }

        public static void GenerateAndAttachScript(Type baseType)
        {
            var selected = Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("선택된 GameObject가 없습니다.");
                return;
            }

            string className = selected.name;
            string folder = EditorDataSetting.ScriptPath;
            string path = Path.Combine(folder, $"{className}.cs");

            if (!File.Exists(path))
            {
                Directory.CreateDirectory(folder);
                string code = GenerateScriptCode(baseType, className);
                File.WriteAllText(path, code);
                AssetDatabase.ImportAsset(path);
            }

            // 캐시 항상 저장 (컴파일 여부와 무관하게)
            var info = new PendingAttach
            {
                className = className,
                gameObjectPath = GetGameObjectPath(selected)
            };
            File.WriteAllText(CachePath, JsonUtility.ToJson(info));

            // 타입이 이미 존재하면 즉시 AddComponent
            var runtimeType = GetTypeByName(className);
            if (runtimeType != null)
            {
                Undo.AddComponent(selected, runtimeType);
                File.Delete(CachePath);
                Debug.Log($"'{className}' 컴포넌트를 즉시 추가했습니다.");
            }
            else
            {
                Debug.Log($"컴파일 완료 후 '{className}' 컴포넌트가 자동으로 추가됩니다.");
            }
        }

        private static string GenerateScriptCode(Type baseType, string className)
        {
            string template = TryFindTemplate(baseType);

            if (!string.IsNullOrEmpty(template))
            {
                return template.Replace("#SCRIPTNAME#", className);
            }

            string baseClass = baseType.Name;
            string usingLine = !string.IsNullOrEmpty(baseType.Namespace) ? $"using {baseType.Namespace};" : "";

            return $@"using UnityEngine;
{usingLine}

public class {className} : {baseClass}
{{
    // Auto-generated
}}";
        }

        private static string TryFindTemplate(Type baseType)
        {
            string scriptPath = FindScriptPath(baseType);
            if (string.IsNullOrEmpty(scriptPath)) return null;

            string scriptDir = Path.GetDirectoryName(scriptPath);
            string moduleRoot = Path.GetDirectoryName(scriptDir);

            // 클래스명이 `CanvasBase`인데 `CanvasBase`가 아닌 경우 방어
            string className = Path.GetFileNameWithoutExtension(scriptPath);
            className = Regex.Replace(className, @"[`<].*", ""); // CanvasBase`1 → CanvasBase

            string templatePath = Path.Combine(moduleRoot, "Template", $"{className}Template.cs.txt");
            return File.Exists(templatePath) ? File.ReadAllText(templatePath) : null;
        }

        private static string FindScriptPath(Type type)
        {
            string searchName = type.Name.Replace("`1", "");
            string[] guids = AssetDatabase.FindAssets($"{searchName} t:Script");

            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

                // MonoScript.GetClass() 실패 시 fallback 파싱
                if (script != null)
                {
                    var cls = script.GetClass();
                    if (cls != null && cls.Name.StartsWith(searchName))
                        return path;

                    // fallback: 텍스트 기반 파싱
                    string text = File.ReadAllText(path);
                    var match = Regex.Match(text, @"class\s+(\w+)");
                    if (match.Success && match.Groups[1].Value.StartsWith(searchName) && path.Contains(match.Groups[1].Value + ".cs"))
                        return path;
                }
            }

            return null;
        }


        private static string GetGameObjectPath(GameObject obj)
        {
            var path = obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = obj.name + "/" + path;
            }
            return path;
        }

        private static GameObject FindByPath(string path)
        {
            var segments = path.Split('/');
            var root = GameObject.Find(segments[0]);
            if (root == null) return null;

            GameObject current = root;
            for (int i = 1; i < segments.Length; i++)
            {
                var child = current.transform.Find(segments[i]);
                if (child == null) return null;
                current = child.gameObject;
            }
            return current;
        }

        private static Type GetTypeByName(string className)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.Name == className && t.IsSubclassOf(typeof(MonoBehaviour)));
        }

        [InitializeOnLoadMethod]
        private static void ProcessAfterScriptReload()
        {
            if (!File.Exists(CachePath)) return;

            var json = File.ReadAllText(CachePath);
            var info = JsonUtility.FromJson<PendingAttach>(json);

            var go = FindByPath(info.gameObjectPath);
            if (go == null)
            {
                Debug.LogWarning($"GameObject '{info.gameObjectPath}' not found.");
                File.Delete(CachePath);
                return;
            }

            var type = GetTypeByName(info.className);
            if (type == null)
            {
                Debug.LogWarning($"Type '{info.className}' not found. 컴파일이 아직 완료되지 않았을 수 있습니다.");
                return; // 다음 컴파일 때 다시 시도됨
            }

            Undo.AddComponent(go, type);
            Debug.Log($"'{info.className}' 컴포넌트를 '{info.gameObjectPath}'에 추가했습니다.");
            File.Delete(CachePath);
        }
    }

}
