using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public class NicknameManager : MonoBehaviour
{
    public TMP_InputField nicknameField; // �г��� �Է� �ʵ�
    public TMP_InputField studentIDField; // �й� �Է� �ʵ�
    public TMP_InputField majorField; // ���� �Է� �ʵ�
    public TMP_InputField nameField; // �̸� �Է� �ʵ�
    public Button submitButton; // ���� ��ư
    // AlertDialog ����
    public AlertDialog alertDialog;

    FirebaseAuth auth;
    DatabaseReference reference;

    private Queue<System.Action> actionsToExecute = new Queue<System.Action>();

    void Awake()
    {
        // Firebase �ʱ�ȭ
        auth = FirebaseAuth.DefaultInstance;
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        // ��ư�� Ŭ�� �̺�Ʈ ����
        submitButton.onClick.AddListener(SubmitData);
    }

    void Update()
    {
        // ť�� ����� ��� �۾��� �����մϴ�.
        while (actionsToExecute.Count > 0)
        {
            actionsToExecute.Dequeue().Invoke();
        }
    }

    void SubmitData()
    {
        string nickname = nicknameField.text;
        string studentID = studentIDField.text;
        string major = majorField.text;
        string name = nameField.text; // �̸� �ʵ忡�� �ؽ�Ʈ ��������

        // �г���, �й�, ����, �̸� ��ȿ�� �˻� �߰�
        if (!ValidateNickname(nickname) || !ValidateStudentID(studentID) || !ValidateMajor(major) || !ValidateName(name))
        {
            return;
        }

        // Firebase�� ������ ����
        SaveDataToFirebase(nickname, studentID, major, name);
    }
    bool ValidateNickname(string nickname)
    {
        // �г����� 12���� �̳��� �Է��ϵ��� �ϸ� Ư�� ���ڸ� ����� �� �����ϴ�.
        if (nickname.Length > 12)
        {
            alertDialog.ShowAlert("�г����� 12���� �̳��� �Է����ּ���.");
            return false;
        }
        if (!Regex.IsMatch(nickname, "^[a-zA-Z0-9��-�R]*$"))
        {
            alertDialog.ShowAlert("�г��ӿ��� Ư�����ڸ� ����� �� �����ϴ�.");
            return false;
        }
        return true;
    }

    bool ValidateStudentID(string studentID)
    {
        // �й��� ���� 8�ڸ��� �Է¹޽��ϴ�.
        if (!Regex.IsMatch(studentID, @"^\d{8}$"))
        {
            alertDialog.ShowAlert("�й��� ���� 8�ڸ��� �Է����ּ���.");
            return false;
        }
        return true;
    }

    bool ValidateMajor(string major)
    {
        // ������ Ư�����ڸ� �������� �ʴ� ���ڿ��� �����մϴ�.
        if (!Regex.IsMatch(major, @"^[a-zA-Z��-�R\s]*$"))
        {
            alertDialog.ShowAlert("������ Ư�����ڸ� ����� �� �����ϴ�.");
            return false;
        }
        return true;
    }
    bool ValidateName(string name)
    {
        // �̸� ��ȿ�� �˻� ������ �߰��մϴ�.
        // ���� ���, �̸��� ��� ���� �ʾƾ� �ϸ� Ư�� ���ڸ� �������� �ʴ� ������ �����մϴ�.
        if (string.IsNullOrEmpty(name))
        {
            alertDialog.ShowAlert("�̸��� �Է����ּ���.");
            return false;
        }
        if (!Regex.IsMatch(name, @"^[a-zA-Z��-�R\s]*$"))
        {
            alertDialog.ShowAlert("�̸��� Ư�����ڸ� ����� �� �����ϴ�.");
            return false;
        }
        return true;
    }

    void SaveDataToFirebase(string nickname, string studentID, string major, string name)
    {
        if (int.TryParse(studentID, out int idNumber))
        {
            string userId = auth.CurrentUser.UserId;
            // Firebase Database�� �̸��� �����Ͽ� �г���, �й�(������), ������ �����մϴ�.
            Task setNicknameTask = reference.Child("users").Child(userId).Child("nickname").SetValueAsync(nickname);
            Task setStudentIDTask = reference.Child("users").Child(userId).Child("studentID").SetValueAsync(idNumber);
            Task setMajorTask = reference.Child("users").Child(userId).Child("major").SetValueAsync(major);
            Task setNameTask = reference.Child("users").Child(userId).Child("name").SetValueAsync(name); // �̸� ����

            Task.WhenAll(setNicknameTask, setStudentIDTask, setMajorTask, setNameTask).ContinueWith(
                saveTask =>
                {
                    if (saveTask.IsCompleted && !saveTask.IsFaulted && !saveTask.IsCanceled)
                    {
                        actionsToExecute.Enqueue(() => SceneManager.LoadScene("SampleScene_1"));
                    }
                    else
                    {
                        Debug.LogError("������ ���忡 �����߽��ϴ�.");
                        actionsToExecute.Enqueue(() => alertDialog.ShowAlert("������ ���忡 �����߽��ϴ�."));
                        if (saveTask.IsFaulted)
                        {
                            Debug.LogError(saveTask.Exception.ToString());
                            actionsToExecute.Enqueue(() => alertDialog.ShowAlert(saveTask.Exception.ToString()));
                        }
                    }
                }
            );
        }
        else
        {
            alertDialog.ShowAlert("�й��� ���ڷθ� �����Ǿ�� �մϴ�.");
        }
    }
}
