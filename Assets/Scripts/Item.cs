using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type
    {
        Chicken,
        Coin
    }

    public Type itemType;

    public float rotateSpeed = 10f;

    //회전
    private void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    //충돌 체크
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            ApplyEffect(other.gameObject);
            Destroy(gameObject);
        }
    }

    //아이템 효과

    private void ApplyEffect(GameObject gameObject)
    {
        switch(itemType)
        {
            case Type.Chicken:

                Player player = gameObject.GetComponent<Player>();

                if(player != null)
                {
                    player.currentHealth = player.maxHealth;
                    player.UpdateHp();
                }

                break;

            case Type.Coin:

                GameManager gameManager = FindObjectOfType<GameManager>();

                if(gameManager != null)
                {
                    gameManager.coin += 10;
                }

                break;
        }
    }
}
