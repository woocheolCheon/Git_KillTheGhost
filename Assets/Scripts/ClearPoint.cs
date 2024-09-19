using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearPoint : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            GroundManager groundManager = FindObjectOfType<GroundManager>();
            if(groundManager != null)
            {
                groundManager.StageClear();
            }
        }
    }
}
