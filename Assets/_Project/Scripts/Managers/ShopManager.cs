using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public PlayerData playerData;

    public void SellItem(Item item, int price)
    {
        playerData.gold += price;
        Debug.Log(item.itemName + " 판매 완료! +" + price + " Gold");
    }
}
