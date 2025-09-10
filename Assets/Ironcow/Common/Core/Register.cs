// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ironcow.Synapse.Core
{
    public partial class Register
    {
        public static bool isRunning;
        private static int intervalMilliseconds = 3000;
        private static Dictionary<int, SynapseBehaviour> objects = new();
        static Task cleaner;
        protected static Action<SynapseBehaviour> onAfterInstantiate;
        protected static Action<SynapseBehaviour> onAfterDestroy;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void StartAutoCleaner()
        {
            if (isRunning) return;
            isRunning = true;

            SceneManager.sceneLoaded += OnSceneLoaded;
            cleaner = Task.Run(async () =>
            {
                while (isRunning)
                {
                    PruneDestroyed();
                    await Task.Delay(intervalMilliseconds);
                }
            });
            RefreshInstances();
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            RefreshInstances();
        }

        public static void RefreshInstances()
        {
            if (objects == null)
                objects = new Dictionary<int, SynapseBehaviour>();

            var toRemove = objects.Where(kv => kv.Value == null).Select(kv => kv.Key).ToList();
            foreach (var id in toRemove)
            {
                objects.Remove(id);
            }

            var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var root in rootObjects)
            {
                var icbs = root.GetComponentsInChildren<SynapseBehaviour>(true);
                foreach (var icb in icbs)
                {
                    if (icb == null) continue;
                    if (!objects.TryAdd(icb.instanceId, icb))
                    {
                        onAfterInstantiate?.Invoke(icb);
                    }
                }
            }
        }

        public static void StopAutoCleaner()
        {
            isRunning = false;
            cleaner?.Dispose();
        }

        private static void PruneDestroyed()
        {
            var toRemove = new List<int>();
            foreach (var kv in objects)
            {
                if (kv.Value == null)
                    toRemove.Add(kv.Key);
            }

            foreach (var id in toRemove)
                Release(id);
        }

        public static T Instantiate<T>(T obj, Vector3 position, Quaternion rotation, Transform parent) where T : SynapseBehaviour
        {
            if (obj == null) return null;
            var inst = GameObject.Instantiate(obj, position, rotation, parent);
            objects.Add(inst.instanceId, inst);
            onAfterInstantiate?.Invoke(inst);
            return inst;
        }

        public static T Instantiate<T>(T obj, Transform parent, bool worldPositionStays) where T : SynapseBehaviour
        {
            if (obj == null) return null;
            var inst = GameObject.Instantiate(obj, parent, worldPositionStays);
            objects.Add(inst.instanceId, inst);
            onAfterInstantiate?.Invoke(inst);
            return inst;
        }

        public static T Instantiate<T>(T obj, Transform parent) where T : SynapseBehaviour
        {
            if (obj == null) return null;
            return Instantiate(obj, obj.transform.position, obj.transform.rotation, parent);
        }

        public static T Instantiate<T>(T obj) where T : SynapseBehaviour
        {
            if (obj == null) return null;
            return Instantiate(obj, obj.transform.position, obj.transform.rotation, null);
        }

        public static void Release(SynapseBehaviour obj, float t = 0)
        {
            Release(obj.instanceId);
#if USE_OBJECT_POOL
            if (obj is IPoolable poolBase)
            {
                PoolManager.instance.Release(obj);
            }
            else
#endif
            {
                Destroy(obj, t);
            }
        }

        public static void Release(GameObject gameObject, float t = 0)
        {
            var instanceId = gameObject.GetInstanceID();
            Release(instanceId);
            if(t == 0)
                Destroy(gameObject);
            else
                Destroy(gameObject, t);
        }

        public static void Release(int instanceId)
        {
            if (objects.TryGetValue(instanceId, out var inst))
            {
                inst.ReleaseUpdatable();
                onAfterDestroy?.Invoke(inst);
            }
            objects.Remove(instanceId);
        }

        public static void Destroy(UnityEngine.Object obj, float t = 0)
        {
            if(obj == null) return;
            if(t == 0)
                UnityEngine.Object.Destroy(obj);
            else
                UnityEngine.Object.Destroy(obj, t);
        }

        public static T GetInstance<T>(int intanceId) where T : SynapseBehaviour
        {
            objects.TryGetValue(intanceId, out var obj);
            return obj as T;
        }

        public static bool TryGetInstance<T>(int instanceId, out T obj) where T : SynapseBehaviour
        {
            obj = GetInstance<T>(instanceId);
            return obj != null;
        }

        public static T GetInterface<T>(int intanceId) where T : class
        {
            objects.TryGetValue(intanceId, out var obj);
            return obj as T;
        }

        public static bool TryGetInterface<T>(int instanceId, out T obj) where T : class
        {
            obj = GetInterface<T>(instanceId);
            return obj != null;
        }
    }
}
