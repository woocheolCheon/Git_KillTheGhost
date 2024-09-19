using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    //열거형, 상수라고 정한 값들,
    // 여러가지 조건을 사용할때 string,0,1,2등은 비 직관적이라 가독성,유지보수 측면에서 사용
    public enum Type
    {
        Melee,
        Range,
        Boss
    }

    public Type enemyType;

    public float maxHealth;
    public float currentHealth;

    public int damage;

    // AI Navigation

    public Transform navigationTarget;
    private NavMeshAgent navMeshAgent;


    //추적 거리
    public float chaseDistance;

    //공격 거리
    public float attackDistance;

    //애니메이션
    private Animator am; 

    //근접 공격 체크
    public BoxCollider demageCollider;
    private bool isAttack;

    //죽음 체크
    private bool isDead;

    //원거리 적 총알
    public GameObject enemyBullet;
    public GameObject bulletPosition;

    //코루틴
    private Coroutine attackCoroutine;

    //피해 파티클
    public GameObject hitParticle;

    //보스 유령 소환
    public GameObject ghostEnemy;

    //체력바
    public HpBar hpBar;

    private void Awake()
    {
        //체력 초기화
        currentHealth = maxHealth;

        //네비게이션 초기화
        navMeshAgent = GetComponent<NavMeshAgent>();

        //애니 초기화
        am = GetComponentInChildren<Animator>();

        //근접 공격 체크
        if(enemyType == Type.Melee)
        {
            demageCollider.enabled = false;
        }

        //체력바 초기화
        UpdateHp();
    }

    private void Update()
    {
        if (isDead == false)
        {
            //적과 플레이어가 얼마나 떨어져있는지 거리 계산
            float distanceToTarget = Vector3.Distance(transform.position, navigationTarget.position);

            //플레이어가 적의 추적 거리안에 있다면
            if (distanceToTarget <= chaseDistance)
            {
                //적의 회전이 어색해서 사용
                Vector3 targetPosition = new Vector3(navigationTarget.position.x, transform.position.y, navigationTarget.position.z);
                transform.LookAt(targetPosition);

                // 공격 거리 안에 있는지 확인
                if (distanceToTarget <= attackDistance)
                {

                    navMeshAgent.ResetPath();
                    am.SetBool("isWalk", false);

                    if (isAttack == false) 
                    {
                        attackCoroutine = StartCoroutine(Attack());
                    }
                }
                else // 아니면 쫓아감
                {
                    navMeshAgent.SetDestination(navigationTarget.position);
                    am.SetBool("isWalk", true);

                    StopAttack();

                }
            }
            else
            {
                navMeshAgent.ResetPath();
                am.SetBool("isWalk", false);

                StopAttack();

            }
        }
    }


    IEnumerator Attack()
    {
        isAttack = true;

        while (true)
        {

            // 공격 타입에 따른 준비 시간

            float prepareTime = 0f;
            float attackInterval = 1f;

            switch (enemyType)
            {
                case Type.Melee:
                    attackInterval = 1f;
                    prepareTime = 0.5f; // 예시로, 근접 공격은 빠르게 준비
                    break;

                case Type.Range:
                    attackInterval = 2f;
                    prepareTime = 1f; // 원거리 공격은 좀 더 긴 준비 시간
                    break;

                case Type.Boss:
                    attackInterval = 5f;
                    prepareTime = 2f; // 보스는 더 긴 준비 시간
                    break;
            }

            yield return new WaitForSeconds(prepareTime);

            am.SetTrigger("doAttack");

            switch(enemyType)
            {
                case Type.Melee:

                    demageCollider.enabled = true;

                    break;

                case Type.Range:

                    Instantiate(enemyBullet,
                        bulletPosition.transform.position, transform.rotation);

                    break;

                case Type.Boss:

                    BossBulletAttack();

                    break;
            }


            yield return new WaitForSeconds(attackInterval);

            switch (enemyType)
            {
                case Type.Melee:
                    demageCollider.enabled = false;
                    break;

                case Type.Range:

                    break;

                case Type.Boss:

                    BossSpawnGhost();

                    break;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void StopAttack()
    {
        if(isAttack == true)
        {
            if(attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }

            isAttack = false;

            switch (enemyType)
            {
                case Type.Melee:
                    demageCollider.enabled = false;
                    break;

                case Type.Range:

                    break;

                case Type.Boss:

                    break;
            }
        }
    }

    public void GetDamage(float damage)
    {
        if (isDead == true)
        {
            return;
        }

        currentHealth -= damage;

        UpdateHp();

        StartCoroutine(HitEffect());

        if (currentHealth <= 0)
        {
            //죽을때
            
            isDead = true;

            currentHealth = 0;

            StopAttack();

            StartCoroutine(Dead());
        }
    }

    IEnumerator Dead()
    {
        am.SetBool("isDead", true);

        // 보스가 죽으면서 스테이지의 문을 연다
        if(enemyType == Type.Boss)
        {
            GroundManager groundManager = FindObjectOfType<GroundManager>();

            if(groundManager != null)
            {
                groundManager.OpenNextGate();
            }

        }

        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }

    IEnumerator HitEffect()
    {
        Instantiate(hitParticle,
            gameObject.transform.position,
            gameObject.transform.rotation);

        //게임 오브젝트의 하위 자식들의 컴포넌트를 가져와서 배열에 담음
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

        for(int i = 0; i< meshRenderers.Length; i++)
        {
            meshRenderers[i].material.color = Color.red;
        }
        
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.color = Color.white;
        }
    }

    public void BossBulletAttack()
    {
        //총알 발사 랜덤 개수 

        int bulletCount = Random.Range(5, 15);

        // 갯수에 따라 360 각도 값을 나눠서 넣음  
        float angleStep = 360f / bulletCount;
        float currentAngle = 0f;

        for(var i = 0; i< bulletCount; i++)
        {
            //3D 공간에서 x,y,z로 회전하기위한 함수 / 오일러 여성 수학자 이름 
            Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);

            //총알 생성 - Quaternion.identity(기본값 0,0,0);
            Instantiate(enemyBullet, transform.position, rotation);

            // 다음 총알 각도 업데이트
            currentAngle += angleStep;

        }

    }

    public void BossSpawnGhost()
    {
        //유령 소환 개수

        int ghostCount = 3;

        for (int i = 0; i < ghostCount; i++)
        {
            GameObject ghost = Instantiate(ghostEnemy, transform.position, transform.rotation);

            Enemy enemy = ghost.GetComponent<Enemy>(); //Find는 남발 금지...다른 방법 Awake나 Start에서 미리 참조
            enemy.navigationTarget = GameObject.Find("Player").transform;
        }
    }

    public void UpdateHp()
    {
        if (hpBar != null)
        {
            hpBar.UpdateHPBar(currentHealth, maxHealth);
        }
    }
}
