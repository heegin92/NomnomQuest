using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public MonsterSpawner monsterSpawner;
    public PlayerData playerData;

    public void StartBattle()
    {
        InvokeRepeating(nameof(SpawnMonster), 1f, 3f); // 3초마다 몬스터 등장
    }

    private void SpawnMonster()
    {
        Monster monster = monsterSpawner.Spawn();
        ResolveBattle(monster);
    }

    private void ResolveBattle(Monster monster)
    {
        monster.TakeDamage(playerData.attack);

        if (monster.IsDead())
        {
            playerData.AddExp(monster.expReward);
            playerData.gold += monster.goldReward;

            Debug.Log($"몬스터 처치! EXP +{monster.expReward}, Gold +{monster.goldReward}");
        }
    }
}
