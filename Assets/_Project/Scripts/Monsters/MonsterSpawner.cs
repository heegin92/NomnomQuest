using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public Monster[] monsters;

    public Monster Spawn()
    {
        int index = Random.Range(0, monsters.Length);
        return new Monster
        {
            monsterName = monsters[index].monsterName,
            health = monsters[index].health,
            attack = monsters[index].attack,
            expReward = monsters[index].expReward,
            goldReward = monsters[index].goldReward
        };
    }
}
