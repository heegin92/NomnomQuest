using UnityEngine;

[System.Serializable]
public class Stage
{
    public string stageName;
    public Enemy[] stageEnemys;
    public int requiredLevel;
}

public class StageManager : MonoBehaviour
{
    public Stage[] stages;
    private int currentStage = 0;

    public Stage GetCurrentStage()
    {
        return stages[currentStage];
    }

    public void UnlockNextStage(PlayerData playerData)
    {
        if (currentStage + 1 < stages.Length &&
            playerData.level >= stages[currentStage + 1].requiredLevel)
        {
            currentStage++;
        }
    }
}
