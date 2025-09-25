using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopBarTabs : MonoBehaviour
{
    [Header("������ ��ư�� (�ޡ�� ����)")]
    [SerializeField] private List<Button> tabButtons;

    [Header("�гε� (��ư ������ ����)")]
    [SerializeField] private List<GameObject> tabPanels;

    [Header("���� �� �ε��� (-1�̸� ���� ���� ����)")]
    [SerializeField] private int startIndex = -1;

    [Header("�κ��丮 �� �ε��� (��: 1=�� ��°)")]
    [SerializeField] private int inventoryTabIndex = 1;
    [SerializeField] private InventoryUI inventoryUI;

    [Header("������ ����")]
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(1f, 0.6f, 0f); // ��Ȳ��

    private int currentIndex = -1;

    private void Awake()
    {
        if (tabButtons.Count != tabPanels.Count)
        {
            Debug.LogError("[TopBarTabs] ��ư ���� �г� ���� �ٸ��ϴ�.");
            return;
        }

        for (int i = 0; i < tabButtons.Count; i++)
        {
            int idx = i; // Ŭ���� ����
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

        // �̹� �����ִ� �� �ٽ� ������ �ݱ�
        if (currentIndex == index)
        {
            HideAll();
            return;
        }

        // ��� �г� ��Ȱ��ȭ + ��ư ���� �ʱ�ȭ
        for (int i = 0; i < tabPanels.Count; i++)
        {
            tabPanels[i].SetActive(i == index);
            tabButtons[i].image.color = (i == index) ? selectedColor : defaultColor;
        }

        currentIndex = index;

        // �κ��丮 �� ����
        if (index == inventoryTabIndex && inventoryUI != null)
            inventoryUI.RefreshUI();

        Debug.Log($"[TopBarTabs] �� ��ȯ: {index}");
    }

    public void HideAll()
    {
        for (int i = 0; i < tabPanels.Count; i++)
        {
            tabPanels[i].SetActive(false);
            tabButtons[i].image.color = defaultColor; // ���� �⺻ ��������
        }

        currentIndex = -1;
        Debug.Log("[TopBarTabs] ��� �г� ����");
    }
}
