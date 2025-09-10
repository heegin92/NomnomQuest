// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using UnityEngine;

namespace Ironcow.Synapse
{
    public class MonoSingleton<T> : SynapseBehaviour where T : MonoSingleton<T>
    {
        // 싱글턴 인스턴스를 저장할 private 필드
        private static T _instance;

        // 싱글턴 인스턴스에 접근하는 프로퍼티
        public static T instance
        {
            get
            {
                // 인스턴스가 null일 경우 씬에서 찾고 없으면 새로 생성
                if (_instance == null)
                {
                    _instance = Object.FindFirstObjectByType<T>();
                    // 씬에서 찾을 수 없으면 새로운 GameObject에 컴포넌트로 추가
                    if (_instance == null)
                    {
                        _instance = new GameObject(typeof(T).Name).AddComponent<T>();
                    }
                }
                return _instance;
            }
            set
            {
                _instance = value;  // 외부에서 인스턴스를 설정할 수 있음
            }
        }

        // 현재 인스턴스가 존재하는지 여부를 확인하는 프로퍼티
        public static bool isInstance { get => _instance != null; }

        // 씬 전환 시 인스턴스를 유지할지 여부를 설정하는 변수
        [SerializeField] public bool isDontDestroy;

        // MonoBehaviour의 Awake 메서드 오버라이드
        protected virtual void Awake()
        {
            // 인스턴스를 this로 설정 (싱글턴 인스턴스를 초기화)
            instance = (T)this;

            // isDontDestroy가 true라면 이 객체를 씬 전환 시에도 파괴되지 않도록 설정
            if (isDontDestroy)
                DontDestroyOnLoad(this);
        }
    }
}
