using System;
using UnityEngine;
using Ironcow.Synapse.Data;
using System.IO;
using System.Collections.Generic;

public partial class DataManager : DataManagerBase<DataManager, UserInfo>
{
    [Header("데이터베이스")]
    [SerializeField] private List<ItemData> itemDatabase; // 아이템 데이터베이스 (SO 리스트)

    public static DataManager Instance => instance;
    private string savePath;

    // ⭐ 골드 변경 이벤트
    public static event Action<int> OnGoldChanged;

    // 내부 캐싱용 Dictionary
    private Dictionary<string, ItemData> itemDict;

    private async void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "save.json");

        if (userInfo == null)
            userInfo = new UserInfo();

        if (!isInit)
            await Init();

        // ✅ ItemData 캐싱
        itemDict = new Dictionary<string, ItemData>();
        foreach (var item in itemDatabase)
        {
            if (item != null && !itemDict.ContainsKey(item.code))
                itemDict.Add(item.code, item);
        }

        LoadData();
    }

    // ✅ 저장하기
    public void SaveData()
    {
        if (userInfo == null) return;

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

    // ✅ ItemData 가져오기
    public ItemData GetItemData(string itemCode)
    {
        if (itemDict != null && itemDict.TryGetValue(itemCode, out var data))
            return data;

        Debug.LogWarning($"[DataManager] ItemData를 찾을 수 없음 → {itemCode}");
        return null;
    }

    // ✅ 인벤토리 추가
    public void AddItem(string itemCode, int amount = 1)
    {
        if (!userInfo.inventory.ContainsKey(itemCode))
            userInfo.inventory[itemCode] = 0;

        userInfo.inventory[itemCode] += amount;
        SaveData();

        // 디버그용
        ItemData itemData = GetItemData(itemCode);
        string name = itemData != null ? itemData.displayName : itemCode;
        Debug.Log($"[DataManager] 아이템 추가됨: {name} x{amount} (총 {userInfo.inventory[itemCode]})");
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

    // ✅ 골드 추가
    public void AddGold(int amount)
    {
        if (userInfo == null) userInfo = new UserInfo();

        userInfo.gold += amount;

        Debug.Log($"[DataManager] 골드 추가됨 → 현재 골드: {userInfo.gold}");
        SaveData();

        // ⭐ 이벤트 호출
        OnGoldChanged?.Invoke(userInfo.gold);
    }
}
