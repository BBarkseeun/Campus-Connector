using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;

public class ChatManager : MonoBehaviourPunCallbacks, IPunObservable
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
    DatabaseReference databaseReference;

    void Start()
    {
        // ���� ������� �ػ󵵸� ���ɴϴ�.
        Resolution currentResolution = Screen.currentResolution;
        // ���� �ػ󵵸� ����Ͽ� ȭ�� ����
        Screen.SetResolution(currentResolution.width, currentResolution.height, true);
        m_ContentText = m_Content.transform.GetChild(0).gameObject;
        photonview = GetComponent<PhotonView>();

        // Firebase �ʱ�ȭ
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {task.Exception}");
                return;
            }
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        });
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
        // Firebase Auth �ν��Ͻ��� �����ɴϴ�.
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;

        if (user != null)
        {
            // Firebase Database���� ������� �г����� �����ɴϴ�.
            databaseReference.Child("users").Child(user.UserId).Child("nickname").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Error retrieving user data: " + task.Exception);
                    return;
                }

                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists && snapshot.Value != null)
                {
                    m_strUserName = snapshot.Value.ToString();
                }
                else
                {
                    Debug.LogError("Nickname does not exist for user: " + user.UserId);
                    m_strUserName = "user" + Random.Range(0, 100); // ���� �� ���� �г��� ����
                }

                PhotonNetwork.LocalPlayer.NickName = m_strUserName;
                AddChatMessage("Connected user: " + m_strUserName);
            });
        }
        else
        {
            Debug.LogError("No user is currently logged in.");
        }

        // ĳ���� ������Ʈ�� ã�Ƽ� ChatBox�� ChatBox_Text�� �ʱ�ȭ�մϴ�.
        InitializeChatComponents();
    }

    void InitializeChatComponents()
    {
        // ĳ���� ������Ʈ�� ã�Ƽ� ChatBox�� ChatBox_Text�� �ʱ�ȭ�մϴ�.
        GameObject character = GameObject.Find("Character Root(Clone)"); // ĳ���� �������� �̸��� �Է����ּ���.
        if (character != null)
        {
            ChatBox = character.GetComponent<CharacterInteraction>().ChatCanvas;
            if (ChatBox != null)
            {
                ChatBox_Text = ChatBox.transform.Find("Canvas/Image/Balloon").GetComponent<TMP_Text>();
            }

            tprotation = character.transform.Find("TP Camera").gameObject;
            if (tprotation != null)
            {
                Debug.Log("TP Camera Rig�� ã�ҽ��ϴ�!");
            }
            else
            {
                Debug.LogError("TP Camera Rig�� ã�� �� �����ϴ�!");
            }
        }
    }

    public void OnEndEditEvent()
    {
        if (!string.IsNullOrEmpty(m_inputField.text))
        {
            string strMessage = m_strUserName + " : " + m_inputField.text;
            photonview.RPC("RPC_Chat", RpcTarget.All, strMessage);
            myView.RPC("OpenChatBox", RpcTarget.AllBuffered, strMessage, PhotonNetwork.LocalPlayer.ActorNumber); // ActorNumber �߰�
            m_inputField.text = "";
        }
    }

    void AddChatMessage(string message)
    {
        GameObject goText = Instantiate(m_ContentText, m_Content.transform);
        goText.GetComponent<TextMeshProUGUI>().text = message;

        // ��ũ�� ��ġ�� �Ʒ��� �̵�
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
        // ��� �÷��̾ ã�Ƽ� actorNumber�� ��ġ�ϴ� �÷��̾ ã���ϴ�.
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); // �÷��̾� �±׸� ���� ��� ������Ʈ�� ã���ϴ�.
        GameObject player = null;
        foreach (GameObject p in players)
        {
            if (p.GetComponent<PhotonView>().Owner.ActorNumber == actorNumber) // actorNumber�� ��ġ�ϴ� �÷��̾ ã���ϴ�.
            {
                player = p;
                break;
            }
        }

        if (player != null)
        {
            ChatBox = player.GetComponent<CharacterInteraction>().ChatCanvas; // �ش� �÷��̾��� ä�� �ڽ��� ã���ϴ�.
            if (ChatBox != null)
            {
                ChatBox_Text = ChatBox.transform.Find("Canvas/Image/Balloon").GetComponent<TMP_Text>();
                ChatBox.SetActive(true);
                ChatBox_Text.text = message; // ���ο� ä�� �޽����� ����
                StartCoroutine(DelayCloseChatBox(5.0f)); // 5�� �Ŀ� ê�ڽ��� �ݵ��� ����
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
        if (stream.IsWriting)
        {
            stream.SendNext(ChatBox_Text.text);
        }
        else
        {
            ChatBox_Text.text = (string)stream.ReceiveNext();
        }
    }
}
