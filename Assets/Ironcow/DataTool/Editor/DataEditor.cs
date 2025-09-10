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


using System.IO;
using System.Text;

using UnityEditor;

using UnityEngine;

namespace Ironcow.Synapse.Data
{
    public class DataEditor : Editor
    {
        private static DataEditor _instance;
        public static DataEditor instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DataEditor();
                }
                return _instance;
            }
        }

        [SerializeField] public TextAsset templete;
        [SerializeField] public TextAsset userInfoTemplete;

        public static void CreatePartialScripts()
        {
            if (string.IsNullOrEmpty(EditorDataSetting.ScriptPath))
            {
                if (EditorUtility.DisplayDialog("Error", "Script Path is missing.\n Please set it in the Editor Data Settings.", "OK"))
                {
                    ProjectSettingTool.Open();
                }
                return;
            }
            var outPath = Application.dataPath.Replace("Assets", EditorDataSetting.ScriptPath);
            if (!Directory.Exists(outPath)) Directory.CreateDirectory(outPath);
            AssetDatabase.Refresh();
            var path = outPath + "/Common/";
            var filePath = path + "UserInfo.cs";
            if (!File.Exists(filePath))
            {
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                var file = File.Create(filePath);
                var bytes = Encoding.UTF8.GetBytes(instance.userInfoTemplete.text);
                file.Write(bytes, 0, bytes.Length);
                file.Dispose();
                file.Close();
                AssetDatabase.Refresh();
            }
            path = outPath + "/Manager/";
            filePath = path + "DataManager.cs";
            if (!File.Exists(filePath))
            {
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                var file = File.Create(filePath);
                var bytes = Encoding.UTF8.GetBytes(instance.templete.text);
                file.Write(bytes, 0, bytes.Length);
                file.Dispose();
                file.Close();
                AssetDatabase.Refresh();
            }
        }

        public static void CreateManagerInstance()
        {
            if (GameObject.Find("DataManager")) return;
            var manager = new GameObject("DataManager");
            manager.AddComponent<DataManager>();
        }
    }
}
