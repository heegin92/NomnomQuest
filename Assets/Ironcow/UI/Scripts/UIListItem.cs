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

namespace Ironcow.Synapse.UI
{
    public class UIListItem : SynapseBehaviour
#if USE_LOCALE
        , ILocale
    {
        public List<LocaleText> texts;
#else
    { 
#endif

        /// <summary>
        /// 현재 UI상 순서에 해당하는 값
        /// </summary>

        public int index { get => transform.GetSiblingIndex(); }

        public RectTransform rectTransform { get => transform as RectTransform; }

#if USE_LOCALE
        void Awake()
        {
            foreach (var text in texts)
            {
                text.text.text = LocaleDataSO.instance.LocaleDic[text.key];
            }
        }
        public void SetLocaleTexts()
        {
            texts.Clear();
            var tmpTexts = GetComponentsInChildren<TMPro.TMP_Text>(true).ToList();
            tmpTexts.ForEach(text =>
            {
                var localeData = LocaleDataSO.instance.localeData.Find(obj => obj.Korean == text.text);
                if (localeData != null)
                {
                    texts.Add(new LocaleText(localeData.Key, text));
                }
            });
        }
#endif

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

    }
}
