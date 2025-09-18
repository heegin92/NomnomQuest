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

    private void Awake()
    {
        Debug.Log("[PlayerHUD] Awake 실행됨");
    }

    private void Start()
    {
        player = GameManager.Instance != null ? GameManager.Instance.Player : null;
        if (player == null)
        {
            Debug.LogWarning("[PlayerHUD] Start에서 Player 못 찾음!");
        }
        else
        {
            Debug.Log("[PlayerHUD] Start에서 Player 찾음: " + player.name);
        }
        UpdateHUD();
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
            if (player == null)
            {
                Debug.LogWarning("[PlayerHUD] UpdateHUD 호출됨 - Player 없음");
                return;
            }
            else
            {
                Debug.Log("[PlayerHUD] UpdateHUD에서 Player 참조 성공!");
            }
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
        goldText.text = $"{player.data.gold} G";
        stageText.text = $"Stage {GameManager.Instance.CurrentStage}";

    }
}
