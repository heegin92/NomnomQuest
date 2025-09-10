using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Player : MonoBehaviour
{
    [Header("HUD 연결 (에디터에서 할당)")]
    [SerializeField] private PlayerHUD hud;
    public PlayerHUD HUD => hud;

    [Header("기본 상태값")]
    [SerializeField] private int maxHP = 100;
    [SerializeField] private int hp;

    [Header("Move")]
    [SerializeField] float moveSpeed = 6f;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;      // 발끝 빈 오브젝트
    [SerializeField] float groundRadius = 0.15f; // 발끝 원 반지름
    [SerializeField] LayerMask groundMask;       // Ground 레이어 지정

    Rigidbody2D rb;
    float inputX;
    bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // 안전 셋업(인스펙터에서 깜빡했을 때 대비)
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.freezeRotation = true; // Z 회전 잠금
        hp = maxHP;
    }

    private void Start()
    {
        // HUD가 비어있으면 씬에서 자동으로 한 번 찾아보기 (선택)
        if (hud == null)
            hud = FindObjectOfType<PlayerHUD>();
    }
    void Update()
    {
        // 좌우 입력
        inputX = Input.GetAxisRaw("Horizontal");
        // 바닥 체크 (OverlapCircle)
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);

        // 이동
        float targetVelX = inputX * moveSpeed;
        rb.velocity = new Vector2(targetVelX, rb.velocity.y);
    }
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }

    public void ResetForStage()
    {
        hp = maxHP;
        if (rb) rb.velocity = Vector2.zero;
        // 필요하면 추가 초기화(애니메이션/버프/입력상태 등) 여기에
    }

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        if (hp <= 0) Die();
    }

    private void Die()
    {
        Debug.Log("플레이어 사망");
        // TODO: 리스폰/게임오버 처리
    }
}
