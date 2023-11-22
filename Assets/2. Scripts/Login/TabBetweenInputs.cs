using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TabBetweenInputs : MonoBehaviour
{
    public TMP_InputField[] inputFields;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SelectNextInputField();
        }
    }

    private void SelectNextInputField()
    {
        EventSystem currentEventSystem = EventSystem.current;

        for (int i = 0; i < inputFields.Length; i++)
        {
            if (inputFields[i].isFocused)
            {
                int nextIndex = (i + 1) % inputFields.Length; // 다음 인덱스 계산 (마지막 InputField에서는 첫 번째로 돌아갑니다)
                currentEventSystem.SetSelectedGameObject(inputFields[nextIndex].gameObject, new BaseEventData(currentEventSystem));
                return;
            }
        }

        // 포커스된 것이 없으면 첫 번째 InputField에 포커스
        if (inputFields.Length > 0)
        {
            currentEventSystem.SetSelectedGameObject(inputFields[0].gameObject, new BaseEventData(currentEventSystem));
        }
    }
}
