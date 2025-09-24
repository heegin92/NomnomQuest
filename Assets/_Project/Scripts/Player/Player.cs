using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Player : MonoBehaviour
{
    [Header("HUD 연결")]
    [SerializeField] private PlayerHUD hud;
    public PlayerHUD HUD => hud;

    [Header("데이터")]
    public PlayerData data = new PlayerData();

    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("공격 설정")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float wanderRadius = 5f;
    [SerializeField] private float wanderInterval = 3f;

    // ✅ HP 관리
    public int CurrentHP
    {
        get => data.health;
        set => data.health = Mathf.Clamp(value, 0, MaxHP);
    }
    public int MaxHP => data.maxHealth;

    private Rigidbody rb;
    private Vector3 targetPos;
    private bool isMoving = false;
    private float lastWanderTime = 0f;

    private Animator animator;
    private float lastAttackTime = -999f;

    // ✅ 렌더러 캐싱
    private Renderer[] renderers;
    private Color[] originalColors;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        animator = GetComponentInChildren<Animator>();

        targetPos = transform.position;

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
        if (hud == null)
            hud = FindObjectOfType<PlayerHUD>();

        if (hud != null)
            Debug.Log("[Player] HUD 연결 성공");
        else
            Debug.LogWarning("[Player] HUD 연결 실패");

        CurrentHP = MaxHP;
        data.exp = 0;

        // ✅ 실행 시작 시 Idle로 고정
        if (animator != null)
            animator.SetBool("IsMove", false);
    }


    private void Update()
    {
        // ✅ PC 클릭 이동
        if (Input.GetMouseButtonDown(0))
            SetTarget(Input.mousePosition);

        // ✅ 모바일 터치 이동
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            SetTarget(Input.GetTouch(0).position);

        if (GameManager.Instance != null && GameManager.Instance.IsTown)
        {
            return;
        }

        // ✅ 전투맵일 때만 자동공격 실행
        TryAutoAttack();
    }


    private void FixedUpdate()
    {
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

            // ✅ 도착했으면 Idle 전환
            if (Vector3.Distance(rb.position, targetPos) < 0.05f)
            {
                isMoving = false;
                if (animator != null) animator.SetBool("IsMove", false);
            }
        }
        else
        {
            // ✅ 이동이 끝나면 Idle 유지
            if (animator != null) animator.SetBool("IsMove", false);
        }
    }

    // ✅ 데미지 처리
    public void TakeDamage(int dmg)
    {
        Debug.Log($"[Player] 데미지 받음: {dmg}");

        data.health -= dmg;
        if (data.health < 0) data.health = 0;

        if (DataManager.Instance != null && DataManager.Instance.userInfo != null)
            DataManager.Instance.userInfo.health = data.health;

        StartCoroutine(HitEffect());

        if (CurrentHP <= 0) Die();
    }

    private IEnumerator HitEffect()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
                renderers[i].material.color = Color.red;
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
                renderers[i].material.color = originalColors[i];
        }
    }

    private void Die()
    {
        Debug.Log("플레이어 사망!");
        isMoving = false;
        if (animator != null) animator.SetBool("IsMove", false);
        StartCoroutine(FadeOutAndDestroy());
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

    // ✅ 골드
    public void GainGold(int amount)
    {
        data.gold += amount;
        if (DataManager.Instance != null && DataManager.Instance.userInfo != null)
        {
            DataManager.Instance.userInfo.gold = data.gold;
            DataManager.Instance.SaveData();
        }
    }

    // ✅ 경험치
    public void GainExp(int amount)
    {
        data.AddExp(amount);

        if (DataManager.Instance != null && DataManager.Instance.userInfo != null)
        {
            DataManager.Instance.userInfo.level = data.level;
            DataManager.Instance.userInfo.exp = data.exp;
            DataManager.Instance.userInfo.expToNextLevel = data.expToNextLevel;
            DataManager.Instance.userInfo.attack = data.attack;
            DataManager.Instance.userInfo.health = data.health;
            DataManager.Instance.SaveData();
        }
    }

    private void SetTarget(Vector3 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Ground")))
        {
            Vector3 newTarget = new Vector3(hit.point.x, 0f, hit.point.z);

            // ✅ 너무 가까우면 이동 취소 → Idle 유지
            if (Vector3.Distance(transform.position, newTarget) < 0.1f)
            {
                isMoving = false;
                if (animator != null) animator.SetBool("IsMove", false);
                return;
            }

            targetPos = newTarget;
            isMoving = true;
            if (animator != null) animator.SetBool("IsMove", true);
        }
    }


    private void TryAutoAttack()
    {
        // ✅ 마을일 때는 실행하지 않음
        if (GameManager.Instance != null && GameManager.Instance.IsTown)
            return;

        if (Time.time < lastAttackTime + attackCooldown)
            return;

        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange * 3f, LayerMask.GetMask("Enemy"));

        if (hits.Length > 0)
        {
            Collider closest = null;
            float minDist = float.MaxValue;
            foreach (var h in hits)
            {
                float dist = Vector3.Distance(transform.position, h.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = h;
                }
            }

            if (closest != null)
            {
                float distToEnemy = Vector3.Distance(transform.position, closest.transform.position);

                if (distToEnemy > attackRange)
                {
                    targetPos = new Vector3(closest.transform.position.x, 0f, closest.transform.position.z);
                    isMoving = true;
                    if (animator != null) animator.SetBool("IsMove", true);
                }
                else
                {
                    isMoving = false;
                    if (animator != null) animator.SetBool("IsMove", false);

                    Vector3 dir = closest.transform.position - transform.position;
                    if (dir.x > 0.01f) transform.localScale = new Vector3(1, 1, 1);
                    else if (dir.x < -0.01f) transform.localScale = new Vector3(-1, 1, 1);

                    if (animator != null)
                        animator.SetTrigger("IsAttack");
                }
            }
        }
        else
        {
            // ✅ 전투맵일 때만 배회 허용
            if (Time.time > lastWanderTime + wanderInterval && !isMoving)
            {
                lastWanderTime = Time.time;

                Vector3 randomOffset = new Vector3(
                    UnityEngine.Random.Range(-wanderRadius, wanderRadius),
                    0f,
                    UnityEngine.Random.Range(-wanderRadius, wanderRadius)
                );

                Vector3 randomPos = transform.position + randomOffset;

                if (Physics.Raycast(randomPos + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 20f, LayerMask.GetMask("Ground")))
                {
                    targetPos = new Vector3(hit.point.x, 0f, hit.point.z);
                    isMoving = true;
                    if (animator != null) animator.SetBool("IsMove", true);
                }
            }
        }
    }

    public void OnAttackHit()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Enemy"));

        if (hits.Length > 0)
        {
            Collider closest = null;
            float minDist = float.MaxValue;
            foreach (var h in hits)
            {
                float dist = Vector3.Distance(transform.position, h.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = h;
                }
            }

            if (closest != null)
            {
                Enemy enemy = closest.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(attackDamage);
                    Debug.Log($"적 {enemy.data.displayName} 에게 {attackDamage} 데미지!");
                }
            }
        }

        lastAttackTime = Time.time;

        if (animator != null)
            animator.SetBool("IsMove", false);
        isMoving = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
