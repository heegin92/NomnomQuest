// InventoryUI.cs
using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("Grid(���� �θ�)")]
    [SerializeField] private RectTransform slotParent;

    private readonly List<InventorySlotUI> slots = new();

    private void Awake()
    {
        if (slotParent == null)
        {
            Debug.LogError("[InventoryUI] slotParent ������! (Grid RectTransform �巡��)");
            return;
        }

        // ���� �ִ� ���Ե��� �ڵ� ���� (��Ȱ�� ����)
        slots.AddRange(slotParent.GetComponentsInChildren<InventorySlotUI>(true));

        if (slots.Count == 0)
            Debug.LogWarning("[InventoryUI] ������ InventorySlotUI�� �ϳ��� �����ϴ�. �� Slot ������Ʈ�� ��ũ��Ʈ�� ���̼���.");
    }

    private void Start()
    {
        foreach (var s in slots) s.ClearSlot();
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (slots.Count == 0) return;

        // DataManager�� �κ��丮(Dictionary<string,int>)���� ������� ä���
        var inv = DataManager.Instance.userInfo.inventory;
        int i = 0;
        foreach (var kv in inv)
        {
            if (i >= slots.Count) break;
            var data = DataManager.Instance.GetItemData(kv.Key);
            slots[i].SetSlot(data, kv.Value);
            i++;
        }
        for (; i < slots.Count; i++) slots[i].ClearSlot();
    }
}
