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
    public HexMapEditor baseTouch;

    // 选择走到哪里
    bool isMoveChoose;

    int targetMask;

    public MoveFunction()
    {
        name = "移动";
        //this.action = action;
    }

    public override void OnStartBtn()
    {
        base.OnStartBtn();

        HexMapEditor inputPanel = HexMapEditor.instance;

        inputPanel.SetMoveAction(ShowMoveHexCell);
        inputPanel.SetClickAction(ChooseCell);
    }

    // 移动到的位置
    public void ShowMoveHexCell(HexCell old, HexCell curr)
    {
        control.ShowMovePath(curr);
    }

    // 选择了这个
    public void ChooseCell(HexCell cell)
    {
        Debug.Log("ChooseCell");
        control.MoveTo(cell);

        CloseFuncView();
    }

    public override bool IsActive()
    {
        return true;
    }

    public override void CloseFuncView()
    {
        base.CloseFuncView();
        HexMapEditor inputPanel = HexMapEditor.instance;
        inputPanel.SetMoveAction(null);
        inputPanel.SetClickAction(null);
    }
}
