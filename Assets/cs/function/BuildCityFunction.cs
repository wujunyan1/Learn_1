using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 在当前位置建立城市，但建造者消失
/// </summary>
public class BuildCityFunction : ObjFunction
{
    public BuildCityFunction()
    {
        name = "建城";
        //this.action = action;
    }

    public override void OnStartBtn()
    {
        base.OnStartBtn();

        GameObject inputPanel = UiManager.instance.OpenInputPanel();
        InputCityName inputCityName = inputPanel.AddComponent<InputCityName>();
        inputCityName.Func = this;
        inputPanel.SetActive(true);

        control.Control.controlView.HideView();
    }

    public override bool IsActive()
    {
        return true;
    }

    public override void CloseFuncView()
    {
        base.CloseFuncView();
    }

    public void BuildCity(string name)
    {
        UiManager.instance.CloseInputPanel();

        HexCoordinates coordinates = control.Point;

        control.camp.CreateNewCity(coordinates, name);

        control.Die();
    }
}
