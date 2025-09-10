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


using Ironcow.Synapse.BT;

using UnityEditor;

using UnityEngine;

namespace Ironcow.Synapse
{
    public partial class ProjectSettingTool
    {
        public SettingMenu OnEnable_BT()
        {
            return new SettingMenu { id = 16, name = "Behaviour Tree", isVisible = () => true, getScritables = Get_BTSO };
        }

        public ScriptableObject Get_BTSO()
        {
            return BTEditor.instance;
        }
    }

}
