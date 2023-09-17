using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;
using TMPro;
using Firebase.Database;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;


public class LoginManager : MonoBehaviour
{

    public TMP_InputField emailField;
    public TMP_InputField passField;

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

    public void login()
    {
        auth.SignInWithEmailAndPasswordAsync(emailField.text, passField.text).ContinueWith(
            task =>
            {
                if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
                {
                    Debug.Log(emailField.text + " �� �α��� �ϼ̽��ϴ�.");

                    // �α��� ���� �� �� ��ȯ
                    actionsToExecute.Enqueue(() => SceneManager.LoadScene("main"));
                }
                else
                {
                    Debug.LogError("�α��ν���");
                    // �α��� ���� �� �˶�â ����
                    actionsToExecute.Enqueue(() => alertDialog.ShowAlert("���̵� �Ǵ� ��й�ȣ�� Ȯ���ϼ�"));
                }
            }
        );
    }

    public void register()
    {
        // ��й�ȣ ��ȿ�� �˻�
        if (!IsValidPassword(passField.text))
        {
            Debug.LogError("��й�ȣ�� �ּ� 6�� �̻��̸� Ư�����ڸ� �����ؾ� �մϴ�.");
            alertDialog.ShowAlert("��й�ȣ�� �ּ� 6�� �̻��̸� Ư�����ڸ� �����ؾ� �մϴ�.");
            return;
        }

        auth.CreateUserWithEmailAndPasswordAsync(emailField.text, passField.text).ContinueWith(
            task =>
            {
                if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
                {
                    Debug.Log(emailField.text + "�� ȸ������ ����");
                }
                else
                {
                    Debug.LogError("�̹� �����ϴ� ���̵� �Դϴ�. �ٸ� ���̵� �Է��ϼ���");
                    actionsToExecute.Enqueue(() => alertDialog.ShowAlert("�̹� �����ϴ� ���̵� �Դϴ�. �ٸ� ���̵� �Է��ϼ���"));
                }
            }
        );
    }

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
    public void authentication()
    {

        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            string name = user.DisplayName;
            string email = user.Email;
            System.Uri photo_url = user.PhotoUrl;
            // The user's Id, unique to the Firebase project.
            // Do NOT use this value to authenticate with your backend server, if you
            // have one; use User.TokenAsync() instead.
            string uid = user.UserId;
        }

    }
}