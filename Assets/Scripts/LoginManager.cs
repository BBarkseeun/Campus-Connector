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
                    actionsToExecute.Enqueue(() => SceneManager.LoadScene("Main"));
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