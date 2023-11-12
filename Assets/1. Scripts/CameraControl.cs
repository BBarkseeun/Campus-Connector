using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform target;  // 카메라가 따라다닐 대상(캐릭터 등)
    public float sensitivity = 2.0f;  // 마우스 감도

    void Update()
    {
        // 마우스 입력을 감지하여 회전 각도 계산
        float mouseX = Input.GetAxis("Mouse X");

        // 캐릭터의 회전 각도 설정
        target.Rotate(Vector3.up * mouseX * sensitivity);

        // 가상 카메라를 캐릭터와 똑같이 회전
        transform.rotation = target.rotation;
    }
}
