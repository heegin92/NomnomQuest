using UnityEngine;

[System.Serializable]
public class Monster
{
    public string monsterName;
    public int health;
    public int attack;
    public int expReward;
    public int goldReward;
    public Item dropItem;

    public void TakeDamage(int dmg)
    {
        health -= dmg;
    }

    public bool IsDead() => health <= 0;
}

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
            goldReward = monsters[index].goldReward,
            dropItem = monsters[index].dropItem
        };
    }
}
