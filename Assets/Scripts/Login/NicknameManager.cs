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
    public TMP_InputField nicknameField;  // �г��� �Է� �ʵ�
    public Button submitButton;  // ���� ��ư

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
        submitButton.onClick.AddListener(SubmitNickname);
    }

    void Update()
    {
        // ť�� ����� ��� �۾��� �����մϴ�.
        while (actionsToExecute.Count > 0)
        {
            actionsToExecute.Dequeue().Invoke();
        }
    }

    void SubmitNickname()
    {
        string nickname = nicknameField.text;

        // Check if the nickname is within the required length
        if (nickname.Length > 12)
        {
            alertDialog.ShowAlert("�г����� 12���� �̳��� �Է����ּ���.");
            return;
        }

        // Check if the nickname contains any special characters using Regex
        if (!System.Text.RegularExpressions.Regex.IsMatch(nickname, "^[a-zA-Z0-9]*$"))
        {
            alertDialog.ShowAlert("�г��ӿ��� Ư�����ڸ� ����� �� �����ϴ�.");
            return;
        }

        if (!string.IsNullOrEmpty(nickname))
        {
            reference.Child("nicknames").Child(nickname).GetValueAsync().ContinueWith(
                task =>
                {
                    if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
                    {
                        DataSnapshot snapshot = task.Result;
                        if (snapshot.Exists)
                        {
                            Debug.LogError("�г����� �̹� ��� ���Դϴ�.");
                            actionsToExecute.Enqueue(() => alertDialog.ShowAlert("�̹� ������� �г����Դϴ�."));
                        }
                        else
                        {
                            string userId = auth.CurrentUser.UserId;
                            Task setNicknameTask = reference.Child("users").Child(userId).Child("nickname").SetValueAsync(nickname);
                            Task setNicknameIndexTask = reference.Child("nicknames").Child(nickname).SetValueAsync(userId);
                            Task.WhenAll(setNicknameTask, setNicknameIndexTask).ContinueWith(
                                saveTask =>
                                {
                                    if (saveTask.IsCompleted && !saveTask.IsFaulted && !saveTask.IsCanceled)
                                    {
                                        actionsToExecute.Enqueue(() => SceneManager.LoadScene("main"));
                                    }
                                    else
                                    {
                                        alertDialog.ShowAlert("�г��� ���忡 �����߽��ϴ�.");
                                        if (saveTask.IsFaulted)
                                        {
                                            alertDialog.ShowAlert(saveTask.Exception.ToString());
                                        }
                                    }
                                }
                            );
                        }
                    }
                    else
                    {
                        alertDialog.ShowAlert("�г��� Ȯ�ο� �����߽��ϴ�.");
                        if (task.IsFaulted)
                        {
                            alertDialog.ShowAlert(task.Exception.ToString());
                        }
                    }
                }
            );
        }
        else
        {
            alertDialog.ShowAlert("�г����� �Է��ϼ���.");
        }
    }
}