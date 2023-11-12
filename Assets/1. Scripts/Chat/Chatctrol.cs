using UnityEngine;
using UnityEngine.UI;

public class Chatctrol : MonoBehaviour
{
    public GameObject chatPanel; // Chat Panel 게임 오브젝트를 Inspector에서 연결해줍니다.
    public Button chatButton;    // ChatBtn 버튼을 Inspector에서 연결해줍니다.
    public Button closeBtn;      // CloseBtn 버튼을 Inspector에서 연결해줍니다.

    // ChatBtn 버튼을 눌렀을 때 호출되는 메서드
    public void OnChatButtonClick()
    {
        // Chat Panel을 활성화/비활성화합니다.
        chatPanel.SetActive(!chatPanel.activeSelf);

        // Chat Panel이 활성화되면 ChatBtn 버튼을 비활성화합니다.
        if (chatPanel.activeSelf)
        {
            chatButton.gameObject.SetActive(false);
        }
    }

    // CloseBtn 버튼을 눌렀을 때 호출되는 메서드
    public void OnCloseButtonClick()
    {
        // Chat Panel을 비활성화하고 ChatBtn 버튼을 활성화합니다.
        chatPanel.SetActive(false);
        chatButton.gameObject.SetActive(true);
    }
}

