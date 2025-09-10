// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using UnityEditor;
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector.Editor;
#endif

namespace Ironcow.Synapse
{
    [CustomEditor(typeof(SynapseBehaviour), true)]
    public partial class SynapseBehaviourEditor : SynapseBaseEditor
    {
        public override void OnInspectorGUI()
        {
            OnDraw();
            OnInputAction();
        }

        partial void OnInputAction();
    }
}
