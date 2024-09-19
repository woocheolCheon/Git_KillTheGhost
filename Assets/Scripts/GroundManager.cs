using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    public GameObject clearDoor;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    //문열림
    public void OpenNextGate()
    {
        //clearDoor.transform.position = Vector3.up * 5;

        clearDoor.transform.position =
            new Vector3(clearDoor.transform.position.x,
            clearDoor.transform.position.y + 5,
            clearDoor.transform.position.z);

        gameManager.Message
            ("The door is open. Please go to the next stage");
    }

    public void StageClear()
    {
        gameManager.NextStage();
    }
}
