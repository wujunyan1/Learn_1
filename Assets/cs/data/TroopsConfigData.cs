using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;

public class TroopsConfigData
{
    
    public int key;
    public string solider_name;
    public int solider_type;

    public int size;
    public int troop_num;

    /// <summary>
    /// 招募回合
    /// </summary>
    public int recruit_round;

    /// <summary>
    /// 招募金币
    /// </summary>
    public int recruit_gold;

    /// <summary>
    /// 维护金币
    /// </summary>
    public int maintain_gold;

    /// <summary>
    /// 血量
    /// </summary>
    public int blood;

    /// <summary>
    /// 攻击 命中概率为  攻击力/敌方防御力 * 0.5
    /// </summary>
    public int ATK;

    public int power;

    /// <summary>
    /// 护甲
    /// </summary>
    public int armor;

    /// <summary>
    /// 防御 
    /// </summary>
    public int dodge;

    /// <summary>
    /// 盾牌
    /// </summary>
    public int shield;


    /// <summary>
    /// 格挡 100 防御远程
    /// </summary>
    public int parry;

    /// <summary>
    /// 冲锋
    /// </summary>
    public int charge;

    /// <summary>
    /// 士气
    /// </summary>
    public int morale;

    /// <summary>
    /// 射程
    /// </summary>
    public int l_ATKRange;

    /// <summary>
    /// 远程威力
    /// </summary>
    public int l_power;

    /// <summary>
    /// 精准
    /// </summary>
    public float accurate;

    /// <summary>
    /// 弹药
    /// </summary>
    public int ammo;

    /// <summary>
    /// 速度
    /// </summary>
    public int speed;

    /// <summary>
    /// 侦查范围
    /// </summary>
    public int visionRange;


    /// <summary>
    /// 食物
    /// </summary>
    public int food;

    /// <summary>
    /// 携带食物
    /// </summary>
    public int carryFood;


    public string head;
    public string modelName;
    public string model;
    
}

public enum TroopsType
{
    步兵,
    巨剑步兵,
    矛兵,
    持盾步兵,
    持盾矛兵,
    弓兵,
    骑兵,
    巨剑骑兵,
    冲击骑兵,
    攻城器械,
    开荒队,
}


public static class TroopConfigExTend
{
    public static string TROOP_TYPE_NAME(this TroopsType _type)
    {
        switch (_type)
        {
            case TroopsType.步兵:
                return "步兵";
            case TroopsType.巨剑步兵:
                return "巨剑步兵";
            case TroopsType.矛兵:
                return "矛兵";
            case TroopsType.持盾步兵:
                return "持盾步兵";
            case TroopsType.持盾矛兵:
                return "持盾矛兵";
            case TroopsType.弓兵:
                return "弓兵";
            case TroopsType.骑兵:
                return "骑兵";
            case TroopsType.巨剑骑兵:
                return "巨剑骑兵";
            case TroopsType.冲击骑兵:
                return "冲击骑兵";
            case TroopsType.攻城器械:
                return "攻城器械";
            case TroopsType.开荒队:
                return "开荒队";
        }

        return "未知兵种";
    }

    public static List<ObjFunction> GetFuncList(this TroopsType _type)
    {
        List<ObjFunction> funcList = new List<ObjFunction>();

        funcList.Add(ObjFunction.GetFunction<NormalFunction>());
        funcList.Add(ObjFunction.GetFunction<BuildbarrackFunction>());

        if(_type == TroopsType.开荒队)
        {
            funcList.Add(ObjFunction.GetFunction<BuildCityFunction>());
        }
        else
        {
            funcList.Add(ObjFunction.GetFunction<PlunderFunction>());
        }

        return funcList;
    }
}

public class TroopsConfigDataManager
{
    private static List<TroopsConfigData> datas;

    public static void LoadConfig()
    {
        datas = new List<TroopsConfigData>();
        DataTable data = NPOIOprateExcel.ExcelUtility.ExcelToDataTable("config/troops.xlsx");

        int lineNum = data.Rows.Count;
        int colNum = data.Columns.Count;

        for (int i = 0; i < lineNum; i++)
        {
            TroopsConfigData _config = new TroopsConfigData();
            for (int j = 0; j < colNum; j++)
            {
                ObjBaseTool.SetProperty(_config, data.Columns[j].ToString(), data.Rows[i][j].ToString());
            }
            
            //ObjBaseTool.PrintObj(_config);

            datas.Add(_config);
        }
    }

    public static List<TroopsConfigData> GetConfigList()
    {
        return datas;
    }

    public static TroopsConfigData GetConfig(int index)
    {
        return datas[index];
    }

    public static TroopsConfigData GetConfigBySoliderName(string solider_name)
    {
        foreach (var item in datas)
        {
            if(item.solider_name == solider_name)
            {
                return item;
            }
        }
        return null;
    }
    
}
