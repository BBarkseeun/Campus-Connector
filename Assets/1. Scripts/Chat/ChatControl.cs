using UnityEngine;
using TMPro;
using System;

public class ChatControl : MonoBehaviour
{
    [SerializeField]
    private GameObject textChatPrefab;
    [SerializeField]
    private Transform parentContent;

    [SerializeField]
    private TMP_InputField inputField;

    private string ID = "DoctorKo";


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && inputField.isFocused == false)
        {
            inputField.ActivateInputField();
        }
    }
    public void OnEditEventMethod()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            UpdateChat();
        }
    }

    // Start is called before the first frame update
   
   public void UpdateChat()
   {
    if ( inputField.text.Equals("") ) return;

    GameObject clone = Instantiate(textChatPrefab, parentContent);

    clone.GetComponent<TextMeshProUGUI>().text = $"{ID} : {inputField.text}";

    inputField.text = "";
   }
}