using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InventoryToggleUI : MonoBehaviour
{
    [Header("토글 버튼/아이콘")]
    [SerializeField] private Button toggleButton;   // 오른쪽 상단 버튼
    [SerializeField] private Image iconImage;       // 버튼 안 아이콘
    [SerializeField] private Sprite closedSprite;   // 닫힘 아이콘 (Checkmark)
    [SerializeField] private Sprite openedSprite;   // 열림 아이콘 (Checkmark2)

    [Header("패널")]
    [SerializeField] private RectTransform inventoryPanel; // 👉 GridLayoutGroup이 붙은 패널

    [Header("애니메이션")]
    [SerializeField] private float slideDuration = 0.3f;   // 애니메이션 시간
    [SerializeField] private float slideOffset = 600f;     // 오른쪽 숨김 거리

    private bool isOpen = false;
    private Vector2 shownPos;
    private Vector2 hiddenPos;

    private void Start()
    {

        if (toggleButton == null)
            Debug.LogError("[InventoryToggleUI] toggleButton이 연결 안 됨!");

        toggleButton.onClick.AddListener(ToggleInventory);
        Debug.Log("[InventoryToggleUI] 버튼 이벤트 연결 완료");
        // 원래 위치 저장
        shownPos = inventoryPanel.anchoredPosition;

        // 숨김 위치 = 오른쪽으로 밀어냄
        hiddenPos = shownPos + new Vector2(slideOffset, 0);

        // 시작은 숨긴 상태
        inventoryPanel.anchoredPosition = hiddenPos;
        iconImage.sprite = isOpen ? openedSprite : closedSprite;

        // 버튼 클릭 이벤트 연결
        toggleButton.onClick.AddListener(ToggleInventory);
    }

    public void ToggleInventory()
    {
        if (inventoryPanel == null)
        {
            Debug.LogError("[InventoryToggleUI] inventoryPanel이 Inspector에 연결 안 됐습니다!");
            return;
        }

        isOpen = !isOpen;

        // 테스트 1: 아이콘 강제 색 바꾸기
        iconImage.color = isOpen ? Color.red : Color.blue;

        // 테스트 2: 패널 강제 위치 옮기기
        inventoryPanel.anchoredPosition = isOpen ? new Vector2(0, 0) : new Vector2(1000, 0);

        Debug.Log($"[InventoryToggleUI] {(isOpen ? "Open" : "Close")}");
    }
}
