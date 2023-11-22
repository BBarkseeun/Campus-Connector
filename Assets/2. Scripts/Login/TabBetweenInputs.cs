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
                int nextIndex = (i + 1) % inputFields.Length; // ���� �ε��� ��� (������ InputField������ ù ��°�� ���ư��ϴ�)
                currentEventSystem.SetSelectedGameObject(inputFields[nextIndex].gameObject, new BaseEventData(currentEventSystem));
                return;
            }
        }

        // ��Ŀ���� ���� ������ ù ��° InputField�� ��Ŀ��
        if (inputFields.Length > 0)
        {
            currentEventSystem.SetSelectedGameObject(inputFields[0].gameObject, new BaseEventData(currentEventSystem));
        }
    }
}
