using System.Collections.Generic;
using UnityEngine;

public class ItemDropManager : MonoBehaviour
{
    public static ItemDropManager Instance { get; private set; }

    [SerializeField] private List<SimpleItemData> itemDatabase;
    private Dictionary<string, SimpleItemData> itemDict = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Spawn(string dropId, Vector3 position)
    {
        Debug.Log($"[ItemDropManager] Spawn() called with {dropId}");

        // 1. 드랍 규칙 찾기
        var dropData = DataManager.Instance.GetData<DropObjectData>(dropId);
        if (dropData == null)
        {
            Debug.LogWarning($"DropObjectData {dropId} 없음!");
            return;
        }

        // 2. 확률 체크
        if (Random.Range(0, 100) >= dropData.probability)
        {
            Debug.Log($"드랍 실패 (확률 {dropData.probability}%)");
            return;
        }

        // 3. 실제 아이템 찾기
        var itemData = DataManager.Instance.GetData<ItemData>(dropData.target);
        if (itemData == null)
        {
            Debug.LogWarning($"ItemData {dropData.target} 없음!");
            return;
        }

        // 4. 드랍 생성
        int count = Random.Range(dropData.minValue, dropData.maxValue + 1);
        for (int i = 0; i < count; i++)
        {
            var pos = new Vector3(position.x + Random.Range(-0.3f, 0.3f), 0f, position.z + Random.Range(-0.3f, 0.3f));
            var go = Instantiate(itemData.prefab, pos, Quaternion.identity);

            // 자동 흡수 스크립트 붙이기
            var pickup = go.GetComponent<ItemPickup>();
            if (pickup == null) pickup = go.AddComponent<ItemPickup>();
            pickup.itemCode = itemData.rcode;
            pickup.amount = 1; // 추후 조정 가능
        }



    }
}
