using UnityEngine;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stageText;

    public void SetStageText(int stageNum)
    {
        if (stageText != null)
            stageText.text = $"Stage {stageNum}";
        else
            Debug.LogWarning("[PlayerHUD] StageText ¹ÌÇÒ´ç");
    }
}
