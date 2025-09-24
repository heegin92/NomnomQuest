using UnityEngine;

[CreateAssetMenu(menuName = "Game/Field Data", fileName = "FieldData")]
public class FieldData : ScriptableObject
{
    public FieldType fieldType;
    public string displayName;   // UI 표시용 (예: "초원", "사막", "바다")
    public int minStage;
    public int maxStage;

    public int GetRandomStage()
    {
        return Random.Range(minStage, maxStage + 1);
    }
}

public enum FieldType { Grassland, Desert, Ocean }
