using UnityEngine;

/// <summary>
/// 프리팹 원래 크기를 유지하면서 좌우 플립만 허용.
/// </summary>
public class KeepScale : MonoBehaviour
{
    private Vector3 baseScale;

    private void Awake()
    {
        // Inspector에서 설정한 기본 크기 기억 (예: 0.4, 0.4, 0.4)
        baseScale = transform.localScale;
    }

    private void LateUpdate()
    {
        // 현재 방향 (좌우)만 반영
        float signX = Mathf.Sign(transform.localScale.x);

        transform.localScale = new Vector3(
            baseScale.x * signX,
            baseScale.y,
            baseScale.z
        );
    }
}
