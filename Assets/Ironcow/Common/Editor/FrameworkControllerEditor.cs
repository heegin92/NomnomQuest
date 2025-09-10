// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

using UnityEngine;

using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Ironcow.Synapse.Common
{
    public class ModuleData
    {
        public int idx;
        public Action draws;
    }

    [CustomEditor(typeof(FrameworkController))]
    public partial class FrameworkControllerEditor : Editor
    {
#if UNITY_EDITOR_WIN
        string PackagePath
        {
            get
            {
                var newPath = "";
                var paths = Application.persistentDataPath.Split('/');
                foreach (var path in paths)
                {
                    newPath += path + "/";

                    if (path.Contains("AppData"))
                        break;
                }
                newPath += "Roaming/Unity/Asset Store-5.x/";
                return newPath;
            }
        }
#elif UNITY_EDITOR_MAC
        string PackagePath
        {
            get
            {
                var newPath = "";
                var paths = Application.persistentDataPath.Split('/');
                foreach (var path in paths)
                {
                    newPath = path + "/";

                    if (path.Contains("Library"))
                        break;
                }
                newPath += "Unity/Asset Store-5.x/";
                return newPath;
            }
        }
#endif
        static bool DrawToolbar(ref int selected, string[] texts)
        {
            bool dirty = false;

            int value = GUILayout.Toolbar(selected, texts, EditorStyles.toolbarButton);
            if (value != selected)
            {
                selected = value;
                dirty = true;
            }

            return dirty;
        }

        private FrameworkController controller = null;
        private List<string> toolbarTexts = null;
        private int selected = 0;

        public List<ModuleData> modules = new();
        private int odinState = -1;

        private void OnEnable()
        {
            this.controller = base.target as FrameworkController;

            this.toolbarTexts = new List<string>();
            this.toolbarTexts.Add("Property");
            this.controller.is2d = IsSymbolAlreadyDefined("USE_2DTOOL");
            this.controller.is3d = IsSymbolAlreadyDefined("USE_3DTOOL");
            this.controller.isUpdatable = IsSymbolAlreadyDefined("USE_UPDATABLE");
            this.controller.isLocale = IsSymbolAlreadyDefined("USE_LOCALE");
            this.controller.isAutoCaching = IsSymbolAlreadyDefined("USE_AUTO_CACHING");
            this.controller.isAddressableAsync = IsSymbolAlreadyDefined("USE_ADDRESSABLE");
            this.controller.isObjectPool = IsSymbolAlreadyDefined("USE_OBJECT_POOL");
            this.controller.isCloudCode = IsSymbolAlreadyDefined("USE_CLOUD_CODE");
            this.controller.isFSM = IsSymbolAlreadyDefined("USE_FSM");
            this.controller.isStrategy = IsSymbolAlreadyDefined("USE_STRATEGY");
            this.controller.isIroncowLifecycle = IsSymbolAlreadyDefined("USE_IRONCOW_CORE");
            this.controller.isOdin = IsSymbolAlreadyDefined("ODIN_INSPECTOR");


            var methods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            List<Func<ModuleData>> getModules = new();
            foreach (var method in methods)
            {
                if (method.ReturnType == typeof(ModuleData))
                {
                    var del = (Func<ModuleData>)Delegate.CreateDelegate(typeof(Func<ModuleData>), this, method);
                    getModules.Add(del);
                }
            }

            foreach (var action in getModules)
            {
                modules.Add(action.Invoke());
            }
            modules.Sort((x, y) => x.idx.CompareTo(y.idx));
        }

        public ModuleData GetModuleData_Updatable()
        {
            return new ModuleData { idx = 2, draws = Draw_Updatable };
        }

        public ModuleData GetModuleData_AutoCaching()
        {
            return new ModuleData { idx = 1, draws = DrawAutoCaching };
        }

        public ModuleData GetModuleData_Odin()
        {
            return new ModuleData { idx = 100, draws = DrawOdin };
        }

        public void Draw_Updatable()
        {
            EditorGUILayout.LabelField("Updatable", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            {
                this.controller.isUpdatable = EditorGUILayout.ToggleLeft("Use Updatable", this.controller.isUpdatable);
                if (this.controller.isUpdatable)
                {
                    AddDefineSymbol("USE_UPDATABLE");
                }
                else
                {
                    RemoveDefineSymbol("USE_UPDATABLE");
                }
            }
            EditorGUI.indentLevel--;
            GUILayout.Space(10f);
        }

        public void DrawAutoCaching()
        {
            EditorGUILayout.LabelField("Auto Caching", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            {
                this.controller.isAutoCaching = EditorGUILayout.ToggleLeft("Use Auto Caching", this.controller.isAutoCaching);
                if (this.controller.isAutoCaching)
                {
                    AddDefineSymbol("USE_AUTO_CACHING");
                }
                else
                {
                    RemoveDefineSymbol("USE_AUTO_CACHING");
                }
            }
            EditorGUI.indentLevel--;
            GUILayout.Space(10f);
        }
        bool IsOdinEditorAssemblyLoaded()
        {
            var type = Type.GetType("Sirenix.OdinInspector.Editor.AddressablesUtility, Sirenix.OdinInspector.Editor");
            return type != null;
        }

        public void DrawOdin()
        {
            if (!IsOdinEditorAssemblyLoaded()) return;
            EditorGUILayout.LabelField("Use Odin Inspector", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            {
                this.controller.isOdin = EditorGUILayout.ToggleLeft("Use Odin Inspector", this.controller.isOdin);
                if (this.controller.isOdin)
                {
                    AddDefineSymbol("ODIN_INSPECTOR");
                }
                else
                {
                    RemoveDefineSymbol("ODIN_INSPECTOR");
                }
            }
            EditorGUI.indentLevel--;
            GUILayout.Space(10f);
        }

        public override void OnInspectorGUI()
        {
            if (this.controller == null)
                return;

            foreach(var module in modules)
            {
                if (module.draws != null)
                    module.draws.Invoke();
            }

            EditorGUILayout.LabelField("UniRX", EditorStyles.boldLabel);
            if (!Directory.Exists(Application.dataPath + "/Plugins/UniRx"))
            {
                if (GUILayout.Button("Import UniRX"))
                {
                    var path = PackagePath + "neuecc/ScriptingIntegration/UniRx - Reactive Extensions for Unity.unitypackage";
                    if (!File.Exists(path))
                    {
                        Application.OpenURL("https://assetstore.unity.com/packages/tools/integration/unirx-reactive-extensions-for-unity-17276");
                    }
                    else
                    {
                        AssetDatabase.ImportPackage(path, true);
                    }
                    //ImportPackageManager("com.intervr.ts.unirx");
                }
                if (IsSymbolAlreadyDefined("USE_UNIRX"))
                    RemoveDefineSymbol("USE_UNIRX");
            }
            else
            {
                EditorGUILayout.LabelField("Imported", EditorStyles.boldLabel);
                AddDefineSymbol("USE_UNIRX");
            }
            GUILayout.Space(10);

            var info = GetPackageInfo("com.cysharp.unitask");
            EditorGUILayout.LabelField("UniTask", EditorStyles.boldLabel);
            if (info == null)
            {
                if (GUILayout.Button("Import UniTask"))
                {
                    AddPackage("https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask");
                }
                if(IsSymbolAlreadyDefined("USE_UNITASK"))
                    RemoveDefineSymbol("USE_UNITASK");
            }
            else
            {
                EditorGUILayout.LabelField("Imported", EditorStyles.boldLabel);
                AddDefineSymbol("USE_UNITASK");
            }
            GUILayout.Space(10);

            info = GetPackageInfo("com.unity.cinemachine");
            EditorGUILayout.LabelField("Cinemachine", EditorStyles.boldLabel);
            if (info == null)
            {
                if (GUILayout.Button("Import Cinemachine"))
                {
                    AddPackage("com.unity.cinemachine");
                }
            }
            else
            {
                EditorGUILayout.LabelField("Imported", EditorStyles.boldLabel);
            }
            GUILayout.Space(10);

        }

        public struct DefineSymbolData
        {
            public BuildTargetGroup buildTargetGroup; // 현재 빌드 타겟 그룹
            public string fullSymbolString;           // 현재 빌드 타겟 그룹에서 정의된 심볼 문자열 전체
            public Regex symbolRegex;

            public DefineSymbolData(string symbol)
            {
                buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
                fullSymbolString = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
                symbolRegex = new Regex(@"\b" + symbol + @"\b(;|$)");
            }
        }

        /// <summary> 심볼이 이미 정의되어 있는지 검사 </summary>
        public static bool IsSymbolAlreadyDefined(string symbol)
        {
            DefineSymbolData dsd = new DefineSymbolData(symbol);

            return dsd.symbolRegex.IsMatch(dsd.fullSymbolString);
        }

        /// <summary> 심볼이 이미 정의되어 있는지 검사 </summary>
        public static bool IsSymbolAlreadyDefined(string symbol, out DefineSymbolData dsd)
        {
            dsd = new DefineSymbolData(symbol);

            return dsd.symbolRegex.IsMatch(dsd.fullSymbolString);
        }

        /// <summary> 특정 디파인 심볼 추가 </summary>
        public static void AddDefineSymbol(string symbol)
        {
            // 기존에 존재하지 않으면 끝에 추가
            if (!IsSymbolAlreadyDefined(symbol, out var dsd))
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(dsd.buildTargetGroup, $"{dsd.fullSymbolString};{symbol}");
            }
        }

        /// <summary> 특정 디파인 심볼 제거 </summary>
        public static void RemoveDefineSymbol(string symbol)
        {
            // 기존에 존재하면 제거
            if (IsSymbolAlreadyDefined(symbol, out var dsd))
            {
                string strResult = dsd.symbolRegex.Replace(dsd.fullSymbolString, "");

                PlayerSettings.SetScriptingDefineSymbolsForGroup(dsd.buildTargetGroup, strResult);
            }
        }

        private PackageInfo GetPackageInfo(string packageName)
        {
            try
            {
                return AssetDatabase.FindAssets("package")
                    .Select(AssetDatabase.GUIDToAssetPath)
                        .Where(x => AssetDatabase.LoadAssetAtPath<TextAsset>(x) != null)
                    .Select(PackageInfo.FindForAssetPath)
                        .Where(x => x != null)
                    .First(x => x.name == packageName);
            }
            catch { return null; }
        }

        private async void AddPackage(string packageName)
        {
            req = Client.Add(packageName);
            EditorApplication.update += Progress;
            while (!req.IsCompleted) { await Task.Yield(); }
        }
        private async void RemovePackage(string packageName)
        {
            var req = Client.Remove(packageName);
            EditorApplication.update += Progress;
            while (!req.IsCompleted) { await Task.Yield(); }
            Debug.Log($"Remove {packageName} is {req.Status}");
        }

        static AddRequest req;
        static void Progress()
        {
            if (req.IsCompleted)
            {
                if (req.Status == StatusCode.Success)
                    Debug.Log("Installed: " + req.Result.packageId);
                else if (req.Status >= StatusCode.Failure)
                    Debug.Log(req.Error.message);

                EditorApplication.update -= Progress;
            }
        }

        private void ImportPackageManager(string packageName)
        {
            //UnityEditor.PackageManager.
        }
    }
}
