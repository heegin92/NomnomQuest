using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReturnToTownUI : MonoBehaviour
{
    [SerializeField] private Button returnButton;

    private void Awake()
    {
        if (returnButton != null)
            returnButton.onClick.AddListener(OnClickReturnToTown);
        else
            Debug.LogError("[ReturnToTownUI] 버튼 참조 없음!");
    }

    private void Start()
    {
        // ✅ 마을이면 버튼 숨기기
        if (GameManager.Instance != null && GameManager.Instance.IsTown)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    private void OnClickReturnToTown()
    {
        Debug.Log("[ReturnToTownUI] 귀환 버튼 클릭됨 → TownScene 로드");

        // ✅ 체력 풀 회복 후 저장
        if (GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            var p = GameManager.Instance.Player;
            p.data.health = p.MaxHP;

            if (DataManager.Instance != null && DataManager.Instance.userInfo != null)
            {
                DataManager.Instance.userInfo.health = p.data.health;
                DataManager.Instance.SaveData();
            }
        }

        // ✅ 마을 씬 로드
        SceneManager.LoadScene("TownScene");
    }
}
