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

namespace Ironcow.Synapse.BT
{
    [CustomEditor(typeof(BTEditor))]
    public class BTSettingEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            OnDraw();
        }

        public void OnDraw()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("savePath"));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }

            if (BTEditor.instance.savePath == null)
            {
                EditorGUILayout.HelpBox("Please assign a save path for the Behaviour Tree Editor.", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("runner"));
            }
        }
    }
}
