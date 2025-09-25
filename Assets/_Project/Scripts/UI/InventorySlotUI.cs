using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text countText;

    public void SetSlot(ItemData data, int count)
    {
        if (data != null)
        {
            // 권장: ItemData에 Sprite icon 필드 하나 추가해서 거기서 씁시다.
            // 임시: Prefab 안 SpriteRenderer에서 꺼내오기 (없으면 null)
            var sr = data.prefab ? data.prefab.GetComponent<SpriteRenderer>() : null;
            icon.sprite = sr ? sr.sprite : null;

            icon.enabled = icon.sprite != null;
            countText.text = count > 1 ? count.ToString() : "";
        }
        else
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        icon.sprite = null;
        icon.enabled = false;
        if (countText) countText.text = "";
    }
}
