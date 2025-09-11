using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform target; // 캐릭터
    [SerializeField] private Transform pivot;  // 카메라 중심 피봇
    [SerializeField] private Vector3 offset = new Vector3(0, 10f, -10f);
    [SerializeField] private float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null || pivot == null) return;

        // Pivot은 항상 캐릭터 위치
        pivot.position = target.position;

        // 카메라 목표 위치 = pivot 위치 + offset
        Vector3 desiredPos = pivot.position + offset;

        // 부드럽게 이동
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);

        // pivot 바라보기
        transform.LookAt(pivot);
    }
}
