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

#if UNITY_EDITOR
using UnityEditor;

#endif
using UnityEngine;

namespace Ironcow.Synapse.BT
{
    [CreateAssetMenu(fileName = "BTEditor", menuName = "ScriptableObjects/BTEditor")]
    public class BTEditor : SOSingleton<BTEditor>
    {
        public Object savePath;
        public BTRunner runner = new BTRunner();

#if UNITY_EDITOR
        public static string ParentPath
        {
            get => AssetDatabase.GetAssetPath(instance.savePath);
        }

        public static string SavePath
        {
            get => Path.Combine(AssetDatabase.GetAssetPath(instance.savePath), "BTSaveData/");
        }
#endif
    }
}
