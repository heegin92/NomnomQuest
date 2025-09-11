using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyData[] enemyTable;

    public Enemy Spawn()
    {
        // �������� EnemyData ����
        EnemyData data = enemyTable[Random.Range(0, enemyTable.Length)];

        // ������ ����
        GameObject go = Instantiate(data.prefab, transform.position, Quaternion.identity);

        // Enemy ������Ʈ�� ������ ����
        Enemy enemy = go.GetComponent<Enemy>();
        enemy.data = data;

        return enemy;
    }
}
