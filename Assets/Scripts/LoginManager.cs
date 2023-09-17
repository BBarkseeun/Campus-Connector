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

    // AlertDialog 참조
    public AlertDialog alertDialog;

    // 인증을 관리할 객체
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
        // 객체 초기화
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    public void login()
    {
        auth.SignInWithEmailAndPasswordAsync(emailField.text, passField.text).ContinueWith(
            task =>
            {
                if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
                {
                    Debug.Log(emailField.text + " 로 로그인 하셨습니다.");

                    // 로그인 성공 시 씬 전환
                    actionsToExecute.Enqueue(() => SceneManager.LoadScene("main"));
                }
                else
                {
                    Debug.LogError("로그인실패");
                    // 로그인 실패 시 알람창 띄우기
                    actionsToExecute.Enqueue(() => alertDialog.ShowAlert("아이디 또는 비밀번호를 확인하셈"));
                }
            }
        );
    }

    public void register()
    {
        // 비밀번호 유효성 검사
        if (!IsValidPassword(passField.text))
        {
            Debug.LogError("비밀번호는 최소 6자 이상이며 특수문자를 포함해야 합니다.");
            alertDialog.ShowAlert("비밀번호는 최소 6자 이상이며 특수문자를 포함해야 합니다.");
            return;
        }

        auth.CreateUserWithEmailAndPasswordAsync(emailField.text, passField.text).ContinueWith(
            task =>
            {
                if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
                {
                    Debug.Log(emailField.text + "로 회원가입 성공");
                }
                else
                {
                    Debug.LogError("이미 존재하는 아이디 입니다. 다른 아이디를 입력하세요");
                    actionsToExecute.Enqueue(() => alertDialog.ShowAlert("이미 존재하는 아이디 입니다. 다른 아이디를 입력하세요"));
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