using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform target; // ĳ����
    [SerializeField] private Transform pivot;  // ī�޶� �߽� �Ǻ�
    [SerializeField] private Vector3 offset = new Vector3(0, 10f, -10f);
    [SerializeField] private float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null || pivot == null) return;

        // Pivot�� �׻� ĳ���� ��ġ
        pivot.position = target.position;

        // ī�޶� ��ǥ ��ġ = pivot ��ġ + offset
        Vector3 desiredPos = pivot.position + offset;

        // �ε巴�� �̵�
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);

        // pivot �ٶ󺸱�
        transform.LookAt(pivot);
    }
}
