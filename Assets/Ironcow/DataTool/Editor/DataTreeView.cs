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
using System.Collections.Generic;
using System;
using UnityEditor.IMGUI.Controls;
using UnityEditor;
using System.IO;
using UnityEngine;

namespace Ironcow.Synapse.Data
{
    public class DataTreeView : TreeView
    {
        private DataTreeMapSO map;
        private Dictionary<int, BaseDataSO> idToAsset = new();
        private Action<BaseDataSO> onSelect;
        public Rect rect => treeViewRect;
        public DataTreeView(TreeViewState state, DataTreeMapSO map, Action<BaseDataSO> onSelect) : base(state)
        {
            this.map = map;
            InitMap();
            this.onSelect = onSelect;
            Reload();
        }

        public void InitMap()
        {
            map.groups.Clear();
            foreach(var sheet in DataToolSetting.instance.sheets)
            {
                if(!map.groups.Contains(sheet))
                {
                    map.groups.Add(sheet);
                }
            }
            var targetPath = DataToolSetting.DataScriptableObjectFullPath;
            var files = Directory.GetFiles(targetPath).ToList();
            files.RemoveAll(obj => obj.Contains(".meta"));
            foreach(var file in files)
            {
                var data = AssetDatabase.LoadAssetAtPath<BaseDataSO>(file.Replace(Application.dataPath, "Assets"));
                var sheet = map.groups.Find(obj => data.rcode.Contains(obj.key));
                if(sheet != null)
                {
                    if(!sheet.items.Contains(data))
                    {
                        sheet.items.Add(data);
                    }
                }
            }
        }

        protected override TreeViewItem BuildRoot()
        {
            int id = 0;
            var root = new TreeViewItem { id = id++, depth = -1, displayName = "Root" };
            if (map.groups == null) return root;
            foreach (var sheet in map.groups)
            {
                var parent = new TreeViewItem(id++, 0, sheet.className);
                root.AddChild(parent);

                foreach (var obj in sheet.items)
                {
                    if (obj is not BaseDataSO so) continue;
                    var child = new TreeViewItem(id++, 1, so.rcode);
                    idToAsset[child.id] = so;
                    parent.AddChild(child);
                }
            }
            if (!root.hasChildren)
            {
                root.AddChild(new TreeViewItem(0, 0, "Empty"));
            }

            SetupDepthsFromParentsAndChildren(root);
            return root;
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            if (selectedIds.Count > 0 && idToAsset.TryGetValue(selectedIds[0], out var asset))
            {
                onSelect?.Invoke(asset);
            }
        }
    }
}
