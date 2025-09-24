using UnityEngine;
using UnityEngine.SceneManagement;

public class FieldSelectUI : MonoBehaviour
{
    public void OnSelectGrass()
    {
        Debug.Log("[FieldSelectUI] Grassland 선택됨!");
        EnterField(FieldType.Grassland);
    }

    public void OnSelectDesert()
    {
        Debug.Log("[FieldSelectUI] Desert 선택됨!");
        EnterField(FieldType.Desert);
    }

    public void OnSelectSea()
    {
        Debug.Log("[FieldSelectUI] Ocean 선택됨!");
        EnterField(FieldType.Ocean);
    }

    private void EnterField(FieldType type)
    {
        int stageNum = GetRandomStage(type);
        GameManager.Instance.IsTown = false;
        GameManager.Instance.CurrentStage = stageNum;

        SceneManager.LoadScene("StageScene");
    }

    private int GetRandomStage(FieldType type)
    {
        switch (type)
        {
            case FieldType.Grassland: return Random.Range(1, 11);
            case FieldType.Desert: return Random.Range(11, 21);
            case FieldType.Ocean: return Random.Range(21, 31);
            default: return -1;
        }
    }
}
