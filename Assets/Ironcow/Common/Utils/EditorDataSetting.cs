// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Ironcow.Synapse
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [CreateAssetMenu(fileName = "EditorDataSetting", menuName = "Synapse/SettingSO/EditorDataSetting")]
    public class EditorDataSetting : SOSingleton<EditorDataSetting>
    {
#if UNITY_EDITOR
        public static string ScriptFolderFullPath { get; private set; }      // "......\이 스크립트가 위치한 폴더 경로"
        public static string ScriptFolderInProjectPath { get; private set; } // "Assets\...\이 스크립트가 위치한 폴더 경로"
        public static string AssetFolderPath { get; private set; }

        [Header("AutoCaching Prefab Path")]
        public List<DefaultAsset> prefabFolders;

        [Header("Scene Path")]
        public List<DefaultAsset> scenePath;
        private static List<string> scenePaths = new List<string>();
        public static List<string> ScenePath {
            get
            {
                if(scenePaths.Count != instance.scenePath.Count)
                {
                    scenePaths = instance.scenePath.Select(folder => AssetDatabase.GetAssetPath(folder)).ToList();
                }
                return scenePaths;
            }
        }

        public Object introScene;
        public static string IntroScenePath { get => instance.introScene == null ? "" : AssetDatabase.GetAssetPath(instance.introScene); }

        public Object dontDestroyScene;
        public static string DontDestroyScenePath { get => instance.dontDestroyScene == null ? "" : AssetDatabase.GetAssetPath(instance.dontDestroyScene); }

        [Header("Create Editor Prefab Path")]
        public DefaultAsset createEditorPrefabPath;
        public static string CreateEditorPrefabPath { get => instance.createEditorPrefabPath == null ? "" : AssetDatabase.GetAssetPath(instance.createEditorPrefabPath); }

        [Header("Create SettingSO Path")]
        public DefaultAsset settingSOPath;
        public static string SettingSOPath { get => instance.settingSOPath == null ? "" : AssetDatabase.GetAssetPath(instance.settingSOPath); }

        [Header("Create Script Path")]
        public DefaultAsset scriptPath;
        public static string ScriptPath { get => instance.scriptPath == null ? "" : AssetDatabase.GetAssetPath(instance.scriptPath); }
        public static string ScriptFullPath { get => instance.scriptPath == null ? "" : Application.dataPath.Replace("Assets", AssetDatabase.GetAssetPath(instance.scriptPath)); }

        [Header("Create Asset Prefab Path")]
        public DefaultAsset createAssetPrefabPath;
        public static string CreateAssetPrefabPath { get => instance.createAssetPrefabPath == null ? "" : AssetDatabase.GetAssetPath(instance.createAssetPrefabPath); }

        [Header("ResourceType Target Path")]
        public Object resourceTypeTarget;
        public static string ResourceTypeTargetPath => instance.resourceTypeTarget == null ? "" : AssetDatabase.GetAssetPath(instance.resourceTypeTarget);
#endif
    }
}
