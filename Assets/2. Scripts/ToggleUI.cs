using UnityEngine;
using UnityEngine.UI;

public class ToggleUI : MonoBehaviour
{
    public GameObject inputField;
    public GameObject toggleButton; // Ȱ��ȭ/��Ȱ��ȭ �� �ٸ� ��ư

    // UI ����� ����/���� ���¸� ����մϴ�.
    public void ToggleVisibility()
    {
        inputField.SetActive(!inputField.activeSelf);
        toggleButton.SetActive(!toggleButton.activeSelf);
    }
}
