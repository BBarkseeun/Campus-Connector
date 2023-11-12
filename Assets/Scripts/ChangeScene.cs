using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    // Start is called before the first frame update
    public void ChangeSceneBtn()
    {
        switch (this.gameObject.name)
        {
            case "StartBtn":
                SceneManager.LoadScene("Login");
                break;
            case "HomeBtn":
                SceneManager.LoadScene("MainMenu");
                break;
            case "GoMenuBtn":
                SceneManager.LoadScene("MainMenu");
                break;
            case "MenuBtn":
                SceneManager.LoadScene("MainMenu");
            break;
        }
    }
}
