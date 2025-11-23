using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 따라다닐 대상
    public float smoothSpeed = 0.125f; // 따라가는 속도 (0 ~ 1 사이)
    public Vector3 offset = new Vector3(0, 0, -10); // 카메라 거리 (Z는 -10 필수)

    void Start()
    {
        // 게임 시작 시 'Player' 태그를 가진 놈을 스스로 찾아서 타겟으로 잡음
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
        }
    }

    void LateUpdate()
    {
        // 타겟이 없으면(죽었거나 못 찾았으면) 아무것도 안 함
        if (target == null) return;

        // 목표 위치 계산
        Vector3 desiredPosition = target.position + offset;

        // 부드럽게 이동 (Lerp)
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 위치 적용
        transform.position = smoothedPosition;
    }
}