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
    [CreateAssetMenu(menuName = "Synapse/DataTool/TreeMap")]
    public class DataTreeMapSO : EditorSOSingleton<DataTreeMapSO>
    {
        public List<SheetInfoSO> groups;

    }

}
