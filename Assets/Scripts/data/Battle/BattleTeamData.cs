using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTeamData
{
    int camp;

    List<SoldierConfigData> soldiers;

    public BattleTeamData(int _camp)
    {
        SetCamp(_camp);
        soldiers = new List<SoldierConfigData>();
    }

    public void SetCamp(int _camp)
    {
        camp = _camp;
    }

    public int GetCamp()
    {
        return camp;
    }

    public void AddSoldier(string key)
    {
        SoldierConfigData soldierConfigData = HeroConfig.GetInstance().GetSoldierConfig(key);
        SoldierConfigData addData = soldierConfigData.Clone(true);

        soldiers.Add(addData);
        
    }

    public void RemoveSoldier(int index)
    {
        soldiers.RemoveAt(index);

        Debug.Log(camp + "  " + soldiers.Count);
    }

    public List<SoldierConfigData> GetSoldierConfigs()
    {
        return soldiers;
    }
}
