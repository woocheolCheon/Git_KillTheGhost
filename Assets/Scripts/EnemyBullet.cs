using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10;

    private void Awake()
    {
        StartCoroutine("DestroyEnemy");
    }

    IEnumerator DestroyEnemy()
    {
        yield return new WaitForSeconds(2f);
        GameObjectDestroy();
    }


    void Update()
    {
        // Translate 캐릭터가 바라보는 방향으로 날아감 // position += 이 방식은 월드 좌표로 날아감
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();

            if(player != null)
            {
                player.GetDamage(damage);
            }

            GameObjectDestroy();
        }
    }

    private void GameObjectDestroy()
    {
        TrailRenderer trail = gameObject.GetComponent<TrailRenderer>();
        if(trail != null)
        {
            trail.Clear();
        }

        Destroy(gameObject);
    }
}
