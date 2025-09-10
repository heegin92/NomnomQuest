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
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Ironcow.Synapse.Data
{
    public static class DataBytesBuilder
    {

        public static void BuildEncryptedDataFile()
        {
            Debug.Log("Building EncryptedData.bytes...");

            Type wrapperType = typeof(DataWrapper);
            object wrapperInstance = Activator.CreateInstance(wrapperType);

            var baseType = typeof(BaseDataSO);
            var fields = wrapperType.GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                Type listElementType = field.FieldType.GetGenericArguments()[0];
                string soName = listElementType.Name + "SO";
                string path = $"Datas/{soName}";

                var so = Resources.Load(path);
                if (so == null)
                {
                    Debug.LogWarning($"Missing SO: {path}");
                    continue;
                }

                var dataListProp = so.GetType().GetField("dataList");
                if (dataListProp == null) continue;

                object dataList = dataListProp.GetValue(so);
                field.SetValue(wrapperInstance, dataList);
            }

            string json = JsonUtility.ToJson(wrapperInstance, true);
            byte[] encrypted = AesEncryptor.Encrypt(json);

            string outputPath = DataToolSetting.DataScriptableObjectPath + "/EncryptedData.bytes";
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            File.WriteAllBytes(outputPath, encrypted);
            AssetDatabase.ImportAsset(outputPath);

            Debug.Log("EncryptedData.bytes build complete");
        }
    }
}
