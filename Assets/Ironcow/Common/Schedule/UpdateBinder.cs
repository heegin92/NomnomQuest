// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using System.Collections.Generic;

using Ironcow.Synapse;

using UnityEngine;

[AddComponentMenu("")]
[DisallowMultipleComponent]
public class UpdatableBinder : MonoBehaviour
{
    [SerializeField] private List<SynapseBehaviour> updatableTargets;
    private void OnValidate()
    {
        if (updatableTargets != null)
            updatableTargets.RemoveAll(obj => obj == null);
    }

    private void Awake()
    {
        foreach (var target in updatableTargets)
            ScheduleManager.instance.SubScribe(target);
    }

    public void SetTarget(SynapseBehaviour target)
    {
        if (updatableTargets == null)
            updatableTargets = new();

        if (!updatableTargets.Contains(target))
            updatableTargets.Add(target);
    }
}
