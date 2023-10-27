using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Gamemanager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        // ���� �����ϰų� �����մϴ�.
        PhotonNetwork.JoinOrCreateRoom("YourUniqueRoomName", new RoomOptions { MaxPlayers = 6 }, null);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate("Character Root", new Vector3(0, 0, 0), Quaternion.identity);
    }
}