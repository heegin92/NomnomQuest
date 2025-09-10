// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using UnityEngine;

namespace Ironcow.Synapse.Sample.Common
{
    public class Door : SynapseBehaviour
#if USE_UPDATABLE
        , IUpdatable
#endif
    {
        private bool isOpen;
        private float openValue = -90;
        private float closeValue = 0;
        public bool isAction;
        Vector3 startAngle;
        float actTime = 0;

#if USE_UPDATABLE
        public void OnUpdate()
#else
    void Update()
#endif
        {
            if (isAction)
            {
                float currentY = transform.localEulerAngles.y;
                if (currentY > 180f) currentY -= 360f;

                if (isOpen)
                {
                    if (currentY <= openValue)
                    {
                        transform.localEulerAngles = new Vector3(0, openValue, 0);
                        isAction = false;
                        return;
                    }
                }
                else
                {
                    if (currentY >= closeValue)
                    {
                        transform.localEulerAngles = new Vector3(0, closeValue, 0);
                        isAction = false;
                        return;
                    }
                }

                transform.Rotate(new Vector3(0, 1, 0) * (isOpen ? -1 : 1) * Time.deltaTime * 100f);
            }
        }

        public string Interaction()
        {
            isAction = true;
            isOpen = !isOpen;
            startAngle = transform.localEulerAngles;
            actTime = 0;
            return isOpen ? "문이 열리네요" : "문이 닫히네요";
        }
    }
}
