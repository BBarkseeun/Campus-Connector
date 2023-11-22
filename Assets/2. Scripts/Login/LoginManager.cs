using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;
using TMPro;
using Firebase.Database;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Linq;
using Firebase.Extensions;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField emailField;
    public TMP_InputField passField;
    public AlertDialog alertDialog;

    FirebaseAuth auth;
    DatabaseReference reference;
    

    private Queue<System.Action> actionsToExecute = new Queue<System.Action>();

    void Awake()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    void Update()
    {
        while (actionsToExecute.Count > 0)
        {
            actionsToExecute.Dequeue().Invoke();
        }
    }

    public void login()
    {
        auth.SignInWithEmailAndPasswordAsync(emailField.text, passField.text).ContinueWith(
            task =>
            {
                if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
                {
                    FirebaseUser user = auth.CurrentUser;

                // �̸��� ���� Ȯ��
                if (!user.IsEmailVerified)
                    {
                        actionsToExecute.Enqueue(() => alertDialog.ShowAlert("�̸��� ������ �ʿ��մϴ�. �̸����� Ȯ���Ͻʽÿ�."));
                        return;
                    }

                    string userId = user.UserId;

                // �����ͺ��̽����� �г��� Ȯ��
                reference.Child("users").Child(userId).Child("nickname").GetValueAsync().ContinueWithOnMainThread(
                        nicknameTask =>
                        {
                            if (nicknameTask.IsCompleted && !nicknameTask.IsFaulted && !nicknameTask.IsCanceled)
                            {
                                DataSnapshot snapshot = nicknameTask.Result;
                                if (snapshot.Exists)
                                {
                                // �г����� �̹� �����ϸ� ���� ������ �̵�
                                actionsToExecute.Enqueue(() => SceneManager.LoadScene("SampleScene_2"));
                                }
                                else
                                {
                                // �г����� ������ �г��� ���� ������ �̵�
                                actionsToExecute.Enqueue(() => SceneManager.LoadScene("name"));
                                }
                            }
                            else
                            {
                            // �г��� Ȯ�ο� �����ϸ� �α׿� ��� �޽��� ���
                            Debug.LogWarning("�г��� Ȯ�ο� �����߽��ϴ�.");
                                if (nicknameTask.IsFaulted)
                                {
                                    Debug.LogError(nicknameTask.Exception.ToString());
                                }
                            }
                        }
                    );
                }
                else
                {
                // �α��� ���� �� �˶�â ����
                actionsToExecute.Enqueue(() => alertDialog.ShowAlert("���̵� �Ǵ� ��й�ȣ�� Ȯ���ϼ���"));
                }
            }
        );
    }

  


    public void authentication()
    {
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            string name = user.DisplayName;
            string email = user.Email;
            System.Uri photo_url = user.PhotoUrl;
            string uid = user.UserId;
        }
    }
}
