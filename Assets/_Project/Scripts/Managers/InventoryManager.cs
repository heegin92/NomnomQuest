using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    // 아이템코드 → 수량
    private Dictionary<string, int> inventory = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Debug.Log("[InventoryManager] 초기화 완료");
    }

    /// <summary> 아이템 추가 </summary>
    public void Add(string itemCode, int amount)
    {
        if (!inventory.ContainsKey(itemCode))
            inventory[itemCode] = 0;

        inventory[itemCode] += amount;

        Debug.Log($"[Inventory] {itemCode} x{amount} 추가됨 → 총 {inventory[itemCode]}개");
    }

    /// <summary> 아이템 개수 조회 </summary>
    public int GetAmount(string itemCode)
    {
        return inventory.ContainsKey(itemCode) ? inventory[itemCode] : 0;
    }

    /// <summary> 전체 인벤토리 출력 (테스트용) </summary>
    public void PrintInventory()
    {
        Debug.Log("===== 📦 현재 인벤토리 =====");
        foreach (var kvp in inventory)
        {
            Debug.Log($"{kvp.Key} : {kvp.Value}");
        }
    }
}
