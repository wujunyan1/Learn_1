using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimation : MonoBehaviour
{
    protected Animator animator;
    public MovingEntity entity;

    bool _isMove = false;
    public bool isMove
    {
        get
        {
            return _isMove;
        }
        set
        {
            bool oldValue = _isMove;
            _isMove = value;
            if (oldValue != _isMove)
            {
                UpdateMoveStatus();
            }
        }
    }

    bool _isRotate = false;
    public bool isRotate
    {
        get
        {
            return _isRotate;
        }
        set
        {
            bool oldValue = _isRotate;
            _isRotate = value;
            if (oldValue != _isRotate)
            {
                UpdateMoveStatus();
            }
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public virtual void UpdateMoveSpeed(float speed)
    {

    }

    public virtual void UpdateMoveStatus()
    {

    }

    public virtual void UpdateStatus(string status)
    {

    }
}
