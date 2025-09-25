using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("인벤토리 슬롯 개수")]
    [SerializeField] private int slotCount = 20;

    public List<InventorySlot> slots = new List<InventorySlot>();

    private void Awake()
    {
        // 슬롯 초기화
        for (int i = 0; i < slotCount; i++)
        {
            slots.Add(new InventorySlot(null, 0));
        }
    }

    /// 아이템 추가
    public bool AddItem(ItemData data, int amount = 1)
    {
        if (data == null) return false;

        // 스택 가능한 경우 → 기존 슬롯에 합치기
        if (data.canStack)
        {
            foreach (var slot in slots)
            {
                if (slot.itemData != null && slot.itemData.code == data.code)
                {
                    slot.quantity = Mathf.Min(slot.quantity + amount, data.maxStackAmount);
                    return true;
                }
            }
        }

        // 빈 슬롯 찾기
        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
            {
                slot.itemData = data;
                slot.quantity = Mathf.Min(amount, data.maxStackAmount);
                return true;
            }
        }

        Debug.Log("⚠ 인벤토리가 가득 찼습니다!");
        return false;
    }

    /// 아이템 제거
    public bool RemoveItem(string itemCode, int amount = 1)
    {
        foreach (var slot in slots)
        {
            if (slot.itemData != null && slot.itemData.code == itemCode)
            {
                if (slot.quantity >= amount)
                {
                    slot.quantity -= amount;
                    if (slot.quantity <= 0) slot.itemData = null;
                    return true;
                }
            }
        }
        return false;
    }
}
