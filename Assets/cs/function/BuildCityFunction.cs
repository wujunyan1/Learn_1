using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 在当前位置建立城市，但建造者消失
/// </summary>
public class BuildCityFunction : ObjFunction
{
    const int buildRound = 3;
    const string buildPropName = "buildCityProp";

    public BuildCityFunction()
    {
        _type = FunctionType.BuildCity;
        name = "buildCity";
        //this.action = action;
    }

   
    public override bool IsActive(Troop troop)
    {
        return true;
    }

    void BuildCity(Troop troop)
    {
        int cellIndex = troop.CellIndex;

        GameCenter.instance.LaterKillTroop(troop);

        City city = troop.camp.CreateNewCity(cellIndex, "ceshi");
        city.battleResourse.persons = troop.data.TroopNum;
    }

    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="troop"></param>
    /// <returns></returns>
    public override bool Execute(Troop troop)
    {
        TroopControl troopControl = troop.control;

        int round = buildRound - troop.FuncRound;

        if (round == 0)
        {
            BuildCity(troop);
            // 建造城市
            return true;
        }

        // 更新UI
        Transform hammer = troopControl.propGameObj.transform.Find(buildPropName);

        TextMesh textMesh = hammer.GetComponentInChildren<TextMesh>();
        textMesh.text = round.ToString();

        return false;
    }

    public override bool IsExecute(Troop troop)
    {
        return base.IsExecute(troop);
    }
    

    public override void UpdateStatusView(Troop troop, TroopControl troopControl)
    {
        GameObject hammer;
        PrefabsManager.GetInstance().GetGameObj(out hammer, "TroopHammer");

        hammer.name = buildPropName;
        hammer.transform.SetParent(troopControl.propGameObj.transform);
        hammer.transform.localPosition = new Vector3(0f, 20f, 0);
        
        int round = buildRound - troop.FuncRound;
        troopControl.funcStatusRoundText.text = round.ToString();

        TextMesh textMesh = hammer.GetComponentInChildren<TextMesh>();
        textMesh.text = round.ToString();
    }
}
