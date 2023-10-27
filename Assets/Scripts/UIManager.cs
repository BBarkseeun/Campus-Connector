using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TMP_InputField inputField; // �˻�â
    public Button searchActionButton; // ������ �˻��� �����ϴ� ��ư
    public Button searchToggleButton; // �ؽ�Ʈ �ʵ�� �˻� ��ư�� ����ϴ� ��ư

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
