// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Ironcow.Synapse;

using UnityEngine;
using UnityEngine.Events;

namespace Ironcow.Synapse
{
    public partial class ScheduleManager : ManagerBase<ScheduleManager>
    {
        public HashSet<IUpdatableBase> removeSet = new();
        public HashSet<IUpdatableBase> removeFixedSet = new();
        public HashSet<IUpdatableBase> addSet = new();
        public HashSet<IUpdatableBase> addFixedSet = new();
        List<IUpdatableBase> toRemove = new();
        public int maxRemoveCount = 20;
        private int frameSampleCount = 0;
        private float accumulatedFrameTime = 0f;
        private int[] frameTargetCounts = new int[3] { 5, 20, 100 };

        public async override Task Init(UnityAction<string> progressTextCallback = null, UnityAction<float> progressValueCallback = null)
        {
            isInit = true;
            Task.Run(async () =>
            {
                while (isInit)
                {
                    await Task.Delay(10000); // Simulate some initialization work
                    foreach (var u in updateList)
                    {
                        if (u == null)
                        {
                            removeSet.Add(u);
                        }
                    }
                    foreach (var f in fixedList)
                    {
                        if (f == null)
                            removeFixedSet.Add(f);
                    }
                    foreach (var l in lateList)
                    {
                        if (l == null)
                            removeSet.Add(l);
                    }
                }
            }).Forget();


        }

        private List<IUpdatable> updateList = new();
        private List<IFixedUpdatable> fixedList = new();
        private List<ILateUpdatable> lateList = new();
        private Dictionary<string, Coroutine> coroutineDict = new();

        public void SubScribe(object obj)
        {
            var priority = 10;
            if (obj is IUpdatable u)
            {
                addSet.Add(u);
                priority = u.Priority;
            }
            if (obj is IFixedUpdatable f)
            {
                addFixedSet.Add(f);
                priority = f.Priority;
            }
            if (obj is ILateUpdatable l)
            {
                addSet.Add(l);
                priority = l.Priority;
            }
        }

        public void SetSort(object obj)
        {
            if (obj is IUpdatable)
                updateList.Sort((a, b) => GetPriority(a).CompareTo(GetPriority(b)));
            if (obj is IFixedUpdatable)
                fixedList.Sort((a, b) => GetPriority(a).CompareTo(GetPriority(b)));
            if (obj is ILateUpdatable)
                lateList.Sort((a, b) => GetPriority(a).CompareTo(GetPriority(b)));
        }

        public void ReOrder()
        {
            updateList.Sort((a, b) => GetPriority(a).CompareTo(GetPriority(b)));
            lateList.Sort((a, b) => GetPriority(a).CompareTo(GetPriority(b)));
        }
        public void ReOrderFixedUpdate()
        {
            fixedList.Sort((a, b) => GetPriority(a).CompareTo(GetPriority(b)));
        }

        private bool IsUnityNull(object obj)
        {
            if (obj == null) return true;
            if (obj is Object unityObj)
                return unityObj == null;
            return false;
        }

        private int GetPriority(IUpdatableBase obj)
        {
            if (IsUnityNull(obj)) return 500;
            if (!obj.IsActive) return 100;
            return obj.Priority;
        }

        public void UnSubScribe(object obj)
        {
            if (obj is IUpdatableBase updatable)
            {
                if (obj is IFixedUpdatable)
                    removeFixedSet.Add(updatable);
                else
                    removeSet.Add(updatable);
            }
        }

        public void StartRoutine(IEnumerator routine, [CallerMemberName] string key = "")
        {
            if (coroutineDict.ContainsKey(key))
                StopCoroutine(coroutineDict[key]);

            coroutineDict[key] = StartCoroutine(routine);
        }

        public void StopRoutine(IEnumerator routine, [CallerMemberName] string key = "")
        {
            if (coroutineDict.TryGetValue(key, out var co))
            {
                StopCoroutine(co);
                coroutineDict.Remove(key);
            }
        }

        public void StopAllRoutines()
        {
            foreach (var co in coroutineDict.Values)
                StopCoroutine(co);
            coroutineDict.Clear();
        }

        private void RunRemoveLists()
        {
            DynamicAdjustMaxRemoveCount();
            int removedCount = 0;

            toRemove.Clear();
            foreach (var obj in removeSet)
            {
                if (removedCount >= maxRemoveCount)
                    break;

                if (obj is IUpdatable u) updateList.Add(u);
                if (obj is IFixedUpdatable f) fixedList.Add(f);
                if (obj is ILateUpdatable l) lateList.Add(l);

                toRemove.Add(obj);
                removedCount++;
            }

            // 제거된 만큼 HashSet에서 삭제
            // (HashSet은 삭제가 위험하니까, 복사본으로 따로 저장 후 제거)
            if (removedCount > 0)
            {
                foreach (var obj in toRemove)
                    removeSet.Remove(obj);
            }
        }

        private void RunRemoveFixedLists()
        {
            DynamicAdjustMaxRemoveCount();
            int removedCount = 0;

            toRemove.Clear();
            foreach (var obj in removeFixedSet)
            {
                if (removedCount >= maxRemoveCount)
                    break;

                if (obj is IFixedUpdatable f) fixedList.Remove(f);

                toRemove.Add(obj);
                removedCount++;
            }

            // 제거된 만큼 HashSet에서 삭제
            // (HashSet은 삭제가 위험하니까, 복사본으로 따로 저장 후 제거)
            if (removedCount > 0)
            {
                foreach (var obj in toRemove)
                    removeFixedSet.Remove(obj);
            }
        }

        private void RunAddLists()
        {
            foreach (var obj in addSet)
            {
                if (obj is IUpdatable u) updateList.Add(u);
                if (obj is ILateUpdatable l) lateList.Add(l);
            }
            addSet.Clear();
            ReOrder();
        }

        private void RunAddFixedLists()
        {
            foreach (var obj in addFixedSet)
            {
                if (obj is IFixedUpdatable f) fixedList.Add(f);
            }
            addFixedSet.Clear();
            ReOrderFixedUpdate();
        }

        public void RunUpdate()
        {
            RunRemoveLists();
            RunAddLists();
#if USE_IRONCOW_CORE
            foreach (var u in updateList)
            {
                if (u == null || !u.IsActive) break;
                u.OnUpdate();
            }
#else
            updateList.RemoveAll(obj => obj == null);
            foreach (var u in updateList)
            {
                if(u != null && u.IsActive)
                u.OnUpdate();
            }
#endif
        }

        public void RunFixedUpdate()
        {
            RunRemoveFixedLists();
            RunAddFixedLists();
#if USE_IRONCOW_CORE
            foreach (var f in fixedList)
            {
                if (f == null || !f.IsActive) break;
                f.OnFixedUpdate();
            }
#else
            fixedList.RemoveAll(obj => obj == null);
            foreach (var f in fixedList)
            {
                if(f != null && f.IsActive)
                f.OnFixedUpdate();
            }
#endif
        }

        public void RunLateUpdate()
        {
#if USE_IRONCOW_CORE
            foreach (var l in lateList)
            {
                if (l == null || !l.IsActive) break;
                l.OnLateUpdate();
            }
#else
            lateList.RemoveAll(obj => obj == null);
            foreach (var l in lateList)
            {
                if(l != null && l.IsActive)
                l.OnLateUpdate();
            }
#endif
        }

        private void DynamicAdjustMaxRemoveCount()
        {
            frameSampleCount++;
            accumulatedFrameTime += Time.deltaTime;

            if (frameSampleCount >= 60) // 매 60프레임마다
            {
                float avgFrameTime = accumulatedFrameTime / frameSampleCount;
                float targetFrameTime = 1f / (Application.targetFrameRate > 0 ? Application.targetFrameRate : 60f);

                if (avgFrameTime > targetFrameTime * 1.2f) // 프레임이 20% 이상 느리면
                {
                    maxRemoveCount = frameTargetCounts[0];
                }
                else if (avgFrameTime < targetFrameTime * 0.8f) // 프레임 여유 있으면
                {
                    maxRemoveCount = frameTargetCounts[2];
                }
                else
                {
                    maxRemoveCount = frameTargetCounts[1];
                }

                frameSampleCount = 0;
                accumulatedFrameTime = 0f;
            }
        }
    }

}
