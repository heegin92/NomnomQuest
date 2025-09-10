// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using System.IO;
using System.Text;

using UnityEditor;

using UnityEngine;

namespace Ironcow.Synapse.Resource
{
    public class ResourceEditor : Editor
    {
        private static ResourceEditor _instance;
        public static ResourceEditor instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ResourceEditor();
                }
                return _instance;
            }
        }

        [SerializeField] public TextAsset templete;
        [SerializeField] public TextAsset typeTemplete;

        public static void CreatePartialScripts()
        {
            var outPath = Path.Combine(EditorDataSetting.ScriptFullPath, "Managers");
            if (!Directory.Exists(outPath)) Directory.CreateDirectory(outPath);
            AssetDatabase.Refresh();
            var filePath = Path.Combine(outPath, "ResourceManager.cs");
            if (!File.Exists(filePath))
            {
                var file = File.Create(filePath);
                var bytes = Encoding.UTF8.GetBytes(instance.templete.text);
                file.Write(bytes, 0, bytes.Length);
                file.Dispose();
                file.Close();
                AssetDatabase.Refresh();
            }
            filePath = Path.Combine(outPath, "ResourceType.cs");
            if (!File.Exists(filePath))
            {
                var file = File.Create(filePath);
                var bytes = Encoding.UTF8.GetBytes(instance.typeTemplete.text);
                file.Write(bytes, 0, bytes.Length);
                file.Dispose();
                file.Close();
                AssetDatabase.Refresh();
            }
        }

        public static void CreateManagerInstance()
        {
            if (GameObject.Find("ResourceManager")) return;
            var manager = new GameObject("ResourceManager");
            manager.AddComponent<ResourceManager>();
        }
    }
}
