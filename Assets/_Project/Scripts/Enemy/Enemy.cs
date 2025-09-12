using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Enemy : MonoBehaviour
{
    [Header("데이터 참조")]
    public EnemyData data;   // ScriptableObject 참조

    [Header("런타임 상태")]
    private int currentHp;

    private Rigidbody rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private Transform player;
    private float lastAttackTime = -999f;

    // 이동 방향
    private Vector3 moveDir = Vector3.zero;

    private void Awake()
    {
        if (data != null)
            currentHp = data.hp;

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        animator = GetComponentInChildren<Animator>();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    private void Start()
    {
        // GameManager에서 플레이어 찾기
        if (GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            player = GameManager.Instance.Player.transform;
        }

        // 보정: 태그 기반 탐색
        if (player == null)
        {
            var pObj = GameObject.FindWithTag("Player");
            if (pObj != null) player = pObj.transform;
        }
    }

    private void Update()
    {
        if (player == null) return;

        // y 무시한 평면 거리 계산
        Vector3 enemyPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 playerPos = new Vector3(player.position.x, 0, player.position.z);
        float dist = Vector3.Distance(enemyPos, playerPos);

        if (dist <= data.atkRange)
        {
            // 공격 범위 → 이동 멈추고 공격
            moveDir = Vector3.zero;
            LookAtPlayer();   // ✅ 공격 범위 안에서는 플레이어 바라보기
            TryAttack();
        }
        else if (dist <= data.detectRange)
        {
            // 탐지 범위 → 추적
            moveDir = (playerPos - enemyPos).normalized;
            LookAtPlayer();   // ✅ 탐지 범위 안에서도 플레이어 바라보기
        }
        else
        {
            // 범위 밖 → Idle
            moveDir = Vector3.zero;
            if (animator != null) animator.SetBool("IsMove", false);
        }
    }

    private void FixedUpdate()
    {
        if (moveDir != Vector3.zero)
        {
            rb.MovePosition(transform.position + moveDir * data.walkSpeed * Time.fixedDeltaTime);

            if (animator != null) animator.SetBool("IsMove", true);
        }
    }

    private void LookAtPlayer()
    {
        if (player == null) return;

        Vector3 dir = player.position - transform.position;
        if (dir.x > 0.01f)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // 오른쪽 볼 때 반전
        else if (dir.x < -0.01f)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);  // 왼쪽 볼 때 그대로
    }

    private void TryAttack()
    {
        if (Time.time < lastAttackTime + data.attackCooldown)
        {
            Debug.Log("쿨타임 때문에 공격 불가");
            return;
        }

        Debug.Log("공격 시도 → 애니메이션 실행");
        if (animator != null)
            animator.SetTrigger("IsAttack");

        lastAttackTime = Time.time;
    }

    // 애니메이션 이벤트에서 호출됨
    public void OnAttackHit()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= data.atkRange + 0.5f)
        {
            Player p = player.GetComponent<Player>();
            if (p != null)
            {
                Debug.Log($"{data.displayName}이(가) 플레이어 공격! 데미지 {data.atk}");
                p.TakeDamage(data.atk);
            }
        }
    }

    public void TakeDamage(int dmg)
    {
        int finalDamage = Mathf.Max(0, dmg - data.def);
        currentHp -= finalDamage;

        Debug.Log($"{data.displayName} 피격! HP: {currentHp}/{data.hp}");

        if (spriteRenderer != null)
            StartCoroutine(HitFlash());

        if (IsDead())
            Die();
    }

    public bool IsDead() => currentHp <= 0;

    public void Die()
    {
        Debug.Log($"{data.displayName} 사망! EXP {data.exp}, Gold {data.gold}");

        DropLoot();
        Destroy(gameObject);
    }

    private IEnumerator HitFlash()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
        }
    }

    private void DropLoot()
    {
        if (data.DropItems == null || data.DropItems.Length == 0) return;

        foreach (var item in data.DropItems)
        {
            if (item == null) continue;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data != null ? data.detectRange : 5f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data != null ? data.atkRange : 2f);
    }
}
