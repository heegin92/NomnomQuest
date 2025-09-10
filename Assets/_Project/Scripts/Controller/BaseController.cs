using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    // Rigidbody2D ������Ʈ ����. �������� �������� �����ϴ� �� ���˴ϴ�.
    protected Rigidbody2D _rigidbody;

    // ĳ������ ��������Ʈ ������ (�ν����Ϳ��� �Ҵ�)
    [SerializeField] private SpriteRenderer spriteRenderer;
    // ���� �Ǻ�(ȸ�� ����)�� Transform (�ν����Ϳ��� �Ҵ�)
    [SerializeField] private Transform weaponPivot;

    // ĳ������ ���� �̵� ���� (�⺻��: (0,0))
    protected Vector2 movementDirection = Vector2.zero;
    // �̵� ������ �ܺο��� ���� �� �ִ� ������Ƽ
    public Vector2 MovementDirection { get { return movementDirection; } }

    // ĳ������ ���� �ٶ󺸴� ���� (�⺻��: (0,0))
    protected Vector2 lookDirection = Vector2.zero;
    // �ٶ󺸴� ������ �ܺο��� ���� �� �ִ� ������Ƽ
    public Vector2 LookDirection { get { return lookDirection; } }

    // �˹� ���� (�˹� �� ĳ���Ϳ� ����Ǵ� ��)
    private Vector2 knockback = Vector2.zero;
    // �˹��� ����Ǵ� ���� �ð�
    private float knockbackDuration = 0f;


    protected bool isAttacking;
    private float timeSinceLastAttack = float.MaxValue;

    // --- Unity Life Cycle Methods ---

    // ������Ʈ�� ������ �� ���� ���� ȣ��˴ϴ�.
    // �ַ� ������Ʈ �ʱ�ȭ�� ���˴ϴ�.
    protected virtual void Awake()
    {
        // Rigidbody2D ������Ʈ�� �����ͼ� _rigidbody ������ �Ҵ��մϴ�.
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // ù ��° ������ ������Ʈ ������ ȣ��˴ϴ�.
    // �ַ� �ʱ�ȭ ������ ���˴ϴ�.
    protected virtual void Start()
    {
        // �� �⺻ Ŭ���������� Ư���� �ʱ�ȭ�� �����ϴ�.
    }

    // �� �����Ӹ��� ȣ��˴ϴ�. (������ ����Ʈ�� ���� �ٸ�)
    // ����� �Է� ó���� �ð����� ������Ʈ�� �����մϴ�.
    protected virtual void Update()
    {
        // ĳ������ Ư�� ������ ó���մϴ�. (��: ����, ��ȣ�ۿ�)
        HandleAction();
        // ĳ���Ͱ� �ٶ󺸴� �������� ȸ����ŵ�ϴ�.
        Rotate(lookDirection);
    }

    // ������ �����Ӹ��� ȣ��˴ϴ�. (���� ������Ʈ�� ����)
    // Rigidbody�� ���� ������ ����� FixedUpdate���� �ϴ� ���� �����ϴ�.
    protected virtual void FixedUpdate()
    {
        // ���� �̵� ���⿡ ���� ĳ���͸� �����Դϴ�.
        Movement(movementDirection);
        // �˹� ���� �ð��� ���������� ���ҽ�ŵ�ϴ�.
        if (knockbackDuration > 0f)
        {
            knockbackDuration -= Time.fixedDeltaTime;
        }
    }

    // --- Core Methods ---

    // �ڽ� Ŭ�������� �������Ͽ� Ư�� ������ ������ �� �ֽ��ϴ�.
    protected virtual void HandleAction()
    {
        // �� �⺻ Ŭ���������� Ư���� ������ �����ϴ�.
    }

    // ĳ���͸� �־��� �������� �̵���ŵ�ϴ�.
    private void Movement(Vector2 direction)
    {

        // �˹��� ���� ���̸� �̵� �ӵ��� ���̰� �˹� ���� �߰��մϴ�.
        if (knockbackDuration > 0f)
        {
            direction *= 0.2f; // �̵� �ӵ� ����
            direction += knockback; // �˹� �� �߰�
        }

        // Rigidbody�� �ӵ��� �����Ͽ� ĳ���͸� �����Դϴ�.
        _rigidbody.velocity = direction;
    }



    // ĳ���Ϳ� ���⸦ �־��� �������� ȸ����ŵ�ϴ�.
    private void Rotate(Vector2 direction)
    {
        // Atan2�� ����Ͽ� ���� ������ ������ ����մϴ�. (������ ���� ��ȯ)
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // z�� ȸ�� ���� 90���� �ʰ��ϸ� ĳ���Ͱ� ������ �ٶ󺸴� ������ �����մϴ�.
        bool isLeft = Mathf.Abs(rotZ) > 90f;

        // characterRenderer�� �Ƹ��� spriteRenderer�� ��Ÿ�� ���Դϴ�.
        // ĳ������ ��������Ʈ�� �¿� ������ŵ�ϴ�.
        spriteRenderer.flipX = isLeft;
    }

    // --- Public Methods ---

    // ĳ���Ϳ��� �˹��� �����մϴ�.
    // other: �˹��� ���� ������Ʈ�� Transform
    // power: �˹��� ��
    // duration: �˹��� ���ӵ� �ð�
    public void ApplyKnockback(Transform other, float power, float duration)
    {
        // �˹� ���� �ð��� ���մϴ�.
        knockbackDuration += duration;
        // �˹� ������ ����մϴ�. (�˹��� ���� ������Ʈ���� ���� ������Ʈ�� ���ϴ� ����)
        knockback = -(other.position - transform.position).normalized * power;
    }

  
    protected virtual void Attack()
    {

    }

    public virtual void Death()
    {
        _rigidbody.velocity = Vector3.zero;

        foreach (SpriteRenderer renderer in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            Color color = renderer.color;
            color.a = 0.3f;
            renderer.color = color;
        }

        foreach (Behaviour componet in transform.GetComponentsInChildren<Behaviour>())
        {
            componet.enabled = false;
        }

        Destroy(gameObject, 2f);
    }
}




