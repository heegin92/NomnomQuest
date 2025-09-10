// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using UnityEditor;

using UnityEngine;

namespace Ironcow.Synapse
{
    public partial class ProjectSettingTool : ProjectSettingToolBase<ProjectSettingTool>
    {
        // "Synapse/Tool/Project Setting" 메뉴 항목을 통해 창을 여는 메뉴 아이템
        [MenuItem("Synapse/Tool/Synapse Control Center #&p")]
        public static void Open()
        {
            var window = GetWindow<ProjectSettingTool>();
            window.titleContent = new GUIContent("Synapse Control Center"); // 창 제목 설정
            window.minSize = new Vector2(512, 728f); // 최소 창 크기 설정
            window.maxSize = new Vector2(1280, 728f); // 최소 창 크기 설정
            instance = window;
        }

        public static void Open(int idx)
        {
            Open();
            if (idx != -1)
            {
                instance.targetId = idx; // 선택된 메뉴 항목 ID 설정
            }
        }

        public SettingMenu OnEnable_Common()
        {
            return new SettingMenu { id = 0, name = "Framework Controller", getScritables = Get_FrameworkController };
        }

        public ScriptableObject Get_FrameworkController()
        {
            return FrameworkController.instance;
        }


    }
}
