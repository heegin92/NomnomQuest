// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using Ironcow.Synapse;
using Ironcow.Synapse.Core;

using UnityEngine;

#if UNITY_EDITOR
[ScriptTemplate("Custom/SynapseBehaviour")]
#endif
public partial class SynapseBehaviour : SynapseBase
{
    public int Priority { get; private set; } = 10;
    public int priority
    {
        get => Priority;
        set
        {
            if (Priority != value)
            {
                Priority = value;
                ScheduleManager.instance.SetSort(this);
            }
        }
    }

    public int instanceId => gameObject.GetInstanceID();
    public bool IsActive => this != null && gameObject != null && gameObject.activeInHierarchy;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        CheckUpdatable();
    }
#endif

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public void CheckUpdatable()
    {
#if USE_UPDATABLE
        if (!Application.isPlaying &&
        (this is IUpdatable || this is IFixedUpdatable || this is ILateUpdatable))
        {
            var binder = this.AddOrGetComponent<UpdatableBinder>();
            binder.hideFlags = HideFlags.HideInInspector;
            binder.SetTarget(this);
        }
#endif
    }

    public void ReleaseUpdatable()
    {
#if USE_UPDATABLE
        if (!Application.isPlaying &&
        (this is IUpdatable || this is IFixedUpdatable || this is ILateUpdatable))
        {
            ScheduleManager.instance.UnSubScribe(this);
        }
#endif
    }

    protected virtual void OnDestroy()
    {
        if (Application.isPlaying)
            Register.Release(instanceId);
    }
}
