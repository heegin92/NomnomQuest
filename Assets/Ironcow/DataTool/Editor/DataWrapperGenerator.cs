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
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ironcow.Synapse.Data
{
    public static class DataWrapperGenerator
    {
        public static void Generate()
        {
            var baseType = typeof(BaseDataSO);
            var scriptBuilder = new StringBuilder();

            scriptBuilder.AppendLine("// This file is auto-generated. Do not modify manually.");
            scriptBuilder.AppendLine("using System;");
            scriptBuilder.AppendLine("using System.Collections.Generic;");
            scriptBuilder.AppendLine("using UnityEngine;");
            scriptBuilder.AppendLine();
            scriptBuilder.AppendLine("namespace Ironcow.Synapse.Data");
            scriptBuilder.AppendLine("{");
            scriptBuilder.AppendLine("    [Serializable]");
            scriptBuilder.AppendLine("    public partial class DataWrapper");
            scriptBuilder.AppendLine("    {");

            var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && baseType.IsAssignableFrom(t))
                .OrderBy(t => t.Name)
                .ToList();

            foreach (var type in allTypes)
            {
                scriptBuilder.AppendLine($"        public List<{type.FullName}> {type.Name};");
            }

            scriptBuilder.AppendLine("    }");
            scriptBuilder.AppendLine("}");

            string folderPath = Path.Combine(DataToolSetting.ClassGeneratePath);
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, "DataWrapper.cs");

            File.WriteAllText(filePath, scriptBuilder.ToString());
            AssetDatabase.Refresh();

            Debug.Log("DataWrapper.cs create complete");
        }
    }

    // Postprocessing Hook
    public class DataWrapperPostProcessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] _, string[] __, string[] ___)
        {
            if (EditorPrefs.GetBool("GenerateClass"))
            {
                EditorPrefs.DeleteKey("GenerateClass");
                EditorApplication.delayCall += () =>
                {
                    Debug.Log("Post-compilation: Regenerating DataWrapper...");
                    DataWrapperGenerator.Generate();
                };
            }
        }
    }
}
