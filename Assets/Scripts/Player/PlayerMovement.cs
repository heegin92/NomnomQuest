using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float fixedY = 0.5f; // 캐릭터가 걷는 고정 높이

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(moveX, 0, moveZ).normalized * moveSpeed * Time.deltaTime;
        transform.position += move;

        // 항상 고정 Y값 유지 (지형 기울기 무시)
        transform.position = new Vector3(transform.position.x, fixedY, transform.position.z);
    }
}
