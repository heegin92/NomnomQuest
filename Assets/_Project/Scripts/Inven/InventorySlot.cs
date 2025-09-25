using System;

[Serializable]
public class InventorySlot
{
    public ItemData itemData;
    public int quantity;

    public InventorySlot(ItemData data, int amount = 1)
    {
        itemData = data;
        quantity = amount;
    }

    public bool IsEmpty => itemData == null || quantity <= 0;
}
