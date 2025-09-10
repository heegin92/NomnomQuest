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


using Ironcow.Synapse.Data;

using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Ironcow.Synapse.Data
{
    public partial class DataWrapper
    {
        public List<BaseDataSO> GetDatas()
        {
            var FlattenedList = new List<BaseDataSO>();

            var fields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var field in fields)
            {
                if (!field.FieldType.IsGenericType) continue;

                var genericType = field.FieldType.GetGenericTypeDefinition();
                if (genericType != typeof(List<>)) continue;

                var elementType = field.FieldType.GetGenericArguments()[0];
                if (!typeof(BaseDataSO).IsAssignableFrom(elementType)) continue;

                var listObj = field.GetValue(this) as IEnumerable;
                if (listObj == null) continue;

                foreach (var item in listObj)
                {
                    if (item is BaseDataSO baseData && !string.IsNullOrEmpty(baseData.rcode))
                        FlattenedList.Add(baseData);
                }
            }
            return FlattenedList;
        }
    }
}
