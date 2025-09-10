// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework © 2025 Ironcow Studio
// Distributed via Gumroad under a paid license
// 
// 🔐 This file is part of a licensed product. Redistribution or sharing is prohibited.
// 🔑 A valid license key is required to unlock all features.
// 
// 🌐 For license terms, support, or team licensing, visit:
//     https://ironcowstudio.duckdns.org/ironcowstudio.html
// ─────────────────────────────────────────────────────────────────────────────


using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace Ironcow.Synapse.Data
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class DataToolSetting : SOSingleton<DataToolSetting>
    {
#if UNITY_EDITOR
        public static string ScriptFolderFullPath { get; private set; }      // "......\이 스크립트가 위치한 폴더 경로"
        public static string ScriptFolderInProjectPath { get; private set; } // "Assets\...\이 스크립트가 위치한 폴더 경로"
        public static string AssetFolderPath { get; private set; }

        private static void InitFolderPath([System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            ScriptFolderFullPath = System.IO.Path.GetDirectoryName(sourceFilePath);
            int rootIndex = ScriptFolderFullPath.IndexOf(@"Assets\");
            if (rootIndex > -1)
            {
                ScriptFolderInProjectPath = ScriptFolderFullPath.Substring(rootIndex, ScriptFolderFullPath.Length - rootIndex);
            }
        }
        [Header("Scriptable Object Data Path")]
        public DefaultAsset dataScriptableObjectPath;
        public static string DataScriptableObjectPath { get => AssetDatabase.GetAssetPath(instance.dataScriptableObjectPath); }
        public static string DataScriptableObjectFullPath { get => Application.dataPath.Replace("Assets", "") + AssetDatabase.GetAssetPath(instance.dataScriptableObjectPath); }

        [Header("Thumbnail Path")]
        public DefaultAsset thumbnailPath;
        public static string ThumbnailPath { get => AssetDatabase.GetAssetPath(instance.thumbnailPath); }

        [Header("Class Generate Path")]
        public DefaultAsset classGeneratePath;
        public static string ClassGeneratePath { get => AssetDatabase.GetAssetPath(instance.classGeneratePath); }

        [Header("Google Sheet Data")]
        public string GSheetUrl;
        public string infoSheet;
        public List<SheetInfoSO> sheets;

#endif
    }

}
