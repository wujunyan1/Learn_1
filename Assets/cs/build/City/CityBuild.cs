using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public enum CityBuildType
{
    Tavern,
    Barracks,
}

// 城市建筑
public class CityBuild : Object
{
    protected CityBuildType type;
    public CityBuildType BuildType
    {
        get
        {
            return type;
        }
    }

    protected string name;
    public string Name
    {
        get
        {
            return name;
        }
    }

    // 等级
    protected int level = 0;

    protected int maxLevel;

    // 升级或建造已过的回合数
    protected int upgradeRound = 0;
    protected int NeedRound = 0;

    public virtual void Save(BinaryWriter writer)
    {
        writer.Write((byte)type);
        writer.Write((byte)level);
        writer.Write((byte)upgradeRound);
        writer.Write((byte)NeedRound);
    }

    public virtual void Load(BinaryReader reader)
    {
        //type = (CityBuildType)reader.ReadByte();
        level = reader.ReadByte();
        upgradeRound = reader.ReadByte();
        NeedRound = reader.ReadByte();
    }

    public virtual void AddLevel()
    {
        level++;
    }

    public virtual void NextRound()
    {
        if(upgradeRound >= NeedRound)
        {
            return;
        }
        upgradeRound++;
        if(upgradeRound == NeedRound)
        {
            AddLevel();
        }
    }
}
