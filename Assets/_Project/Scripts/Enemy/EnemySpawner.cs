using UnityEngine;
using Ironcow.Synapse.Data;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    [SerializeField] private string[] enemyCodes = { "ENE00001" };
    [SerializeField] private int maxEnemies = 5;         // 씬에 항상 유지할 적 개수
    [SerializeField] private float spawnRadius = 5f;
    [SerializeField] private float respawnDelay = 2f;    // 죽은 뒤 리스폰 딜레이

    private List<Enemy> activeEnemies = new List<Enemy>();
    private bool isSpawning = false;

    private void Start()
    {
        BeginSpawn();
    }

    public void BeginSpawn()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            StartCoroutine(SpawnWhenReady());
        }
    }

    private IEnumerator SpawnWhenReady()
    {
        while (DataManager.Instance == null || !DataManager.Instance.isInit)
            yield return null;

        Debug.Log("[EnemySpawner] DataManager 준비 완료, 몬스터 스폰 시작");

        // 시작 시 maxEnemies 만큼 채움
        for (int i = 0; i < maxEnemies; i++)
            Spawn();
    }

    public Enemy Spawn()
    {
        if (activeEnemies.Count >= maxEnemies)
            return null;

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

        // 바닥에 붙이기 (Ground 레이어 필요)
        if (Physics.Raycast(pos + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 50f, LayerMask.GetMask("Ground")))
        {
            pos = hit.point; // 바닥 위 정확한 위치로 수정
        }


        GameObject go = Instantiate(data.prefab, pos, Quaternion.identity);
        var enemy = go.GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogError($"[Spawner] {data.prefab.name} 에 Enemy 스크립트 없음!");
            return null;
        }

        enemy.SetCode(code);
        activeEnemies.Add(enemy);

        // 죽으면 Spawner에 알리도록 Tracker 부착
        enemy.gameObject.AddComponent<EnemyTracker>().Init(this, enemy);

        // EnemySpawner.Spawn() 안에서
        var anim = enemy.GetComponentInChildren<Animator>();
        if (anim != null)
        {
            // Spawn 애니메이션 실행
            anim.Play("Spawn", -1, 0f);

        }


        Debug.Log($"[EnemySpawner] {code} 소환 성공! 현재 수: {activeEnemies.Count}");
        return enemy;
    }
    private IEnumerator PlaySpawnAndIdle(Animator anim)
    {
        // 현재 실행 중인 클립 길이 가져오기
        AnimatorClipInfo[] clipInfo = anim.GetCurrentAnimatorClipInfo(0);
        float length = clipInfo.Length > 0 ? clipInfo[0].clip.length : 1f;

        yield return new WaitForSeconds(length);

        anim.Play("Idle");
    }


    public void RemoveEnemy(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            Debug.Log($"[EnemySpawner] {enemy.name} 제거됨. 현재 수: {activeEnemies.Count}");

            // 2초 뒤 다시 채움
            StartCoroutine(RespawnAfterDelay());
        }
    }

    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);

        if (activeEnemies.Count < maxEnemies)
            Spawn();
    }
}

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
