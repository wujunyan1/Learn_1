using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CollionClick : MonoBehaviour
{

    bool isTouchDown;

    // 点击事件
    public delegate void ClickAction();
    ClickAction clickAction;

    // 移动事件
    public delegate void MoveAction(Vector3 old, Vector3 curr);
    MoveAction moveAction;

    MoveAction touchMoveAction;

    Vector3 mousePosition;

    public void Awake()
    {
    }
    

    //当鼠标进入在网格上时，
    void OnMouseEnter()
    {
        Debug.Log("OnMouseEnter");

        mousePosition = Input.mousePosition;
    }

    // ...当鼠标悬浮在物体上
    void OnMouseOver()
    {
        Vector3 curr = Input.mousePosition;
        if (mousePosition != curr)
        {
            if(moveAction != null)
            {
                moveAction.Invoke(mousePosition, curr);
            }

            if (isTouchDown && touchMoveAction != null)
            {
                touchMoveAction.Invoke(mousePosition, curr);
            }
        }

        mousePosition = curr;
    }

    void OnMouseDrag()
    {
        
    }

    // ...当鼠标移开时
    void OnMouseExit()
    {
        isTouchDown = false;
    }
    // ...当鼠标点击
    void OnMouseDown()
    {
        isTouchDown = true;
    }
    // ...当鼠标抬起
    void OnMouseUp()
    {
        // 点击了物品
        if (isTouchDown)
        {
            Click();
        }

        isTouchDown = false;
    }

    void Click()
    {
        if (clickAction != null)
        {
            clickAction.Invoke();
        }
    }

    public void SetClickAction(ClickAction action)
    {
        this.clickAction = action;
    }

    public void SetMoveAction(MoveAction action)
    {
        this.moveAction = action;
    }

    public void SetTouchMoveAction(MoveAction action)
    {
        this.touchMoveAction = action;
    }
}
