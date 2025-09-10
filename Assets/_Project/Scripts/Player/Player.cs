using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Player : MonoBehaviour
{
    [Header("HUD ���� (�����Ϳ��� �Ҵ�)")]
    [SerializeField] private PlayerHUD hud;
    public PlayerHUD HUD => hud;

    [Header("�⺻ ���°�")]
    [SerializeField] private int maxHP = 100;
    [SerializeField] private int hp;

    [Header("Move")]
    [SerializeField] float moveSpeed = 6f;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;      // �߳� �� ������Ʈ
    [SerializeField] float groundRadius = 0.15f; // �߳� �� ������
    [SerializeField] LayerMask groundMask;       // Ground ���̾� ����

    Rigidbody2D rb;
    float inputX;
    bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // ���� �¾�(�ν����Ϳ��� �������� �� ���)
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.freezeRotation = true; // Z ȸ�� ���
        hp = maxHP;
    }

    private void Start()
    {
        // HUD�� ��������� ������ �ڵ����� �� �� ã�ƺ��� (����)
        if (hud == null)
            hud = FindObjectOfType<PlayerHUD>();
    }
    void Update()
    {
        // �¿� �Է�
        inputX = Input.GetAxisRaw("Horizontal");
        // �ٴ� üũ (OverlapCircle)
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);

        // �̵�
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
        // �ʿ��ϸ� �߰� �ʱ�ȭ(�ִϸ��̼�/����/�Է»��� ��) ���⿡
    }

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        if (hp <= 0) Die();
    }

    private void Die()
    {
        Debug.Log("�÷��̾� ���");
        // TODO: ������/���ӿ��� ó��
    }
}
