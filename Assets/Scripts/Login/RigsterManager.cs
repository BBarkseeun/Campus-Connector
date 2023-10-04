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
    DatabaseReference reference;

    // ������ ������ ��ü
    Firebase.Auth.FirebaseAuth auth;

    private Queue<System.Action> actionsToExecute = new Queue<System.Action>();

    private void Update()
    {
        while (actionsToExecute.Count > 0)
        {
            Debug.Log("Executing an action from the queue.");
            actionsToExecute.Dequeue().Invoke();
        }
    }

    void Awake()
    {
        // ��ü �ʱ�ȭ
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void CheckDuplicateEmail()
    {
        string emailToCheck = emailField.text;

        // ��ȿ�� �̸��� �������� ���� �˻�
        if (!IsValidEmail(emailToCheck))
        {
            Debug.LogError("hallym.ac.kr ������ �̸��� �ּҸ� ���˴ϴ�.");
            alertDialog.ShowAlert("hallym.ac.kr ������ �̸��� �ּҸ� ���˴ϴ�.");
            return;
        }

        // �ӽ� ��й�ȣ�� ����� �߰��� �õ�
        auth.CreateUserWithEmailAndPasswordAsync(emailToCheck, "temporaryPassword1234").ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
            {
                // ����� �߰� ���� = �̸��� �ߺ� �ƴ�. �ٷ� ����� ����.
                Firebase.Auth.FirebaseUser newUser = task.Result.User;
                newUser.DeleteAsync();
                Debug.Log("��� ������ ���̵� �Դϴ�.");
                actionsToExecute.Enqueue(() => alertDialog.ShowAlert("��� ������ ���̵� �Դϴ�."));
            }
            else
            {
                if (task.Exception != null)
                {
                    string errorMessage = task.Exception.InnerException.Message;
                    if (errorMessage.Contains("The email address is already in use by another account."))
                    {
                        Debug.Log("�̹� �����ϴ� ���̵� �Դϴ�."); // ���� �޽��� ��� �Ϲ� �α� �޽����� ����
                        actionsToExecute.Enqueue(() => alertDialog.ShowAlert("�̹� �����ϴ� ���̵� �Դϴ�."));
                    }
                    else
                    {
                        Debug.LogError("���� �߻�: " + errorMessage);
                    }
                }

            }

        });
    }


    public void register()
    {
        /*
        // �̸��� ��ȿ�� �˻�
        if (!IsValidEmail(emailField.text))
        {
            Debug.LogError("hallym.ac.kr ������ �̸��� �ּҸ� ���˴ϴ�.");
            alertDialog.ShowAlert("hallym.ac.kr ������ �̸��� �ּҸ� ���˴ϴ�.");
            return;
        }
        */
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
                    SaveEmailToDatabase(emailField.text);
                    actionsToExecute.Enqueue(() => SceneManager.LoadScene("Login"));
                }

                /*
                else
                {
                    Debug.LogError("�̹� �����ϴ� ���̵� �Դϴ�. �ٸ� ���̵� �Է��ϼ���");
                    actionsToExecute.Enqueue(() => alertDialog.ShowAlert("�̹� �����ϴ� ���̵� �Դϴ�. �ٸ� ���̵� �Է��ϼ���"));
                }
                */
            }
        );
    }

    void SaveEmailToDatabase(string email)
    {
        string userId = auth.CurrentUser.UserId;
        reference.Child("users").Child(userId).Child("email").SetValueAsync(email);
    }

    private bool IsValidEmail(string email)
    {
        return email.EndsWith("@hallym.ac.kr");
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
