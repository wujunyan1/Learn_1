using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDataManager : EventObject
{
    /// <summary>
    /// 阵营数量
    /// </summary>
    int campNum;

    /// <summary>
    /// 玩家的阵营
    /// </summary>
    int playerCamp;

    BattleTeamData[] teamDatas;

    public int octaves;
    public float lacunarity;
    public float persistence;

    public int warMapSeed = 23;
    public float frequency = 10;

    public EnumSeason season;


    static BattleDataManager CurrBattleDataManager;

    public BattleDataManager(int _campNum, int _playerCamp)
    {
        campNum = _campNum;
        playerCamp = _playerCamp;
        teamDatas = new BattleTeamData[campNum];

        for(int i = 0; i < campNum; i++)
        {
            teamDatas[i] = new BattleTeamData(i);
        }
    }

    /// <summary>
    /// 添加一个人物
    /// </summary>
    public void AddSoldierByConfig(int camp, string key)
    {
        BattleTeamData teamData = teamDatas[camp];
        teamData.AddSoldier(key);
    }

    /// <summary>
    /// 移除一个人物
    /// </summary>
    public void RemoveSoldierByConfig(int camp, int index)
    {
        BattleTeamData teamData = teamDatas[camp];
        teamData.RemoveSoldier(index);
    }

    public int GetTeamNum()
    {
        return campNum;
    }

    public BattleTeamData[] GetTeamDatas()
    {
        return teamDatas;
    }

    public BattleTeamData GetTeamData(int camp)
    {
        if(camp >= campNum)
        {
            return null;
        }
        return teamDatas[camp];
    }

    public BattleSoldierData GetBattleSoldierData(string key)
    {
        SoldierConfigData soldierConfigData = HeroConfig.GetInstance().GetSoldierConfig(key);
        SoldierConfigData addData = soldierConfigData.Clone();

        BattleSoldierData data = new BattleSoldierData();
        data.SetConfigData(addData);

        return data;
    }

    public BattleSoldierData GetBattleSoldierData(int camp, int index)
    {
        BattleTeamData teamData = teamDatas[camp];
        SoldierConfigData soldierConfigData = teamData.GetSoldierConfigs()[index];
        SoldierConfigData addData = soldierConfigData.Clone();

        BattleSoldierData data = new BattleSoldierData();
        data.SetConfigData(addData);

        return data;
    }

    public bool CanStartBattle()
    {
        if(campNum < 2)
        {
            return false;
        }

        foreach(BattleTeamData teamData in teamDatas)
        {
            if(teamData.GetSoldierConfigs().Count < 1)
            {
                return false;
            }
        }

        return true;
    }

    public void SetBattleData()
    {
        CurrBattleDataManager = this;
    }

    public static BattleDataManager GetBattleData()
    {
        return CurrBattleDataManager;
    }

    public int GetPlayerCamp()
    {
        return playerCamp;
    }
}
