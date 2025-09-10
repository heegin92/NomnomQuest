// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using System;
using System.IO;
using UnityEditor;

using UnityEngine;

namespace Ironcow.Synapse
{
    // UNITY_EDITOR에서만 실행되도록 하여, Unity 에디터에서 초기화할 수 있도록 설정
    [InitializeOnLoad]
    public class EditorSOSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        // 싱글턴 인스턴스를 저장할 private static 필드
        static private T _instance = null;

        // 싱글턴 인스턴스에 접근하는 public static 프로퍼티
        static public T instance
        {
            get
            {
                // 인스턴스가 null일 경우 로딩하여 반환
                if (_instance == null)
                {
                    var name = typeof(T).Name;
                    // Resources 폴더에서 이름을 기준으로 로드
                    _instance = Resources.Load<T>(name);
                    if (_instance == null)
                    {
                        // 에디터에서만 실행되는 코드
                        // EditorDataSetting.SettingSOPath 경로에서 에셋 로드
                        _instance = AssetDatabase.LoadAssetAtPath<T>(Path.Combine(EditorDataSetting.SettingSOPath, name + ".asset"));
                        if (_instance == null)
                        {
                            // 에셋이 없다면 새로운 인스턴스를 생성
                            _instance = CreateInstance<T>();

                            // 해당 경로에 디렉토리가 없다면 생성
                            string directory = Application.dataPath.Replace("Assets", EditorDataSetting.SettingSOPath);
                            if (!System.IO.Directory.Exists(directory))
                            {
                                System.IO.Directory.CreateDirectory(directory);
                                AssetDatabase.Refresh();  // 에셋 데이터베이스 새로 고침
                            }

                            // 새로 생성한 인스턴스를 에셋으로 저장
                            string assetPath = $"{EditorDataSetting.SettingSOPath}/{name}.asset";
                            AssetDatabase.CreateAsset(instance, assetPath);
                        }
                    }
                }

                return _instance;
            }
        }

        public float pstWidth;

        // 에디터에서 인스턴스를 선택하도록 하는 함수
        private static void Edit()
        {
            Selection.activeObject = instance;
        }

        // 데이터를 수정한 후 에디터에서 변경 사항을 저장하도록 하는 함수
        public void SaveData()
        {
            EditorUtility.SetDirty(_instance);
        }

        public static void Release()
        {
            _instance = null;
            ReleaseCallback?.Invoke();
        }

        public void ReleaseSelf()
        {
            Release();
        }

        public static Action ReleaseCallback;
        public static bool IsInstance => _instance != null;
    }
}
