// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using UnityEngine;

namespace Ironcow.Synapse.Sample.Common
{
    public class TrackingCamera : SynapseBehaviour
#if USE_UPDATABLE
        , ILateUpdatable
#endif
    {
        [SerializeField] private Transform target;

#if USE_UPDATABLE
        public void OnLateUpdate()
#else
    void LateUpdate()
#endif
        {
            transform.position = target.transform.position + new Vector3(0, 2, -10);
        }
    }
}
