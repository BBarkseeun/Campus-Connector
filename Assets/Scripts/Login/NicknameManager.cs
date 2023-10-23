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
    public TMP_InputField nicknameField;  // 닉네임 입력 필드
    public Button submitButton;  // 제출 버튼

    // AlertDialog 참조
    public AlertDialog alertDialog;

    FirebaseAuth auth;
    DatabaseReference reference;

    private Queue<System.Action> actionsToExecute = new Queue<System.Action>();

    void Awake()
    {
        // Firebase 초기화
        auth = FirebaseAuth.DefaultInstance;
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        // 버튼의 클릭 이벤트 설정
        submitButton.onClick.AddListener(SubmitNickname);
    }

    void Update()
    {
        // 큐에 예약된 모든 작업을 실행합니다.
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
            alertDialog.ShowAlert("닉네임은 12글자 이내로 입력해주세요.");
            return;
        }

        // Check if the nickname contains any special characters using Regex
        if (!System.Text.RegularExpressions.Regex.IsMatch(nickname, "^[a-zA-Z0-9]*$"))
        {
            alertDialog.ShowAlert("닉네임에는 특수문자를 사용할 수 없습니다.");
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
                            Debug.LogError("닉네임이 이미 사용 중입니다.");
                            actionsToExecute.Enqueue(() => alertDialog.ShowAlert("이미 사용중인 닉네임입니다."));
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
                                        alertDialog.ShowAlert("닉네임 저장에 실패했습니다.");
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
                        alertDialog.ShowAlert("닉네임 확인에 실패했습니다.");
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
            alertDialog.ShowAlert("닉네임을 입력하세요.");
        }
    }
}