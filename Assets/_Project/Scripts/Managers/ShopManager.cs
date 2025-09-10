using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public PlayerData playerData;

    public void SellItem(Item item, int price)
    {
        playerData.gold += price;
        Debug.Log(item.itemName + " �Ǹ� �Ϸ�! +" + price + " Gold");
    }
}
