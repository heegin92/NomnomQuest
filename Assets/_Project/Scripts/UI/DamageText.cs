using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [SerializeField] private float floatUpSpeed = 1f;
    [SerializeField] private float duration = 1f;

    [Header("치명타 연출")]
    [SerializeField] private float critScale = 1.5f;      // 최대 크기
    [SerializeField] private float critPunchDuration = 0.2f; // 팡! 하고 커졌다가 줄어드는 시간

    private TextMeshPro textMesh;
    private Color originalColor;
    private float elapsed;
    private Transform cam;

    // 치명타 관련
    private bool isCrit;
    private float critElapsed;

    private void Awake()
    {
        textMesh = GetComponentInChildren<TextMeshPro>();
        if (textMesh == null)
        {
            Debug.LogError("[DamageText] TextMeshPro 없음!");
            return;
        }

        textMesh.sortingOrder = 9999; // 항상 최상단
        cam = Camera.main != null ? Camera.main.transform : null;
    }

    public void Init(int damage, bool isCrit, Vector3 worldPos)
    {
        if (textMesh == null) return;

        transform.position = worldPos + Vector3.up * 1.2f;

        this.isCrit = isCrit;

        if (isCrit)
        {
            textMesh.text = $"<b>{damage}</b>";
            textMesh.color = Color.yellow;
            transform.localScale = Vector3.one * 0.8f; // 처음엔 살짝 작게 시작
            critElapsed = 0f;
        }
        else
        {
            textMesh.text = damage.ToString();
            textMesh.color = Color.red;
            transform.localScale = Vector3.one;
        }

        originalColor = textMesh.color;
    }

    private void Update()
    {
        if (textMesh == null) return;

        elapsed += Time.deltaTime;

        // === 치명타 크기 애니메이션 ===
        if (isCrit && critElapsed < critPunchDuration)
        {
            critElapsed += Time.deltaTime;
            float t = critElapsed / critPunchDuration;

            // EaseOutBack 느낌: 빠르게 커졌다가 원래 크기로
            float scale = Mathf.Lerp(critScale, 1f, t);
            transform.localScale = Vector3.one * scale;
        }

        // 카메라 빌보드
        if (cam != null)
            transform.LookAt(transform.position + cam.forward);

        // 위로 이동
        transform.position += Vector3.up * floatUpSpeed * Time.deltaTime;

        // 투명도 감소
        float fadeT = elapsed / duration;
        textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1 - fadeT);

        if (elapsed >= duration) Destroy(gameObject);
    }
}
