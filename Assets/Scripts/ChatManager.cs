using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class ChatManager : MonoBehaviourPunCallbacks
{
    public GameObject m_Content;
    public TMP_InputField m_inputField;

    PhotonView photonview;

    GameObject m_ContentText;

    string m_strUserName;

    void Start()
    {
        // 현재 모니터의 해상도를 얻어옵니다.
        Resolution currentResolution = Screen.currentResolution;

        // 얻어온 해상도를 사용하여 화면 설정
        Screen.SetResolution(currentResolution.width, currentResolution.height, true);

        m_ContentText = m_Content.transform.GetChild(0).gameObject;
        photonview = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnEndEditEvent();
        }
    }

    public override void OnConnectedToMaster()
    {

        PhotonNetwork.JoinOrCreateRoom("YourUniqueRoomName", new RoomOptions { MaxPlayers = 6 }, null);
    }

    public override void OnJoinedRoom()
    {
        int nRandomKey = Random.Range(0, 100);
        m_strUserName = "user" + nRandomKey;
        PhotonNetwork.LocalPlayer.NickName = m_strUserName;
        AddChatMessage("connect user : " + PhotonNetwork.LocalPlayer.NickName);
    }

    public void OnEndEditEvent()
    {
        if (!string.IsNullOrEmpty(m_inputField.text))
        {
            string strMessage = m_strUserName + " : " + m_inputField.text;
            photonview.RPC("RPC_Chat", RpcTarget.All, strMessage);
            m_inputField.text = "";
        }
    }

    void AddChatMessage(string message)
    {
        GameObject goText = Instantiate(m_ContentText, m_Content.transform);
        goText.GetComponent<TextMeshProUGUI>().text = message;

        // 스크롤 위치를 아래로 이동
        Canvas.ForceUpdateCanvases();
        m_Content.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
    }

    [PunRPC]
    void RPC_Chat(string message)
    {
        AddChatMessage(message);
    }
}
