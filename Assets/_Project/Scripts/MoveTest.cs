using UnityEngine;

public class MoveTest : MonoBehaviour
{
    public float Speed { get; private set; } = 1f;
    private Vector2 m_InputPosition;
    public void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            m_InputPosition = Input.mousePosition;// 마우스 클릭 했을 때 위치
        }
        if(Input.GetMouseButton(0))
        {
            var mouseDuration = (Vector2)Input.mousePosition - m_InputPosition;     //마우스 드레그 한만큼 힘을 받음, 최고속도를 제한하던가 해야함
            //mouseDuration = mouseDuration.normalized;       //최고속도를 1로 제한(normalized는 백터값을 1로 제한)
            var speed = new Vector3(mouseDuration.x, 0, mouseDuration.y) * Speed * Time.deltaTime;  //Time.deltaTime으로 스피드를 유니티 속도에 맞춰줌, mouseDuration.y값이 가운데 가면 Y축 기준 게임, 세번째로 가면 Z축 게임
            if (speed.magnitude > 0)
            {
                transform.Translate(speed);     //speed값 만큼 이동
            }
        }
    }
}
