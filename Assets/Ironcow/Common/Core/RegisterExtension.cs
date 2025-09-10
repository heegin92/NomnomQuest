// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

namespace Ironcow.Synapse.Core
{
    public static class RegisterExtention
    {
        public static T Instantiate<T>(this T prefab) where T : SynapseBehaviour
        {
            var obj = Register.Instantiate(prefab);

            var icbs = obj.GetComponentsInChildren<SynapseBehaviour>(true);

            foreach (var icb in icbs)
            {
                ScheduleManager.instance.SubScribe(icb);
            }

            return obj;
        }

        public static void Release<T>(this T obj) where T : SynapseBehaviour
        {
            Register.Release(obj.gameObject);
        }
    }
}
