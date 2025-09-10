// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using System.Collections.Generic;

using UnityEngine;

namespace Ironcow.Synapse.Sample.Common
{
    public class WorldObject : SynapseBehaviour
    {
        [SerializeField] private List<string> strings = new List<string>();
        int idx = 0;

        public string Interaction()
        {
            var retStr = strings[idx];
            idx = idx.Next(strings.Count, false);
            return retStr;
        }
    }
}
