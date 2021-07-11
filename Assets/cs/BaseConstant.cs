using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumSeason
{
    /// <summary>
    /// 春
    /// </summary>
    spring,

    /// <summary>
    /// 夏
    /// </summary>
    summer,

    /// <summary>
    /// 秋
    /// </summary>
    autumn,
    
    /// <summary>
    /// 冬
    /// </summary>
    winter
}

public static class EnumSeasonExtensions
{

    public static EnumSeason Next(this EnumSeason season)
    {
        return (int)season < 3 ? (season + 1) : 0;
    }


    public static string GetBattleTreeResource(this EnumSeason season)
    {
        return "prefabs/terrain/Tree/birch_tree";
    }

    public static string GetName(this EnumSeason season)
    {
        string s = "";
        switch (season)
        {
            case EnumSeason.spring:
                s = "春";
                break;
            case EnumSeason.summer:
                s = "夏";
                break;
            case EnumSeason.autumn:
                s = "秋";
                break;
            case EnumSeason.winter:
                s = "冬";
                break;
        }

        return s;
    }
}



public class BaseConstant
{
    public static readonly string[] LevelText = {"" , "Ⅰ","Ⅱ","Ⅲ","Ⅳ", "Ⅴ", "Ⅵ", "Ⅶ", "Ⅷ", "Ⅸ", "Ⅹ", "Ⅺ", "Ⅻ" };

    public const int RoundOfYear = 40;
    public const int RoundOfEeason = RoundOfYear / 4;


    public const int FarmYield = 400;

    // 每回合每个人口吃的粮食
    public const int PersonProvisions = 1;

    // 每回合每个普通兵吃的粮食
    public const int TroopProvisions = 2;

    // 每回合每个普通骑兵吃的粮食
    public const int TropCavalryProvisions = 12;

    static float[] temperatureBands = { 0.1f, 0.3f, 0.6f };

    static float[] moistureBands = { 0.12f, 0.28f, 0.85f };

    /* 草地0|沙漠2|土地3|冰4|泥地5
     * 无0|树1|
    t
    0.6 1         3           4         5
    0.3 1         2           3         4
    0.1 1         2           2         3
    0   0         0           0         0
        0        0.12       0.28       0.85 m
    */
    static int[] farmLevel = {
        0,0,0,0,
        1,2,2,3,
        1,2,3,4,
        1,3,4,5,
    };

    // 计算肥沃度
    public static int GetFertileLevel(HexCell cell)
    {
        if (cell.IsUnderwater)
        {
            return 0;
        }

        ClimateData climate = HexGrid.instance.GetClimateData(cell.index);
        float moisture = climate.moisture;
        float temperature = climate.temperature;

        int t = 0;
        for (; t < temperatureBands.Length; t++)
        {
            if (temperature < temperatureBands[t])
            {
                break;
            }
        }
        int m = 0;
        for (; m < moistureBands.Length; m++)
        {
            if (moisture < moistureBands[m])
            {
                break;
            }
        }
        int level = farmLevel[t * 4 + m];

        if(level == 0)
        {
            return level;
        }

        // 自己是河流或周围是河流，则加1的肥沃
        if (cell.HasRiver())
        {
            return level + 1;
        }

        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            HexCell nei_cell = cell.GetNeighbor(d);
            if(nei_cell != null && nei_cell.HasRiver())
            {
                return level + 1;
            }
        }

        return level;
    }
}
