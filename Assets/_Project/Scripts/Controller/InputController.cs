using UnityEngine;

public static class InputController
{
    public static Vector3 InputDistance { get; private set; }
    public static bool IsTouching { get; private set; }

    private static Vector3 startTouch;

    public static void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouch = (Vector3)touch.position; // Vector3로 변환
                IsTouching = true;
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                Vector3 drag = (Vector3)touch.position - startTouch; // Vector3 - Vector3
                InputDistance = drag.normalized; // 방향만 사용
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                IsTouching = false;
                InputDistance = Vector3.zero;
            }
        }
        else
        {
            IsTouching = false;
            InputDistance = Vector3.zero;
        }
    }
}
