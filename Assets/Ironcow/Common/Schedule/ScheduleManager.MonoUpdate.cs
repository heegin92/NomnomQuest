// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

namespace Ironcow.Synapse
{
#if !USE_IRONCOW_CORE
    public partial class ScheduleManager
    {

        private void Update()
        {
            RunUpdate();
        }

        private void FixedUpdate()
        {
            RunFixedUpdate();
        }

        private void LateUpdate()
        {
            RunLateUpdate();
        }
    }
#endif
}
