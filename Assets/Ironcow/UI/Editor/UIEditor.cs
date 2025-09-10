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


using UnityEditor;

using UnityEngine;


namespace Ironcow.Synapse.UI
{
    internal class UIEditor : Editor
    {
        public static void CreateManagerInstance()
        {
            if (GameObject.Find("UIManager")) return;
            var obj = new GameObject("UIManager");
            obj.AddComponent<UIManager>();
        }

    }
}
