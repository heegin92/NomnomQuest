using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private Dictionary<string, int> inventory = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Add(string itemCode, int amount)
    {
        if (!inventory.ContainsKey(itemCode))
            inventory[itemCode] = 0;

        inventory[itemCode] += amount;

        Debug.Log($"[Inventory] {itemCode} x{amount} Ãß°¡µÊ ¡æ ÃÑ {inventory[itemCode]}°³");
    }

    public int GetAmount(string itemCode)
    {
        return inventory.ContainsKey(itemCode) ? inventory[itemCode] : 0;
    }
}
