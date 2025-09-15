using System.Collections;
using UnityEngine;
using Ironcow.Synapse.Data;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Enemy : MonoBehaviour
{
    [Header("데이터 코드 (예: ENE00001)")]
    [SerializeField] private string enemyCode;

    public EnemyData data;   // EnemyData (SO)
    private int currentHp;

    private Rigidbody rb;
    private Animator animator;
    private Transform player;
    private float lastAttackTime = -999f;
    private Vector3 moveDir = Vector3.zero;

    // ✅ 캐싱용
    private Renderer[] renderers;
    private Color[] originalColors;

    [Header("배회 설정")]
    [SerializeField] private float wanderRadius = 5f;
    [SerializeField] private float wanderInterval = 3f;
    private float lastWanderTime = 0f;
    private Vector3 wanderTarget;
    private bool isWandering = false;

    private Collider groundCollider;

    // ⚡ 코드 세팅 메서드
    public void SetCode(string code)
    {
        enemyCode = code;
        data = DataManager.Instance.GetData<EnemyData>(enemyCode);

        if (data == null)
        {
            Debug.LogError($"[Enemy] EnemyData {enemyCode} 못 찾음!");
            return;
        }

        currentHp = data.hp;
    }

    private void Awake()
    {
        // ⚡ Inspector에 enemyCode가 세팅돼 있으면 자동 로드
        if (!string.IsNullOrEmpty(enemyCode))
            SetCode(enemyCode);

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        animator = GetComponentInChildren<Animator>();

        // ✅ Renderer & 원래 색상 캐싱
        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
                originalColors[i] = renderers[i].material.color;
            else
                originalColors[i] = Color.white;
        }
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

        // Ground Collider 찾기
        GameObject groundObj = GameObject.FindWithTag("Ground");
        if (groundObj != null)
            groundCollider = groundObj.GetComponent<Collider>();
    }


    private void Update()
    {
        if (player != null)
        {
            // y 무시한 평면 거리 계산
            Vector3 enemyPos = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 playerPos = new Vector3(player.position.x, 0, player.position.z);
            float dist = Vector3.Distance(enemyPos, playerPos);

            if (Input.GetKeyDown(KeyCode.P))
            {
                Debug.Log("[Enemy] P pressed, force spawn");
                ItemDropManager.Instance.Spawn("ITE00001", transform.position);
            }

            if (dist <= data.atkRange)
            {
                // 공격 범위 → 이동 멈추고 공격
                moveDir = Vector3.zero;
                LookAtPlayer();
                TryAttack();
            }
            else if (dist <= data.detectRange)
            {
                // 탐지 범위 → 추적
                moveDir = (playerPos - enemyPos).normalized;
                LookAtPlayer();
                if (animator != null) animator.SetBool("IsMove", true);
            }
            else
            {
                // 플레이어가 탐지 범위 밖 → 배회
                Wander();
            }
        }
        else
        {
            // 플레이어 자체가 없을 때 → 배회
            Wander();
        }
    }

    private void FixedUpdate()
    {
        if (moveDir != Vector3.zero)
        {
            Vector3 newPos = transform.position + moveDir * data.walkSpeed * Time.fixedDeltaTime;

            // 바닥 보정
            newPos.y = 0f;
            rb.MovePosition(newPos);

            if (animator != null) animator.SetBool("IsMove", true);
        }
    }

    private void Wander()
    {
        if (groundCollider == null) return;

        if (Time.time > lastWanderTime + wanderInterval && !isWandering)
        {
            lastWanderTime = Time.time;

            Bounds b = groundCollider.bounds;
            float randX = Random.Range(b.min.x, b.max.x);
            float randZ = Random.Range(b.min.z, b.max.z);

            wanderTarget = new Vector3(randX, 0f, randZ);

            isWandering = true;
        }

        if (isWandering)
        {
            Vector3 enemyPos = new Vector3(transform.position.x, 0, transform.position.z);
            moveDir = (wanderTarget - enemyPos).normalized;

            // 목표 지점에 거의 도착하면 배회 종료
            if (Vector3.Distance(enemyPos, wanderTarget) < 0.5f)
            {
                moveDir = Vector3.zero;
                isWandering = false;
                if (animator != null) animator.SetBool("IsMove", false);
            }
        }
    }

    private void LookAtPlayer()
    {
        if (player == null) return;

        Vector3 dir = player.position - transform.position;
        if (dir.x > 0.01f)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (dir.x < -0.01f)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private void TryAttack()
    {
        if (Time.time < lastAttackTime + data.attackCooldown) return;

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
                p.TakeDamage(data.atk);
                Debug.Log($"{data.displayName}이(가) 플레이어 공격! 데미지 {data.atk}");
            }
        }
    }

    public void TakeDamage(int dmg)
    {
        int finalDamage = Mathf.Max(0, dmg - data.def);
        currentHp -= finalDamage;

        Debug.Log($"{data.displayName} 피격! HP: {currentHp}/{data.hp}");

        StartCoroutine(HitFlash());

        if (IsDead())
            Die();
    }

    public bool IsDead() => currentHp <= 0;

    public void Die()
    {
        Debug.Log($"{data.displayName} 사망! EXP {data.exp}, Gold {data.gold}");

        moveDir = Vector3.zero;

        if (animator != null)
            animator.SetBool("IsMove", false);

        DropLoot();
        DropItems();

        StartCoroutine(FadeOutAndDestroy());
    }
    private void DropItems()
    {
        Debug.Log($"[Enemy] DropItems() called, dropItems={data.DropItems}");

        if (string.IsNullOrEmpty(data.DropItems))
        {
            Debug.Log("[Enemy] No drop data");
            return;
        }

        string[] drops = data.DropItems.Split('|');
        foreach (var dropId in drops)
        {
            Debug.Log($"[Enemy] Try spawn {dropId}");
            ItemDropManager.Instance.Spawn(dropId, transform.position);
        }
    }


    private IEnumerator FadeOutAndDestroy()
    {
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);

            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i].material.HasProperty("_Color"))
                {
                    Color c = originalColors[i];
                    c.a = alpha;
                    renderers[i].material.color = c;
                }
            }

            yield return null;
        }

        Destroy(gameObject);
    }

    private IEnumerator HitFlash()
    {
        // 빨강으로 변경
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
                renderers[i].material.color = Color.red;
        }

        yield return new WaitForSeconds(0.1f);

        // 원래 색으로 복구
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
                renderers[i].material.color = originalColors[i];
        }
    }

    private void DropLoot()
    {
        if (data.DropItems == null || data.DropItems.Length == 0) return;

        foreach (var item in data.DropItems)
        {
            if (item == null) continue;
            // TODO: 아이템 드랍 처리
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
