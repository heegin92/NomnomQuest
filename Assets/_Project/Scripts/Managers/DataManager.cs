using UnityEngine;
using Ironcow.Synapse.Data;
using System.IO;
using System.Collections.Generic;

public partial class DataManager : DataManagerBase<DataManager, UserInfo>
{
    public static DataManager Instance => instance;
    private string savePath;

    private async void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "save.json");

        if (userInfo == null)
            userInfo = new UserInfo();

        if (!isInit)
            await Init();

        LoadData();
    }

    // ✅ 저장하기
    public void SaveData()
    {
        if (userInfo == null) return;

        // Dictionary를 JSON으로 저장하기 위해 래퍼 사용
        SaveWrapper wrapper = new SaveWrapper(userInfo);
        string json = JsonUtility.ToJson(wrapper, true);

        File.WriteAllText(savePath, json);
        Debug.Log($"[DataManager] 저장 완료: {savePath}");
    }

    // ✅ 불러오기
    public void LoadData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveWrapper wrapper = JsonUtility.FromJson<SaveWrapper>(json);
            userInfo = wrapper.ToUserInfo();
            Debug.Log("[DataManager] 저장된 데이터 불러오기 완료");
        }
        else
        {
            Debug.Log("[DataManager] 저장 파일 없음 → 새로 생성");
            userInfo = new UserInfo();
        }
    }

    // ✅ 인벤토리 추가
    public void AddItem(string itemCode, int amount = 1)
    {
        if (!userInfo.inventory.ContainsKey(itemCode))
            userInfo.inventory[itemCode] = 0;

        userInfo.inventory[itemCode] += amount;
        SaveData();
    }

    // ✅ 아이템 개수 확인
    public int GetItemCount(string itemCode)
    {
        return userInfo.inventory.ContainsKey(itemCode) ? userInfo.inventory[itemCode] : 0;
    }

    // ✅ 저장용 래퍼 클래스
    [System.Serializable]
    private class SaveWrapper
    {
        public int level;
        public int exp;
        public int expToNextLevel;
        public int attack;
        public int health;
        public int gold;
        public List<string> itemKeys = new List<string>();
        public List<int> itemValues = new List<int>();

        public SaveWrapper(UserInfo info)
        {
            level = info.level;
            exp = info.exp;
            expToNextLevel = info.expToNextLevel;
            attack = info.attack;
            health = info.health;
            gold = info.gold;

            foreach (var kvp in info.inventory)
            {
                itemKeys.Add(kvp.Key);
                itemValues.Add(kvp.Value);
            }
        }

        public UserInfo ToUserInfo()
        {
            UserInfo info = new UserInfo
            {
                level = level,
                exp = exp,
                expToNextLevel = expToNextLevel,
                attack = attack,
                health = health,
                gold = gold,
                inventory = new Dictionary<string, int>()
            };

            for (int i = 0; i < itemKeys.Count; i++)
            {
                info.inventory[itemKeys[i]] = itemValues[i];
            }

            return info;
        }
    }
}
