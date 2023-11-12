using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CharacterInteraction : MonoBehaviourPun
{
    public Camera characterCamera;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = characterCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.collider.gameObject;
                PhotonView photonView = clickedObject.GetComponent<PhotonView>();

                // Remove "photonView.IsMine" condition
                if (photonView != null)
                {
                    Debug.Log("Ŭ��");
                    GameObject canvas = clickedObject.transform.Find("Information").gameObject;

                    if(canvas.activeSelf){
                        canvas.SetActive(false);
                    }
                    else{
                        canvas.SetActive(true);
                    }
                }
            }
        }
    }
}