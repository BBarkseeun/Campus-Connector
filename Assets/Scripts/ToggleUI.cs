using UnityEngine;
using UnityEngine.UI;

public class ToggleUI : MonoBehaviour
{
    public GameObject inputField;
    public GameObject toggleButton; // 활성화/비활성화 할 다른 버튼

    // UI 요소의 보임/숨김 상태를 토글합니다.
    public void ToggleVisibility()
    {
        inputField.SetActive(!inputField.activeSelf);
        toggleButton.SetActive(!toggleButton.activeSelf);
    }
}
