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

    public void register()
    {

        if (!IsValidPassword(passField.text))
        {
            Debug.LogError("비밀번호는 최소 6자 이상이며 특수문자를 포함해야 합니다.");
            alertDialog.ShowAlert("비밀번호는 최소 6자 이상이며 특수문자를 포함해야 합니다.");
            return;
        }

        if (!IsPasswordMatching(passField.text, confirmPassField.text)) // 추가: 비밀번호 일치 여부 확인
        {
            Debug.LogError("비밀번호가 일치하지 않습니다.");
            alertDialog.ShowAlert("비밀번호가 일치하지 않습니다.");
            return;
        }

        auth.CreateUserWithEmailAndPasswordAsync(emailField.text, passField.text).ContinueWith(
            task =>
            {
                if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
                {
                    Debug.Log(emailField.text + "로 회원가입 성공");
                    // 로그인 성공 시 씬 전환
                    actionsToExecute.Enqueue(() => SceneManager.LoadScene("Login"));
                }
                else
                {
                    Debug.LogError("이미 존재하는 아이디 입니다. 다른 아이디를 입력하세요");
                    actionsToExecute.Enqueue(() => alertDialog.ShowAlert("이미 존재하는 아이디 입니다. 다른 아이디를 입력하세요"));
                }
            }
        );
    }

    private bool IsPasswordMatching(string password, string confirmPassword)
    {
        return password == confirmPassword;
    }


    // 비밀번호 유효성 검사

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
