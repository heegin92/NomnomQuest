using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform target; // 따라갈 대상(플레이어)
    private Vector3 offset; // 초기 거리 저장

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("[PlayerCamera] 타겟이 비어있습니다!");
            return;
        }

        // 처음 카메라와 플레이어 사이의 거리 저장
        offset = transform.position - target.position;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * 5f); // 5는 따라오는 속도
    }

}
