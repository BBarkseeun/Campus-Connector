using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform target;  // ī�޶� ����ٴ� ���(ĳ���� ��)
    public float sensitivity = 2.0f;  // ���콺 ����

    void Update()
    {
        // ���콺 �Է��� �����Ͽ� ȸ�� ���� ���
        float mouseX = Input.GetAxis("Mouse X");

        // ĳ������ ȸ�� ���� ����
        target.Rotate(Vector3.up * mouseX * sensitivity);

        // ���� ī�޶� ĳ���Ϳ� �Ȱ��� ȸ��
        transform.rotation = target.rotation;
    }
}
