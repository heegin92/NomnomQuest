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


using System.Reflection;
using System;
#if UNITY_EDITOR
using UnityEditor.Events;
#endif
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

namespace Ironcow.Synapse.UI
{
    public abstract partial class UIBase
    {
        partial void AutoBindButtons()
        {
#if UNITY_EDITOR
            var type = GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                if (field.FieldType != typeof(Button)) continue;
                if (!field.Name.StartsWith("Button")) continue;

                var button = field.GetValue(this) as Button;
                if (button == null) continue;

                string key = field.Name.Substring("Button".Length); // ex: "Select"

                MethodInfo method = null;

                // ✅ ButtonClose → HideDirect 예외 처리
                if (key == "Close")
                {
                    method = type.GetMethod("HideDirect", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                }
                else
                {
                    var methodName = $"OnClick{key}";
                    method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                }

                if (method != null && method.GetParameters().Length == 0)
                {
                    bool isContain = false;
                    for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
                    {
                        if (button.onClick.GetPersistentMethodName(i) == method.Name)
                        {
                            isContain = true;
                            break;
                        }
                    }
                    if (isContain) continue;
                    UnityEventTools.AddPersistentListener(button.onClick, Delegate.CreateDelegate(typeof(UnityAction), this, method) as UnityAction);
                    //button.onClick.AddListener(() => method.Invoke(this, null));
                    Debug.Log($"[AutoBind] {field.Name} → {method.Name}()");
                }
            }
#endif
        }

    }
}
