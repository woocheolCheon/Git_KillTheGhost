using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public GameObject failedScreen;
    public GameObject clearScreen;

    //문이 열렸습니다. 클리어 메시지
    public void Message(string str)
    {
        messageText.gameObject.SetActive(true);
        messageText.text = str;
        StartCoroutine(closeMessage());
    }

    IEnumerator closeMessage()
    {
        yield return new WaitForSeconds(5f);
        messageText.gameObject.SetActive(false);
    }


    public void FailedScreen()
    {
        failedScreen.SetActive(true);
    }

    public void ClearScreen()
    {
        clearScreen.SetActive(true);
    }

}
