using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HexTerrainType
{
    /// <summary>
    /// 草地
    /// </summary>
    Grassplot,          // 草地

    /// <summary>
    /// 山脊 // 不可移动
    /// </summary>
    Ridge,              // 山脊 // 不可移动
    /// <summary>
    /// 沙漠
    /// </summary>
    Desert,             // 沙漠
    /// <summary>
    /// 土地
    /// </summary>
    Land,               // 土地
    /// <summary>
    /// 冰
    /// </summary>
    Snow,               // 冰
    /// <summary>
    /// 泥地
    /// </summary>
    Mud               // 泥地
}

public static class HexTerrainTypeExtensions
{
    public static int[] distance =
    {
        1,  //"草地",
        1,  //"山脊",
        3,  //"沙漠",
        1,  //"土地",
        4,  //"雪地",
        4,  //泥地
        
    };

    public static int Distance(this HexTerrainType type)
    {
        return distance[(int)type];
    }

    // 0 沙子 1绿地 2泥地 3石头 4雪
    public static int[] TerrainLandType =
    {
        1,  //"草地",
        3,  //"山脊",
        0,  //"沙漠",
        3,  //"土地",
        4,  //"雪地",
        2,  //泥地
    };

    public static int LandType(this HexTerrainType type)
    {
        return TerrainLandType[(int)type];
    }

    public static string[] names =
    {
        "草地",
        "山脊",
        "沙漠",
        "土地",
        "雪地",
        "泥地",
    };

    public static string Name(this HexTerrainType type)
    {
        return names[(int)type];
    }

    public static float[] windDampings =
    {
        0.0f, //"草地",
        0.0f, //"山脊",
        0.0f, //"沙漠",
        0.0f, //"土地",
        0.0f, // 冰
        0.0f, // 泥地
    };

    public static float GetWindDampings(this HexTerrainType type)
    {
        return windDampings[(int)type];
    }

    // 扩散水系数
    public static float[] moistureDampings =
    {
        0.7f, //"草地",
        0.8f, //"山脊",
        1f, //"沙漠",
        0.8f, //"土地",
        0f, //  冰
        0.6f, // 泥地
    };

    public static float GetMoistureDampings(this HexTerrainType type)
    {
        return moistureDampings[(int)type];
    }

    // 温度吸收系数
    public static float[] temperatureDampings =
    {
        0.7f, //"草地",
        1f, //"山脊",
        1f, //"沙漠",
        0.9f, //"土地",
        0.4f, //  冰
        0.7f, // 泥地
    };

    public static float GetTemperatureDampings(this HexTerrainType type)
    {
        return temperatureDampings[(int)type];
    }

    // 水蒸发系数
    public static float[] evaporationDampings =
    {
        0.8f, //"草地",
        1f, //"山脊",
        1f, //"沙漠",
        0.9f, //"土地",
        0.8f, //  冰
        0.7f, // 泥地
    };

    public static float GetEvaporationDampings(this HexTerrainType type)
    {
        int index = (int)type;
        return evaporationDampings[index] / temperatureDampings[index];
    }
    
}





// 根据数据返回格子的类型
public static class HexTerrain
{
    public static void SetTerrainType(HexCell cell)
    {
        if (CheckWater(cell)) {

        }
        else if (CheckDesert(cell))
        {
            cell.TerrainType = HexTerrainType.Desert;
        }
        else if (CheckGrassplot(cell))
        {
            cell.TerrainType = HexTerrainType.Grassplot;
        }
    }

    // 判断是否是水
    static bool CheckWater(HexCell cell)
    {
        return cell.IsUnderwater;
    }

    // 判断是否是沙漠
    static bool CheckDesert(HexCell cell)
    {
        return false;
    }

    // 判断是否是树林
    static bool CheckWood(HexCell cell)
    {
        return false;
    }

    // 判断是否是草地
    static bool CheckGrassplot(HexCell cell)
    {
        
        return false;
    }

    // 判断是否是石矿
    static bool CheckQuarry(HexCell cell)
    {
        // 高度越高概率越大
        float m = 4 * Mathf.Sqrt(cell.Height / 10);
        float rand = Random.Range(0, 100);
        return rand < m;
    }

    // 判断是否是铁矿
    static bool CheckIron(HexCell cell)
    {
        // 高度越高概率越大
        float m = 2 * Mathf.Sqrt(cell.Height / 10);
        float rand = Random.Range(0, 100);
        return rand < m;
    }

    // 判断是否是金矿
    static bool CheckGold(HexCell cell)
    {
        // 高度越高概率越大
        float m = 1 * Mathf.Sqrt(cell.Height / 10);
        float rand = Random.Range(0, 100);
        return rand < m;
    }

    // 设置
    public static void SetCellStore(HexCell cell)
    {
        
    }

    // 获得格子的增加量
    public static int GetTerrainDeposit(HexCell cell)
    {
        HexTerrainType _type = cell.TerrainType;
        int deposit = 0;
        

        return deposit;
    }



    /// <summary>
    /// 温度吸收
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public static float GetTemperatureDampings(this HexCell cell)
    {
        if (cell.IsUnderwater)
        {
            return 0.5f;
        }

        return cell.TerrainType.GetTemperatureDampings();
    }

    /// <summary>
    /// 扩散水系数
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public static float GetMoistureDampings(this HexCell cell)
    {
        float baseMoisture = 1f;
        if (cell.IsUnderwater)
        {
            return baseMoisture;
        }
        if(cell.FeatureType == HexFeatureType.Wood)
        {
            baseMoisture = 0.8f;
        }
        return baseMoisture * cell.TerrainType.GetMoistureDampings();
    }


    /// <summary>
    /// 水蒸发系数
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static float GetEvaporationDampings(this HexCell cell)
    {
        float baseMoisture = 1f;
        if (cell.IsUnderwater)
        {
            return baseMoisture;
        }
        if (cell.FeatureType == HexFeatureType.Wood)
        {
            baseMoisture = 0.8f;
        }
        return baseMoisture * cell.TerrainType.GetEvaporationDampings();
    }

    /// <summary>
    /// 风滞留系数
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static float GetWindDampings(this HexCell cell)
    {
        return cell.TerrainType.GetWindDampings();
    }
}
