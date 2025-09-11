using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform target; // ���� ���(�÷��̾�)
    private Vector3 offset; // �ʱ� �Ÿ� ����

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("[PlayerCamera] Ÿ���� ����ֽ��ϴ�!");
            return;
        }

        // ó�� ī�޶�� �÷��̾� ������ �Ÿ� ����
        offset = transform.position - target.position;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * 5f); // 5�� ������� �ӵ�
    }

}
