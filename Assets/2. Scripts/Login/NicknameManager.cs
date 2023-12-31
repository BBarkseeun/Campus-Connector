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
    public TMP_InputField nicknameField; // 닉네임 입력 필드
    public TMP_InputField studentIDField; // 학번 입력 필드
    public TMP_InputField majorField; // 전공 입력 필드
    public TMP_InputField nameField; // 이름 입력 필드
    public Button submitButton; // 제출 버튼
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
        submitButton.onClick.AddListener(SubmitData);
    }

    void Update()
    {
        // 큐에 예약된 모든 작업을 실행합니다.
        while (actionsToExecute.Count > 0)
        {
            actionsToExecute.Dequeue().Invoke();
        }
    }

    void SubmitData()
    {
        string nickname = nicknameField.text;
        string studentID = studentIDField.text;
        string major = majorField.text;
        string name = nameField.text; // 이름 필드에서 텍스트 가져오기

        // 닉네임, 학번, 전공, 이름 유효성 검사 추가
        if (!ValidateNickname(nickname) || !ValidateStudentID(studentID) || !ValidateMajor(major) || !ValidateName(name))
        {
            return;
        }

        // Firebase에 데이터 저장
        SaveDataToFirebase(nickname, studentID, major, name);
    }
    bool ValidateNickname(string nickname)
    {
        // 닉네임은 12글자 이내로 입력하도록 하며 특수 문자를 사용할 수 없습니다.
        if (nickname.Length > 12)
        {
            alertDialog.ShowAlert("닉네임은 12글자 이내로 입력해주세요.");
            return false;
        }
        if (!Regex.IsMatch(nickname, "^[a-zA-Z0-9가-힣]*$"))
        {
            alertDialog.ShowAlert("닉네임에는 특수문자를 사용할 수 없습니다.");
            return false;
        }
        return true;
    }

    bool ValidateStudentID(string studentID)
    {
        // 학번은 숫자 8자리로 입력받습니다.
        if (!Regex.IsMatch(studentID, @"^\d{8}$"))
        {
            alertDialog.ShowAlert("학번은 숫자 8자리로 입력해주세요.");
            return false;
        }
        return true;
    }

    bool ValidateMajor(string major)
    {
        // 전공은 특수문자를 포함하지 않는 문자열로 제한합니다.
        if (!Regex.IsMatch(major, @"^[a-zA-Z가-힣\s]*$"))
        {
            alertDialog.ShowAlert("전공에 특수문자를 사용할 수 없습니다.");
            return false;
        }
        return true;
    }
    bool ValidateName(string name)
    {
        // 이름 유효성 검사 로직을 추가합니다.
        // 예를 들어, 이름은 비어 있지 않아야 하며 특수 문자를 포함하지 않는 것으로 제한합니다.
        if (string.IsNullOrEmpty(name))
        {
            alertDialog.ShowAlert("이름을 입력해주세요.");
            return false;
        }
        if (!Regex.IsMatch(name, @"^[a-zA-Z가-힣\s]*$"))
        {
            alertDialog.ShowAlert("이름에 특수문자를 사용할 수 없습니다.");
            return false;
        }
        return true;
    }

    void SaveDataToFirebase(string nickname, string studentID, string major, string name)
    {
        if (int.TryParse(studentID, out int idNumber))
        {
            string userId = auth.CurrentUser.UserId;
            // Firebase Database에 이름을 포함하여 닉네임, 학번(정수형), 전공을 저장합니다.
            Task setNicknameTask = reference.Child("users").Child(userId).Child("nickname").SetValueAsync(nickname);
            Task setStudentIDTask = reference.Child("users").Child(userId).Child("studentID").SetValueAsync(idNumber);
            Task setMajorTask = reference.Child("users").Child(userId).Child("major").SetValueAsync(major);
            Task setNameTask = reference.Child("users").Child(userId).Child("name").SetValueAsync(name); // 이름 저장

            Task.WhenAll(setNicknameTask, setStudentIDTask, setMajorTask, setNameTask).ContinueWith(
                saveTask =>
                {
                    if (saveTask.IsCompleted && !saveTask.IsFaulted && !saveTask.IsCanceled)
                    {
                        actionsToExecute.Enqueue(() => SceneManager.LoadScene("SampleScene_2"));
                    }
                    else
                    {
                        Debug.LogError("데이터 저장에 실패했습니다.");
                        actionsToExecute.Enqueue(() => alertDialog.ShowAlert("데이터 저장에 실패했습니다."));
                        if (saveTask.IsFaulted)
                        {
                            Debug.LogError(saveTask.Exception.ToString());
                            actionsToExecute.Enqueue(() => alertDialog.ShowAlert(saveTask.Exception.ToString()));
                        }
                    }
                }
            );
        }
        else
        {
            alertDialog.ShowAlert("학번은 숫자로만 구성되어야 합니다.");
        }
    }
}
