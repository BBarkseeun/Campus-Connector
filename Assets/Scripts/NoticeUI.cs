using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class NoticeUI : MonoBehaviour
{
    [Header("subNotice")]
    public GameObject subbox;
    public TMP_Text subintext;
    public Animator subani;  // 여기를 Animator로 바꿔야 합니다.

    private WaitForSeconds _UIdelay1 = new WaitForSeconds(2.0f);
    private WaitForSeconds _UIdelay2 = new WaitForSeconds(0.3f);

    void Start()
    {
        subbox.SetActive(false);
    }

    public void SUB(string message)
    {
        subintext.text = message;
        subbox.SetActive(false);
        StopAllCoroutines();
        StartCoroutine(SUBDelay());
    }
    IEnumerator SUBDelay()
    {
        subbox.SetActive(true);
        subani.SetBool("isOn", true);
        yield return _UIdelay1;
        subani.SetBool("isOn", false);
        yield return _UIdelay2;
        subbox.SetActive(false);
    }
}
