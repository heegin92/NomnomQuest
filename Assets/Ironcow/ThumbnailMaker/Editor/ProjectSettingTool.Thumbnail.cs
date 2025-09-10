// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using System;
using System.Reflection;

using Ironcow.Synapse.Thumbnail;

using UnityEditor;

using UnityEngine;

namespace Ironcow.Synapse
{
    public partial class ProjectSettingTool
    {
        public const int THUMBNAIL_MAKER = 13;

        [MenuItem("Synapse/Thumbnail Maker/Open Locked Inspector For Selection #&t")]
        public static void OpenNewInspector()
        {
            // 내부 타입 UnityEditor.InspectorWindow 찾기
            var inspType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
            if (inspType == null)
            {
                Debug.LogWarning("InspectorWindow type not found.");
                return;
            }

            // 새 인스펙터 창 생성 및 표시
            var window = ScriptableObject.CreateInstance(inspType) as EditorWindow;
            window.Show();                         // 유사 팝업 / 독립창
            window.Focus();
            window.titleContent = new GUIContent("ThumbnailMaker");

            EditorApplication.update += CheckWindow;

            void CheckWindow()
            {
                try
                {
                    if (window == null)
                    {
                        ThumbnailMaker.Release();
                    }
                }
                finally
                {
                    if (window == null)
                        EditorApplication.update -= CheckWindow;
                }
            }
            EditorApplication.delayCall += () =>
            {
                // 고정하고 싶은 대상을 먼저 선택
                if (ThumbnailMaker.instance != null)
                {
                    Selection.activeObject = ThumbnailMaker.instance;
                    // 선택 반영을 위해 한 프레임 양보 (즉시 반영이 필요한 경우 Repaint 호출)
                    window.Repaint();
                }

                // isLocked 프로퍼티 설정 (Unity 버전에 따라 NonPublic일 수 있어 BindingFlags 모두 시도)
                var prop = inspType.GetProperty("isLocked",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(window, true, null);     // 🔒 잠금!
                }
                else
                {
                    Debug.LogWarning("Could not set isLocked on InspectorWindow.");
                }
            };
        }
        public SettingMenu OnEnable_Thumbnail()
        {
            return new SettingMenu() { id = THUMBNAIL_MAKER, name = "ThumbnailMaker", isVisible = () => true, getScritables = GetThumbnailMakerSO };
        }

        public ScriptableObject GetThumbnailMakerSO()
        {
            return Thumbnail.ThumbnailMaker.instance;
        }

        public void OnDestroy_Thumbnail()
        {
            Thumbnail.ThumbnailMaker.Release();
        }
    }
}
