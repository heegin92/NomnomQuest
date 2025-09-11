using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro 사용 시

public class PlayerHUD : MonoBehaviour
{
    [Header("스테이지 번호 표시")]
    [SerializeField] private TextMeshProUGUI stageText;

    [Header("체력 표시")]
    [SerializeField] private Slider hpBar;

    [Header("EXP 표시")]
    [SerializeField] private Slider expBar;

    public void SetStageText(int stageNum)
    {
        if (stageText != null)
            stageText.text = $"Stage {stageNum}";
    }

    public void SetHP(float current, float max)
    {
        if (hpBar != null)
            hpBar.value = current / max;
    }

    public void SetExp(float current, float max)
    {
        if (expBar != null)
            expBar.value = current / max;
    }
}
