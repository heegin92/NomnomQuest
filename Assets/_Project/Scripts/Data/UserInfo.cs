using System;
using System.Collections.Generic;

public partial class UserInfo
{
    public int level = 1;
    public int exp = 0;
    public int expToNextLevel = 100;
    public int attack = 10;

    public int maxHealth = 100;   // ✅ 추가
    public int health = 100;      // ✅ 현재 체력

    public int gold = 0;

    // ✅ 인벤토리: 아이템코드 → 개수
    public Dictionary<string, int> inventory = new Dictionary<string, int>();
}
