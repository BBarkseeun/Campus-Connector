using Firebase;
using Firebase.Database;
using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;

public class FirebaseNicknameLoader : MonoBehaviour
{
    //public TextMesh nicknameTextMesh; // Unity Editor에서 할당
    public TMP_Text nicknameTextMesh;

    private DatabaseReference databaseReference;

    // Start is called before the first frame update
    void Start()
    {
        // Firebase 초기화
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

            // 현재 로그인한 사용자의 ID를 가져옵니다.
            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            FirebaseUser user = auth.CurrentUser;
            if (user != null)
            {
                // 닉네임 가져오기
                FetchNickname(user.UserId);
            }
            else
            {
                Debug.LogError("No user is currently logged in.");
            }
        });
    }

    void FetchNickname(string userId)
    {
        Debug.Log("Fetching nickname for user ID: " + userId);
        // 메인 스레드에서 실행하도록 Firebase Unity SDK의 확장 메소드를 사용합니다.
        databaseReference.Child("users").Child(userId).Child("nickname").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // 에러 처리
                Debug.LogError("Error retrieving nickname: " + task.Exception.ToString());
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log("Fetched nickname: " + snapshot.Value.ToString());
                // 메인 스레드에서 UI 업데이트
                nicknameTextMesh.text = snapshot.Value.ToString();
            }
        });
    }

    void UpdateNicknameText(string nickname)
    {
        Debug.Log("Updating TextMesh with nickname: " + nickname);
        if (nicknameTextMesh != null)
        {
            nicknameTextMesh.text = nickname;
        }
        else
        {
            Debug.LogError("nicknameTextMesh is not assigned!");
        }
    }
}