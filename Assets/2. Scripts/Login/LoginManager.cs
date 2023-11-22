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

                // 이메일 인증 확인
                if (!user.IsEmailVerified)
                    {
                        actionsToExecute.Enqueue(() => alertDialog.ShowAlert("이메일 인증이 필요합니다. 이메일을 확인하십시오."));
                        return;
                    }

                    string userId = user.UserId;

                // 데이터베이스에서 닉네임 확인
                reference.Child("users").Child(userId).Child("nickname").GetValueAsync().ContinueWithOnMainThread(
                        nicknameTask =>
                        {
                            if (nicknameTask.IsCompleted && !nicknameTask.IsFaulted && !nicknameTask.IsCanceled)
                            {
                                DataSnapshot snapshot = nicknameTask.Result;
                                if (snapshot.Exists)
                                {
                                // 닉네임이 이미 존재하면 메인 씬으로 이동
                                actionsToExecute.Enqueue(() => SceneManager.LoadScene("SampleScene_2"));
                                }
                                else
                                {
                                // 닉네임이 없으면 닉네임 생성 씬으로 이동
                                actionsToExecute.Enqueue(() => SceneManager.LoadScene("name"));
                                }
                            }
                            else
                            {
                            // 닉네임 확인에 실패하면 로그에 경고 메시지 출력
                            Debug.LogWarning("닉네임 확인에 실패했습니다.");
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
                // 로그인 실패 시 알람창 띄우기
                actionsToExecute.Enqueue(() => alertDialog.ShowAlert("아이디 또는 비밀번호를 확인하세요"));
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
