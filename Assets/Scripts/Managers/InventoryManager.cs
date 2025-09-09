using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<Item> items = new List<Item>();

    public void AddItem(Item item)
    {
        Item existing = items.Find(i => i.itemName == item.itemName);
        if (existing != null)
            existing.quantity += 1;
        else
            items.Add(new Item { itemName = item.itemName, quantity = 1 });
    }
}
