// InventoryUI.cs
using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("Grid(슬롯 부모)")]
    [SerializeField] private RectTransform slotParent;

    private readonly List<InventorySlotUI> slots = new();

    private void Awake()
    {
        if (slotParent == null)
        {
            Debug.LogError("[InventoryUI] slotParent 미지정! (Grid RectTransform 드래그)");
            return;
        }

        // 씬에 있는 슬롯들을 자동 수집 (비활성 포함)
        slots.AddRange(slotParent.GetComponentsInChildren<InventorySlotUI>(true));

        if (slots.Count == 0)
            Debug.LogWarning("[InventoryUI] 하위에 InventorySlotUI가 하나도 없습니다. 각 Slot 오브젝트에 스크립트를 붙이세요.");
    }

    private void Start()
    {
        foreach (var s in slots) s.ClearSlot();
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (slots.Count == 0) return;

        // DataManager의 인벤토리(Dictionary<string,int>)에서 순서대로 채우기
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
