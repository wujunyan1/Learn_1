using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 建造者
/// </summary>
public class Creater : Person
{
    public Creater()
    {
        personName = "建造者";
        this.Point = new HexCoordinates(0, 0);
        speed = 20;
        DefaultSpeed = 20;

        res = HeroConfigPool.GetHeroRes(0);

        AddFunction(new MoveFunction());
    }

    public Creater(HexCoordinates point)
    {
        personName = "建造者";
        this.Point = point;
        speed = 20;
        DefaultSpeed = 20;

        res = HeroConfigPool.GetHeroRes(0);

        AddFunction(new MoveFunction());
    }

    public override void Save(BinaryWriter writer)
    {
        base.Save(writer);
    }

    public override void Load(BinaryReader reader)
    {
        base.Load(reader);
    }

    public override void ClearData()
    {
        base.ClearData();
    }
}
