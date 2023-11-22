using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerMovement : MonoBehaviourPun
{
    Animator anim;
    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;
    public float speed;
    float hAxis;
    float vAxis;
    bool wDown;
    Vector3 moveVec;

    void Awake()
    {
        anim = gameObject.GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            // ���� �÷��̾��� �̵� ����

            hAxis = Input.GetAxisRaw("Horizontal");
            vAxis = Input.GetAxisRaw("Vertical");
            wDown = Input.GetButton("Run");

            moveVec = new Vector3(hAxis, 0, vAxis).normalized;
            transform.position += moveVec * speed * (wDown ? 2f : 1f) * Time.deltaTime;

            anim.SetBool("isWalk", moveVec != Vector3.zero);
            anim.SetBool("isRun", wDown);
            transform.LookAt(transform.position + moveVec);

            // �̵� �����͸� ����ȭ
            photonView.RPC("SyncMovement", RpcTarget.Others, transform.position, transform.rotation);
        }
    }

    [PunRPC]
    void SyncMovement(Vector3 position, Quaternion rotation)
    {
        // �ٸ� �÷��̾��� �̵� �����͸� �޾ƿͼ� ����
        transform.position = position;
        transform.rotation = rotation;
    }
}