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
    public TMP_InputField nicknameField;  // ´Ð³×ÀÓ ÀÔ·Â ÇÊµå
    public Button submitButton;  // Á¦Ãâ ¹öÆ°

    // AlertDialog ÂüÁ¶
    public AlertDialog alertDialog;

    FirebaseAuth auth;
    DatabaseReference reference;

    private Queue<System.Action> actionsToExecute = new Queue<System.Action>();

    void Awake()
    {
        // Firebase ÃÊ±âÈ­
        auth = FirebaseAuth.DefaultInstance;
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        // ¹öÆ°ÀÇ Å¬¸¯ ÀÌº¥Æ® ¼³Á¤
        submitButton.onClick.AddListener(SubmitNickname);
    }

    void Update()
    {
        // Å¥¿¡ ¿¹¾àµÈ ¸ðµç ÀÛ¾÷À» ½ÇÇàÇÕ´Ï´Ù.
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
            alertDialog.ShowAlert("´Ð³×ÀÓÀº 12±ÛÀÚ ÀÌ³»·Î ÀÔ·ÂÇØÁÖ¼¼¿ä.");
            return;
        }

        // Check if the nickname contains any special characters using Regex
        if (!System.Text.RegularExpressions.Regex.IsMatch(nickname, "^[a-zA-Z0-9°¡-ÆR]*$"))
        {
            alertDialog.ShowAlert("´Ð³×ÀÓ¿¡´Â Æ¯¼ö¹®ÀÚ¸¦ »ç¿ëÇÒ ¼ö ¾ø½À´Ï´Ù.");
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
                            Debug.LogError("´Ð³×ÀÓÀÌ ÀÌ¹Ì »ç¿ë ÁßÀÔ´Ï´Ù.");
                            actionsToExecute.Enqueue(() => alertDialog.ShowAlert("ÀÌ¹Ì »ç¿ëÁßÀÎ ´Ð³×ÀÓÀÔ´Ï´Ù."));
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
                                        actionsToExecute.Enqueue(() => SceneManager.LoadScene("New Scene"));
                                    }
                                    else
                                    {
                                        alertDialog.ShowAlert("´Ð³×ÀÓ ÀúÀå¿¡ ½ÇÆÐÇß½À´Ï´Ù.");
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
                        alertDialog.ShowAlert("´Ð³×ÀÓ È®ÀÎ¿¡ ½ÇÆÐÇß½À´Ï´Ù.");
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
            alertDialog.ShowAlert("´Ð³×ÀÓÀ» ÀÔ·ÂÇÏ¼¼¿ä.");
        }
    }
}