using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    //체력바 게이지
    public Image foreground;

    // 메인 카메라의 좌표
    private Transform cameraTransform;


    private void Start()
    {
        //메인 카메라를 찾아서 변수에 담음
        cameraTransform = Camera.main.transform;
    }

    // 현재 체력과 최대 체력을 기반으로 체력 바를 업데이트하는 함수

    public void UpdateHPBar(float currentHealth, float maxHealth)
    {
        if(foreground != null)
        {
            //현재 체력을 최대 체력으로 나누어 체력 비율 계산
            //Mathf.Clamp01은 0~1 사이의 값으로 제한하도록 하는 명령어
            float hpPercentage = Mathf.Clamp01(currentHealth / maxHealth);
            
            foreground.fillAmount = hpPercentage;
        }
    }

    //후처리일때 보통 사용
    private void LateUpdate()
    {
        //체력바가 카메라를 바라보도록 설정

        //카메라가 바라보는 방향
        transform.LookAt(transform.position + cameraTransform.forward);
    }
}
