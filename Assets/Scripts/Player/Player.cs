using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("HUD ���� (�����Ϳ��� �Ҵ�)")]
    [SerializeField] private PlayerHUD hud;
    public PlayerHUD HUD => hud;

    [Header("�⺻ ���°�")]
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
        // HUD�� ��������� ������ �ڵ����� �� �� ã�ƺ��� (����)
        if (hud == null)
            hud = FindObjectOfType<PlayerHUD>();
    }

    public void ResetForStage()
    {
        hp = maxHP;
        if (rb2d) rb2d.velocity = Vector2.zero;
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
