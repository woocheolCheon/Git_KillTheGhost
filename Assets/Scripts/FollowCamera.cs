using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{

    public Transform target; //움직일 타겟
    public Vector3 offset; //보정값


    void Update()
    {
        //타겟의 위치를 항상 따라 다니고 카메라의 위치를 더해줌

        transform.position = target.position + offset;
    }
}
