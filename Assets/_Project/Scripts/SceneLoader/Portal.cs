using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private GameObject fieldSelectUI; // Inspector에 넣어도 되고, 비어있으면 자동 연결

    private void Awake()
    {
        // 🔎 Inspector에서 비어있으면 자동으로 씬에서 찾아 연결 (비활성 객체 포함)
        if (fieldSelectUI == null)
        {
#if UNITY_2020_1_OR_NEWER
            var ui = FindObjectOfType<FieldSelectUI>(true);  // includeInactive = true
#else
            var ui = FindObjectOfType<FieldSelectUI>();      // (구버전이면 패널을 한 번 켜서 찾게 하거나, 아래 SetUI 사용)
#endif
            if (ui != null)
            {
                fieldSelectUI = ui.gameObject;
                Debug.Log("[Portal] fieldSelectUI 자동 연결: " + fieldSelectUI.name);
            }
            else
            {
                Debug.LogError("[Portal] FieldSelectUI를 씬에서 찾지 못했어요!");
            }
        }
    }

    private void Start()
    {
        // 시작 시 항상 꺼두기
        if (fieldSelectUI != null) fieldSelectUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (fieldSelectUI != null)
        {
            fieldSelectUI.SetActive(true);
            Debug.Log("[Portal] UI 자동 오픈");
        }
        else
        {
            Debug.LogError("[Portal] fieldSelectUI가 비어있어요 (자동 연결 실패)");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (fieldSelectUI != null)
            fieldSelectUI.SetActive(false);
    }

    // 필요하면 외부에서 주입도 가능
    public void SetUI(GameObject panel) => fieldSelectUI = panel;
}
