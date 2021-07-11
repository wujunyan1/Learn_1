using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// 军营
public class Barracks : CityBuild, SaveLoadInterface, RoundObject
{
    List<int> troopList;

    // param1 最大建筑数
    public Barracks()
    {
        type = CityBuildType.Barracks;

        troopList = new List<int>();
    }


    public override void Init(CityBuildConfig cityBuildConfig)
    {
        level = cityBuildConfig.level;
        isBuild = true;
        SetConfig(CityBuildConfigExtend.GetCityBuildConfig(type, level));
    }

    public override void SetCity(City city)
    {
        base.SetCity(city);

        city.RegisterListener("CanRecruitTroop", CanRecruitTroop);
    }

    public override void StartEffect()
    {
        base.StartEffect();

        troopList.Clear();

        string[] strs = config.param1.Split(',');
        foreach (var str in strs)
        {
            troopList.Add(int.Parse(str));
        }
    }
    

    public override void Save(BinaryWriter writer)
    {
        base.Save(writer);
    }

    public override IEnumerator Load(BinaryReader reader)
    {
        IEnumerator itor = base.Load(reader);

        while (itor.MoveNext())
        {
        }

        SetConfig(CityBuildConfigExtend.GetCityBuildConfig(type, level));
        yield return null;
    }

    public override void NextRound()
    {
        base.NextRound();
    }

    public override void LaterNextRound()
    {
        base.LaterNextRound();
    }

    void CanRecruitTroop(UEvent e)
    {
        UObject o = (UObject)e.eventParams;

        // o.GetT<City>("city", null);
        o.GetT<City>("city", null);

        int troopsIndex = (int)o.Get("troopIndex");
        bool result = (bool)o.Get("result");

        foreach (var troopId in troopList)
        {
            if(troopId == troopsIndex)
            {
                o.Set("result", true);
                return;
            }
        }
    }
}
