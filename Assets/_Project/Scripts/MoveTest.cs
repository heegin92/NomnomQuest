using UnityEngine;

public class MoveTest : MonoBehaviour
{
    public float Speed { get; private set; } = 1f;
    private Vector2 m_InputPosition;
    public void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            m_InputPosition = Input.mousePosition;// ���콺 Ŭ�� ���� �� ��ġ
        }
        if(Input.GetMouseButton(0))
        {
            var mouseDuration = (Vector2)Input.mousePosition - m_InputPosition;     //���콺 �巹�� �Ѹ�ŭ ���� ����, �ְ�ӵ��� �����ϴ��� �ؾ���
            //mouseDuration = mouseDuration.normalized;       //�ְ�ӵ��� 1�� ����(normalized�� ���Ͱ��� 1�� ����)
            var speed = new Vector3(mouseDuration.x, 0, mouseDuration.y) * Speed * Time.deltaTime;  //Time.deltaTime���� ���ǵ带 ����Ƽ �ӵ��� ������, mouseDuration.y���� ��� ���� Y�� ���� ����, ����°�� ���� Z�� ����
            if (speed.magnitude > 0)
            {
                transform.Translate(speed);     //speed�� ��ŭ �̵�
            }
        }
    }
}
