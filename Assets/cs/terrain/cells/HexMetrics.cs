using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 连接地形
public enum HexEdgeType
{
    Flat, Slope, Cliff
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

    public static Vector3 GetSolidCorner(HexDirection direction)
    {
        return (corners[(int)direction] + corners[(int)direction + 1]) * solidFactor / 2;
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

        return hierarchyNum - 1;
    }

    // 每个网格的大小
    public const int chunkSizeX = 5, chunkSizeZ = 5;

    // 海岸线
    public const float coastline = 10f;

    // 最大降雨海拔
    public const float maxRainElevation = 100f; //80f;

    // 干湿格子的最大蓄水量
    public const int cellMaxPondage = 100;//100f;

    // 格子的蓄水量
    public const int cellWaterStore = 80;//100f;

    // 每单位高度湖泊格子的最大蓄水量
    public const int waterCellMaxPondage = 10; //120f;//100f;

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

    public static Texture2D noiseSource;
    public const float noiseScale = 0.003f;
    public const float cellPerturbStrength = 4f;

    public static Vector4 SampleNoise(Vector3 position)
    {
        return noiseSource.GetPixelBilinear(
            position.x * noiseScale,
            position.z * noiseScale
        );
    }

    public static Vector3 Perturb(Vector3 position)
    {
        Vector4 sample = SampleNoise(position);
        position.x += (sample.x * 2f - 1f) * cellPerturbStrength;
        position.z += (sample.z * 2f - 1f) * cellPerturbStrength;
        return position;
    }

    // 在三角区域内，按密度随机生成N个点
    public static Vector3[] RandomTriangleVectors(Vector3 A, Vector3 B, Vector3 C, float density)
    {
        float a = Vector3.Distance(A, B);
        float b = Vector3.Distance(B, C);
        float c = Vector3.Distance(C, A);

        float p = (a + b + c) / 2.0f;//计算半周长
        float area = Mathf.Sqrt(p * (p - a) * (p - b) * (p - c));//海伦公式求面积

        int n = (int)(area * density);
        n = n < 1 ? 1 : n;

        Vector3[] positions = new Vector3[n];
        for (int i = 0; i < n; i++)
        {
            // 扰乱位置
            positions[i] = HexMetrics.RandomTriangleVector3(A, B, C);
        }

        return positions;
    }

    public static Vector3 RandomTriangleVector3(Vector3 A, Vector3 B, Vector3 C)
    {
        float r1 = Random.value;
        float r2 = Random.value;
        Vector3 P = (1 - Mathf.Sqrt(r1)) * A + Mathf.Sqrt(r1) * (1 - r2) * B + Mathf.Sqrt(r1) * r2 * C;
        return P;
    }

    // 木头产量系数
    public const float WoodDepositCoefficient = 0.01f;
    // 石头产量系数
    public const float QuarryDepositCoefficient = 0.3f;
    // 铁产量系数
    public const float IronDepositCoefficient = 0.1f;

    public const int InitialCellResourceNum = 100;

    public static int HexDirectionNum = 6;
}

