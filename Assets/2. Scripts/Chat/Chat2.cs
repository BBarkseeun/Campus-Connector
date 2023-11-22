using UnityEngine;
using TMPro;
using System;

public class Chat2 : MonoBehaviour
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
    }
}

