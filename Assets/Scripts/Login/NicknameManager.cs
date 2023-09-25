using System.Collections;
using System.Collections.Generic;  // 이 네임스페이스를 추가합니다.
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class NicknameManager : MonoBehaviour
{
    public TMP_InputField nicknameField;  // 닉네임 입력 필드
    public Button submitButton;  // 제출 버튼

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
                            // 닉네임이 이미 존재하면 경고 메시지 출력
                            Debug.LogWarning("닉네임이 이미 사용 중입니다.");
                        }
                        else
                        {
                            // 닉네임이 사용 가능하면 저장하고 다음 씬으로 이동
                            string userId = auth.CurrentUser.UserId;
                            Task setNicknameTask = reference.Child("users").Child(userId).Child("nickname").SetValueAsync(nickname);
                            Task setNicknameIndexTask = reference.Child("nicknames").Child(nickname).SetValueAsync(userId);
                            Task.WhenAll(setNicknameTask, setNicknameIndexTask).ContinueWith(
                                saveTask =>
                                {
                                    if (saveTask.IsCompleted && !saveTask.IsFaulted && !saveTask.IsCanceled)
                                    {
                                        // 닉네임 저장에 성공하면 다음 씬으로 이동
                                        actionsToExecute.Enqueue(() => SceneManager.LoadScene("main"));

                                    }
                                    else
                                    {
                                        // 닉네임 저장에 실패하면 로그에 경고 메시지 출력
                                        Debug.LogWarning("닉네임 저장에 실패했습니다.");
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
                        // 닉네임 확인 작업에 실패하면 로그에 경고 메시지 출력
                        Debug.LogWarning("닉네임 확인에 실패했습니다.");
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
            Debug.LogWarning("닉네임을 입력하세요.");
        }
    }
}
