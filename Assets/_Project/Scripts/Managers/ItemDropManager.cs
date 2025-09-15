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

        // 1. ��� ��Ģ ã��
        var dropData = DataManager.Instance.GetData<DropObjectData>(dropId);
        if (dropData == null)
        {
            Debug.LogWarning($"DropObjectData {dropId} ����!");
            return;
        }

        // 2. Ȯ�� üũ
        if (Random.Range(0, 100) >= dropData.probability)
        {
            Debug.Log($"��� ���� (Ȯ�� {dropData.probability}%)");
            return;
        }

        // 3. ���� ������ ã��
        var itemData = DataManager.Instance.GetData<ItemData>(dropData.target);
        if (itemData == null)
        {
            Debug.LogWarning($"ItemData {dropData.target} ����!");
            return;
        }

        // 4. ��� ����
        int count = Random.Range(dropData.minValue, dropData.maxValue + 1);
        for (int i = 0; i < count; i++)
        {
            var pos = new Vector3(position.x + Random.Range(-0.3f, 0.3f), position.y + 0.5f, 0f);
            Instantiate(itemData.prefab, pos, Quaternion.identity);
        }

        Debug.Log($"��� ���� �� {itemData.rcode} x{count}");
    }


}
