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

        Debug.Log("[ItemDropManager] Awake");

        itemDict.Clear();
        foreach (var item in itemDatabase)
        {
            if (item == null) continue;
            if (string.IsNullOrEmpty(item.code)) continue;

            itemDict[item.code] = item;
            Debug.Log($"[ItemDropManager] Loaded item code: {item.code}, prefab={(item.prefab == null ? "NULL" : item.prefab.name)}");
        }
    }

    public void Spawn(string dropId, Vector3 position)
    {
        Debug.Log($"[ItemDropManager] Try spawn: {dropId} at {position}");

        if (!itemDict.TryGetValue(dropId, out var data))
        {
            Debug.LogWarning($"[ItemDropManager] 아이템 {dropId} 없음!");
            return;
        }

        if (data.prefab == null)
        {
            Debug.LogWarning($"[ItemDropManager] {dropId} 프리팹 비어있음!");
            return;
        }

        var spawnPos = new Vector3(position.x, position.y + 0.5f, 0f);
        var go = Instantiate(data.prefab, spawnPos, Quaternion.identity);
        go.name = $"DROP_{dropId}";
        Debug.Log($"[ItemDropManager] Spawned {dropId} at {go.transform.position}");
    }
}
