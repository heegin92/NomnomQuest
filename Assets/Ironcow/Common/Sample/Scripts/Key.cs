// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using UnityEngine;

namespace Ironcow.Synapse.Sample.Common
{
    public class Key : SynapseBehaviour
    {
        public string Interaction()
        {
            Destroy(gameObject);
            return "열쇠 획득!";
        }

        protected override void OnDestroy()
        {
            Debug.Log("열쇠가 사라졌습니다.");
        }
    }
}
