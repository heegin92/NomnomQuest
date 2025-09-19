using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public string itemCode;
    public int amount = 1;
    public bool isGold = false;   // 💰 골드 전용 여부

    [SerializeField] private float absorbRange = 2.0f;   // 흡수 시작 거리
    [SerializeField] private float absorbSpeed = 5.0f;   // 흡수 속도
    [SerializeField] private float autoAbsorbTime = 5.0f; // ⏱ 자동 흡수 시간

    private Transform player;
    private bool isAbsorbing = false;
    private float spawnTime;

    private void Start()
    {
        var pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj != null) player = pObj.transform;

        spawnTime = Time.time; // 생성 시각 저장
    }

    private void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // ✅ 자동 흡수 조건: 일정 시간 지난 경우
        if (!isAbsorbing && (dist < absorbRange || Time.time - spawnTime > autoAbsorbTime))
        {
            isAbsorbing = true;
        }

        if (isAbsorbing)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                absorbSpeed * Time.deltaTime
            );

            dist = Vector3.Distance(transform.position, player.position);

            if (dist < 0.5f)
            {
                if (isGold)
                {
                    DataManager.Instance.AddGold(amount);
                    Debug.Log($"[ItemPickup] 골드 {amount} 획득! 현재 골드: {DataManager.Instance.userInfo.gold}");
                }
                else
                {
                    DataManager.Instance.AddItem(itemCode, amount);
                    Debug.Log($"[ItemPickup] {itemCode} x{amount} 획득!");
                }

                Destroy(gameObject);
            }
        }
    }
}
