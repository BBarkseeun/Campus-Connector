using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;
using TMPro;
using Firebase.Database;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class RigsterManager : MonoBehaviour
{
    public TMP_InputField emailField;
    public TMP_InputField passField;
    public TMP_InputField confirmPassField;

    // AlertDialog ����
    public AlertDialog alertDialog;

    // ������ ������ ��ü
    Firebase.Auth.FirebaseAuth auth;

    private Queue<System.Action> actionsToExecute = new Queue<System.Action>();

    private void Update()
    {
        while (actionsToExecute.Count > 0)
        {
            actionsToExecute.Dequeue().Invoke();
        }
    }

    void Awake()
    {
        // ��ü �ʱ�ȭ
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    public void register()
    {

        if (!IsValidPassword(passField.text))
        {
            Debug.LogError("��й�ȣ�� �ּ� 6�� �̻��̸� Ư�����ڸ� �����ؾ� �մϴ�.");
            alertDialog.ShowAlert("��й�ȣ�� �ּ� 6�� �̻��̸� Ư�����ڸ� �����ؾ� �մϴ�.");
            return;
        }

        if (!IsPasswordMatching(passField.text, confirmPassField.text)) // �߰�: ��й�ȣ ��ġ ���� Ȯ��
        {
            Debug.LogError("��й�ȣ�� ��ġ���� �ʽ��ϴ�.");
            alertDialog.ShowAlert("��й�ȣ�� ��ġ���� �ʽ��ϴ�.");
            return;
        }

        auth.CreateUserWithEmailAndPasswordAsync(emailField.text, passField.text).ContinueWith(
            task =>
            {
                if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
                {
                    Debug.Log(emailField.text + "�� ȸ������ ����");
                    // �α��� ���� �� �� ��ȯ
                    actionsToExecute.Enqueue(() => SceneManager.LoadScene("Login"));
                }
                else
                {
                    Debug.LogError("�̹� �����ϴ� ���̵� �Դϴ�. �ٸ� ���̵� �Է��ϼ���");
                    actionsToExecute.Enqueue(() => alertDialog.ShowAlert("�̹� �����ϴ� ���̵� �Դϴ�. �ٸ� ���̵� �Է��ϼ���"));
                }
            }
        );
    }

    private bool IsPasswordMatching(string password, string confirmPassword)
    {
        return password == confirmPassword;
    }


    // ��й�ȣ ��ȿ�� �˻�

    private bool IsValidPassword(string password)
    {
        if (password.Length < 6) return false;

        bool containsSpecialChar = false;
        foreach (var ch in password)
        {
            if (!char.IsLetterOrDigit(ch))
            {
                containsSpecialChar = true;
                break;
            }
        }
        if (!containsSpecialChar) return false;

        return true;
    }
}
