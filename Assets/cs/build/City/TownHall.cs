using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TownHall : CityBuild
{
    CityBuff_AddMaxBuildNum maxBuildNumBuff;
    CityBuff_AddMaxPersonNum maxPersonNumBuff;

    // param1 最大建筑数
    public TownHall()
    {
        type = CityBuildType.TownHall;
        maxBuildNumBuff = new CityBuff_AddMaxBuildNum();
        maxPersonNumBuff = new CityBuff_AddMaxPersonNum();

        buffs.Add(maxBuildNumBuff);
        buffs.Add(maxPersonNumBuff);
    }

    public override void Init(CityBuildConfig cityBuildConfig)
    {
        level = 1;
        isBuild = false;
        SetConfig(CityBuildConfigExtend.GetCityBuildConfig(type, level));
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

    public override void SetCity(City city)
    {
        base.SetCity(city);

        city.AddCityBuff(maxBuildNumBuff);
        city.AddCityBuff(maxPersonNumBuff);
    }

    public override void StartEffect()
    {
        maxBuildNumBuff.SetMaxBuildNum(int.Parse(config.param1));
        maxPersonNumBuff.SetMaxPersonNum(int.Parse(config.param3));
    }

    public override void AddLevel()
    {
        base.AddLevel();
    }
    
}
