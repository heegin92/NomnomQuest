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


using Ironcow.Synapse.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Ironcow.Synapse.UI
{
#if UNITY_EDITOR
    [ScriptTemplate("Custom/UIBase")]
#endif
    public abstract partial class UIBase : SynapseBehaviour
    {
        public eUIPosition uiPosition;
        public UIOptions uiOptions;
        public UnityAction<object[]> opened;
        public UnityAction<object[]> closed;
        public RectTransform rectTransform { get => transform as RectTransform; }

        protected virtual void Awake()
        {
            opened = Opened;
            closed = Closed;

            SetLocale();
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        public abstract void HideDirect();

        public virtual void Opened(object[] param)
        {
            if (this is IUIInputHandler handler)
                handler.BindInput();
            bool isBinded = false;
            for (int i = 0; i < param.Length; i++)
            {
                if (param[i] is BaseModel model)
                {
                    BindMvvm(param[i] as BaseModel);
                    isBinded = true;
                    break;
                }
            }
            if(!isBinded)
                BindMvvm();
        }

        public virtual void Closed(object[] param)
        {
            if (this is IUIInputHandler handler)
                handler.UnbindInput();
        }

#if UNITY_EDITOR
#if USE_AUTO_CACHING
        protected override void OnValidate()
        {
            base.OnValidate();
#else
        void OnValidate()
        {
#endif
            CacheFocusTargets();
            SetUIType();
            AutoBindButtons();
        }
#endif

        public void SetUIType()
        {
            if (name.Contains("Gnb"))
            {
                uiPosition = eUIPosition.GNB;
            }
            else if (name.Contains("Top"))
            {
                uiPosition = eUIPosition.Top;
            }
            else if (name.Contains("Popup"))
            {
                uiPosition = eUIPosition.Popup;
            }
            else if (name.Contains("UI"))
            {
                uiPosition = eUIPosition.UI;
            }
        }

        partial void BindMvvm();
        partial void BindMvvm(BaseModel model);
        partial void CacheFocusTargets();
        partial void AutoBindButtons();
        partial void SetLocale();
    }

    [System.Serializable]
    public class UIOptions
    {
        [Tooltip("로딩 시 자동 활성화 여부")]
        public bool isActiveOnLoad = true;
        [Tooltip("종료 시 파괴 여부")]
        public bool isDestroyOnHide = true;
        [Tooltip("여러개의 창을 사용할 지 여부")]
        public bool isMultiple = false;
    }

}
