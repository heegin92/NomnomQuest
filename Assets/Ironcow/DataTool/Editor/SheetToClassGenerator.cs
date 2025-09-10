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


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using UnityEditor;

using UnityEngine;
using UnityEngine.Networking;

namespace Ironcow.Synapse.Data
{
    public static class SheetToClassGenerator
    {
        [MenuItem("Synapse/Tool/Generate Classes From Info Sheet")]
        public static async void GenerateFromInfo()
        {
            string baseUrl = DataToolSetting.instance.GSheetUrl;
            string savePath = DataToolSetting.ClassGeneratePath;
            Directory.CreateDirectory(savePath);

            string url = $"{baseUrl}export?format=tsv&gid={DataToolSetting.instance.infoSheet}";
            var req = UnityWebRequest.Get(url);
            var op = req.SendWebRequest();

            while (!op.isDone)
                await Task.Yield();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"❌ Failed to download info sheet\n{req.error}");
                return;
            }

            string tsvText = req.downloadHandler.text;
            GenerateClassesFromInfo(tsvText, savePath);
            AssetDatabase.Refresh();
        }

        private static void GenerateClassesFromInfo(string tsv, string savePath, string baseClass = "BaseDataSO")
        {
            var lines = tsv.Trim().Split('\n').Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
            if (lines.Count < 2) return;

            var baseFields = typeof(BaseDataSO).GetFields(BindingFlags.Public | BindingFlags.Instance)
                                               .Select(f => f.Name).ToHashSet();

            var header = lines[0].Trim().Split('\t');
            int colName = Array.IndexOf(header, "values");
            int colType = Array.IndexOf(header, "type");
            int colKey = Array.IndexOf(header, "key");
            int colClass = Array.IndexOf(header, "className");
            int colSheet = Array.IndexOf(header, "sheetId");

            var classMap = new Dictionary<string, List<FieldDef>>();
            var sheetMap = new Dictionary<string, string>();
            var keyMap = new Dictionary<string, string>();

            foreach (var line in lines.Skip(1))
            {
                var cols = line.Trim().Split('\t');
                if (cols.Length <= colClass) continue;

                string className = cols[colClass].Trim();
                if (!classMap.ContainsKey(className))
                    classMap[className] = new List<FieldDef>();

                string name = cols[colName].Trim();
                string type = cols[colType].Trim();
                string key = cols[colKey].Trim();
                string sheetId = cols[colSheet].Trim();
                if (!sheetMap.ContainsKey(className))
                    sheetMap.Add(className, sheetId);
                if (!keyMap.ContainsKey(className))
                    keyMap.Add(className, key);

                if (baseFields.Contains(name)) continue;

                classMap[className].Add(new FieldDef
                {
                    name = name,
                    type = type,
                });;
            }

            foreach (var kv in classMap)
            {
                string className = kv.Key;
                var fields = kv.Value;
                var usings = new HashSet<string> { "System", "Ironcow.Synapse.Data" };

                foreach (var f in fields)
                {
                    if (f.type.Contains("List<") || f.type.Contains("Dictionary<"))
                        usings.Add("System.Collections.Generic");
                    if (f.type == "Vector3" || f.type == "Color" || f.type == "GameObject")
                        usings.Add("UnityEngine");
                    if (f.type.Contains("UnityEvent"))
                        usings.Add("UnityEngine.Events");
                }

                var result = new List<string>();
                foreach (var u in usings.OrderBy(x => x))
                    result.Add($"using {u};");

                result.Add("");
                result.Add("[System.Serializable]");
                result.Add($"public partial class {className} : {baseClass}");
                result.Add("{");
                foreach (var f in fields)
                    result.Add($"    public {f.type} {f.name};");
                result.Add("}");

                string filePath = Path.Combine(savePath, $"{className}.cs");
                File.WriteAllText(filePath, string.Join("\n", result));
                Debug.Log($"✅ Generated: {filePath}");

                var idx = DataToolSetting.instance.sheets.FindIndex(s => s.sheetId == sheetMap[className]);
                if (idx >= 0)
                {
                    DataToolSetting.instance.sheets[idx].className = className;
                    DataToolSetting.instance.sheets[idx].key = keyMap[className];
                }
                else
                {
                    DataToolSetting.instance.sheets.Add(new SheetInfoSO
                    {
                        className = className,
                        sheetId = sheetMap[className],
                        key = keyMap[className]
                    });
                }
            }

            AssetDatabase.Refresh();
        }

        private class FieldDef
        {
            public string name;
            public string type;
            public string desc;
        }
    }
}
