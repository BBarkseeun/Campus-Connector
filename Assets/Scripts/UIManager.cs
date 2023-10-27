using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TMP_InputField inputField; // 검색창
    public Button searchActionButton; // 실제로 검색을 실행하는 버튼
    public Button searchToggleButton; // 텍스트 필드와 검색 버튼을 토글하는 버튼

    private void Start()
    {
        inputField.gameObject.SetActive(false);
        searchActionButton.gameObject.SetActive(false);
    }

    public void ToggleSearchUI()
    {
        bool currentState = inputField.gameObject.activeSelf;
        inputField.gameObject.SetActive(!currentState);
        searchActionButton.gameObject.SetActive(!currentState);

        if (!currentState)
        {
            inputField.Select();
            inputField.ActivateInputField();
        }
    }
}
