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


using UnityEngine;

namespace Ironcow.Synapse.Data
{
    public static class DataLoader
    {
        public static bool TryGetWrapper(out DataWrapper wrapper)
        {
            Debug.Log("Loading and decrypting EncryptedData.bytes...");
            wrapper = null;

            var textAsset = ResourceManager.instance.LoadAsset<TextAsset>("EncryptedData", ResourceType.Datas);
            if (textAsset == null)
            {
                Debug.LogError("EncryptedData.bytes not found");
                return false;
            }

            string json = AesEncryptor.Decrypt(textAsset.bytes);

            wrapper = JsonUtility.FromJson<DataWrapper>(json);
            if (wrapper == null)
            {
                Debug.LogError("Failed to deserialize DataWrapper");
                return false;
            }

            Debug.Log("DataManager initialized with decrypted data");
            return true;
        }
    }
}
