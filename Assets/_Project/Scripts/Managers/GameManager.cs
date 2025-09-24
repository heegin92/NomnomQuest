using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Player Player { get; set; }
    public int CurrentStage { get; set; }   // ✅ object → int 로 변경

    public bool IsTown { get; set; }   // ✅ 현재 스테이지가 마을인지 여부


    public BattleManager battleManager;
    public InventoryManager inventoryManager;
    public PlayerData playerData;
    public StageManager stageManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // DataManager 초기화
        if (DataManager.Instance.userInfo == null)
            DataManager.Instance.userInfo = new UserInfo();
    }

}
