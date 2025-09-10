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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Ironcow.Synapse;
using Ironcow.Synapse.BT;

using UnityEngine;

[Serializable]
public class BTRunner
{
    [SerializeReference] public RootNode root;
    private BTSaveData data;

#if UNITY_EDITOR
    public BTSaveData Data => data;
#endif

    public string lastData;

    public BTRunner()
    {
        if (root == null)
            root = new RootNode();
    }

    public BTRunner(string dataName)
    {
        SetRoot(dataName);
    }

    public void SetRoot(string dataName)
    {
        data = ResourceManager.instance.LoadAsset<BTSaveData>(dataName, ResourceType.Datas);
        if (data == null) return;
        root = JsonUtility.FromJson<RootNode>(data.data);
    }

    public BTRunner SetActions<T>(T parent) where T : Component
    {
        FindAction(root.node, parent);
        return this;
    }

    public void FindAction<T>(BTNode node, T parent) where T : Component
    {
        if (node is SelectorNode selectorNode)
        {
            foreach (var child in selectorNode.childs)
                FindAction(child, parent);
        }
        else if (node is SequenceNode sequenceNode)
        {
            foreach (var child in sequenceNode.childs)
                FindAction(child, parent);
        }
        else if (node is ActionNode actionNode)
        {
            var type = Type.GetType(actionNode.targetClassName)
                       ?? AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(a => a.GetTypes())
                            .FirstOrDefault(t => t.FullName == actionNode.targetClassName);

            if (type != null)
            {
                var instance = parent.AddOrGetComponent(type);
                var method = BTActionsBase.CachedMethods(type).FirstOrDefault(m => m.Name == actionNode.funcName);
                if (method != null)
                {
                    var contextParam = Expression.Parameter(typeof(DataContext), "context");

                    var call = Expression.Call(
                        Expression.Constant(instance), // this
                        method,
                        contextParam                   // ← 꼭 필요
                    );
                    var del = Expression.Lambda<Func<DataContext, eNodeState>>(call, contextParam).Compile();
                    actionNode.SetFunc(del);
                }
            }
        }
    }

    public BTNode AddSelector() => root.AddSelector();
    public BTNode AddAction(Func<DataContext, eNodeState> func) => root.AddAction(func);
    public BTNode AddSequence() => root.AddSequence();
    public void Operate() => root.Evaluate();
}
