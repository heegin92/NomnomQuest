using UnityEngine;
using Ironcow.Synapse.Data;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    [SerializeField] private string[] enemyCodes = { "ENE00001" }; // 몬스터 코드들
    [SerializeField] private int spawnCount = 3;      // 시작할 때 몇 마리 스폰
    [SerializeField] private float spawnRadius = 5f;  // 스폰 반경
    [SerializeField] private bool loopSpawn = true;   // 지속 스폰 여부
    [SerializeField] private float spawnInterval = 5f; // 지속 스폰 간격(초)
    [SerializeField] private int maxEnemies = 10;     // 동시에 존재할 수 있는 최대 수

    private List<Enemy> activeEnemies = new List<Enemy>();

    private void Start()
    {
        StartCoroutine(SpawnWhenReady());
    }

    private IEnumerator SpawnWhenReady()
    {
        while (DataManager.Instance == null || !DataManager.Instance.isInit)
            yield return null;

        Debug.Log("[EnemySpawner] DataManager 준비 완료, 몬스터 스폰 시작");

        // 처음에 지정된 수만큼 소환
        for (int i = 0; i < spawnCount; i++)
            Spawn();

        // 반복 스폰 모드
        if (loopSpawn)
            StartCoroutine(LoopSpawn());
    }

    private IEnumerator LoopSpawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            if (activeEnemies.Count < maxEnemies)
                Spawn();
        }
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

        // 무작위 위치
        Vector3 offset = Random.insideUnitSphere * spawnRadius;
        offset.y = 0f;
        Vector3 pos = transform.position + offset;

        GameObject go = Instantiate(data.prefab, pos, Quaternion.identity);
        var enemy = go.GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogError($"[Spawner] {data.prefab.name} 에 Enemy 스크립트 없음!");
            return null;
        }

        enemy.SetCode(code);
        activeEnemies.Add(enemy);

        // 죽으면 리스트에서 제거
        enemy.gameObject.AddComponent<EnemyTracker>().Init(this, enemy);

        Debug.Log($"[EnemySpawner] {code} 소환 성공! 현재 수: {activeEnemies.Count}");
        return enemy;
    }

    // Enemy 죽으면 리스트에서 제거하기 위한 헬퍼
    public void RemoveEnemy(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            Debug.Log($"[EnemySpawner] {enemy.name} 제거됨. 현재 수: {activeEnemies.Count}");
        }
    }
}

// Enemy가 Destroy될 때 Spawner에 알려주는 컴포넌트
public class EnemyTracker : MonoBehaviour
{
    private EnemySpawner spawner;
    private Enemy enemy;

    public void Init(EnemySpawner spawner, Enemy enemy)
    {
        this.spawner = spawner;
        this.enemy = enemy;
    }

    private void OnDestroy()
    {
        if (spawner != null && enemy != null)
        {
            spawner.RemoveEnemy(enemy);
        }
    }
}
