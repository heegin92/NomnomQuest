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


using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Ironcow.Synapse.UI
{
#if UNITY_EDITOR
    [ScriptTemplate("Custom/CanvasBase")]
#endif
#if USE_AUTO_CACHING
    public class CanvasBase : SynapseBehaviour, CanvasOption 
#else
public class CanvasBase : SynapseBehaviour, CanvasOption
#endif
    {
        [SerializeField] protected List<Transform> parents;
        [SerializeField] private bool isCreateSafeArea;
        [SerializeField] private Vector2 targetSize = new Vector2(1080, 1920);
        private CanvasScaler scaler;

        void Awake()
        {

            InitParents();
            if (isCreateSafeArea)
            {
                InitSafeArea();
            }
        }

        private void InitParents()
        {
            if (parents.Count > 0) UIManager.SetParents(parents);
        }

        private void InitSafeArea()
        {
            foreach (var t in parents)
            {
                if (t is RectTransform rt)
                    ApplySafeArea(rt);
            }
            var safeArea = ResourceManager.instance.LoadAsset<GameObject>("SafeArea", ResourceType.Prefabs);
            if (safeArea)
            {
                var ui = Instantiate(safeArea, transform);
                ui.transform.SetAsFirstSibling();
            }
        }

        private void ApplySafeArea(RectTransform rt)
        {
            var safeArea = Screen.safeArea;
            var min = safeArea.position;
            var max = min + safeArea.size;

            min.x /= Screen.width;
            min.y /= Screen.height;
            max.x /= Screen.width;
            max.y /= Screen.height;

            rt.anchorMin = min;
            rt.anchorMax = max;
        }

#if USE_AUTO_CACHING && UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
#else
        void OnValidate()
        {
#endif
            SetScaler();
            SetParent();
        }

        private void SetScaler()
        {
            if (scaler == null)
            {
                scaler = GetComponent<CanvasScaler>();
            }

            if (scaler == null)
            {
                Debug.LogWarning($"[{name}] CanvasScaler가 존재하지 않습니다.");
                return;
            }

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = targetSize;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        }

        public void SetParent()
        {
            if (parents == null) parents = new List<Transform>();
            if (parents.Count == 0)
            {
                foreach (var name in Enum.GetNames(typeof(eUIPosition)))
                {
                    var parent = transform.Find(name);
                    if (parent != null && !parents.Contains(parent)) parents.Add(parent);
                }
            }
        }

    }


}
