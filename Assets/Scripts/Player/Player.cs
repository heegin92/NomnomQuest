using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("HUD 연결 (에디터에서 할당)")]
    [SerializeField] private PlayerHUD hud;
    public PlayerHUD HUD => hud;

    [Header("기본 상태값")]
    [SerializeField] private int maxHP = 100;
    [SerializeField] private int hp;

    private Rigidbody2D rb2d;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        hp = maxHP;
    }

    private void Start()
    {
        // HUD가 비어있으면 씬에서 자동으로 한 번 찾아보기 (선택)
        if (hud == null)
            hud = FindObjectOfType<PlayerHUD>();
    }

    public void ResetForStage()
    {
        hp = maxHP;
        if (rb2d) rb2d.velocity = Vector2.zero;
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
