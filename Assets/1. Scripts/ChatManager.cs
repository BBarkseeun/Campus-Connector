using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class ChatManager : MonoBehaviourPunCallbacks,IPunObservable
{
    public GameObject m_Content;
    public TMP_InputField m_inputField;
    public TMPro.TMP_Text ChatBox_Text;
    public PhotonView myView;
    public GameObject ChatBox;
    public GameObject tprotation;

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

        if (ChatBox != null && tprotation != null)
        {
            ChatBox.transform.rotation = tprotation.transform.rotation;
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
        
        // 캐릭터 오브젝트를 찾아서 ChatBox와 ChatBox_Text를 초기화합니다.
        GameObject character = GameObject.Find("Character Root(Clone)");  // 캐릭터 프리팹의 이름을 입력해주세요.
        if (character != null)
        {
            ChatBox = character.transform.Find("ChatBalloon").gameObject;
            if (ChatBox != null)
            {
                ChatBox_Text = ChatBox.transform.Find("Canvas/Image/Balloon").GetComponent<TMP_Text>();
            }

            tprotation = character.transform.Find("TP Camera Rig").gameObject;
        }
    }

public void OnEndEditEvent()
{
    if (!string.IsNullOrEmpty(m_inputField.text))
    {
        string strMessage = m_strUserName + " : " + m_inputField.text;
        photonview.RPC("RPC_Chat", RpcTarget.All, strMessage);
        myView.RPC("OpenChatBox", RpcTarget.AllBuffered, strMessage, PhotonNetwork.LocalPlayer.ActorNumber);  // ActorNumber 추가
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

        IEnumerator DelayCloseChatBox(float t)
    {
        yield return new WaitForSeconds(t);
        myView.RPC("CloseChatBox", RpcTarget.AllBuffered);
    }

[PunRPC]
public void OpenChatBox(string message, int actorNumber)
{
    // 모든 플레이어를 찾아서 actorNumber가 일치하는 플레이어를 찾습니다.
    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");  // 플레이어 태그를 가진 모든 오브젝트를 찾습니다.
    GameObject player = null;
    foreach (GameObject p in players)
    {
        if (p.GetComponent<PhotonView>().Owner.ActorNumber == actorNumber)  // actorNumber가 일치하는 플레이어를 찾습니다.
        {
            player = p;
            break;
        }
    }
    
    if (player != null)
    {
        ChatBox = player.transform.Find("ChatBalloon").gameObject;  // 해당 플레이어의 채팅 박스를 찾습니다.
        if (ChatBox != null)
        {
            ChatBox_Text = ChatBox.transform.Find("Canvas/Image/Balloon").GetComponent<TMP_Text>();
            ChatBox.SetActive(true);
            ChatBox_Text.text = message;  // 새로운 채팅 메시지로 설정
            StartCoroutine(DelayCloseChatBox(5.0f));  // 5초 후에 챗박스를 닫도록 설정
        }
    }
}
    [PunRPC]
    public void CloseChatBox()
    {
        ChatBox.SetActive(false);
        
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(ChatBox_Text.text);
        }
        else
        {
            ChatBox_Text.text = (string)stream.ReceiveNext();
        }
    }
}
