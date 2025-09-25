using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopBarTabs : MonoBehaviour
{
    [Header("아이콘 버튼들 (왼→오 순서)")]
    [SerializeField] private List<Button> tabButtons;

    [Header("패널들 (버튼 순서와 동일)")]
    [SerializeField] private List<GameObject> tabPanels;

    [Header("시작 탭 인덱스 (-1이면 전부 숨김 시작)")]
    [SerializeField] private int startIndex = -1;

    [Header("인벤토리 탭 인덱스 (예: 1=두 번째)")]
    [SerializeField] private int inventoryTabIndex = 1;
    [SerializeField] private InventoryUI inventoryUI;

    [Header("아이콘 색상")]
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(1f, 0.6f, 0f); // 주황색

    private int currentIndex = -1;

    private void Awake()
    {
        if (tabButtons.Count != tabPanels.Count)
        {
            Debug.LogError("[TopBarTabs] 버튼 수와 패널 수가 다릅니다.");
            return;
        }

        for (int i = 0; i < tabButtons.Count; i++)
        {
            int idx = i; // 클로저 방지
            tabButtons[i].onClick.RemoveAllListeners();
            tabButtons[i].onClick.AddListener(() => ShowTab(idx));
        }
    }

    private void Start()
    {
        if (startIndex >= 0 && startIndex < tabPanels.Count)
            ShowTab(startIndex);
        else
            HideAll();
    }

    public void ShowTab(int index)
    {
        if (index < 0 || index >= tabPanels.Count) return;

        // 이미 열려있던 탭 다시 누르면 닫기
        if (currentIndex == index)
        {
            HideAll();
            return;
        }

        // 모든 패널 비활성화 + 버튼 색상 초기화
        for (int i = 0; i < tabPanels.Count; i++)
        {
            tabPanels[i].SetActive(i == index);
            tabButtons[i].image.color = (i == index) ? selectedColor : defaultColor;
        }

        currentIndex = index;

        // 인벤토리 탭 갱신
        if (index == inventoryTabIndex && inventoryUI != null)
            inventoryUI.RefreshUI();

        Debug.Log($"[TopBarTabs] 탭 전환: {index}");
    }

    public void HideAll()
    {
        for (int i = 0; i < tabPanels.Count; i++)
        {
            tabPanels[i].SetActive(false);
            tabButtons[i].image.color = defaultColor; // 전부 기본 색상으로
        }

        currentIndex = -1;
        Debug.Log("[TopBarTabs] 모든 패널 숨김");
    }
}
