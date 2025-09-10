// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using UnityEditor;

namespace Ironcow.Synapse
{
#if UNITY_EDITOR
    // UNITY_EDITOR에서만 실행되도록 하여, Unity 에디터에서 초기화할 수 있도록 설정
    [InitializeOnLoad]
#endif
    // FrameworkController는 SOSingleton을 상속받아 ScriptableObject 기반의 싱글턴 클래스입니다.
    public class FrameworkController : EditorSOSingleton<FrameworkController>
    {
        // 2D 모드 여부를 나타내는 변수
        public bool is2d;

        // 3D 모드 여부를 나타내는 변수
        public bool is3d;

        // Update 및 코루틴을 하나의 매니저에서 관리할지를 나타내는 변수
        public bool isUpdatable;

        // 로케일(지역 설정) 사용 여부를 나타내는 변수
        public bool isLocale;

        // 자동 캐싱 사용 여부를 나타내는 변수
        public bool isAutoCaching;

        // ScriptableObject 데이터를 사용하는지 여부를 나타내는 변수
        public bool isScriptableObjectData;

        // Addressable 시스템에서 비동기 처리 여부를 나타내는 변수
        public bool isAddressableAsync;

        // 오브젝트 풀(Object Pool) 사용 여부를 나타내는 변수
        public bool isObjectPool;

        // Cloud Code 사용 여부를 나타내는 변수
        public bool isCloudCode;

        // FSM(Finite State Machine) 사용 여부를 나타내는 변수
        public bool isFSM;

        // 전략 머신 사용 여부
        public bool isStrategy;

        // 최적화 엔진 사용 여부
        public bool isIroncowLifecycle;

        // 오딘 사용 시 오딘 적용 여부
        public bool isOdin;

        // Mvvm 모듈 사용 여부
        public bool isMvvm;
    }
}
