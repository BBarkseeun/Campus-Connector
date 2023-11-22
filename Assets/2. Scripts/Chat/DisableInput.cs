using UnityEngine;
using TMPro;

public class DisableInput : MonoBehaviour
{
    // TMP InputField 참조를 저장합니다.
    public TMP_InputField inputField;

    private void Update()
    {
        // TMP InputField가 활성화되어 있을 때만 입력을 무시합니다.
        if (inputField.isFocused == true)
        {
            // 방향키와 스페이스바, W, A, S, D 키 입력을 무시합니다.
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) ||
                Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            {
                // 입력을 처리하지 않고 무시합니다.
            }
        }
    }
}



