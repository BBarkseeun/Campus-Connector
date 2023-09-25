using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlertDialog : MonoBehaviour
{
    public GameObject alertDialog; // �˶�â Panel�� ����
    public TMP_Text alertText; // �˶� �޽����� ǥ���� Text�� ����

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