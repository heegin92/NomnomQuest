﻿// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework © 2025 Ironcow Studio
// Distributed via Gumroad under a paid license
// 
// 🔐 This file is part of a licensed product. Redistribution or sharing is prohibited.
// 🔑 A valid license key is required to unlock all features.
// 
// 🌐 For license terms, support, or team licensing, visit:
//     https://ironcowstudio.duckdns.org/ironcowstudio.html
// ─────────────────────────────────────────────────────────────────────────────


using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum eUIPosition
{
    UI,
    Popup,
    GNB,
    Top,
}

namespace Ironcow.Synapse.UI
{
    public class UIManager : ManagerBase<UIManager>
    {
        [SerializeField] private List<Transform> parents;
        [SerializeField] private Transform worldParent;

        private List<UIBase> uiList = new List<UIBase>();

        public override async Task Init(UnityAction<string> progressTextCallback = null, UnityAction<float> progressValueCallback = null)
        {
            await base.Init(progressTextCallback, progressValueCallback);
            isInit = true;
        }

        public static void SetWorldCanvas(Transform worldCanvas)
        {
            instance.worldParent = worldCanvas;
        }

        public static void SetParents(List<Transform> parents)
        {
            instance.parents = parents;
            instance.uiList.Clear();
        }

        public static T Show<T>(params object[] param) where T : UIBase
        {
            var key = typeof(T).ToString();
            var ui = instance.uiList.FindLast(obj => obj.name == key);
            if (ui == null || ui.uiOptions.isMultiple)
            {
                var prefab = ResourceManager.instance.LoadAsset<T>(key, ResourceType.UI);
                ui = Instantiate(prefab, instance.parents[(int)prefab.uiPosition]);
                ui.name = key;
                instance.uiList.Add(ui);
            }
            if (ui.uiPosition == eUIPosition.UI && ui.uiOptions.isActiveOnLoad)
            {
                instance.uiList.ForEach(obj =>
                {
                    if (obj.uiPosition == eUIPosition.UI) obj.gameObject.SetActive(false);
                });
            }
            ui.AddOrGetComponent<Canvas>();
            ui.AddOrGetComponent<GraphicRaycaster>();
            ui.SetActive(ui.uiOptions.isActiveOnLoad);
            ui.opened?.Invoke(param);
            ui.uiOptions.isActiveOnLoad = true;
            return (T)ui;
        }

        public static void Hide<T>(params object[] param) where T : UIBase
        {
            var key = typeof(T).ToString();
            var ui = instance.uiList.FindLast(obj => obj.name == key);
            if (ui != null)
            {
                instance.uiList.Remove(ui);
                if (ui.uiPosition == eUIPosition.UI)
                {
                    var prevUI = instance.uiList.FindLast(obj => obj.uiPosition == eUIPosition.UI);
                    prevUI.SetActive(true);
                }
                ui.closed?.Invoke(param);
                if (ui.uiOptions.isDestroyOnHide)
                {
                    Destroy(ui.gameObject);
                }
                else
                {
                    ui.SetActive(false);
                }
            }
        }


        public static T Get<T>() where T : UIBase
        {
            var key = typeof(T).ToString();
            return (T)instance.uiList.Find(obj => obj.name == key);
        }

        public static bool IsOpened<T>() where T : UIBase
        {
            var key = typeof(T).ToString();
            var ui = instance.uiList.Find(obj => obj.name == key);
            return ui != null && ui.gameObject.activeInHierarchy;
        }

        public static bool IsOpened<T>(T opened) where T : UIBase
        {
            var ui = instance.uiList.Find(obj => obj.uiPosition == eUIPosition.UI && obj != opened);
            return ui != null;
        }

        public static void ShowIndicator()
        {

        }

        public static void HideIndicator()
        {

        }

        public static void ShowAlert(string desc, string title = "", string okBtn = "OK", UnityAction okCallback = null, string cancelBtn = "Cancel", UnityAction cancelCallback = null)
        {
            Show<PopupAlert>(desc, title, okBtn, cancelBtn, okCallback, cancelCallback);
        }


        public static void ShowAlert<T>(string desc, string title = "", string okBtn = "OK", string cancelBtn = "Cancel", UnityAction okCallback = null, UnityAction cancelCallback = null, T image = default)
        {
            Show<PopupAlert>(desc, title, okBtn, cancelBtn, okCallback, cancelCallback, image);
        }

        public static void ShowInputAlert(string desc, string title = "", UnityAction<string> okCallback = null, UnityAction cancelCallback = null, string okBtn = "", string cancelBtn = "")
        {
            Show<PopupAlert>(desc, title, okBtn, cancelBtn, okCallback, cancelCallback);
        }

        public static void HideAlert()
        {
            Hide<PopupAlert>();
        }
    }

}
