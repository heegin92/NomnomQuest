using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Player : MonoBehaviour
{
    [Header("이동 속도")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private PlayerHUD hud;
    public PlayerHUD HUD => hud;

    private Rigidbody rb;
    private Vector3 targetPos;
    private bool isMoving = false;

    private Animator animator;

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
        {
            SetTarget(Input.mousePosition);
        }

        // 모바일 터치
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            SetTarget(Input.GetTouch(0).position);
        }
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            Vector3 newPos = Vector3.MoveTowards(rb.position, targetPos, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);

            // 이동 방향 (XZ 평면)
            Vector3 dir = targetPos - rb.position;
            dir.y = 0;

            if (dir.sqrMagnitude > 0.01f)
            {
                // 좌우 이동에 따라 플립만 적용
                if (dir.x > 0.01f)       // 오른쪽 이동
                    transform.localScale = new Vector3(1, 1, 1);
                else if (dir.x < -0.01f) // 왼쪽 이동
                    transform.localScale = new Vector3(-1, 1, 1);
            }

            // 목표 도착 체크
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

        // Ground 레이어만 클릭/터치 허용
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Ground")))
        {
            targetPos = new Vector3(hit.point.x, 0f, hit.point.z); // y=0 고정
            isMoving = true;
            if (animator != null) animator.SetBool("IsMove", true);
        }
    }
}
