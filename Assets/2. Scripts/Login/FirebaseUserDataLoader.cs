using Firebase;
using Firebase.Database;
using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;

public class FirebaseUserDataLoader : MonoBehaviour
{
    private DatabaseReference databaseReference;
    private UserDataSync _userDataSync;

    // Start is called before the first frame update
    void Start()
    {
        _userDataSync = GetComponent<UserDataSync>();
        InitializeFireBase();
    }

    private void InitializeFireBase()
    {
        // Firebase 초기화
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

            // 현재 로그인한 사용자의 ID를 가져옵니다.
            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            FirebaseUser user = auth.CurrentUser;
            if (user != null)
            {
                // 사용자 정보 가져오기
                FetchUserData(user.UserId);
            }
            else
            {
                Debug.LogError("No user is currently logged in.");
            }
        });
    }

    void FetchUserData(string userId)
    {
        Debug.Log("Fetching user data for user ID: " + userId);
        databaseReference.Child("users").Child(userId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // 에러 처리
                Debug.LogError("Error retrieving user data: " + task.Exception.ToString());
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists && snapshot.HasChildren)
                {
                    string name = snapshot.Child("name").Value.ToString();
                    string studentId = snapshot.Child("studentID").Value.ToString();
                    string major = snapshot.Child("major").Value.ToString();
                    string nickname = snapshot.Child("nickname").Value.ToString();

                    Debug.Log("Fetched user data: " + name + ", " + studentId + ", " + major + ", " + nickname);

                    UserData userData = new UserData()
                    {
                        UserName = name,
                        UserID = studentId,
                        MajorName = major,
                        NickName = nickname,
                    };

                    _userDataSync.UpdateUserInfo(userData);
                }
                else
                {
                    Debug.LogError("User data does not exist.");
                }
            }
        });
    }
}
