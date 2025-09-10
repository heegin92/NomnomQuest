// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Ironcow.Synapse.Resource
{
    public class ResourcesHandler
    {
        public Dictionary<string, object> assetPools = new Dictionary<string, object>();

        public void Init()
        {

        }

        public T LoadAsset<T>(string key, params string[] type) where T : UnityEngine.Object
        {
            var path = Path.Combine(type);
            var poolKey = Path.Combine(path, key);
            if (assetPools.TryGetValue(poolKey, out var obj)) return (T)obj;
            var asset = Resources.Load<T>(poolKey);
            if (asset != null) assetPools.Add(poolKey, asset);
            return asset;
        }

        public List<T> LoadAssets<T>(string key, string type) where T : UnityEngine.Object
        {
            var path = type.ToString() + "/" + key;
            List<T> retList = new List<T>();
            var keys = new List<string>(assetPools.Keys);
            foreach (var val in keys)
            {
                if (val.Contains(path))
                {
                    retList.Add((T)assetPools[val]);
                }
            }
            if (retList.Count == 0)
            {
                retList = Resources.LoadAll<T>(path).ToList();
                var filename = key.FileName().ToUpper();
                foreach (var item in retList)
                {
                    if (!assetPools.ContainsKey(item.name))
                        assetPools.Add(path + "/" + item.name, item);
                }
            }
            return retList;
        }

        public List<T> LoadDataAssets<T>() where T : UnityEngine.Object
        {
            return LoadAssets<T>("", ResourceType.Datas);
        }

        public T LoadThumbnail<T>(string key) where T : UnityEngine.Object
        {
            return LoadAsset<T>(key, ResourceType.Thumbnail);
        }

        public T LoadUI<T>(string key) where T : UnityEngine.Object
        {
            return LoadAsset<T>(key, ResourceType.UI);
        }
    }
}
