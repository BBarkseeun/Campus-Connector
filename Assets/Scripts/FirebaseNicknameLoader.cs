using Firebase;
using Firebase.Database;
using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;

public class FirebaseNicknameLoader : MonoBehaviour
{
    //public TextMesh nicknameTextMesh; // Unity Editor���� �Ҵ�
    public TMP_Text nicknameTextMesh;

    private DatabaseReference databaseReference;

    // Start is called before the first frame update
    void Start()
    {
        // Firebase �ʱ�ȭ
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

            // ���� �α����� ������� ID�� �����ɴϴ�.
            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            FirebaseUser user = auth.CurrentUser;
            if (user != null)
            {
                // �г��� ��������
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
        // ���� �����忡�� �����ϵ��� Firebase Unity SDK�� Ȯ�� �޼ҵ带 ����մϴ�.
        databaseReference.Child("users").Child(userId).Child("nickname").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // ���� ó��
                Debug.LogError("Error retrieving nickname: " + task.Exception.ToString());
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log("Fetched nickname: " + snapshot.Value.ToString());
                // ���� �����忡�� UI ������Ʈ
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