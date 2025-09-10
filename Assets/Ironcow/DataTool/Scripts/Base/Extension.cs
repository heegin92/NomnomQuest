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


using System.Collections.Generic;
using System.Linq;

using Ironcow.Synapse;

using UnityEngine;

namespace Ironcow.Synapse.Data
{
    public static class Extension
    {
        public static Vector3 ToVector3(this string str)
        {
            if (str[0] == '(' && str.Last() == ')')
            {
                var pos = str.Substring(1, str.Length - 2).Split(',');
                return new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
            }
            return Vector3.zero;
        }

        /// <summary>
        /// BaseDataSO 객체들의 리스트를 딕셔너리에 추가합니다.
        /// </summary>
        public static void AddRange<T>(this Dictionary<string, BaseDataSO> dic, List<T> datas) where T : BaseDataSO
        {
            foreach (var data in datas)
            {
                if (!dic.ContainsKey(data.rcode)) dic.Add(data.rcode, data);
            }
        }

        /// <summary>
        /// BaseDataSO 객체를 복제하여 반환합니다.
        /// </summary>
        public static T Clone<T>(this T data) where T : BaseDataSO
        {
            return (T)data.clone;
        }

    }
}
