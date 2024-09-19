using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Has Exit time // true인 경우 애니메이션을 타임라인 끝까지 실행
    //Has Exit time // false 경우 애니메이션을 실행하다가 다른 명령이 있는 경우 넘어감.


    //이동
    public float speed;

    //키보드 입력값
    private float hAxis; //입력 값  // 축
    private float vAxis;

    //입력 값을 넣은 이동 값
    private Vector3 moveVector;

    //애니메이션
    Animator am;

    //점프버튼을 눌렀나?
    private bool pressJump;

    //점프 중인가? 
    private bool isJump;

    //물리컴포넌트 Rigidbody
    private Rigidbody rb;

    //공격
    private bool isAttack;

    //무기
    public BoxCollider weaponCollider;

    //체력
    public float maxHealth;
    public float currentHealth;

    //피해 파티클
    public GameObject hitParticle;

    //hpBar
    public HpBar hpBar;

    //사망 여부
    public bool isDead;


    private void Awake()
    {
        //자식의 게임오브젝트에서 애니메이션 컴포넌트를 가져옴
        am = GetComponentInChildren<Animator>();

        //게임 오브젝트에 달린 Rigidbody 컴포넌트를 가져옴
        rb = GetComponent<Rigidbody>();

        //무기 비활성화
        weaponCollider.enabled = false;

        //체력 초기화
        currentHealth = maxHealth;

        UpdateHp();
    }

    public void UpdateHp()
    {
        //체력바 초기화
        if (hpBar != null)
        {
            hpBar.UpdateHPBar(currentHealth, maxHealth);
        }
    }

    // 매 프레임을 실시간으로 확인할 상황 -
    // 데이터 연산시 주로 사용 - 물리 제외.
    // 1초에 60번 계산하지만 변경 가능, 하드웨어 성능에 따라 달라짐

    void Update()
    {
        Move();
        Rotation();
        Jump();
        Attack();
    }

    private void Move()
    {
        //이동
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");

        //Debug.Log(hAxis);

        moveVector = new Vector3(hAxis, 0, vAxis);

        //delta.time 은 전 프레임과 현 프레임간의 흐른 시간 // 저사양 고사양 컴퓨터와는 관계 없이 일정한 속도

        transform.position += moveVector * speed * Time.deltaTime;

        //애니메이션
        am.SetBool("isWalk", moveVector != Vector3.zero);

    }

    private void Rotation()
    {
        //현재 위치에서 목표지점을 바라본다. 
        transform.LookAt(transform.position + moveVector);
    }

    private void Jump()
    {
        //점프눌렀는지?
        pressJump = Input.GetButtonDown("Jump");

        //점프를 눌렀고, 점프중이 아니라면.

        if (pressJump == true && isJump == false)
        {
            //힘을 추가함 // new vector(0,1,0);
            rb.AddForce(Vector3.up * 5, ForceMode.Impulse);

            //점프중
            isJump = true;

            //점프 애니 //일회성은 Trigger 사용

            am.SetTrigger("doJump");

            //점프 애니가 끝나면 idle로 자동으로 돌아가지 않게 락을 거는 느낌

            am.SetBool("isJump", true);
            
        }
    }

    //바닥 충돌 체크 // 부딪힐때마다 1번 실행
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            isJump = false;
            am.SetBool("isJump", false);
        }
    }

    private void Attack()
    {
        //마우스 왼쪽 버튼
        if (Input.GetMouseButtonDown(0) == true && isAttack == false)
        {
            //공격 시작
            StartCoroutine("Attacking");
        }
    }
    private void FixedUpdate()
    {
        //속도와 회전,방향을 초기화
        //rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }


    IEnumerator Attacking()
    {
        isAttack = true;

        //공격 애니 //일회성은 Trigger 사용
        am.SetTrigger("doAttack");

        yield return new WaitForSeconds(0.2f);

        //무기 활성화
        weaponCollider.enabled = true;

        yield return new WaitForSeconds(0.2f);

        isAttack = false;

        //무기 비활성화
        weaponCollider.enabled = false;
    }

    //플레이어 피해 이펙트

    public void GetDamage(int enemyDamage)
    {
        if(isDead == true)
        {
            return;
        }

        currentHealth -= enemyDamage;

        UpdateHp();

        StartCoroutine("HitEffect");

        if (currentHealth <= 0)
        {
            isDead = true;

            currentHealth = 0;

            GameManager gameManager = FindObjectOfType<GameManager>();

            if(gameManager != null)
            {
                gameManager.PlayerDeath();
            }
        }
    }

    IEnumerator HitEffect()
    {
        GameObject particle = Instantiate(hitParticle,
            gameObject.transform.position,
            gameObject.transform.rotation);


        yield return new WaitForSeconds(0.1f);

        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.color = Color.red;
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.color = Color.white;
        }
    }

}
