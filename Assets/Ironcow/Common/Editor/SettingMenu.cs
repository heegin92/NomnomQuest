// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using System;

using UnityEngine;

namespace Ironcow.Synapse
{
    // 각 메뉴 항목에 대한 설정 정보를 담고 있는 클래스
    public class SettingMenu
    {
        public int id; // 메뉴 항목 ID
        public string name; // 메뉴 항목 이름
        public Func<bool> isVisible; // 메뉴 항목이 보일지 여부를 결정하는 함수
        public Func<ScriptableObject> getScritables; // 메뉴 항목에 대한 ScriptableObject를 반환하는 함수
    }
}
