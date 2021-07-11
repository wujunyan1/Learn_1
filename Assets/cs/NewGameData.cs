using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NewGameData : SaveLoadInterface
{
    int x, z;
    public int X
    {
        get
        {
            return x;
        }
    }

    public int Z
    {
        get
        {
            return z;
        }
    }
    
    public int mapSeed;

    public int width
    {
        get
        {
            return x;
        }
    }

    public int height
    {
        get
        {
            return z;
        }
    }

    public bool isCanEditor = true;

    public bool isRandMap = true;

    // 后续未加入
    // 湿度
    // 降雨度
    // 高度
    // 平整度

    int cellCount = 0;
    public int CellCount
    {
        get
        {
            return cellCount;
        }
    }

    public NewGameData(int x, int z)
    {
        this.x = x;
        this.z = z;
        cellCount = x * z;
    }

    public int campNum;

    /// <summary>
    /// 聚集度，越高的聚集度越是呈现的像圆形
    /// </summary>
    [Range(0f, 0.5f)]
    public float jitterProbability = 0.25f;

    /// <summary>
    /// 聚集块最小格子数
    /// </summary>
    [Range(20, 200)]
    public int chunkSizeMin = 30;

    /// <summary>
    /// 聚集块最大格子数
    /// </summary>
    [Range(20, 200)]
    public int chunkSizeMax = 100;

    /// <summary>
    /// 陆地占比
    /// </summary>
    [Range(5, 95)]
    public int landPercentage = 50;

    /// <summary>
    /// 海平面
    /// </summary>
    [Range(1, 5)]
    public int waterLevel = 3;

    // 更高的提高高度概率
    [Range(0f, 1f)]
    public float highRiseProbability = 0.25f;

    /// <summary>
    /// 土地下沉概率， 目标是让陆地更不规则
    /// </summary>
    [Range(0f, 0.4f)]
    public float sinkProbability = 0.2f;

    /// <summary>
    /// 最低海拔高度
    /// </summary>
    [Range(-4, 0)]
    public int elevationMinimum = -2;

    /// <summary>
    /// 最高海拔高度
    /// </summary>
    [Range(6, 10)]
    public int elevationMaximum = 10;

    /// <summary>
    /// 是否使用固定的种子
    /// </summary>
    public bool useFixedSeed;

    /// <summary>
    /// 地图边缘 不会变为陆地的距离
    /// </summary>
    [Range(0, 10)]
    public int mapBorderX = 5;

    /// <summary>
    /// 地图边缘 不会变为陆地的距离
    /// </summary>
    [Range(0, 10)]
    public int mapBorderZ = 5;

    /// <summary>
    /// 大陆块之间的间距
    /// </summary>
    [Range(0, 10)]
    public int regionBorder = 5;

    /// <summary>
    /// 大陆块数量
    /// </summary>
    [Range(1, 4)]
    public int regionCount = 4;

    /// <summary>
    /// 侵蚀率，使相邻的格子更平缓
    /// </summary>
    [Range(0, 100)]
    public int erosionPercentage = 50;

    /// <summary>
    /// 赤道
    /// </summary>
    public int equator {
        get
        {
            return Mathf.RoundToInt(f_equator);
        }
        set
        {
            f_equator = value;
        }
    }

    public float f_equator;

    /// <summary>
    /// 赤道移动方向
    /// </summary>
    public int dir_equator = 1;

    /// <summary>
    /// 赤道最低点
    /// </summary>
    [Range(0f, 1f)]
    public float lowEquator = 0.4f;

    /// <summary>
    /// 赤道最高点
    /// </summary>
    [Range(0f, 1f)]
    public float highEquator = 0.6f;


    [Range(0f, 1f)]
    public float lowTemperature = 0f;

    [Range(0f, 1f)]
    public float highTemperature = 50f;

    /// <summary>
    /// 增加一点温度的随机性，这点随机性
    /// </summary>
    [Range(0f, 1f)]
    public float temperatureJitter = 0.1f;

    /// <summary>
    /// 温度4种随机性，使得气候变化成为可能
    /// </summary>
    [Range(0, 4)]
    public int temperatureJitterChannel = 0;

    /// <summary>
    /// 冰点
    /// </summary>
    [Range(0f, 1f)]
    public float temperatureIcePoint = 0.1f;

    float icePoint;
    public float IcePoint
    {
        get
        {
            return icePoint;
        }
    }

    /// <summary>
    /// 蒸发因子
    /// </summary>
    [Range(0f, 1f)]
    public float evaporationFactor = 1f;

    /// <summary>
    /// 蒸发因子
    /// </summary>
    [Range(0f, 1f)]
    public float landEvaporationFactor = 0.5f;

    /// <summary>
    /// 风，气压扩散速度
    /// </summary>
    [Range(0f, 1f)]
    public float cloudsDiffusion = 0.5f;

    /// <summary>
    /// 湿润因子，指云通过非大降雨方式变为水的因子
    /// </summary>
    [Range(0f, 1f)]
    public float precipitationFactor = 0.125f;

    /// <summary>
    /// 降雨因子，概率
    /// </summary>
    [Range(0f, 1f)]
    public float rainFactor = 0.1f;

    /// <summary>
    /// 流失水分，向低处格子流的水量
    /// </summary>
    [Range(0f, 1f)]
    public float runoffFactor = 0.25f; // 0.125f;

    /// <summary>
    /// 扩散水分，向周围平坦格子流的水量
    /// </summary>
    [Range(0f, 1f)]
    public float seepageFactor = 0.125f; // 0.0625f;

    /// <summary>
    /// 气候演化回合
    /// </summary>
    public int boostClimate = 40;

    /// <summary>
    /// 每个格子的初始湿度
    /// </summary>
    [Range(0f, 1f)]
    public float startingMoisture = 0.1f;

    /// <summary>
    /// 太阳经度，表示白天和黑夜,每个回合偏移一点，10回合转一圈
    /// </summary>
    public int sunLongitude = 0;

    /// <summary>
    /// 河流格子数 占比 陆地数 %
    /// </summary>
    [Range(0, 20)]
    public int riverPercentage = 10;

    /// <summary>
    /// 实际的陆地数量
    /// </summary>
    public int landCells = 0;

    /// <summary>
    /// 河流变为湖泊概率
    /// </summary>
    [Range(0f, 1f)]
    public float extraLakeProbability = 0.25f;

    /// <summary>
    /// 是否是类星球，东西循环
    /// </summary>
    public bool wrapping = false;

    public void InitData()
    {
        icePoint = Mathf.Lerp(lowTemperature, highTemperature, temperatureIcePoint);

        regionCount = 1+ Mathf.FloorToInt(Random.value * 4);
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(x);
        writer.Write(z);

        writer.Write(mapSeed);
        writer.Write(isCanEditor);
        writer.Write(isRandMap);

        writer.Write((byte)campNum);
        writer.Write(jitterProbability);
        writer.Write((byte)chunkSizeMin);

        writer.Write((byte)chunkSizeMax);
        writer.Write((byte)landPercentage);
        writer.Write((byte)waterLevel);
        writer.Write(highRiseProbability);
        writer.Write(sinkProbability);

        writer.Write((byte)elevationMinimum);
        writer.Write((byte)elevationMaximum);
        writer.Write(useFixedSeed);

        writer.Write((byte)mapBorderX);
        writer.Write((byte)mapBorderZ);

        writer.Write((byte)regionBorder);

        writer.Write(regionCount);

        writer.Write((byte)erosionPercentage);
        writer.Write(f_equator);

        writer.Write((byte)(dir_equator+1));
        writer.Write(lowEquator);
        writer.Write(highEquator);


        writer.Write(lowTemperature);
        writer.Write(highTemperature);

        writer.Write(temperatureJitter);
        writer.Write((byte)temperatureJitterChannel);

        writer.Write(temperatureIcePoint);

        writer.Write(evaporationFactor);

        writer.Write(landEvaporationFactor);

        writer.Write(cloudsDiffusion);

        writer.Write(precipitationFactor);

        writer.Write(rainFactor);

        writer.Write(runoffFactor); // 0.125f;

        writer.Write(seepageFactor); // 0.0625f;

        writer.Write(boostClimate);

        writer.Write(startingMoisture);

        writer.Write(sunLongitude);

        writer.Write((byte)riverPercentage);

        writer.Write(landCells);

        writer.Write(extraLakeProbability);

        writer.Write(wrapping);
    }

    public IEnumerator Load(BinaryReader reader)
    {
        x = reader.ReadInt32();
        z = reader.ReadInt32();
        

        mapSeed = reader.ReadInt32();
        isCanEditor = reader.ReadBoolean();
        isRandMap = reader.ReadBoolean();

        campNum = reader.ReadByte();
        jitterProbability = reader.ReadSingle();
        chunkSizeMin = reader.ReadByte();
        chunkSizeMax = reader.ReadByte();

        landPercentage = reader.ReadByte();
        waterLevel = reader.ReadByte();
        highRiseProbability = reader.ReadSingle();
        sinkProbability = reader.ReadSingle();

        elevationMinimum = reader.ReadByte();
        elevationMaximum = reader.ReadByte();
        useFixedSeed = reader.ReadBoolean();

        mapBorderX = reader.ReadByte();
        mapBorderZ = reader.ReadByte();
        regionBorder = reader.ReadByte();

        regionCount = reader.ReadInt32();
        erosionPercentage = reader.ReadByte();
        f_equator = reader.ReadSingle();

        dir_equator = (int)reader.ReadByte() - 1;
        lowEquator = reader.ReadSingle();
        highEquator = reader.ReadSingle();

        lowTemperature = reader.ReadSingle();
        highTemperature = reader.ReadSingle();
        temperatureJitter = reader.ReadSingle();
        temperatureJitterChannel = reader.ReadByte();

        temperatureIcePoint = reader.ReadSingle();
        evaporationFactor = reader.ReadSingle();
        landEvaporationFactor = reader.ReadSingle();
        cloudsDiffusion = reader.ReadSingle();

        precipitationFactor = reader.ReadSingle();
        rainFactor = reader.ReadSingle();

        runoffFactor = reader.ReadSingle();
        seepageFactor = reader.ReadSingle();

        boostClimate = reader.ReadInt32();

        startingMoisture = reader.ReadSingle();
        sunLongitude = reader.ReadInt32();

        riverPercentage = reader.ReadByte();
        landCells = reader.ReadInt32();

        extraLakeProbability = reader.ReadSingle();
        wrapping = reader.ReadBoolean();

        yield return null;

        cellCount = x * z;
        icePoint = Mathf.Lerp(lowTemperature, highTemperature, temperatureIcePoint);
    }
}
