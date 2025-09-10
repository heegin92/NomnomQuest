[System.Serializable]
public class PlayerData
{
    public int level = 1;
    public int exp = 0;
    public int expToNextLevel = 100;

    public int attack = 10;
    public int health = 100;
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
        health += 20;
    }
}
