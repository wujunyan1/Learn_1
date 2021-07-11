using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public enum CityBuildType
{
    TownHall,   //市政厅
    Dwellings,  // 民居
    Barracks,   // 兵营
    Tavern,     // 酒馆
    Granary,    // 粮仓
}

// 城市建筑
public class CityBuild : SaveLoadInterface, RoundObject
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

    protected City city;

    // 等级
    protected int level = 0;

    // 升级或建造已过的回合数
    protected int upgradeRound = 0;
    protected bool isBuild = false;

    protected CityBuildConfig config;

    protected List<CityBuff> buffs = new List<CityBuff>();


    ~CityBuild()
    {
        if(city != null)
        {
            foreach (var buff in buffs)
            {
                city.RemoveCityBuff(buff);
            }
        }
    }

    public virtual void SetCity(City city)
    {
        this.city = city;
    }

    public virtual void Init(CityBuildConfig cityBuildConfig)
    {
        
    }

    public virtual void StartEffect()
    {
    }

    public virtual void AddLevel()
    {
        level++;
        isBuild = true;
        upgradeRound = 0;
        SetConfig(CityBuildConfigExtend.GetCityBuildConfig(type, level));
        
    }

    public void SetConfig(CityBuildConfig cityBuildConfig)
    {
        config = cityBuildConfig;
        level = config.level;

        if (!isBuild)
        {
            StartEffect();
        }
    }

    public int GetLevel()
    {
        return level;
    }

    public bool IsBuilding()
    {
        return isBuild;
    }

    public int GetBuildedNeedRoundNum()
    {
        return config.buildRound - upgradeRound;
    }

    public CityBuildConfig GetCityBuildConfig()
    {
        return config;
    }

    public virtual void Save(BinaryWriter writer)
    {
        writer.Write((byte)level);
        writer.Write((byte)upgradeRound);
        writer.Write(isBuild);
    }

    public virtual IEnumerator Load(BinaryReader reader)
    {
        level = reader.ReadByte();
        upgradeRound = reader.ReadByte();
        isBuild = reader.ReadBoolean();
        yield return null;
    }

    public virtual void NextRound()
    {
        upgradeRound++;

        if (isBuild && upgradeRound >= config.buildRound)
        {
            isBuild = false;
            StartEffect();
        }
    }

    public virtual void LaterNextRound()
    {
    }

    public static CityBuild CreateCityBuildByLoad(CityBuildType cityBuildType)
    {
        CityBuild cityBuild = null;

        switch (cityBuildType)
        {
            case CityBuildType.TownHall:
                cityBuild = new TownHall();
                break;
            case CityBuildType.Dwellings:
                break;
            case CityBuildType.Barracks:
                cityBuild = new Barracks();
                break;
            case CityBuildType.Tavern:
                break;
            default:
                break;
        }

        return cityBuild;
    }
}
