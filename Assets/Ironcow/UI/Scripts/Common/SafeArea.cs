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


using System.Collections;
using System.Collections.Generic;

using Ironcow.Synapse;

using UnityEngine;
using UnityEngine.UI;

namespace Ironcow.Synapse.UI
{
    public class SafeArea : SynapseBehaviour
    {
        private void Awake()
        {
            var rectTransform = transform as RectTransform;
            var safeArea = Screen.safeArea;
            var minAnchor = safeArea.position;
            var maxAnchor = minAnchor + safeArea.size;

            minAnchor.x /= Screen.width;
            minAnchor.y /= Screen.height;
            maxAnchor.x /= Screen.width;
            maxAnchor.y /= Screen.height;

            rectTransform.anchorMin = minAnchor;
            rectTransform.anchorMax = maxAnchor;
        }
    }
}
