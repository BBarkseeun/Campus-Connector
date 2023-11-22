using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlertDialog : MonoBehaviour
{
    public GameObject alertDialog; // �˶�â Panel�� ����
    public TMP_Text alertText; // �˶� �޽����� ǥ���� Text�� ����
    public Button confirmButton; // The button the user clicks to acknowledge the alert.

    public delegate void ConfirmAction();
    public event ConfirmAction OnConfirm;

    private void Start()
    {
        confirmButton.onClick.AddListener(HandleConfirmClick);
    }

    public void ShowAlert(string message)
    {
        alertText.text = message;
        alertDialog.SetActive(true);
    }

    private void HandleConfirmClick()
    {
        CloseAlert();

        // If there are any subscribers to the OnConfirm event, invoke them.
        OnConfirm?.Invoke();
    }

    public void CloseAlert()
    {
        alertDialog.SetActive(false);
    }
}
