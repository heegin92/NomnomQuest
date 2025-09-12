using UnityEngine;
using Ironcow.Synapse.Data;

public partial class DataManager : DataManagerBase<DataManager, UserInfo>
{
    // 별칭 프로퍼티 (MonoSingleton.instance → Instance)
    public static DataManager Instance => instance;

    private void Awake()
    {
        if (userInfo == null)
        {
            userInfo = new UserInfo();
            Debug.Log("[DataManager] UserInfo 새로 생성됨");
        }
    }

    public void AddGold(int amount)
    {
        if (userInfo == null) userInfo = new UserInfo();

        userInfo.gold += amount;
        Debug.Log($"[DataManager] 골드 추가됨 → 현재 골드: {userInfo.gold}");
    }

    public void AddExp(int amount)
    {
        if (userInfo == null) userInfo = new UserInfo();

        userInfo.exp += amount;
        if (userInfo.exp >= userInfo.expToNextLevel)
        {
            userInfo.level++;
            userInfo.exp = 0;
            userInfo.expToNextLevel += 50;
            Debug.Log($"[DataManager] 레벨업! 현재 레벨: {userInfo.level}");
        }
    }
}
