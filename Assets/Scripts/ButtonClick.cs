using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonClick : MonoBehaviour
{
        public void OnClickExit()
    {
        Application.Quit();
        Debug.Log("게임종료");
    }

    public void Register()
    {
        SceneManager.LoadScene("Register");
    }
}
