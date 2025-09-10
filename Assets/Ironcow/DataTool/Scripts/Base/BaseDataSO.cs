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


#if USE_LOCALE
using Ironcow.Synapse.LocalizeTool;

#endif
using UnityEngine;

namespace Ironcow.Synapse.Data
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "BaseData", menuName = "ScriptableObjects/Base Data")]
    public class BaseDataSO
#if UNITY_EDITOR
    : ScriptableObject
#endif
    {
        public string rcode;
        public string name;
#if USE_LOCALE
        public string localeName { get => LocaleDataSO.GetString("name_" + rcode); }
#endif
        public string desc;
#if USE_LOCALE
        public string localeDesc { get => LocaleDataSO.GetString("desc_" + rcode); }
        public string GetLocaleDesc(params object[] param)
        {
            return LocaleDataSO.GetString("desc_" + rcode, param);
        }
#endif
        public object clone
        {
            get
            {
                var obj = MemberwiseClone();
                return obj;
            }
        }

        protected virtual void OnClone(object obj) { }
    }
}
