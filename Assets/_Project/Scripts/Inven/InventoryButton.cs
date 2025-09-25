using UnityEngine;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private InventoryToggleUI inventoryToggleUI; // ✅ InventoryUI → InventoryToggleUI로 교체

    private void Start()
    {
        if (button == null) button = GetComponent<Button>();

        if (inventoryToggleUI == null)
        {
            Debug.LogError("[InventoryButton] InventoryToggleUI 참조가 비어있습니다!");
            return;
        }

        button.onClick.AddListener(() =>
        {
            inventoryToggleUI.ToggleInventory(); // ✅ Toggle() 대신 ToggleInventory() 호출
        });
    }
}
