// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using UnityEditor;

using UnityEngine;

namespace Ironcow.Synapse
{
    [CustomEditor(typeof(SynapseBase), true)]
    public class SynapseBaseEditor :
#if ODIN_INSPECTOR
        Sirenix.OdinInspector.Editor.OdinEditor
#else
        Editor
#endif
    {
        private SynapseBase icb;
        private SerializedObject serializedTarget;

        private List<GameObject> dragTargets = new();
        private Dictionary<GameObject, string> renameMap = new();
        private Dictionary<GameObject, List<Component>> componentMap = new();
        private Dictionary<GameObject, int> selectedIndices = new();
        private HashSet<string> cachedExistingFields = new();
        private bool foldout = true;

        private void OnEnable()
        {
            icb = (SynapseBase)target;
            serializedTarget = new SerializedObject(icb);
            if (dragTargets == null) dragTargets = new();
            dragTargets.Clear();
            if (renameMap == null) renameMap = new();
            renameMap = renameMap.Where(kvp => kvp.Key != null).ToDictionary(k => k.Key, v => v.Value);
            CacheExistingFields();

            var self = icb.gameObject;
            renameMap[self] = ToPascalCase(self.name);
            var comps = icb.GetComponents<Component>()
                .Where(c => c != null && c != icb).ToList();

            if (!comps.Any(c => c is Transform))
                comps.Add(icb.transform);
            comps.Add(new GameObjectProxy(icb.gameObject));

            componentMap[self] = comps.OrderBy(c =>
            {
                if (c is GameObjectProxy) return 3;
                if (c is Transform) return 2;
                return 0;
            }).ToList();
        }

        public override void OnInspectorGUI()
        {
            OnDraw();
        }

        public void OnDraw()
        {
            DrawICBInspector();
#if ODIN_INSPECTOR
            this.Tree?.Draw(true);
#else
            DrawDefaultInspector();
#endif
        }

        public void DrawEditorButton(SynapseBase target)
        {
            // "Auto Caching Refresh" 버튼을 추가하고, 클릭 시 AutoCaching 메서드를 호출
            if (GUILayout.Button("Auto Caching Refresh"))
            {
                // MonoAutoCaching 인스턴스를 타겟으로 캐스팅하여 AutoCaching 메서드 호출
                var instance = target;
                instance.AutoCaching(); // Auto Caching을 갱신하는 메서드 실행
            }
        }

        private void DrawICBInspector()
        {
            serializedTarget.Update();

            EditorGUILayout.Space();
            foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, "SynapseBehaviour Auto Binder");
            if (foldout)
            {
                DrawDragArea();
                DrawRenameFields();

                if (GUILayout.Button("Change name and create values"))
                {
                    ApplyComponentAutoBindings();
                    ApplyNameChanges();
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (icb is SynapseBase sb)
                    {
                        DrawEditorButton(sb);
                    }
                    var instance = target as MonoBehaviour;
                    if (instance != null)
                    {
                        var type = instance.GetType();

                        DrawButton();
                    }
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space();
        }


        public virtual void DrawButton() { }

        private void DrawDragArea()
        {
            Rect dropArea = GUILayoutUtility.GetRect(0f, 40f, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, "Drop the gameObject this", EditorStyles.helpBox);

            Event evt = Event.current;
            if ((evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform) && dropArea.Contains(evt.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    foreach (var obj in DragAndDrop.objectReferences)
                    {
                        if (obj is GameObject go && go != icb.gameObject)
                        {
                            if (!dragTargets.Contains(go))
                                dragTargets.Add(go);
                            if (!renameMap.ContainsKey(go))
                                renameMap[go] = ToPascalCase(go.name);

                            var comps = go.GetComponents<Component>().Where(c => c != null).ToList();
                            if (!comps.Any(c => c is Transform))
                                comps.Add(go.transform);
                            comps.Add(new GameObjectProxy(go));

                            componentMap[go] = comps.OrderBy(c =>
                            {
                                if (c is GameObjectProxy) return 3;
                                if (c is Transform) return 2;
                                return 0;
                            }).ToList();

                            selectedIndices[go] = 0;
                        }
                    }
                    GUI.FocusControl(null);
                    Repaint();
                }
                evt.Use();
            }
        }

        private void DrawRenameFields()
        {
            var visibleTargets = dragTargets.Where(go => go != icb.gameObject).ToList();
            if (visibleTargets.Count == 0) return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Object List:", EditorStyles.boldLabel);

            foreach (var go in visibleTargets)
            {
                if (go == null || !renameMap.ContainsKey(go)) continue;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(go, typeof(GameObject), true, GUILayout.Width(60));

                var components = componentMap[go];
                var names = components.Select(c =>
                {
                    if (c is GameObjectProxy) return "GameObject";
                    return c.GetType().Name;
                }).ToArray();
                selectedIndices[go] = EditorGUILayout.Popup(selectedIndices[go], names, GUILayout.Width(110));

                renameMap[go] = EditorGUILayout.TextField(renameMap[go]);

                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    renameMap.Remove(go);
                    dragTargets.Remove(go);
                    componentMap.Remove(go);
                    selectedIndices.Remove(go);
                    GUI.backgroundColor = Color.white;
                    Repaint();
                    break;
                }
                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndHorizontal();
            }
        }

        private void ApplyNameChanges()
        {
            foreach (var pair in renameMap)
            {
                GameObject go = pair.Key;
                string newPascal = ToPascalCase(pair.Value);
                if (!string.IsNullOrEmpty(newPascal))
                {
                    Undo.RecordObject(go, "Change name");
                    go.name = newPascal;
                    EditorUtility.SetDirty(go);
                }
            }
        }

        private void ApplyComponentAutoBindings()
        {
            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(icb));
            string[] lines = File.ReadAllLines(scriptPath);
            var fieldLines = new List<string>(lines);

            int regionStart = fieldLines.FindIndex(line => line.Contains("#region AutoGenerated"));
            int regionEnd = fieldLines.FindIndex(line => line.Contains("#endregion"));

            List<string> preservedFields = new();
            cachedExistingFields.Clear();

            if (regionStart != -1 && regionEnd != -1 && regionEnd > regionStart)
            {
                for (int i = regionStart + 1; i < regionEnd; i++)
                {
                    string line = fieldLines[i].Trim();
                    preservedFields.Add(line);

                    var match = Regex.Match(line, @"private\s+\S+\s+(\S+);");
                    if (match.Success)
                    {
                        string varName = match.Groups[1].Value.TrimEnd(';');
                        cachedExistingFields.Add(varName);
                    }
                }
                fieldLines.RemoveRange(regionStart, regionEnd - regionStart + 1);
            }

            List<string> newFields = new() { "    #region AutoGenerated" };
            foreach (var oldLine in preservedFields)
            {
                newFields.Add("    " + oldLine);
            }

            var pendingList = new AutoBindingEntryList();

            foreach (var go in dragTargets.Prepend(icb.gameObject).Distinct())
            {
                string baseName = renameMap.ContainsKey(go) ? renameMap[go] : go.name;
                string camelName = Char.ToLowerInvariant(baseName[0]) + baseName.Substring(1);
                var targetsToBind = go == icb.gameObject
                    ? componentMap[go].Where(c => c is not Transform).ToList()
                    : new List<Component> { componentMap[go][selectedIndices[go]] };

                string varName;
                foreach (var comp in targetsToBind)
                {
                    if (comp is GameObjectProxy proxy)
                    {
                        if (proxy.gameObjectRef == icb.gameObject) continue;

                        varName = camelName;

                        if (cachedExistingFields.Contains(varName)) // 충돌 시 타입 접미사 추가
                            varName = camelName + "GameObject";

                        if (cachedExistingFields.Contains(varName)) continue;

                        newFields.Add($"    [SerializeField] private GameObject {varName};");
                        cachedExistingFields.Add(varName);

                        pendingList.list.Add(new AutoBindingEntry
                        {
                            scriptPath = scriptPath,
                            fieldName = varName,
                            targetInstanceId = icb.GetInstanceID(),
                            componentInstanceId = proxy.gameObjectRef.GetInstanceID()
                        });

                        continue; // 이후 Component 로직 스킵
                    }

                    if (comp == null) continue;

                    string typeName = comp.GetType().Name.Contains("TextMeshProUGUI") ? "TMP_Text" : comp.GetType().Name.Replace("Proxy", "");


                    string baseVarName = camelName; // 기본 이름 + 충돌 시 타입 접미사
                    varName = baseVarName;

                    if (preservedFields.Any(f => f.Contains($"private {typeName} {varName}"))) // 동일 이름의 동일 타입이 이미 존재하면 무조건 건너뜀
                        continue;

                    if (cachedExistingFields.Contains(varName)) // 중복 이름이 있다면 접미사 추가
                        varName = baseVarName + typeName;

                    if (cachedExistingFields.Contains(varName)) continue; // 다시 최종 이름 중복 체크

                    newFields.Add($"    [SerializeField] private {typeName} {varName};");
                    cachedExistingFields.Add(varName);

                    pendingList.list.Add(new AutoBindingEntry
                    {
                        scriptPath = scriptPath,
                        fieldName = varName,
                        targetInstanceId = icb.GetInstanceID(),
                        componentInstanceId = comp.GetInstanceID()
                    });

                }
            }

            newFields.Add("    #endregion");

            string newRegion = string.Join("\n", newFields);
            string oldRegion = regionStart != -1 && regionEnd != -1 ? string.Join("\n", lines.Skip(regionStart).Take(regionEnd - regionStart + 1)) : "";

            if (oldRegion == newRegion)
            {
                Debug.Log("not changed");
                return;
            }

            int insertIndex = fieldLines.FindIndex(l => l.Contains("class") && l.Contains(":"));
            if (insertIndex != -1)
            {
                insertIndex++;
                while (insertIndex < fieldLines.Count && !fieldLines[insertIndex].Contains("{"))
                    insertIndex++;
                fieldLines.InsertRange(insertIndex + 1, newFields);
            }

            if (!lines.Any(line => line.Contains("using TMPro")) && newFields.Any(f => f.Contains("TMP_")))
            {
                int usingIndex = fieldLines.FindLastIndex(l => l.StartsWith("using"));
                fieldLines.Insert(usingIndex + 1, "using TMPro;");
            }

            if (!lines.Any(line => line.Contains("using System.Collections.Generic")) && newFields.Any(f => f.Contains("List<")))
            {
                int usingIndex = fieldLines.FindLastIndex(l => l.StartsWith("using"));
                fieldLines.Insert(usingIndex + 1, "using System.Collections.Generic;");
            }

            File.WriteAllLines(scriptPath, fieldLines);
            EditorPrefs.SetString("ICB_AutoBindings", JsonUtility.ToJson(pendingList));
            AssetDatabase.Refresh();
        }

        private void CacheExistingFields()
        {
            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(icb));
            string[] lines = File.ReadAllLines(scriptPath);

            foreach (var line in lines)
            {
                var match = System.Text.RegularExpressions.Regex.Match(line, @"private\\s+(\\S+)\\s+(\\S+);");
                if (match.Success)
                {
                    string varName = match.Groups[2].Value.TrimEnd(';');
                    cachedExistingFields.Add(varName);
                }
            }
        }

        private string ToPascalCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            string cleaned = new string(input.Where(c => char.IsLetterOrDigit(c) || c == ' ' || c == '_').ToArray());
            string[] parts = cleaned.Split(new[] { '_', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Concat(parts.Select(p => char.ToUpperInvariant(p[0]) + p.Substring(1)));
        }
    }

    public class GameObjectProxy : Component
    {
        public GameObject gameObjectRef;
        public GameObjectProxy(GameObject go) => gameObjectRef = go;
        public override string ToString() => "GameObject";
    }

    [Serializable]
    public class AutoBindingEntry
    {
        public string scriptPath;
        public string fieldName;
        public int targetInstanceId;
        public int componentInstanceId;
    }

    [Serializable]
    public class AutoBindingEntryList
    {
        public List<AutoBindingEntry> list = new();
    }

    [InitializeOnLoad]
    public static class ICBPostBindingProcessor
    {
        static ICBPostBindingProcessor()
        {
            EditorApplication.delayCall += TryBind;
        }

        static void TryBind()
        {
            if (!EditorPrefs.HasKey("ICB_AutoBindings")) return;

            var json = EditorPrefs.GetString("ICB_AutoBindings");
            var entries = JsonUtility.FromJson<AutoBindingEntryList>(json);

            foreach (var entry in entries.list)
            {
                var target = EditorUtility.InstanceIDToObject(entry.targetInstanceId) as MonoBehaviour;
                var obj = EditorUtility.InstanceIDToObject(entry.componentInstanceId);

                if (target == null || obj == null) continue;

                var so = new SerializedObject(target);
                var sp = so.FindProperty(entry.fieldName);
                if (sp != null)
                {

                    if (obj is GameObject go) // GameObject인지 Component인지 분기해서 처리
                        sp.objectReferenceValue = go;
                    else if (obj is Component comp)
                        sp.objectReferenceValue = comp;

                    so.ApplyModifiedProperties();
                }
            }

            EditorPrefs.DeleteKey("ICB_AutoBindings");
        }
    }
}
