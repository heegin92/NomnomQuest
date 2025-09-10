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
using System.Linq;
using System.Reflection;

using UnityEngine;

namespace Ironcow.Synapse.BT
{
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    [BTActions]
    public class BTActionsBase : SynapseBehaviour
    {
        private static string groupPath = "Unknown";
        private static List<MethodInfo> cachedMethods = new List<MethodInfo>();
        public static string GroupPath
        {
            get
            {
                if (groupPath.Equals("Unknown"))
                {
                    var stackTrace = new System.Diagnostics.StackTrace();
                    for (int i = 1; i < stackTrace.FrameCount; i++)
                    {
                        var method = stackTrace.GetFrame(i).GetMethod();
                        var declaringType = method?.DeclaringType;

                        if (declaringType != null && declaringType.IsSubclassOf(typeof(BTActionsBase)))
                        {
                            groupPath = BTGroupUtil.GetGroupPath(declaringType);
                        }
                    }
                }
                return groupPath;
            }
        }

        public static IReadOnlyList<MethodInfo> CachedMethods<T>()
        {
            if (cachedMethods.Count == 0)
            {
                var methods = typeof(T).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
                cachedMethods = methods
                    .Where(m =>
                    {
                        var t = m.ReturnType;
                        return t == typeof(eNodeState);
                    })
                    .ToList();
            }
            return cachedMethods;
        }

        public static IReadOnlyList<MethodInfo> CachedMethods(Type type)
        {
            var methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            cachedMethods = methods
                .Where(m =>
                {
                    var t = m.ReturnType;
                    return t == typeof(eNodeState);
                })
                .ToList();
            return cachedMethods;
        }


        public eNodeState __Sample()
        {
            return eNodeState.failure;
        }
    }
}
