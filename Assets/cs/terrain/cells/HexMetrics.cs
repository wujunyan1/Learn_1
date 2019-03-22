using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 连接地形
public enum HexEdgeType
{
    Flat, Slope, Cliff
}

public enum HexTerrainType
{
    Water,              // 水
    Grassplot,          // 草地
    Wood,               // 树林
    Ridge,              // 山脊
    Desert,             // 沙漠
    Land,               // 土地
}


// 六边形 数据
public class HexMetrics : MonoBehaviour
{
    public const float outerRadius = 10f;

    public const float innerRadius = outerRadius * 0.866025404f;

    // 小六边形占比
    public const float solidFactor = 0.75f;

    public const float blendFactor = 1f - solidFactor;

    // 每个阶层的差值
    public const float elevationStep = 5f;

    // 画一个六边形所需的三角数量
    public const int HexTrianglesNum = 6;

    public static Vector3[] corners = {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius)
    };

    // 斜坡平台数量
    public const int terracesPerSlope = 2;

    // 斜坡斜面
    public const int terraceSteps = terracesPerSlope * 2 + 1;

    // 每个斜面比例
    public const float horizontalTerraceStepSize = 1f / terraceSteps;

    public const float verticalTerraceStepSize = 1f / (terracesPerSlope + 1);

    public static Vector3 GetFirstCorner(HexDirection direction)
    {
        return corners[(int)direction];
    }

    public static Vector3 GetSecondCorner(HexDirection direction)
    {
        return corners[(int)direction + 1];
    }

    public static Vector3 GetFirstSolidCorner(HexDirection direction)
    {
        return corners[(int)direction] * solidFactor;
    }

    public static Vector3 GetSecondSolidCorner(HexDirection direction)
    {
        return corners[(int)direction + 1] * solidFactor;
    }

    public static Vector3 GetBridge(HexDirection direction)
    {
        return (corners[(int)direction] + corners[(int)direction + 1]) * blendFactor;
    }

    // 插值公式为（1-t）a+tb。
    // 注意（1-t）a+tb=a-ta+tb=a+t（b-a）
    public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step)
    {
        float h = step * HexMetrics.horizontalTerraceStepSize;
        a.x += (b.x - a.x) * h;
        a.z += (b.z - a.z) * h;
        float v = ((step + 1) / 2) * HexMetrics.verticalTerraceStepSize;
        a.y += (b.y - a.y) * v;
        return a;
    }

    // 颜色阶梯变化
    public static Color TerraceLerp(Color a, Color b, int step)
    {
        float h = step * HexMetrics.horizontalTerraceStepSize;
        return Color.Lerp(a, b, h);
    }

    // 2条边之间插入4个4边行，算5个点
    public static EdgeVertices TerraceLerp(EdgeVertices a, EdgeVertices b, int step)
    {
        EdgeVertices result;
        result.v1 = HexMetrics.TerraceLerp(a.v1, b.v1, step);
        result.v2 = HexMetrics.TerraceLerp(a.v2, b.v2, step);
        result.v3 = HexMetrics.TerraceLerp(a.v3, b.v3, step);
        result.v4 = HexMetrics.TerraceLerp(a.v4, b.v4, step);
        result.v5 = HexMetrics.TerraceLerp(a.v5, b.v5, step);
        return result;
    }

    public static HexEdgeType GetEdgeType(int elevation1, int elevation2)
    {
        if (elevation1 == elevation2)
        {
            return HexEdgeType.Flat;
        }
        int delta = elevation2 - elevation1;
        if (delta == 1 || delta == -1)
        {
            return HexEdgeType.Slope;
        }
        return HexEdgeType.Cliff;
    }

    // 最大高度
    public static int maxElevation = 100;
    public static int hierarchyNum = 6;
    // 层级高度
    public static int[] elevationHierarchy = { 0, 20, 40, 60, 80, 100 };

    public static int GetElevationHierarchy(float height)
    {
        for (int i = 0; i < hierarchyNum; i++)
        {
            if (height <= elevationHierarchy[i])
            {
                return i;
            }
        }

        return 0;
    }

    // 每个网格的大小
    public const int chunkSizeX = 5, chunkSizeZ = 5;

    // 海岸线
    public const float coastline = 10f;

    // 最大降雨海拔
    public const float maxRainElevation = 100f; //80f;

    // 干湿格子的最大蓄水量
    public const float cellMaxPondage = 80f;//100f;

    // 每单位高度湖泊格子的最大蓄水量
    public const float waterCellMaxPondage = 30f; //120f;//100f;

    // 河床高度
    public const float streamBedElevationOffset = -1f;

    // 河水偏移高度
    public const float waterElevationOffset = -0.5f;

    public static Color[] terrainTypeColor = {
        //new Color(0.26f, 0.61f, 0.85f),
        new Color(0.91f, 0.34f, 0.11f),
        new Color(0.91f, 0.34f, 0.11f),
        new Color(0.91f, 0.34f, 0.11f),
        new Color(0.91f, 0.34f, 0.11f),
        new Color(0.91f, 0.34f, 0.11f),
        new Color(0.91f, 0.34f, 0.11f),
    };

    public static Color GetTerrainTypeColor(HexTerrainType type)
    {
        Color color = terrainTypeColor[(int)type];
        return color;
    }

    // 将海岸的浪花分为2边
    public const float lakesFactor = 0.6f;

    public static Vector3 GetFirstLakesCorner(HexDirection direction)
    {
        return corners[(int)direction] * lakesFactor;
    }

    public static Vector3 GetSecondLakesCorner(HexDirection direction)
    {
        return corners[(int)direction + 1] * lakesFactor;
    }

    public const float lakesBlendFactor = 1f - lakesFactor;

    public static Vector3 GetLakesBridge(HexDirection direction)
    {
        return (corners[(int)direction] + corners[(int)direction + 1]) *
            lakesBlendFactor;
    }
}

