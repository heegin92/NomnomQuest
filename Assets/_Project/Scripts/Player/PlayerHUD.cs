using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro ��� ��

public class PlayerHUD : MonoBehaviour
{
    [Header("�������� ��ȣ ǥ��")]
    [SerializeField] private TextMeshProUGUI stageText;

    [Header("ü�� ǥ��")]
    [SerializeField] private Slider hpBar;

    [Header("EXP ǥ��")]
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
