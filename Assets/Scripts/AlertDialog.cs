using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlertDialog : MonoBehaviour
{
    public GameObject alertDialog; // 알람창 Panel을 연결
    public TMP_Text alertText; // 알람 메시지를 표시할 Text를 연결

    public void ShowAlert(string message)
    {
        alertText.text = message;
        alertDialog.SetActive(true);
    }

    public void CloseAlert()
    {
        alertDialog.SetActive(false);
    }
}