using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyData[] enemyTable;

    public Enemy Spawn()
    {
        // 랜덤으로 EnemyData 선택
        EnemyData data = enemyTable[Random.Range(0, enemyTable.Length)];

        // 프리팹 생성
        GameObject go = Instantiate(data.prefab, transform.position, Quaternion.identity);

        // Enemy 컴포넌트에 데이터 주입
        Enemy enemy = go.GetComponent<Enemy>();
        enemy.data = data;

        return enemy;
    }
}
