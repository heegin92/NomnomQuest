[System.Serializable]
public class PlayerData
{
    public int level = 1;
    public int exp = 0;
    public int expToNextLevel = 100;

    public int attack = 10;

    public int maxHealth = 100;   // ✅ 최대 체력
    public int health = 100;      // ✅ 현재 체력

    public int gold = 0;

    public void AddExp(int amount)
    {
        exp += amount;
        if (exp >= expToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        exp = 0;
        expToNextLevel += 50;
        attack += 5;
        maxHealth += 20;   // ✅ 최대 체력 증가
        health = maxHealth; // ✅ 레벨업 시 체력 회복
    }
}
