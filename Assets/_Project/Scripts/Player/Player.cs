using Ironcow.Synapse.Sample.Common;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Player : MonoBehaviour
{
    [Header("이동 속도")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private PlayerHUD hud;
    public PlayerHUD HUD => hud;

    [Header("공격 설정")]
    [SerializeField] private float attackRange = 2f;      // 공격 범위 반경
    [SerializeField] private float attackCooldown = 1f;   // 공격 쿨타임
    [SerializeField] private int attackDamage = 10;       // 공격 데미지

    private Rigidbody rb;
    private Vector3 targetPos;
    private bool isMoving = false;

    private Animator animator;
    private float lastAttackTime = -999f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // PC 클릭
        if (Input.GetMouseButtonDown(0))
            SetTarget(Input.mousePosition);

        // 모바일 터치
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            SetTarget(Input.GetTouch(0).position);

        // 자동 공격 체크
        TryAutoAttack();
    }

    private void FixedUpdate()
    {
        // 공격 중일 땐 이동 애니 차단
        if (animator != null && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            return;

        if (isMoving)
        {
            Vector3 newPos = Vector3.MoveTowards(rb.position, targetPos, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);

            Vector3 dir = targetPos - rb.position;
            dir.y = 0;

            if (dir.sqrMagnitude > 0.01f)
            {
                if (dir.x > 0.01f)
                    transform.localScale = new Vector3(1, 1, 1);
                else if (dir.x < -0.01f)
                    transform.localScale = new Vector3(-1, 1, 1);
            }

            if (Vector3.Distance(rb.position, targetPos) < 0.05f)
            {
                isMoving = false;
                if (animator != null) animator.SetBool("IsMove", false);
            }
        }
    }

    private void SetTarget(Vector3 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Ground")))
        {
            targetPos = new Vector3(hit.point.x, 0f, hit.point.z);
            isMoving = true;
            if (animator != null) animator.SetBool("IsMove", true);
        }
    }

    private void TryAutoAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Enemy"));

        if (hits.Length > 0)
        {
            if (animator != null)
                animator.SetTrigger("IsAttack");

            lastAttackTime = Time.time;
        }
    }

    /// <summary>
    /// 공격 애니메이션 중 Animation Event로 호출
    /// </summary>
    public void OnAttackHit()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Enemy"));

        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
