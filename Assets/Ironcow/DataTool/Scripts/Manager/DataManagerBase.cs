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
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Events;

namespace Ironcow.Synapse.Data
{
    public class DataManagerBase<T, U> : ManagerBase<T> where T : DataManagerBase<T, U> where U : class
    {
        [SerializeField] public U userInfo;

        private Dictionary<string, BaseDataSO> dataDics = new Dictionary<string, BaseDataSO>();

        public override async Task Init(UnityAction<string> progressTextCallback = null, UnityAction<float> progressValueCallback = null)
        {
            await base.Init(progressTextCallback, progressValueCallback);
#if UNITY_EDITOR
            AddDataDics(ResourceManager.instance.LoadDataAssets<BaseDataSO>());
#else
            if (DataLoader.TryGetWrapper(out DataWrapper wrapper))
            {
                foreach (var data in wrapper.GetDatas())
                {
                    dataDics[data.rcode] = data;
                }
            }
#endif
            isInit = true;
        }

        public T GetData<T>(string rcode) where T : BaseDataSO
        {
            return (T)dataDics[rcode];
        }

        public T GetCloneData<T>(string rcode) where T : BaseDataSO
        {
            return (T)dataDics[rcode].clone;
        }

        public void AddDataDics<T>(List<T> datas) where T : BaseDataSO
        {
            dataDics.AddRange(datas);
        }

        public List<T> GetDatas<T>() where T : BaseDataSO
        {
            return dataDics.Values
                .Where(data => data is T)
                .Cast<T>()
                .ToList();
        }
    }
}
