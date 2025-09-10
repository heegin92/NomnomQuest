// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using System.Collections.Generic;
using System.Threading.Tasks;

using Ironcow.Synapse.Core;

using UnityEngine;
using UnityEngine.Events;
#if USE_ADDRESSABLE
using Ironcow.Synapse.Addressable;
#endif

namespace Ironcow.Synapse.Resource
{
    public class ResourceManagerBase<T> : ManagerBase<T> where T : ResourceManagerBase<T>
    {
        public bool isAutoLoading = false;

#if !USE_ADDRESSABLE
        private ResourcesHandler handler = new ResourcesHandler();
#elif USE_ADDRESSABLE
        private AddressableHandler handler = new AddressableHandler();
#endif

        public async override Task Init(UnityAction<string> progressTextCallback = null, UnityAction<float> progressValueCallback = null)
        {
            await base.Init(progressTextCallback, progressValueCallback);
#if USE_ADDRESSABLE
            await handler.LoadAddressable(progressTextCallback, progressValueCallback);
#endif
            isInit = true;
        }

        public void InitAddressableMap()
        {
#if USE_ADDRESSABLE
            handler.InitAddressableMap();
#endif
        }

        public V LoadAsset<V>(string key, string type) where V : UnityEngine.Object
        {
            var asset = handler.LoadAsset<V>(key, type);
            return asset;
        }

        public V LoadAsset<V>(string key, params string[] type) where V : UnityEngine.Object
        {
            var asset = handler.LoadAsset<V>(key, type);
            return asset;
        }

        public V InstantiateAsset<V>(string key, string type, Transform parent = null) where V : SynapseBehaviour
        {
            var asset = Register.Instantiate(LoadAsset<V>(key, type), parent);
            return asset;
        }

        public V InstantiateAsset<V>(string type, Transform parent = null) where V : SynapseBehaviour
        {
            var asset = Register.Instantiate(LoadAsset<V>(typeof(V).ToString(), type), parent);
            return asset;
        }

        public List<V> LoadAssets<V>(string key, string type) where V : UnityEngine.Object
        {
            var assets = handler.LoadAssets<V>(key, type);
            return assets;
        }

        public List<V> LoadDataAssets<V>() where V : UnityEngine.Object
        {
            return handler.LoadDataAssets<V>();
        }

        public V LoadThumbnail<V>(string key) where V : UnityEngine.Object
        {
            return handler.LoadAsset<V>(key, ResourceType.Thumbnail);
        }

        public V LoadUI<V>(string key) where V : UnityEngine.Object
        {
            return handler.LoadAsset<V>(key, ResourceType.UI);
        }
    }
}
