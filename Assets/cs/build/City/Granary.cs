using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Granary : CityBuild
{
    // param1 最大建筑数
    public Granary()
    {
        type = CityBuildType.Granary;
        
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
        
    }

    public override void StartEffect()
    {
    }

    public override void AddLevel()
    {
        base.AddLevel();
    }

    public override void NextRound()
    {
        base.NextRound();

        if (!isBuild)
        {
            
        }
    }
}
