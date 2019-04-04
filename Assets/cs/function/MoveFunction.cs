using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// 移动功能
/// </summary>
public class MoveFunction : ObjFunction
{
    public Button moveButtonPrefab;
    public HexMapEditor baseTouch;

    // 选择走到哪里
    bool isMoveChoose;

    int targetMask;

    UnityAction action;

    public MoveFunction()
    {
        name = "移动";
        //this.action = action;
    }

    public override Button GetFunctionBtn()
    {
        Button btn = Instantiate(moveButtonPrefab);
        Text text = btn.GetComponentInChildren<Text>();
        text.text = name;

        btn.onClick.AddListener(
            delegate
            {
                OnMoveBtn();
            }
            );

        return btn;
    }

    public void OnMoveBtn()
    {
        isMoveChoose = true;
    }

    void Update()
    {
        if (isMoveChoose)
        {
            if (Input.GetMouseButtonUp(0))
            {
                HandleInput();
            }
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.Log(string.Format("-------------"));

        // 没有碰到不需要的layer 并且碰到地图
        if (Physics.Raycast(inputRay, out hit, 200f, targetMask))  // && !EventSystem.current.IsPointerOverGameObject()
        {

            HexCell moveToCell = HexGrid.instance.GetCell(hit.point);

            action.Invoke();
        }
    }

    public override bool IsActive()
    {
        return true;
    }
}
