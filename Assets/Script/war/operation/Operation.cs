using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Operation : MonoBehaviour
{
    // 选中的控制器
    public TeamControl selectControl;

    // 地面
    int floorMask;

    private static Operation instance;

    bool isRightDown = false;
    Vector3 rightDownVector;

    public static Operation getInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
        floorMask = LayerMask.GetMask("floor");
    }

    // 控制
    private void Update()
    {
        if (selectControl == null)
        {
            return;
        }

        // 点击了右键
        if (Input.GetMouseButtonDown(1))
        {
            isRightDown = true;

            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 点击的地面坐标
            if (Physics.Raycast(inputRay, out hit, 200f, floorMask))
            {
                rightDownVector = hit.point;
                RightMouseButtonDown(rightDownVector);
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            isRightDown = false;
        }

        if (isRightDown)
        {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 点击的地面坐标
            if (Physics.Raycast(inputRay, out hit, 200f, floorMask))
            {
                Vector3 v = hit.point;

                if(Vector3.Distance(rightDownVector, v ) > 2.0f)
                {
                    SetTeamFormationWidthAndPosition(rightDownVector, v);
                }
            }
        }

    }

    // 右键按下
    private void RightMouseButtonDown( Vector3 point )
    {
        switch (War.CurrWarType)
        {
            // 战前则是 设置部队位置
            case WarType.PREWAR:
                selectControl.SetPosition(point);
                break;

            //战斗中 则是部队移动
            case WarType.WAR:
                selectControl.MoveTo(point);
                break;
        }
    }

    private void SetTeamFormationWidthAndPosition(Vector3 left, Vector3 right)
    {
        switch (War.CurrWarType)
        {
            // 战前则是 设置部队位置
            case WarType.PREWAR:
                selectControl.SetTeamFormationWidthAndPosition(left, right);
                break;

            //战斗中 则是部队移动
            case WarType.WAR:
                selectControl.MoveTo(left, right);
                break;
        }
    }
}
