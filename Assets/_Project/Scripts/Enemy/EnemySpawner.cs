using UnityEngine;
using Ironcow.Synapse.Data;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private string[] enemyCodes = { "ENE00001" }; // "ENE00001", "ENE00002" ...

    private void Start()
    {
        StartCoroutine(SpawnWhenReady());
    }

    private IEnumerator SpawnWhenReady()
    {
        int frame = 0;
        while (DataManager.Instance == null || !DataManager.Instance.isInit)
        {
            if (frame % 60 == 0) // 1초마다 출력
                Debug.Log($"[EnemySpawner] Waiting... DataManager ready? {(DataManager.Instance != null ? DataManager.Instance.isInit : false)}");
            frame++;
            yield return null;
        }

        Debug.Log("[EnemySpawner] DataManager 준비 완료, Spawn 호출!");
        Spawn();
    }

    public Enemy Spawn()
    {
        string code = enemyCodes[Random.Range(0, enemyCodes.Length)];
        var data = DataManager.Instance.GetData<EnemyData>(code);
        if (data == null || data.prefab == null)
        {
            Debug.LogError($"[Spawner] EnemyData {code} 또는 prefab 없음!");
            return null;
        }

        GameObject go = Instantiate(data.prefab, transform.position, Quaternion.identity);
        var enemy = go.GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogError($"[Spawner] {data.prefab.name} 에 Enemy 스크립트 없음!");
            return null;
        }

        enemy.SetCode(code); // 코드 주입
        return enemy;
    }
}
