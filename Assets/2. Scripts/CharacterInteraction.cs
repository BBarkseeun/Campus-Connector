using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CharacterInteraction : MonoBehaviourPun
{
    public Camera characterCamera;

    public GameObject InfomationCanvas;
    public GameObject ChatCanvas;

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
                    Debug.Log("Å¬¸¯");
                    
                    GameObject canvas = clickedObject.GetComponent<CharacterInteraction>().InfomationCanvas;

                    if (canvas.activeSelf){
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