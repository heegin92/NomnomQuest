// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using System;

using Ironcow.Synapse.Core;

using UnityEngine;

namespace Ironcow.Synapse.Sample.Common
{
    public class Enemy : SynapseBehaviour, IUpdatable
    {
        [SerializeField] private Rigidbody rb;
        [SerializeField] private float speed = 5f;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private CapsuleCollider col;
        int hp;
        public Action<Enemy> destroyCallback;
        public Vector3 TopPosition => transform.position + Vector3.up * meshRenderer.bounds.extents.y;

        public int Hp => hp;
        int atk;
        private void Awake()
        {
            meshRenderer.sharedMaterial = meshRenderer.material;
            this.hp = 10;
            atk = 1;
        }

        public void OnDamage(int damage)
        {
            hp -= damage;
            if(hp <= 0)
            {
                OnDead();
            }
        }

        public void OnDead()
        {
            destroyCallback?.Invoke(this);
            this.Release();
        }

        public void OnCollisionEnter(Collision collision)
        {
            if(collision.TryGetInstance<Player>(out var player))
            {
                player.OnDamage(atk);
            }
        }

        public void SetVisible(bool isVisible)
        {
            if (meshRenderer != null)
            {
                meshRenderer.enabled = isVisible;
                col.enabled = isVisible;
            }
        }

        public void OnUpdate()
        {
            
        }
    }
}
