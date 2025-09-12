using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Player : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("공격 설정")]
    [SerializeField] private float attackRange = 2f;      // 공격 범위
    [SerializeField] private float attackCooldown = 1f;   // 쿨타임
    [SerializeField] private int attackDamage = 10;       // 공격력
    [SerializeField] private PlayerHUD hud;
    public PlayerHUD HUD => hud;

    private int hp = 100;
    private int currentHP;

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

        targetPos = transform.position; // 초기값
    }

    void Update()
    {
        // PC 클릭 이동
        if (Input.GetMouseButtonDown(0))
            SetTarget(Input.mousePosition);

        // 모바일 터치 이동
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            SetTarget(Input.GetTouch(0).position);

        // 자동 공격 체크
        TryAutoAttack();
    }

    private void FixedUpdate()
    {
        // 공격 중일 땐 이동 차단
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
                // 좌우 플립만
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
    private void Start()
    {
        currentHP = hp;
        if (hud != null)
            hud.SetHP(currentHP, hp);
    }
    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        if (hud != null)
            hud.SetHP(currentHP, hp);

        // 피격 효과 실행
        StartCoroutine(HitEffect());

        if (currentHP <= 0) Die();
    }

    private IEnumerator HitEffect()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        List<Color> originalColors = new List<Color>();

        // 원래 색상 저장
        foreach (var r in renderers)
        {
            if (r.material.HasProperty("_Color"))
                originalColors.Add(r.material.color);
            else
                originalColors.Add(Color.white);
        }

        // 빨강으로 변경
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
            {
                renderers[i].material.color = Color.red;
            }
        }

        // 0.1초 정도 유지
        yield return new WaitForSeconds(0.1f);

        // 원래 색으로 복귀
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
            {
                renderers[i].material.color = originalColors[i];
            }
        }
    }

    private void Die()
    {
        Debug.Log("플레이어 사망!");
        // 공격/이동 중단
        isMoving = false;
        if (animator != null) animator.SetBool("IsMove", false);

        // 사망 처리 코루틴 실행
        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        float duration = 2f; // 2초 동안
        float elapsed = 0f;

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        // 원래 색상 저장
        List<Color> originalColors = new List<Color>();
        foreach (var r in renderers)
        {
            if (r.material.HasProperty("_Color"))
                originalColors.Add(r.material.color);
            else
                originalColors.Add(Color.white);
        }

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

        // 완전히 사라진 뒤 오브젝트 제거
        Destroy(gameObject);
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
        // 쿨타임 확인
        if (Time.time < lastAttackTime + attackCooldown)
        {
            return;
        }

        // Enemy 레이어 탐지
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Enemy"));

        if (hits.Length > 0)
        {
            // 가까운 적 찾기
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

                // 좌우 방향만 맞추기
                Vector3 dir = closest.transform.position - transform.position;
                if (dir.x > 0.01f)
                    transform.localScale = new Vector3(1, 1, 1);
                else if (dir.x < -0.01f)
                    transform.localScale = new Vector3(-1, 1, 1);

                // 애니메이션 트리거 발동
                if (animator != null)
                    animator.SetTrigger("IsAttack");

                // ⚠️ 쿨타임은 여기서 안 갱신 → OnAttackHit에서 처리
            }
        }
    }

    public void OnAttackHit()
    {

        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Enemy"));

        if (hits.Length > 0)
        {
            // 가장 가까운 적 1명만 찾기
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

        // ✅ 공격 판정 들어간 순간 쿨타임 갱신
        lastAttackTime = Time.time;
    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
