using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClick : MonoBehaviour
{
        public void OnClickExit()
    {
        Application.Quit();
        Debug.Log("게임종료");
    }
}
