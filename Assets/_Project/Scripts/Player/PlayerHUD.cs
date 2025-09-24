using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("상태바")]
    public Slider hpBar;
    public Slider expBar;

    [Header("텍스트")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI stageText;

    private Player player;

    private void OnEnable()
    {
        // ⭐ 이벤트 구독
        DataManager.OnGoldChanged += UpdateGoldUI;
    }

    private void OnDisable()
    {
        // ⭐ 이벤트 해제 (메모리 릭 방지)
        DataManager.OnGoldChanged -= UpdateGoldUI;
    }

    private void Start()
    {
        if (player == null)
            player = GameManager.Instance != null ? GameManager.Instance.Player : null;

        if (player == null)
        {
            Debug.LogWarning("[PlayerHUD] Start에서 Player 못 찾음!");
        }
        else
        {
            Debug.Log("[PlayerHUD] Start에서 Player 찾음: " + player.name);
        }

        // 초기화 시 한 번 반영
        UpdateHUD();
        if (DataManager.Instance != null && DataManager.Instance.userInfo != null)
            UpdateGoldUI(DataManager.Instance.userInfo.gold);
    }

    private void Update()
    {
        UpdateHUD();
    }

    private void UpdateHUD()
    {
        if (player == null)
        {
            player = GameManager.Instance != null ? GameManager.Instance.Player : null;
            if (player == null) return;
        }

        if (hpBar == null || expBar == null || levelText == null || goldText == null || stageText == null)
        {
            Debug.LogError("[PlayerHUD] Inspector 슬롯 연결 안 됨!");
            return;
        }

        float hpRatio = (float)player.CurrentHP / player.MaxHP;
        float expRatio = (float)player.data.exp / player.data.expToNextLevel;

        hpBar.value = hpRatio;
        expBar.value = expRatio;
        levelText.text = $"Lv. {player.data.level}";
        stageText.text = $"Stage {GameManager.Instance.CurrentStage}";
    }

    // ⭐ 골드 전용 UI 업데이트
    private void UpdateGoldUI(int newGold)
    {
        if (goldText != null)
            goldText.text = $"{newGold} G";
    }

    // ⭐ Player 직접 연결 메서드
    public void SetPlayer(Player p)
    {
        player = p;
        Debug.Log("[PlayerHUD] Player 연결 완료: " + player.name);
    }
}
