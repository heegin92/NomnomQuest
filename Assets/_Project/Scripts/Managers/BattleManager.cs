using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public EnemySpawner EnemySpawner;
    public PlayerData playerData;

    public void StartBattle()
    {
        InvokeRepeating(nameof(SpawnEnemy), 1f, 3f); // 3�ʸ��� ���� ����
    }

    private void SpawnEnemy()
    {
        Enemy enemy = EnemySpawner.Spawn();
        ResolveBattle(enemy);
    }

    private void ResolveBattle(Enemy enemy)
    {
        enemy.TakeDamage(playerData.attack);

        if (enemy.IsDead())
        {
            playerData.AddExp(enemy.data.exp);
            playerData.gold += enemy.data.gold;

            Debug.Log($"���� óġ! EXP +{enemy.data.exp}, Gold +{enemy.data.gold}");

            enemy.Die();
        }
    }


}
