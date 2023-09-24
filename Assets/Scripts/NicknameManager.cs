using System.Collections;
using System.Collections.Generic;  // �� ���ӽ����̽��� �߰��մϴ�.
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class NicknameManager : MonoBehaviour
{
    public TMP_InputField nicknameField;  // �г��� �Է� �ʵ�
    public Button submitButton;  // ���� ��ư

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
                            // �г����� �̹� �����ϸ� ��� �޽��� ���
                            Debug.LogWarning("�г����� �̹� ��� ���Դϴ�.");
                        }
                        else
                        {
                            // �г����� ��� �����ϸ� �����ϰ� ���� ������ �̵�
                            string userId = auth.CurrentUser.UserId;
                            Task setNicknameTask = reference.Child("users").Child(userId).Child("nickname").SetValueAsync(nickname);
                            Task setNicknameIndexTask = reference.Child("nicknames").Child(nickname).SetValueAsync(userId);
                            Task.WhenAll(setNicknameTask, setNicknameIndexTask).ContinueWith(
                                saveTask =>
                                {
                                    if (saveTask.IsCompleted && !saveTask.IsFaulted && !saveTask.IsCanceled)
                                    {
                                        // �г��� ���忡 �����ϸ� ���� ������ �̵�
                                        actionsToExecute.Enqueue(() => SceneManager.LoadScene("main"));

                                    }
                                    else
                                    {
                                        // �г��� ���忡 �����ϸ� �α׿� ��� �޽��� ���
                                        Debug.LogWarning("�г��� ���忡 �����߽��ϴ�.");
                                        if (saveTask.IsFaulted)
                                        {
                                            Debug.LogError(saveTask.Exception.ToString());
                                        }
                                    }
                                }
                            );
                        }
                    }
                    else
                    {
                        // �г��� Ȯ�� �۾��� �����ϸ� �α׿� ��� �޽��� ���
                        Debug.LogWarning("�г��� Ȯ�ο� �����߽��ϴ�.");
                        if (task.IsFaulted)
                        {
                            Debug.LogError(task.Exception.ToString());
                        }
                    }
                }
            );
        }
        else
        {
            Debug.LogWarning("�г����� �Է��ϼ���.");
        }
    }
}
