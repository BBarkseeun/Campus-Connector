using Photon.Pun;
using TMPro;
using UnityEngine;

public class UserData
{
    public string UserName;
    public string UserID;
    public string MajorName;
    public string NickName;
}


public class UserDataSync : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private GameObject FPCamera;
    
    private PhotonView _photonView;

    public TMP_Text nameText;
    public TMP_Text studentIdText;
    public TMP_Text majorText;
    public TMP_Text nicknameText;

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
        InitMainCamera();
    }

    private void InitMainCamera()
    {
        if (_photonView.IsMine) FPCamera.SetActive(true);
    }

    public void UpdateUserInfo(UserData data)
    {
        if (_photonView.IsMine)
        {
            nameText.text = data.UserName;
            studentIdText.text = data.UserID;
            majorText.text = data.MajorName;
            nicknameText.text = data.NickName;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) 
    {
        if (stream.IsWriting)
        {
            stream.SendNext(nameText.text);
            stream.SendNext(studentIdText.text);
            stream.SendNext(majorText.text);
            stream.SendNext(nicknameText.text);
        }
        else
        {
            nameText.text = (string)stream.ReceiveNext();
            studentIdText.text = (string)stream.ReceiveNext();
            majorText.text = (string)stream.ReceiveNext();
            nicknameText.text = (string)stream.ReceiveNext();
        }
    }
}
