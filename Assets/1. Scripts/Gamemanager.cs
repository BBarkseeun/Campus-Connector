using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Gamemanager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    //public GameObject UIInformation;
    //public GameObject tprotation;

    // Start is called before the first frame update
    void Start()
    {
        // 방을 생성하거나 조인합니다.
        PhotonNetwork.JoinOrCreateRoom("YourUniqueRoomName", new RoomOptions { MaxPlayers = 6 }, null);
    }

    /*void Update()
    {
        if (UIInformation != null && tprotation != null)
        {
            UIInformation.transform.rotation = tprotation.transform.rotation;
        }
    }*/

    public override void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate("Character Root", new Vector3(0, 0, 0), Quaternion.identity);

        /*GameObject character = GameObject.Find("Character Root(Clone)");
        if (character != null)
        {
            tprotation = character.transform.Find("TP Camera Rig").gameObject;
            UIInformation = character.transform.Find("Information").gameObject;
        }*/
    }

}