// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using System.Threading.Tasks;

using UnityEngine.Events;

namespace Ironcow.Synapse
{
    // ManagerBase<T> 클래스는 MonoSingleton을 상속받아 싱글턴 패턴을 적용하고,
    // IManagerInit 인터페이스를 구현하여 매니저 초기화 기능을 제공합니다.
    public abstract class ManagerBase<T> : MonoSingleton<T>, IManagerInit where T : ManagerBase<T>
    {
        // 매니저의 초기화 상태를 나타내는 속성
        public bool isInit { get; protected set; }

        // 초기화 함수, 비동기로 실행되며 진행 상태를 나타낼 수 있는 콜백을 받음
        // 기본 구현은 완료된 Task를 반환하는 기본 함수입니다.
        public async virtual Task Init(UnityAction<string> progressTextCallback = null, UnityAction<float> progressValueCallback = null)
        {
            await Task.CompletedTask;  // 기본 구현은 아무 작업 없이 완료된 Task를 반환
        }
    }
}
