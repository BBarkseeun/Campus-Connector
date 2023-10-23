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

    public AlertDialog alertDialog;
    DatabaseReference reference;

    FirebaseAuth auth;
    private Queue<System.Action> actionsToExecute = new Queue<System.Action>();

    private void Update()
    {
        while (actionsToExecute.Count > 0)
        {
            actionsToExecute.Dequeue().Invoke();
        }
    }

    public void RegisterAndSendVerificationEmail()
    {
        if (!IsValidEmail(emailField.text))
        {
            alertDialog.ShowAlert("hallym.ac.kr 형식의 이메일 주소만 허용됩니다.");
            return;
        }

        if (!IsValidPassword(passField.text))
        {
            alertDialog.ShowAlert("비밀번호는 최소 6자 이상이며 특수문자를 포함해야 합니다.");
            return;
        }

        if (!IsPasswordMatching(passField.text, confirmPassField.text))
        {
            alertDialog.ShowAlert("비밀번호가 일치하지 않습니다.");
            return;
        }

        auth.CreateUserWithEmailAndPasswordAsync(emailField.text, passField.text).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle error messages and queue them for execution in the main thread.
            }
            else if (task.IsCompleted)
            {
                FirebaseUser newUser = auth.CurrentUser;
                newUser.SendEmailVerificationAsync().ContinueWith(verificationTask =>
                {
                    if (verificationTask.IsCompleted)
                    {
                        actionsToExecute.Enqueue(() =>
                        {
                            alertDialog.OnConfirm -= HandleOnConfirm;  // Remove previous handler
                            alertDialog.OnConfirm += HandleOnConfirm;  // Add new handler
                            alertDialog.ShowAlert("인증 이메일을 전송했습니다. 확인해주세요.");
                        });
                    }
                    else
                    {
                        actionsToExecute.Enqueue(() => alertDialog.ShowAlert("인증 이메일 전송에 실패했습니다."));
                    }
                });
            }
        });
    }

    private void HandleOnConfirm()
    {
        SceneManager.LoadScene("Login");
        alertDialog.OnConfirm -= HandleOnConfirm;  // Important: Remove the handler to avoid accumulation
    }

    void Awake()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private bool IsValidEmail(string email)
    {
        return email.EndsWith("@hallym.ac.kr");
    }

    private bool IsPasswordMatching(string password, string confirmPassword)
    {
        return password == confirmPassword;
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
}
