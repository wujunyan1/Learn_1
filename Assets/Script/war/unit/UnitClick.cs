using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitClick : MonoBehaviour
{
    bool isTouchDown;


    //当鼠标进入在网格上时，
    void OnMouseEnter()
    {
        Debug.Log("OnMouseEnter");
    }

    // ...当鼠标悬浮在物体上
    void OnMouseOver()
    {
       
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
        Debug.Log("click");
        Operation.getInstance().selectControl = GetComponent<Soldier>().control;
    }
}
