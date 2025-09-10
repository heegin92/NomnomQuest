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
using System.Collections;
using System.Collections.Generic;

using Ironcow.Synapse;

using UnityEngine;

namespace Ironcow.Synapse.UI
{
    public abstract class UIListBase<T> : UIBase, IUIList<T> where T : SynapseBehaviour
    {
        [SerializeField] protected Transform listParent;
        [SerializeField] protected T itemPrefab;
        protected List<T> items = new List<T>();

        protected Queue<T> pooledItems = new();

        public T AddItem()
        {
            T item;
            if (pooledItems.Count > 0)
            {
                item = pooledItems.Dequeue();
                item.gameObject.SetActive(true);
            }
            else
            {
                item = Instantiate(itemPrefab, listParent);
            }

            items.Add(item);
            return item;
        }

        public void ClearList()
        {
            foreach (var obj in items)
            {
                obj.gameObject.SetActive(false);
                pooledItems.Enqueue(obj);
            }
            items.Clear();
        }

        public abstract void SetList();
    }
}
