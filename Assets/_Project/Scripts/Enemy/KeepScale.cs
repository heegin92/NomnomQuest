using UnityEngine;

/// <summary>
/// ������ ���� ũ�⸦ �����ϸ鼭 �¿� �ø��� ���.
/// </summary>
public class KeepScale : MonoBehaviour
{
    private Vector3 baseScale;

    private void Awake()
    {
        // Inspector���� ������ �⺻ ũ�� ��� (��: 0.4, 0.4, 0.4)
        baseScale = transform.localScale;
    }

    private void LateUpdate()
    {
        // ���� ���� (�¿�)�� �ݿ�
        float signX = Mathf.Sign(transform.localScale.x);

        transform.localScale = new Vector3(
            baseScale.x * signX,
            baseScale.y,
            baseScale.z
        );
    }
}
