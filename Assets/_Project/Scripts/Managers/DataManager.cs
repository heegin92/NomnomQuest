using UnityEngine;
using Ironcow.Synapse.Data;

public partial class DataManager : DataManagerBase<DataManager, UserInfo>
{
    // ��Ī ������Ƽ (MonoSingleton.instance �� Instance)
    public static DataManager Instance => instance;

    private void Awake()
    {
        if (userInfo == null)
        {
            userInfo = new UserInfo();
            Debug.Log("[DataManager] UserInfo ���� ������");
        }
    }

    public void AddGold(int amount)
    {
        if (userInfo == null) userInfo = new UserInfo();

        userInfo.gold += amount;
        Debug.Log($"[DataManager] ��� �߰��� �� ���� ���: {userInfo.gold}");
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
            Debug.Log($"[DataManager] ������! ���� ����: {userInfo.level}");
        }
    }
}
