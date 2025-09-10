// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using System.Collections.Generic;

using UnityEngine;

using Ironcow.Synapse;

using UnityEngine.Events;

using System.Threading.Tasks;
using System.Linq;

#if USE_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace Ironcow.Synapse
{
    // Initializer 클래스는 MonoSingleton을 상속받아 싱글턴 패턴을 사용하여 관리되는 초기화 담당 클래스입니다.
    public class Initializer : MonoSingleton<Initializer>
    {
        // 초기화할 매니저들을 담을 리스트
        [SerializeField] private List<IManagerInit> managerInits = new();

        // MonoBehaviour에서 Awake 함수 오버라이드
        protected override void Awake()
        {
            base.Awake();  // 기본 Awake 호출
            CollectManagersIfNeeded();  // 매니저 리스트가 비어있다면 자동으로 매니저들을 수집
        }

        // 필요할 경우 매니저 리스트를 수집하는 함수
        private void CollectManagersIfNeeded()
        {
            // managerInits 리스트가 비어 있으면, 씬에 있는 모든 IManagerInit 타입의 객체를 찾음
            if (managerInits == null || managerInits.Count == 0)
            {
                managerInits = FindObjectsOfType<MonoBehaviour>().OfType<IManagerInit>().ToList();
            }
        }

#if UNITY_EDITOR
        // Unity 에디터에서 매니저 리스트를 자동으로 업데이트하는 함수
        protected override void OnValidate()
        {
            // 씬에서 IManagerInit 타입의 객체들을 찾음
            var found = FindObjectsOfType<MonoBehaviour>().OfType<IManagerInit>().ToList();

            // 찾은 객체들이 managerInits에 없으면 추가
            foreach (var m in found)
            {
                if (!managerInits.Contains(m))
                    managerInits.Add(m);
            }

            // managerInits 리스트에서 null인 객체들을 제거
            managerInits.RemoveAll(m => m == null);
        }
#endif

        // 모든 매니저를 초기화하는 비동기 함수
        public async Task InitAllManagers(UnityAction<string> progressTextCallback = null, UnityAction<float> progressValueCallback = null)
        {
            // managerInits가 비어있다면 매니저를 수집
            if (managerInits == null || managerInits.Count == 0)
                CollectManagersIfNeeded();

            int count = managerInits.Count;  // 초기화할 매니저의 개수
            for (int i = 0; i < count; i++)
            {
                var manager = managerInits[i];  // 현재 초기화할 매니저
                string managerName = manager.GetType().Name;  // 매니저의 타입 이름

                // 진행 상태 텍스트 콜백을 사용해 현재 초기화 중인 매니저 이름 표시
                progressTextCallback?.Invoke($"{managerName} 초기화 중...");

                // 매니저의 Init 함수 호출 (비동기)
                await manager.Init(progressTextCallback, progressValueCallback);

                // 진행 상태 값 콜백을 사용해 진행률 업데이트
                progressValueCallback?.Invoke((float)(i + 1) / count);
            }

            // 모든 초기화가 끝난 후 완료 메시지 표시
            progressTextCallback?.Invoke("모든 매니저 초기화 완료!");
            progressValueCallback?.Invoke(1f);  // 진행률을 100%로 설정
        }
    }
}
