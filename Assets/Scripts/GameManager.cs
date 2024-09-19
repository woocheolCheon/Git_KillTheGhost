using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private UIManager uIManager;

    //코인수
    public int coin;

    private void Start()
    {
        uIManager = FindObjectOfType<UIManager>();
    }


    //플레이어가 죽음
    public void PlayerDeath()
    {
        uIManager.FailedScreen();
    }

    //클리어 포인트 도착
    public void NextStage()
    {
        uIManager.ClearScreen();
    }

    //보스 처치 메시지
    public void Message(string str)
    {
        uIManager.Message(str);
    }
}
