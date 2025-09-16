using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public string itemCode;
    public int amount = 1;

    [SerializeField] private float absorbRange = 2.0f;   // 흡수 시작 거리
    [SerializeField] private float absorbSpeed = 5.0f;   // 흡수 속도

    private Transform player;
    private bool isAbsorbing = false;

    private void Start()
    {
        // Player 태그로 찾아오기
        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj != null) player = pObj.transform;
    }

    private void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // 일정 거리 안으로 들어오면 흡수 시작
        if (!isAbsorbing && dist < absorbRange)
        {
            isAbsorbing = true;
        }

        if (isAbsorbing)
        {
            // 플레이어 쪽으로 이동
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                absorbSpeed * Time.deltaTime
            );

            // 충분히 가까워지면 아이템 획득 처리
            if (dist < 0.3f)
            {
                Debug.Log($"[ItemPickup] {itemCode} {amount}개 획득!");
                // 나중에 인벤토리에 넣을 자리: InventoryManager.Instance.Add(itemCode, amount);
                Destroy(gameObject);
            }
        }
    }
}
