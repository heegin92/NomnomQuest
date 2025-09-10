// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using System.Collections.Generic;

using UnityEngine;

namespace Ironcow.Synapse.Sample.Common
{
    public class Spawner : MonoSingleton<Spawner>
#if USE_UPDATABLE
        , IUpdatable
#endif
    {
        [SerializeField] float spawnTiming = 0.5f;
        [SerializeField] private int createCount = 1;
        float spawnTimer = 0f;
        [HideInInspector] public List<Enemy> enemies = new();
        protected override async void Awake()
        {
            ResourceManager.instance.InitAddressableMap();
            await ScheduleManager.instance.Init();
            await ResourceManager.instance.Init();
        }

#if USE_UPDATABLE
    public void OnUpdate()
#else
    void Update()
#endif
        {
            if (!ResourceManager.instance.isInit) return;
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnTiming)
            {
                spawnTimer = 0;
                Spawn();
            }
        }

        private void Spawn()
        {
            for (int i = 0; i < createCount; i++)
            {
                var prefab = ResourceManager.instance.InstantiateAsset<Enemy>("Enemy", ResourceType.Prefabs);
                if (prefab != null)
                {
                    prefab.transform.position = new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
                    prefab.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                    prefab.SetActive(true);
                    enemies.Add(prefab);
                }
                else
                {
                    Debug.LogError("Prefab not found!");
                }
            }
        }
    }
}
