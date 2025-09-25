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
            // ����: ItemData�� Sprite icon �ʵ� �ϳ� �߰��ؼ� �ű⼭ ���ô�.
            // �ӽ�: Prefab �� SpriteRenderer���� �������� (������ null)
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
