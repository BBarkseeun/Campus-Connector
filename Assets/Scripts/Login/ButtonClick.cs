using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonClick : MonoBehaviour
{
        public void OnClickExit()
    {
        Application.Quit();
        Debug.Log("��������");
    }

    public void Register()
    {
        SceneManager.LoadScene("Register");
    }
    public void back()
    {
        SceneManager.LoadScene("Login");
    }
}
