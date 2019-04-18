using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HexTerrainType
{
    /// <summary>
    /// 水
    /// </summary>
    Water,              // 水
    /// <summary>
    /// 草地
    /// </summary>
    Grassplot,          // 草地
    /// <summary>
    /// 树林
    /// </summary>
    Wood,               // 树林
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
    /// 石矿
    /// </summary>
    Quarry,             // 石矿
    /// <summary>
    /// 铁矿
    /// </summary>
    Iron,               // 铁矿
    /// <summary>
    /// 金矿
    /// </summary>
    Gold,               // 金矿
}

public static class HexTerrainTypeExtensions
{
    public static int[] distance =
    {
        int.MaxValue,
        0,
        3,
        int.MaxValue,
        2,
        1,
        4,
        4,
        4,
    };

    public static int Distance(this HexTerrainType type)
    {
        return distance[(int)type];
    }
}

// 根据数据返回格子的类型
public class HexTerrain
{
    public static void SetTerrainType(HexCell cell)
    {
        if (CheckWater(cell)) {

        }
        else if (CheckWater(cell))
        {
            cell.TerrainType = HexTerrainType.Desert;
        }
        else if (CheckWood(cell))
        {
            cell.TerrainType = HexTerrainType.Wood;
        }
        else if (CheckDesert(cell))
        {
            cell.TerrainType = HexTerrainType.Desert;
        }
        else if (CheckQuarry(cell))
        {
            cell.TerrainType = HexTerrainType.Quarry;
        }
        else if (CheckIron(cell))
        {
            cell.TerrainType = HexTerrainType.Iron;
        }
        else if (CheckGold(cell))
        {
            cell.TerrainType = HexTerrainType.Gold;
        }
        else if (CheckGrassplot(cell))
        {
            cell.TerrainType = HexTerrainType.Grassplot;
        }
    }

    // 判断是否是水
    static bool CheckWater(HexCell cell)
    {
        return cell.TerrainType == HexTerrainType.Water;
    }

    // 判断是否是沙漠
    static bool CheckDesert(HexCell cell)
    {
        // 降雨量少，蓄水量也少
        if(cell.Rain < 20 && cell.Pondage < 20)
        {
            return true;
        }
        return false;
    }

    // 判断是否是树林
    static bool CheckWood(HexCell cell)
    {
        //降雨量正常，蓄水量正常
        if (cell.Rain >= 20 && cell.Rain < 70 && cell.Pondage >= 50 && cell.Pondage < 90)
        {
            return true;
        }
        return false;
    }

    // 判断是否是草地
    static bool CheckGrassplot(HexCell cell)
    {
        //降雨量正常，蓄水量少
        if (cell.Rain >= 20 && cell.Rain < 70 && cell.Pondage >= 20 && cell.Pondage < 50)
        {
            return true;
        }
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
        HexTerrainType _type = cell.TerrainType;
        switch (_type)
        {
            case HexTerrainType.Wood:
                // 木头，看蓄水量和降雨量
                float c = cell.Pondage * HexMetrics.WoodDepositCoefficient + cell.Rain * HexMetrics.WoodDepositCoefficient;
                cell.Store = (int)c * 10 * HexMetrics.InitialCellResourceNum;
                break;
            case HexTerrainType.Quarry:
                // 石头，看高度  固定产量
                cell.Store = (int)(cell.Height * HexMetrics.QuarryDepositCoefficient * HexMetrics.InitialCellResourceNum);
                break;
            case HexTerrainType.Land:
                // 
                break;
            case HexTerrainType.Iron:
                // 铁，看高度 固定产量
                cell.Store = (int)(cell.Height * HexMetrics.IronDepositCoefficient) * HexMetrics.InitialCellResourceNum;
                break;
            case HexTerrainType.Gold:
                // 金，没有增加量 所有存储量大
                cell.Store = (int)(cell.Height * HexMetrics.IronDepositCoefficient) * 100 * HexMetrics.InitialCellResourceNum;
                break;
        }
    }

    // 获得格子的增加量
    public static int GetTerrainDeposit(HexCell cell)
    {
        HexTerrainType _type = cell.TerrainType;
        int deposit = 0;
        switch (_type)
        {
            case HexTerrainType.Wood:
                // 木头，看蓄水量和降雨量
                float c = cell.Pondage * HexMetrics.WoodDepositCoefficient + cell.Rain * HexMetrics.WoodDepositCoefficient;
                deposit = (int)(cell.Store * c);
                break;
            case HexTerrainType.Quarry:
                // 石头，看高度  固定产量
                deposit = (int)(cell.Height * HexMetrics.QuarryDepositCoefficient);
                break;
            case HexTerrainType.Land:
                // 
                break;
            case HexTerrainType.Iron:
                // 铁，看高度 固定产量
                deposit = (int)(cell.Height * HexMetrics.IronDepositCoefficient);
                break;
            case HexTerrainType.Gold:
                // 金，没有增加量
                break;
        }

        return deposit;
    }
}
