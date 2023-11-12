using UnityEngine;
using TMPro;
using System;

public class ChatController : MonoBehaviour
{
    [SerializeField]
    private GameObject textChatPrefab;
    [SerializeField]
    private Transform parentContent;

    [SerializeField]
    private TMP_InputField inputField;

    private string userID = "User";
    private string aiName = "AI Bot";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && inputField.isFocused == false)
        {
            inputField.ActivateInputField();
        }
    }

    public void OnEditEventMethod()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            UpdateChat();
        }
    }

    public void UpdateChat()
    {
        if (inputField.text.Equals("")) return;

        GameObject clone = Instantiate(textChatPrefab, parentContent);

        string message = inputField.text;
        string response = "";

        // 사용자가 "/help"를 입력하면 AI가 "Hello"로 대답하도록 설정
        if (message.Equals("/help"))
        {
            response = "무슨 도움이 필요하신가요?";
        }

        clone.GetComponent<TextMeshProUGUI>().text = $"{userID} : {message}";
        inputField.text = "";

        // AI의 응답을 생성하고 표시
        if (!string.IsNullOrEmpty(response))
        {
            GameObject aiResponse = Instantiate(textChatPrefab, parentContent);
            aiResponse.GetComponent<TextMeshProUGUI>().text = $"{aiName} : {response}";
        }
    }
}




