using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    // Rigidbody2D 컴포넌트 참조. 물리적인 움직임을 제어하는 데 사용됩니다.
    protected Rigidbody2D _rigidbody;

    // 캐릭터의 스프라이트 렌더러 (인스펙터에서 할당)
    [SerializeField] private SpriteRenderer spriteRenderer;
    // 무기 피봇(회전 지점)의 Transform (인스펙터에서 할당)
    [SerializeField] private Transform weaponPivot;

    // 캐릭터의 현재 이동 방향 (기본값: (0,0))
    protected Vector2 movementDirection = Vector2.zero;
    // 이동 방향을 외부에서 읽을 수 있는 프로퍼티
    public Vector2 MovementDirection { get { return movementDirection; } }

    // 캐릭터의 현재 바라보는 방향 (기본값: (0,0))
    protected Vector2 lookDirection = Vector2.zero;
    // 바라보는 방향을 외부에서 읽을 수 있는 프로퍼티
    public Vector2 LookDirection { get { return lookDirection; } }

    // 넉백 벡터 (넉백 시 캐릭터에 적용되는 힘)
    private Vector2 knockback = Vector2.zero;
    // 넉백이 적용되는 남은 시간
    private float knockbackDuration = 0f;


    protected bool isAttacking;
    private float timeSinceLastAttack = float.MaxValue;

    // --- Unity Life Cycle Methods ---

    // 오브젝트가 생성될 때 가장 먼저 호출됩니다.
    // 주로 컴포넌트 초기화에 사용됩니다.
    protected virtual void Awake()
    {
        // Rigidbody2D 컴포넌트를 가져와서 _rigidbody 변수에 할당합니다.
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // 첫 번째 프레임 업데이트 직전에 호출됩니다.
    // 주로 초기화 로직에 사용됩니다.
    protected virtual void Start()
    {
        // 이 기본 클래스에서는 특별한 초기화가 없습니다.
    }

    // 매 프레임마다 호출됩니다. (프레임 레이트에 따라 다름)
    // 사용자 입력 처리나 시각적인 업데이트에 적합합니다.
    protected virtual void Update()
    {
        // 캐릭터의 특정 동작을 처리합니다. (예: 공격, 상호작용)
        HandleAction();
        // 캐릭터가 바라보는 방향으로 회전시킵니다.
        Rotate(lookDirection);
    }

    // 고정된 프레임마다 호출됩니다. (물리 업데이트에 적합)
    // Rigidbody를 통한 움직임 제어는 FixedUpdate에서 하는 것이 좋습니다.
    protected virtual void FixedUpdate()
    {
        // 현재 이동 방향에 따라 캐릭터를 움직입니다.
        Movement(movementDirection);
        // 넉백 지속 시간이 남아있으면 감소시킵니다.
        if (knockbackDuration > 0f)
        {
            knockbackDuration -= Time.fixedDeltaTime;
        }
    }

    // --- Core Methods ---

    // 자식 클래스에서 재정의하여 특정 동작을 구현할 수 있습니다.
    protected virtual void HandleAction()
    {
        // 이 기본 클래스에서는 특별한 동작이 없습니다.
    }

    // 캐릭터를 주어진 방향으로 이동시킵니다.
    private void Movement(Vector2 direction)
    {

        // 넉백이 적용 중이면 이동 속도를 줄이고 넉백 힘을 추가합니다.
        if (knockbackDuration > 0f)
        {
            direction *= 0.2f; // 이동 속도 감소
            direction += knockback; // 넉백 힘 추가
        }

        // Rigidbody의 속도를 설정하여 캐릭터를 움직입니다.
        _rigidbody.velocity = direction;
    }



    // 캐릭터와 무기를 주어진 방향으로 회전시킵니다.
    private void Rotate(Vector2 direction)
    {
        // Atan2를 사용하여 방향 벡터의 각도를 계산합니다. (라디안을 도로 변환)
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // z축 회전 값이 90도를 초과하면 캐릭터가 왼쪽을 바라보는 것으로 간주합니다.
        bool isLeft = Mathf.Abs(rotZ) > 90f;

        // characterRenderer는 아마도 spriteRenderer의 오타일 것입니다.
        // 캐릭터의 스프라이트를 좌우 반전시킵니다.
        spriteRenderer.flipX = isLeft;
    }

    // --- Public Methods ---

    // 캐릭터에게 넉백을 적용합니다.
    // other: 넉백을 가한 오브젝트의 Transform
    // power: 넉백의 힘
    // duration: 넉백이 지속될 시간
    public void ApplyKnockback(Transform other, float power, float duration)
    {
        // 넉백 지속 시간을 더합니다.
        knockbackDuration += duration;
        // 넉백 방향을 계산합니다. (넉백을 가한 오브젝트에서 현재 오브젝트로 향하는 방향)
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




