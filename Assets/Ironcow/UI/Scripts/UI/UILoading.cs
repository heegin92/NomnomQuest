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

using UnityEngine;
using UnityEngine.UI;
#if USE_ADDRESSABLE
using UnityEngine.ResourceManagement.AsyncOperations;
#endif

namespace Ironcow.Synapse.UI
{
    public class UILoading : MonoSingleton<UILoading>
    {
        [SerializeField] private Image bg;
        [SerializeField] private Image progressFill;
        [SerializeField] private TMPro.TMP_Text progressText;
        [SerializeField] private TMPro.TMP_Text progressDesc;

        protected override void Awake()
        {
            base.Awake();
            gameObject.SetActive(false);
        }

        public static void Show(Sprite bg = null)
        {
            instance.SetBG(bg);
            instance.gameObject.SetActive(true);
        }

        public void SetBG(Sprite bg = null)
        {
            if (bg != null)
                this.bg.sprite = bg;
        }

        public static void Hide()
        {
            instance.gameObject.SetActive(false);
        }

        public void SetProgress(float progress, string desc = "")
        {
            this.progressDesc.text = desc;
            progressFill.fillAmount = progress;
        }

        public void SetProgress(AsyncOperation op, string desc = "")
        {
            this.progressDesc.text = desc;
            StartCoroutine(Progress(op));
        }

        public IEnumerator Progress(AsyncOperation op)
        {
            while (op.isDone)
            {
                progressFill.fillAmount = op.progress;
                yield return new WaitForEndOfFrame();
            }
            progressFill.fillAmount = 1;
        }

#if USE_ADDRESSABLE
        public void SetProgress(AsyncOperationHandle op, string desc = "")
        {
            this.progressDesc.text = desc;
            StartCoroutine(Progress(op));
        }

        public IEnumerator Progress(AsyncOperationHandle op)
        {
            while (op.IsDone)
            {
                progressFill.fillAmount = op.GetDownloadStatus().Percent;
                yield return new WaitForEndOfFrame();
            }
            progressFill.fillAmount = 1;
        }
#endif
    }
}
