using UnityEngine;
using UnityEngine.UI;

public class ButtonCl : MonoBehaviour
{
    public GameObject targetObject; // 활성화할 대상 오브젝트

    void Start()
    {
        Button buttonOn = transform.Find("ButtonOn").GetComponent<Button>(); // 활성화 버튼 가져오기
        buttonOn.onClick.AddListener(ButtonOn); // 활성화 함수 등록

        Button buttonOff = transform.Find("ButtonOff").GetComponent<Button>(); // 비활성화 버튼 가져오기
        buttonOff.onClick.AddListener(ButtonOff); // 비활성화 함수 등록
    }

    public void ButtonOn()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(true); // 오브젝트를 활성화
        }
    }

    public void ButtonOff()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(false); // 오브젝트를 비활성화
        }
    }
}





