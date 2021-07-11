using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CavalryAnimation : UnitAnimation
{
    float currAnimationSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {
        animator.SetInteger("move", 0);
        //animator.SetInteger("move", 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void UpdateMoveSpeed(float speed)
    {
        float oldSpeed = currAnimationSpeed;
        currAnimationSpeed = speed;

        float sp = entity.m_dMaxSpeed * 0.5f;
        if (oldSpeed < sp && currAnimationSpeed > sp)
        {
            UpdateMoveStatus();
        }
        else if (oldSpeed > sp && currAnimationSpeed < sp)
        {
            UpdateMoveStatus();
        }
    }

    public override void UpdateStatus(string status)
    {

    }

    public override void UpdateMoveStatus()
    {
        // 速度快，则跑
        if(isMove && currAnimationSpeed > entity.m_dMaxSpeed * 0.5f)
        {
            animator.SetInteger("move", 2);
        }
        // 速度慢则是走
        else if (isMove && currAnimationSpeed > 0)
        {
            animator.SetInteger("move", 1);
        }
        // 在转向则是走
        else if (isRotate)
        {
            animator.SetInteger("move", 1);
        }
        else
        {
            animator.SetInteger("move", 0);
        }
    }
}
