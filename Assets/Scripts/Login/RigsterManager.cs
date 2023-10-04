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
    DatabaseReference reference;

    // 인증을 관리할 객체
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
        // 객체 초기화
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void CheckDuplicateEmail()
    {
        string emailToCheck = emailField.text;

        // 유효한 이메일 형식인지 먼저 검사
        if (!IsValidEmail(emailToCheck))
        {
            Debug.LogError("hallym.ac.kr 형식의 이메일 주소만 허용됩니다.");
            alertDialog.ShowAlert("hallym.ac.kr 형식의 이메일 주소만 허용됩니다.");
            return;
        }

        // 임시 비밀번호로 사용자 추가를 시도
        auth.CreateUserWithEmailAndPasswordAsync(emailToCheck, "temporaryPassword1234").ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
            {
                // 사용자 추가 성공 = 이메일 중복 아님. 바로 사용자 삭제.
                Firebase.Auth.FirebaseUser newUser = task.Result.User;
                newUser.DeleteAsync();
                Debug.Log("사용 가능한 아이디 입니다.");
                actionsToExecute.Enqueue(() => alertDialog.ShowAlert("사용 가능한 아이디 입니다."));
            }
            else
            {
                if (task.Exception != null)
                {
                    string errorMessage = task.Exception.InnerException.Message;
                    if (errorMessage.Contains("The email address is already in use by another account."))
                    {
                        Debug.Log("이미 존재하는 아이디 입니다."); // 오류 메시지 대신 일반 로그 메시지로 변경
                        actionsToExecute.Enqueue(() => alertDialog.ShowAlert("이미 존재하는 아이디 입니다."));
                    }
                    else
                    {
                        Debug.LogError("오류 발생: " + errorMessage);
                    }
                }

            }

        });
    }


    public void register()
    {
        /*
        // 이메일 유효성 검사
        if (!IsValidEmail(emailField.text))
        {
            Debug.LogError("hallym.ac.kr 형식의 이메일 주소만 허용됩니다.");
            alertDialog.ShowAlert("hallym.ac.kr 형식의 이메일 주소만 허용됩니다.");
            return;
        }
        */
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
                    SaveEmailToDatabase(emailField.text);
                    actionsToExecute.Enqueue(() => SceneManager.LoadScene("Login"));
                }

                /*
                else
                {
                    Debug.LogError("이미 존재하는 아이디 입니다. 다른 아이디를 입력하세요");
                    actionsToExecute.Enqueue(() => alertDialog.ShowAlert("이미 존재하는 아이디 입니다. 다른 아이디를 입력하세요"));
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
