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
    public TMP_InputField nicknameField; // ´Ð³×ÀÓ ÀÔ·Â ÇÊµå
    public TMP_InputField studentIDField; // ÇÐ¹ø ÀÔ·Â ÇÊµå
    public TMP_InputField majorField; // Àü°ø ÀÔ·Â ÇÊµå
    public TMP_InputField nameField; // ÀÌ¸§ ÀÔ·Â ÇÊµå
    public Button submitButton; // Á¦Ãâ ¹öÆ°
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
        submitButton.onClick.AddListener(SubmitData);
    }

    void Update()
    {
        // Å¥¿¡ ¿¹¾àµÈ ¸ðµç ÀÛ¾÷À» ½ÇÇàÇÕ´Ï´Ù.
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
        string name = nameField.text; // ÀÌ¸§ ÇÊµå¿¡¼­ ÅØ½ºÆ® °¡Á®¿À±â

        // ´Ð³×ÀÓ, ÇÐ¹ø, Àü°ø, ÀÌ¸§ À¯È¿¼º °Ë»ç Ãß°¡
        if (!ValidateNickname(nickname) || !ValidateStudentID(studentID) || !ValidateMajor(major) || !ValidateName(name))
        {
            return;
        }

        // Firebase¿¡ µ¥ÀÌÅÍ ÀúÀå
        SaveDataToFirebase(nickname, studentID, major, name);
    }
    bool ValidateNickname(string nickname)
    {
        // ´Ð³×ÀÓÀº 12±ÛÀÚ ÀÌ³»·Î ÀÔ·ÂÇÏµµ·Ï ÇÏ¸ç Æ¯¼ö ¹®ÀÚ¸¦ »ç¿ëÇÒ ¼ö ¾ø½À´Ï´Ù.
        if (nickname.Length > 12)
        {
            alertDialog.ShowAlert("´Ð³×ÀÓÀº 12±ÛÀÚ ÀÌ³»·Î ÀÔ·ÂÇØÁÖ¼¼¿ä.");
            return false;
        }
        if (!Regex.IsMatch(nickname, "^[a-zA-Z0-9°¡-ÆR]*$"))
        {
            alertDialog.ShowAlert("´Ð³×ÀÓ¿¡´Â Æ¯¼ö¹®ÀÚ¸¦ »ç¿ëÇÒ ¼ö ¾ø½À´Ï´Ù.");
            return false;
        }
        return true;
    }

    bool ValidateStudentID(string studentID)
    {
        // ÇÐ¹øÀº ¼ýÀÚ 8ÀÚ¸®·Î ÀÔ·Â¹Þ½À´Ï´Ù.
        if (!Regex.IsMatch(studentID, @"^\d{8}$"))
        {
            alertDialog.ShowAlert("ÇÐ¹øÀº ¼ýÀÚ 8ÀÚ¸®·Î ÀÔ·ÂÇØÁÖ¼¼¿ä.");
            return false;
        }
        return true;
    }

    bool ValidateMajor(string major)
    {
        // Àü°øÀº Æ¯¼ö¹®ÀÚ¸¦ Æ÷ÇÔÇÏÁö ¾Ê´Â ¹®ÀÚ¿­·Î Á¦ÇÑÇÕ´Ï´Ù.
        if (!Regex.IsMatch(major, @"^[a-zA-Z°¡-ÆR\s]*$"))
        {
            alertDialog.ShowAlert("Àü°ø¿¡ Æ¯¼ö¹®ÀÚ¸¦ »ç¿ëÇÒ ¼ö ¾ø½À´Ï´Ù.");
            return false;
        }
        return true;
    }
    bool ValidateName(string name)
    {
        // ÀÌ¸§ À¯È¿¼º °Ë»ç ·ÎÁ÷À» Ãß°¡ÇÕ´Ï´Ù.
        // ¿¹¸¦ µé¾î, ÀÌ¸§Àº ºñ¾î ÀÖÁö ¾Ê¾Æ¾ß ÇÏ¸ç Æ¯¼ö ¹®ÀÚ¸¦ Æ÷ÇÔÇÏÁö ¾Ê´Â °ÍÀ¸·Î Á¦ÇÑÇÕ´Ï´Ù.
        if (string.IsNullOrEmpty(name))
        {
            alertDialog.ShowAlert("ÀÌ¸§À» ÀÔ·ÂÇØÁÖ¼¼¿ä.");
            return false;
        }
        if (!Regex.IsMatch(name, @"^[a-zA-Z°¡-ÆR\s]*$"))
        {
            alertDialog.ShowAlert("ÀÌ¸§¿¡ Æ¯¼ö¹®ÀÚ¸¦ »ç¿ëÇÒ ¼ö ¾ø½À´Ï´Ù.");
            return false;
        }
        return true;
    }

    void SaveDataToFirebase(string nickname, string studentID, string major, string name)
    {
        if (int.TryParse(studentID, out int idNumber))
        {
            string userId = auth.CurrentUser.UserId;
            // Firebase Database¿¡ ÀÌ¸§À» Æ÷ÇÔÇÏ¿© ´Ð³×ÀÓ, ÇÐ¹ø(Á¤¼öÇü), Àü°øÀ» ÀúÀåÇÕ´Ï´Ù.
            Task setNicknameTask = reference.Child("users").Child(userId).Child("nickname").SetValueAsync(nickname);
            Task setStudentIDTask = reference.Child("users").Child(userId).Child("studentID").SetValueAsync(idNumber);
            Task setMajorTask = reference.Child("users").Child(userId).Child("major").SetValueAsync(major);
            Task setNameTask = reference.Child("users").Child(userId).Child("name").SetValueAsync(name); // ÀÌ¸§ ÀúÀå

            Task.WhenAll(setNicknameTask, setStudentIDTask, setMajorTask, setNameTask).ContinueWith(
                saveTask =>
                {
                    if (saveTask.IsCompleted && !saveTask.IsFaulted && !saveTask.IsCanceled)
                    {
                        actionsToExecute.Enqueue(() => SceneManager.LoadScene("SampleScene_2"));
                    }
                    else
                    {
                        Debug.LogError("µ¥ÀÌÅÍ ÀúÀå¿¡ ½ÇÆÐÇß½À´Ï´Ù.");
                        actionsToExecute.Enqueue(() => alertDialog.ShowAlert("µ¥ÀÌÅÍ ÀúÀå¿¡ ½ÇÆÐÇß½À´Ï´Ù."));
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
            alertDialog.ShowAlert("ÇÐ¹øÀº ¼ýÀÚ·Î¸¸ ±¸¼ºµÇ¾î¾ß ÇÕ´Ï´Ù.");
        }
    }
}
