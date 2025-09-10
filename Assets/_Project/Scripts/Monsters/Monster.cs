using UnityEngine;

[System.Serializable]
public class Monster : MonoBehaviour
{
    public string monsterName;
    public int health;
    public int attack;
    public int expReward;
    public int goldReward;

    public void TakeDamage(int dmg)
    {
        health -= dmg;
    }

    public bool IsDead() => health <= 0;
}
