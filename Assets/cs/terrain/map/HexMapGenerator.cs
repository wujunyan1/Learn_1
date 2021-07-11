using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapGenerator : EventComponent
{
    public HexGrid grid;
    NewGameData data;

    HexCellPriorityQueue searchFrontier;
    int searchFrontierPhase;

    struct Biome
    {
        public HexTerrainType terrain;
        public HexFeatureType feature;
        public int plant;

        public Biome(int terrain, int feature, int plant)
        {
            this.terrain = (HexTerrainType)terrain;
            this.feature = (HexFeatureType)feature;
            this.plant = plant;
        }
    }

    //HexTerrainType.Snow;
    //HexTerrainType.Desert;
    //HexTerrainType.Land;
    //HexTerrainType.Grassplot;
    //HexTerrainType.Wood;
    //HexTerrainType.Mud;

    static float[] temperatureBands = { 0.1f, 0.3f, 0.6f };

    static float[] moistureBands = { 0.12f, 0.28f, 0.85f };

    /* 草地0|沙漠2|土地3|冰4|泥地5
     * 无0|树1|
    t
    0.6 沙漠|无  草地|树1  草地|树2   草地|树3
    0.3 沙漠|无  草地|无   草地|树1   草地|树2
    0.1 沙漠|无  陆地|无   陆地|树1   陆地|树1
    0   沙漠|无  冰|无     冰|无       冰|无
        0        0.12       0.28       0.85 m
    */


    static Biome[] biomes = {
        new Biome(2,0,0), new Biome(4,0,0), new Biome(4,0,0), new Biome(4,0,0),
        new Biome(2,0,0), new Biome(3,0,0), new Biome(3,1,1), new Biome(3,1,1),
        new Biome(2,0,0), new Biome(0,0,0), new Biome(0,1,1), new Biome(0,1,2),
        new Biome(2,0,0), new Biome(0,1,1), new Biome(0,1,2), new Biome(0,1,3)
    };

    // 地图区域块，可以将地图切割为几块，表示不同的大陆，让 他们不相连
    struct MapRegion
    {
        public int xMin, xMax, zMin, zMax;
    }

    List<MapRegion> regions;

    public void SetNewGameData(NewGameData data)
    {
        this.data = data;
    }


    public IEnumerator GenerateMap(NewGameData data)
    {
        this.data = data;
        yield return null;

        IEnumerator itor = grid.CreateMap(data);
        while (itor.MoveNext()) {
            yield return null;
        }
        Random.State originalRandomState = Random.state;
        if (!data.useFixedSeed)
        {
            data.mapSeed = Random.Range(0, int.MaxValue);
            data.mapSeed ^= (int)System.DateTime.Now.Ticks;
            data.mapSeed ^= (int)Time.time;
            data.mapSeed &= int.MaxValue;
        }
        Random.InitState(data.mapSeed);

        data.InitData();

        // 搜素队列， 用于抬高或降低大块土地时，搜索周围土地用
        if (searchFrontier == null)
        {
            searchFrontier = new HexCellPriorityQueue();
        }
        // 起始默认所有格子都是水
        for (int i = 0; i < data.CellCount; i++)
        {
            HexCell cell = grid.GetCell(i);
            cell.LakesLevel = data.waterLevel;
        }

        CreateRegions();
        yield return null;
        CreateLand();
        yield return null;
        ErodeLand();
        yield return null;
        SetTerrainType();
        yield return null;

        for (int i = 0; i < data.boostClimate; i++)
        {
            EvolveClimate();
            yield return null;
        }
        CreateRivers();
        yield return null;
        SetTerrainType();
        yield return null;

        // 测试 设置格子属性
        //SetTerrainType();

        HexGrid.instance.Refresh();

        yield return null;

        // 所搜 结束后将所有的标记重置，保证游戏进行时，路径搜索的正确性
        for (int i = 0; i < data.CellCount; i++)
        {
            grid.GetCell(i).SearchPhase = 0;
        }

        yield return null;
        Random.state = originalRandomState;
    }
    
    /// <summary>
    /// 创建大陆块
    /// </summary>
    void CreateRegions()
    {
        if (regions == null)
        {
            regions = new List<MapRegion>();
        }
        else
        {
            regions.Clear();
        }


        int borderX = data.wrapping ? 0 : data.mapBorderX;
        MapRegion region;
        region.xMin = borderX;
        region.xMax = grid.cellCountX - borderX;
        region.zMin = data.mapBorderZ;
        region.zMax = grid.cellCountZ - data.mapBorderZ;
        regions.Add(region);

        while(regions.Count < data.regionCount)
        {
            MapRegion re = regions[0];
            regions.RemoveAt(0);

            MapRegion r1, r2;
            RandomMapRegion(re, out r1, out r2);
            regions.Add(r1);
            regions.Add(r2);
        }

        /**
        int randN = data.regionBorder / 2;
        switch (data.regionCount)
        {
            default:
                region.xMin = data.mapBorderX;
                region.xMax = grid.cellCountX - data.mapBorderX;
                region.zMin = data.mapBorderZ;
                region.zMax = grid.cellCountZ - data.mapBorderZ;
                regions.Add(region);
                break;
            case 2:
                if (Random.value < 0.5f)
                {
                    region.xMin = data.mapBorderX;
                    region.xMax = grid.cellCountX / 2 - data.regionBorder + Random.Range(-randN, randN);
                    region.zMin = data.mapBorderZ;
                    region.zMax = grid.cellCountZ - data.mapBorderZ;
                    regions.Add(region);
                    region.xMin = grid.cellCountX / 2 + data.regionBorder + Random.Range(-randN, randN);
                    region.xMax = grid.cellCountX - data.mapBorderX;
                    regions.Add(region);
                }
                else
                {
                    region.xMin = data.mapBorderX;
                    region.xMax = grid.cellCountX - data.mapBorderX;
                    region.zMin = data.mapBorderZ;
                    region.zMax = grid.cellCountZ / 2 - data.regionBorder + Random.Range(-randN, randN);
                    regions.Add(region);
                    region.zMin = grid.cellCountZ / 2 + data.regionBorder + Random.Range(-randN, randN);
                    region.zMax = grid.cellCountZ - data.mapBorderZ;
                    regions.Add(region);
                }
                break;
            case 3:
                region.xMin = data.mapBorderX;
                region.xMax = grid.cellCountX / 3 - data.regionBorder + Random.Range(-randN, randN);
                region.zMin = data.mapBorderZ;
                region.zMax = grid.cellCountZ - data.mapBorderZ;
                regions.Add(region);
                region.xMin = grid.cellCountX / 3 + data.regionBorder + Random.Range(-randN, randN);
                region.xMax = grid.cellCountX * 2 / 3 - data.regionBorder + Random.Range(-randN, randN);
                regions.Add(region);
                region.xMin = grid.cellCountX * 2 / 3 + data.regionBorder + Random.Range(-randN, randN);
                region.xMax = grid.cellCountX - data.mapBorderX;
                regions.Add(region);
                break;
            case 4:
                region.xMin = data.mapBorderX;
                region.xMax = grid.cellCountX / 2 - data.regionBorder + Random.Range(-randN, randN);
                region.zMin = data.mapBorderZ;
                region.zMax = grid.cellCountZ / 2 - data.regionBorder + Random.Range(-randN, randN);
                regions.Add(region);
                region.xMin = grid.cellCountX / 2 + data.regionBorder + Random.Range(-randN, randN);
                region.xMax = grid.cellCountX - data.mapBorderX;
                regions.Add(region);
                region.zMin = grid.cellCountZ / 2 + data.regionBorder + Random.Range(-randN, randN);
                region.zMax = grid.cellCountZ - data.mapBorderZ;
                regions.Add(region);
                region.xMin = data.mapBorderX;
                region.xMax = grid.cellCountX / 2 - data.regionBorder + Random.Range(-randN, randN);
                regions.Add(region);
                break;
        }
        
        */
    }

    // 随机将region 分为2块
    void RandomMapRegion(MapRegion region, out MapRegion r1, out MapRegion r2)
    {
        int randN = data.regionBorder / 2;
        MapRegion[] regions = new MapRegion[2];

        if(Random.value < 0.5f)
        {
            int len = region.xMax - region.xMin;
            int n = len / 6;

            int x = region.xMin + len / 2 + Random.Range(-n, n);
            r1.xMin = region.xMin;
            r1.xMax = x - data.regionBorder;
            r1.zMin = region.zMin;
            r1.zMax = region.zMax;

            r2.xMin = x + data.regionBorder;
            r2.xMax = region.xMax;
            r2.zMin = region.zMin;
            r2.zMax = region.zMax;
        }
        else
        {
            int len = region.zMax - region.zMin;
            int n = len / 6;

            int z = region.zMin + (len / 2) + Random.Range(-n, n);
            r1.xMin = region.xMin;
            r1.xMax = region.xMax;
            r1.zMin = region.zMin;
            r1.zMax = z - data.regionBorder;

            r2.xMin = region.xMin;
            r2.xMax = region.xMax;
            r2.zMin = z + data.regionBorder;
            r2.zMax = region.zMax;
        }
    }

    /// <summary>
    /// 创建陆地
    /// </summary>
    void CreateLand()
    {
        // 所需陆地块数
        int landBudget = Mathf.RoundToInt(data.CellCount * data.landPercentage * 0.01f);
        data.landCells = landBudget;

        // 防止错误的配置导致死循环
        for (int guard = 0; guard < 10000; guard++)
        {
            bool sink = Random.value < data.sinkProbability;
            for (int i = 0; i < regions.Count; i++)
            {
                MapRegion region = regions[i];
                int chunkSize = Random.Range(data.chunkSizeMin, data.chunkSizeMax - 1);
                if (sink)
                {
                    landBudget = SinkTerrain(chunkSize, landBudget, region);
                }
                else
                {
                    landBudget = RaiseTerrain(chunkSize, landBudget, region);
                    if (landBudget == 0)
                    {
                        return;
                    }
                }
            }
        }

        // 死循环跳出的
        if (landBudget > 0)
        {
            data.landCells -= landBudget;
            Debug.LogWarning("Failed to use up " + landBudget + " land budget.");
        }
    }

    int RaiseTerrain(int chunkSize, int budget, MapRegion region)
    {
        searchFrontierPhase += 1;
        HexCell firstCell = GetRandomCell(region);
        firstCell.SearchPhase = searchFrontierPhase;
        firstCell.Distance = 0;
        firstCell.SearchHeuristic = 0;
        searchFrontier.Enqueue(firstCell);

        HexCoordinates center = firstCell.coordinates;

        int rise = Random.value < data.highRiseProbability ? 2 : 1;
        int size = 0;
        while (size < chunkSize && searchFrontier.Count > 0)
        {
            HexCell current = searchFrontier.Dequeue();
            int originalElevation = current.Elevation;
            int newElevation = originalElevation + rise;

            //防止海拔 过高
            if (newElevation > data.elevationMaximum)
            {
                continue;
            }
            current.Elevation = newElevation;

            // 如果他没有变为陆地，则计数减1，增加一个陆地
            if (
                originalElevation < data.waterLevel &&
                newElevation >= data.waterLevel && --budget == 0)
            {
                break;
            }
            size += 1;

            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);
                if (neighbor && neighbor.SearchPhase < searchFrontierPhase)
                {
                    neighbor.SearchPhase = searchFrontierPhase;
                    neighbor.Distance = neighbor.coordinates.DistanceTo(center);
                    neighbor.SearchHeuristic = Random.value < data.jitterProbability ? 1 : 0;
                    searchFrontier.Enqueue(neighbor);
                }
            }
        }
        searchFrontier.Clear();

        return budget;
    }

    int SinkTerrain(int chunkSize, int budget, MapRegion region)
    {
        searchFrontierPhase += 1;
        HexCell firstCell = GetRandomCell(region);
        firstCell.SearchPhase = searchFrontierPhase;
        firstCell.Distance = 0;
        firstCell.SearchHeuristic = 0;
        searchFrontier.Enqueue(firstCell);

        HexCoordinates center = firstCell.coordinates;

        int sink = Random.value < data.highRiseProbability ? 2 : 1;
        int size = 0;
        while (size < chunkSize && searchFrontier.Count > 0)
        {
            HexCell current = searchFrontier.Dequeue();
            int originalElevation = current.Elevation;

            // 防止海拔过低
            int newElevation = current.Elevation - sink;
            if (newElevation < data.elevationMinimum)
            {
                continue;
            }
            current.Elevation = newElevation;

            // 如果他没有变为陆地，则计数减1，增加一个陆地
            if (
                originalElevation >= data.waterLevel &&
                newElevation < data.waterLevel)
            {
                budget += 1;
                break;
            }
            size += 1;

            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);
                if (neighbor && neighbor.SearchPhase < searchFrontierPhase)
                {
                    neighbor.SearchPhase = searchFrontierPhase;
                    neighbor.Distance = neighbor.coordinates.DistanceTo(center);
                    neighbor.SearchHeuristic = Random.value < data.jitterProbability ? 1 : 0;
                    searchFrontier.Enqueue(neighbor);
                }
            }
        }
        searchFrontier.Clear();

        return budget;
    }


    HexCell GetRandomCell(MapRegion region)
    {
        return grid.GetCell(
            Random.Range(region.xMin, region.xMax), 
            Random.Range(region.zMin, region.zMax));
    }

    /// <summary>
    /// 侵蚀陆地 ，让陆地变得更平缓
    /// </summary>
    void ErodeLand()
    {
        // list池
        List<HexCell> erodibleCells = ListPool<HexCell>.Get();
        for (int i = 0; i < data.CellCount; i++)
        {
            // 判断是否需要侵蚀
            HexCell cell = grid.GetCell(i);
            if (IsErodible(cell))
            {
                erodibleCells.Add(cell);
            }
        }

        // 不需要侵蚀的数量
        int targetErodibleCount =
            (int)(erodibleCells.Count * (100 - data.erosionPercentage) * 0.01f);

        while (erodibleCells.Count > targetErodibleCount)
        {
            int index = Random.Range(0, erodibleCells.Count);
            HexCell cell = erodibleCells[index];
            HexCell targetCell = GetErosionTarget(cell);

            cell.Elevation -= 1;
            targetCell.Elevation += 1;

            // 不需要侵蚀了，删除
            if (!IsErodible(cell))
            {
                erodibleCells[index] = erodibleCells[erodibleCells.Count - 1];
                erodibleCells.RemoveAt(erodibleCells.Count - 1);
            }

            if (IsErodible(targetCell) && !erodibleCells.Contains(targetCell))
            {
                erodibleCells.Add(targetCell);
            }

            // 侵蚀后，判断现在的他的周边是否需要侵蚀
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = cell.GetNeighbor(d);
                if (
                    neighbor && neighbor.Elevation == cell.Elevation + 2 &&
                    !erodibleCells.Contains(neighbor)
                )
                {
                    erodibleCells.Add(neighbor);
                }
            }

            // 侵蚀一个格子，周围低的格子随机抬高一个
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = targetCell.GetNeighbor(d);
                if (
                    neighbor && neighbor != cell &&
                    neighbor.Elevation == targetCell.Elevation + 1 && 
                    !IsErodible(neighbor)
                )
                {
                    erodibleCells.Remove(neighbor);
                }
            }
        }

        // 返回池
        ListPool<HexCell>.Add(erodibleCells);
    }

    bool IsErodible(HexCell cell)
    {
        int erodibleElevation = cell.Elevation - 2;
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            HexCell neighbor = cell.GetNeighbor(d);
            if (neighbor && neighbor.Elevation <= erodibleElevation)
            {
                return true;
            }
        }
        return false;
    }
    
    HexCell GetErosionTarget(HexCell cell)
    {
        List<HexCell> candidates = ListPool<HexCell>.Get();
        int erodibleElevation = cell.Elevation - 2;
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            HexCell neighbor = cell.GetNeighbor(d);
            if (neighbor && neighbor.Elevation <= erodibleElevation)
            {
                candidates.Add(neighbor);
            }
        }
        HexCell target = candidates[Random.Range(0, candidates.Count)];
        ListPool<HexCell>.Add(candidates);
        return target;
    }

    /// <summary>
    /// 创建温度图
    /// </summary>
    void CreateTemperature()
    {
        for (int i = 0; i < data.CellCount; i++)
        {
            HexCell cell = grid.GetCell(i);

            ClimateData climate = grid.GetNextClimateData(i);

            float temperature = DetermineTemperature(cell);
            climate.temperature = temperature;
        }
    }

    /// <summary>
    /// 创建云，最基本的云，后续根据扩散和风进行移动
    /// </summary>
    void CreateCloud()
    {
        for (int i = 0; i < data.CellCount; i++)
        {
            EvaporationWaterAddClouds(i);
        }
    }

    /// <summary>
    /// 创建风, 风由温度低的地方吹向温度高的地方
    /// </summary>
    void CreateWind()
    {
        for (int i = 0; i < data.CellCount; i++)
        {
            EvolveWind(i);
        }
    }

    /// <summary>
    /// 扩散云
    /// </summary>
    void DiffusionCloud()
    {
        for (int i = 0; i < data.CellCount; i++)
        {
            EvaporationClouds(i);
        }
    }

    /// <summary>
    /// 时间偏移
    /// </summary>
    void DelayTime()
    {
        data.temperatureJitterChannel++;
        if (data.temperatureJitterChannel > 3)
        {
            data.temperatureJitterChannel = 0;
        }

        // 每回合偏移
        float d_equator = (data.highEquator - data.lowEquator) / BaseConstant.RoundOfYear;
        d_equator *= data.dir_equator;

        data.f_equator += d_equator;
        if (data.f_equator > data.highEquator ||
            data.f_equator < data.lowEquator)
        {
            data.dir_equator = -data.dir_equator;
        }

        // 经度
        data.sunLongitude += data.width / 10;
        if (data.sunLongitude > data.width)
        {
            data.sunLongitude = data.sunLongitude - data.width;
        }

    }

    public void EvolveClimate()
    {
        DelayTime();

        grid.ReplaceClimateData();

        CreateTemperature();
        CreateCloud();
        //DiffusionCloud();
        CreateWind();
        DiffusionCloud();
        SetTerrainType();
    }


    // 每回合
    public void NextRound()
    {
        // 时间变化，太阳照射变化
        DelayTime();
        grid.ReplaceClimateData();

        // 温度
        CreateTemperature();
        // 云雨
        CreateCloud();
        //DiffusionCloud();

        // 风
        CreateWind();

        //
        DiffusionCloud();
    }



    void SetTerrainType()
    {
        // 越高的地区的沙子 变为 岩石，
        int rockDesertElevation =
            data.elevationMaximum - (data.elevationMaximum - data.waterLevel) / 2;

        for (int i = 0; i < data.CellCount; i++)
        {
            HexCell cell = grid.GetCell(i);

            ClimateData climate = grid.GetClimateData(i);
            float moisture = climate.moisture;
            float temperature = climate.temperature;
            if (!cell.IsUnderwater)
            {
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
                Biome cellBiome = biomes[t * 4 + m];

                // 是沙漠而且是高海拔，则是岩石
                if (cellBiome.terrain == HexTerrainType.Desert)
                {
                    if (cell.Elevation >= rockDesertElevation)
                    {
                        cellBiome.terrain = HexTerrainType.Mud;
                    }
                }
                // 最高海拔一定是雪
                else if (cell.Elevation == data.elevationMaximum)
                {
                    cellBiome.terrain = HexTerrainType.Snow;
                }

                if (cellBiome.terrain ==HexTerrainType.Snow)
                {
                    cellBiome.plant -= 1;
                }
                else if (cellBiome.plant < 3 && cell.HasRiver())
                {
                    cellBiome.plant += 1;
                }

                cell.TerrainType = cellBiome.terrain;
                cell.FeatureType = cellBiome.feature;
                cell.FeatureLevel = cellBiome.plant;
            }
            else
            {
                // 不同的水深不同的海底类型
                HexTerrainType terrain;

                // 看邻格类型，可能是岩石或草地或悬崖等
                if (cell.Elevation == data.waterLevel - 1)
                {
                    int cliffs = 0, slopes = 0;
                    for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                    {
                        HexCell neighbor = cell.GetNeighbor(d);
                        if (!neighbor)
                        {
                            continue;
                        }
                        int delta = neighbor.Elevation - cell.LakesLevel;
                        if (delta == 0)
                        {
                            slopes += 1;
                        }
                        else if (delta > 0)
                        {
                            cliffs += 1;
                        }
                    }

                    if (cliffs + slopes > 3)
                    {
                        terrain = HexTerrainType.Grassplot;
                    }
                    else if (cliffs > 0)
                    {
                        terrain = HexTerrainType.Mud;
                    }
                    else if (slopes > 0)
                    {
                        terrain = HexTerrainType.Desert;
                    }
                    else
                    {
                        terrain = HexTerrainType.Grassplot;
                    }
                }
                else if (cell.Elevation >= data.waterLevel)
                {
                    terrain = HexTerrainType.Grassplot;
                }
                else if (cell.Elevation < 0)
                {
                    terrain = HexTerrainType.Mud;
                }
                else
                {
                    terrain = HexTerrainType.Desert;
                }

                // cell.TerrainType = HexTerrainType.Water;
                // 低温的谁下不会有水草
                if (terrain == HexTerrainType.Grassplot && temperature < temperatureBands[0])
                {
                    terrain = HexTerrainType.Desert;
                }

                cell.TerrainType = terrain;
            }
            
        }
    }

    float DetermineTemperature(HexCell cell)
    {
        float latitude = (float)Mathf.Abs(cell.coordinates.Z - data.equator) / grid.cellCountZ;

        float sun_xishu = 0.9f;

        int s_1;
        if (cell.coordinates.X < data.sunLongitude)
        {
            s_1 = Mathf.Min(
                    data.sunLongitude - cell.coordinates.X,
                    cell.coordinates.X - data.sunLongitude + data.width
                );
        }
        else
        {
            s_1 = Mathf.Min(
                    cell.coordinates.X - data.sunLongitude,
                    data.sunLongitude - cell.coordinates.X + data.width
                );
        }
        
        if(s_1 < data.width / 4)
        {
            sun_xishu = 1 -  (0.1f * 4 * s_1 / data.width);
        }

        float temperature = Mathf.LerpUnclamped(1f, 0f, latitude);
        //Mathf.LerpUnclamped(data.highTemperature, data.lowTemperature, latitude);

        temperature *= sun_xishu;

        //// 是否在水下，水对光照的反射，使得水获得的温度更低
        //if (cell.IsUnderwater)
        //{
        //    temperature *= 0.7f;
        //}

        temperature *= cell.GetTemperatureDampings();

        // 高度越高温度越低
        temperature *= 1f - (cell.ViewElevation - data.waterLevel) /
            (1.5f * data.elevationMaximum - data.waterLevel + 1f);

        // 4种变化
        float jitter =
            HexMetrics.SampleNoise(cell.Position * 0.1f)[data.temperatureJitterChannel];

        // 增加噪声扰动
        temperature +=
            (jitter * 2f - 1f) *
            data.temperatureJitter;

        return temperature;
    }


    void EvaporationWaterAddClouds(int cellIndex)
    {
        HexCell cell = grid.GetCell(cellIndex);
        ClimateData cellClimate = grid.GetClimateData(cellIndex);
        ClimateData cellNextClimate = grid.GetNextClimateData(cellIndex);

        // 是水面则 蒸发水
        if (cell.IsUnderwater)
        {
            cellNextClimate.moisture = 1f;
            // 蒸发量和温度有关
            cellNextClimate.clouds = cellClimate.clouds + 
                3 * data.evaporationFactor * cellClimate.temperature * cellClimate.temperature;
        }
        else
        {
            float evaporation = 
                cellClimate.moisture * data.landEvaporationFactor
                * 3 * cellClimate.temperature * cellClimate.temperature * cell.GetEvaporationDampings();
            evaporation = Mathf.Min(evaporation, cellClimate.moisture);

            cellNextClimate.moisture = cellClimate.moisture - evaporation;
            cellNextClimate.clouds = cellClimate.clouds + evaporation;
        }
    }
    

    // 气候压力
    float ClimatePressure(int index)
    {
        HexCell cell = grid.GetCell(index);
        ClimateData cellClimate = grid.GetClimateData(index);

        float temperature = cellClimate.temperature;
        float cloud = cellClimate.clouds;
        float height = cell.ViewElevation * 1.0f / data.elevationMaximum;

        // 气压与 高度和云雨量有关, 高度越高，雨量越大 气压越高，
        return height * 0.2f + temperature * 0f + cellClimate.clouds * 2f;
    }

    // 风吹到旁边的格子
    void blowWindToNei(Vector2 flyWind, float windF,
        HexCell cell, HexCell neighbor)
    {
        int viewEle = cell.ViewElevation;
        float windDamping = cell.GetWindDampings();

        Vector2 winDirV2 = flyWind.normalized;

        if (neighbor.ViewElevation > viewEle)
        {
            // 留下的风
            float wf = 0.03f * (neighbor.ViewElevation - viewEle);
            wf = wf > 0.95f ? 0.95f : wf;

            windF *= (1 - wf);

            //float lostF = windF * wf;
            //windF -= lostF;

            //ClimateData cellNextClimate = grid.GetNextClimateData(cell.index);
            // cellNextClimate.wind += winDirV2 * lostF;
        }

        ClimateData neighbor_cellClimate = grid.GetNextClimateData(neighbor.index);
        neighbor_cellClimate.wind += winDirV2 * windF;
    }

    // 变化风
    void EvolveWind(int cellIndex)
    {
        HexCell cell = grid.GetCell(cellIndex);
        ClimateData cellClimate = grid.GetClimateData(cellIndex);
        ClimateData cellNextClimate = grid.GetNextClimateData(cellIndex);

        
        //cellNextClimate.wind += wind;

        // 压力
        float pressure = ClimatePressure(cellIndex);

        int viewEle = cell.ViewElevation;

        // 压力产生的风
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            HexCell neighbor = cell.GetNeighbor(d);
            if (!neighbor)
            {
                continue;
            }

            int dirIndex = (int)d;
            // 压力产生风
            float neighbor_pressure = ClimatePressure(neighbor.index);

            if (neighbor_pressure < pressure)
            {
                float diffusion = (pressure - neighbor_pressure);
                Vector3 v = HexMetrics.GetFirstCorner(d) + HexMetrics.GetSecondCorner(d);

                if (neighbor.ViewElevation > viewEle)
                {
                    // 留下的风
                    float wf = 0.2f * (neighbor.ViewElevation - viewEle);
                    wf = wf > 0.95f ? 0.95f : wf;
                    float w = diffusion * wf;


                    // 剩余飞走的风
                    diffusion -= w;
                }

                //cellClimate.neiWindForce[dirIndex] += diffusion;

                // 风力
                cellClimate.wind += Vector3Tool.ToVector2(v).normalized * diffusion;
            }
        }

        Vector2 wind = cellClimate.wind;
        float windDamping = 0.5f; // cell.TerrainType.GetWindDampings();        

        float force = wind.magnitude;

        bool hasWind = true;
        if(force < 0.0001f)
        {
            hasWind = false;
        }

        HexDirection windDir = HexDirection.E;
        HexDirection p_windDir = windDir;

        float windForce = 0;
        float p_windForce = 0;

        float fx = force;
        if (hasWind)
        {
            float angle = Vector2Tool.Angle(Vector2.up, wind);

            Vector2 flyWind = wind * (1f - windDamping);
            // 留下的风
            Vector2 lostWind = wind * windDamping;
            cellNextClimate.wind += lostWind;

            // 吹走的风
            // 分为2股风
            fx = windForce = (1f - windDamping) * force;
            p_windForce = 0;

            int winD = Mathf.FloorToInt(angle / 60f);
            windDir = (HexDirection)winD;
            p_windDir = windDir;

            if(winD < 0 || winD > 5)
            {
                Debug.Log(winD);
                Debug.Log(angle);
                Debug.Log(wind);
            }

            Vector2 winDirV2 = HexMetrics.GetSolidEdgeMiddle(windDir);
            Vector2 p_winDirV2 = winDirV2;

            float p_winD = angle - (winD * 60);
            if (p_winD > 30)
            {
                p_windDir = windDir.Next();
                //p_winDirV2 = HexMetrics.GetSolidEdgeMiddle(p_windDir);

                //Vector2 v = Vector2Tool.DirToLocalSpace60(flyWind, p_winDirV2.normalized, winDirV2.normalized);

                //p_windForce = v.x;
                //windForce = v.y;

                p_windForce = flyWind.magnitude * ((p_winD - 30) / 60);
                windForce = flyWind.magnitude - p_windForce;
            }
            else if (p_winD < 30)
            {
                p_windDir = windDir.Previous();
                //p_winDirV2 = HexMetrics.GetSolidEdgeMiddle(p_windDir);

                //Vector2 v = Vector2Tool.DirToLocalSpace60(flyWind, winDirV2.normalized, p_winDirV2.normalized);

                //windForce = v.x;
                //p_windForce = v.y;

                p_windForce = flyWind.magnitude * ((30 - p_winD) / 60);
                windForce = flyWind.magnitude - p_windForce;
            }
            else
            {
                windForce = flyWind.magnitude;
            }

            //cellNextClimate.wind += lostWind;
            //cellNextClimate.wind -= flyWind;
        }

        float x = windForce + p_windForce - fx;
        if (x < 0 && x > fx * 0.1)
        {
            Debug.Log("------------windForce + p_windForce < force---------------");
            Debug.Log(windForce + p_windForce - force);
            Debug.Log(windForce);
            Debug.Log(p_windForce);
            Debug.Log(force);
        }
        

        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            HexCell neighbor = cell.GetNeighbor(d);
            if (!neighbor)
            {
                continue;
            }
            int dirIndex = (int)d;
            ClimateData neighbor_cellClimate = grid.GetNextClimateData(neighbor.index);

            //  风挂到邻居
            if (hasWind)
            {
                // 主风
                if (windDir == d)
                {
                    //blowWindToNei(windDir, windForce, cell, neighbor);
                    blowWindToNei(wind, windForce, cell, neighbor);
                    cellClimate.neiWindForce[dirIndex] += windForce;
                }
                // 次风
                else if(p_windDir == d)
                {
                    blowWindToNei(wind, p_windForce, cell, neighbor);
                    cellClimate.neiWindForce[dirIndex] += p_windForce;

                    //blowWindToNei(p_windDir, p_windForce, cell, neighbor);
                }
            }
        }
        
    }

    void EvaporationClouds(int cellIndex)
    {
        HexCell cell = grid.GetCell(cellIndex);
        ClimateData cellClimate = grid.GetClimateData(cellIndex);
        ClimateData cellNextClimate = grid.GetNextClimateData(cellIndex);

        // 风的方向  风一般为 0.01
        Vector2 wind = cellClimate.wind;
        //float angle = Vector2Tool.Angle(Vector2.up, wind);

        float force = wind.magnitude;

        bool hasWind = true;
        if (force < 0.0001f)
        {
            hasWind = false;
        }

        HexDirection windDir = HexDirection.E;
        HexDirection p_windDir = windDir;

        float windForce = 0;
        float p_windForce = 0;

        if (hasWind)
        {
            float angle = Vector2Tool.Angle(Vector2.up, wind);
            
            // 吹走的风
            // 分为2股风
            windForce = force;
            p_windForce = 0;

            int winD = Mathf.FloorToInt(angle / 60f);
            windDir = (HexDirection)winD;
            p_windDir = windDir;
            
            float p_winD = angle - (winD * 60);
            if (p_winD > 30)
            {
                p_windDir = windDir.Next();
                p_windForce = force * ((p_winD - 30) / 60);
                windForce = force - p_windForce;
            }
            else if (p_winD < 30)
            {
                p_windDir = windDir.Previous();
                p_windForce = force * ((30 - p_winD) / 60);
                windForce = force - p_windForce;
            }
            else
            {
                windForce = force;
            }
        }


        float precipitation = cellClimate.clouds * data.precipitationFactor;
        cellNextClimate.clouds = cellNextClimate.clouds - precipitation;
        cellNextClimate.moisture = cellNextClimate.moisture + precipitation;
        cellNextClimate.rain += precipitation;

        // 概率降雨 小雨 含云越多概率越大
        if (Random.value < data.rainFactor * cellClimate.clouds)
        {
            float rain = cellClimate.clouds * data.rainFactor;
            cellNextClimate.clouds = cellNextClimate.clouds - rain;
            cellNextClimate.moisture = cellNextClimate.moisture + rain;
            cellNextClimate.rain += rain;
        }

        // 最大含云量
        float cloudMaximum = 2 * (1f - cell.ViewElevation / (data.elevationMaximum + 1f));
        // 超过了，则是降雨，额外降雨
        if (cellClimate.clouds > cloudMaximum)
        {
            float rain = cellNextClimate.clouds - cloudMaximum;
            cellNextClimate.moisture = cellNextClimate.moisture + rain;
            cellNextClimate.clouds = cellNextClimate.clouds - rain;
            cellClimate.clouds = Mathf.Min(cloudMaximum, cellNextClimate.clouds); // cloudMaximum;

            cellNextClimate.rain = rain;


            if (cellNextClimate.clouds < 0)
            {
                Debug.Log("-----------------------");
                Debug.Log(rain);
                Debug.Log(cellNextClimate.clouds);
                Debug.Log(cellNextClimate.clouds + rain);
                Debug.Log("*************************");
            }
        }

        
        float cloudWindForce = wind.magnitude;
        cloudWindForce = Mathf.Max(cloudWindForce, 12f);

        cloudWindForce *= 5;

        cloudWindForce = 0;
        foreach (var _force in cellClimate.neiWindForce)
        {
            cloudWindForce += _force;
        }

        //hasWind = false;
        //cloudWindForce = 0;

        // Debug.Log(windForce);
        // 自动扩散云
        float cloudDispersal = cellClimate.clouds * (1f / (12f + cloudWindForce));

        // 扩散水
        float runoff = cellClimate.moisture * data.runoffFactor  * (1f / 6f) * cell.GetMoistureDampings();
        float seepage = cellClimate.moisture * data.seepageFactor * (1f / 6f) * cell.GetMoistureDampings();
        
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            HexCell neighbor = cell.GetNeighbor(d);
            if (!neighbor)
            {
                continue;
            }
            ClimateData neighborClimate = grid.GetNextClimateData(neighbor.index);

            float _force = cellClimate.neiWindForce[(int)d];

            float n = cloudDispersal * (1 + _force);
            neighborClimate.clouds += n;
            cellNextClimate.clouds -= n;

            /*
            if (hasWind)
            {
                if (d == windDir)
                {
                    neighborClimate.clouds += cloudDispersal * windForce;
                    cellNextClimate.clouds -= cloudDispersal * windForce;

                    if (cellNextClimate.clouds < 0)
                    {
                        Debug.Log("-----------------------");
                        Debug.Log(cloudDispersal * windForce);
                        Debug.Log(cellNextClimate.clouds);
                        Debug.Log(cellNextClimate.clouds + cloudDispersal * windForce);
                        Debug.Log("*************************");
                    }
                }
                else if (d == p_windDir)
                {
                    neighborClimate.clouds += cloudDispersal * p_windForce;
                    cellNextClimate.clouds -= cloudDispersal * p_windForce;

                    if (cellNextClimate.clouds < 0)
                    {
                        Debug.Log("-----------------------");
                        Debug.Log(cloudDispersal * p_windForce);
                        Debug.Log(cellNextClimate.clouds);
                        Debug.Log(cellNextClimate.clouds + cloudDispersal * p_windForce);
                        Debug.Log("*************************");
                    }
                }
                else
                {
                    neighborClimate.clouds += cloudDispersal;
                    cellNextClimate.clouds -= cloudDispersal;

                    if (cellNextClimate.clouds < 0)
                    {
                        Debug.Log("-----------------------");
                        Debug.Log(cloudDispersal);
                        Debug.Log(cellNextClimate.clouds);
                        Debug.Log(cellNextClimate.clouds + cloudDispersal);
                        Debug.Log("*************************");
                    }
                }
            }
            else
            {
                neighborClimate.clouds += cloudDispersal;
                cellNextClimate.clouds -= cloudDispersal;

                if(cellNextClimate.clouds < 0)
                {
                    Debug.Log("-----------------------");
                    Debug.Log(cloudDispersal);
                    Debug.Log(cellNextClimate.clouds);
                    Debug.Log(cellNextClimate.clouds + cloudDispersal);
                    Debug.Log("*************************");
                }
            }

            */
            // 向低处流失水分
            int elevationDelta = neighbor.ViewElevation - cell.ViewElevation;
            if (elevationDelta < 0)
            {
                cellNextClimate.moisture = cellNextClimate.moisture - runoff;
                neighborClimate.moisture += runoff;
            }
            else if (elevationDelta == 0)
            {
                cellNextClimate.moisture = cellNextClimate.moisture - seepage;
                neighborClimate.moisture += seepage;
            }

            //Debug.Log(neighborClimate.moisture);
        }

        //cellNextClimate.clouds = cellClimate.clouds;

        //cellNextClimate.moisture += cellClimate.moisture;
    }


    List<HexDirection> flowDirections = new List<HexDirection>();

    // 这个特殊，不能每回合进行
    void CreateRivers()
    {
        List<HexCell> riverOrigins = ListPool<HexCell>.Get();
        for (int i = 0; i < data.CellCount; i++)
        {
            HexCell cell = grid.GetCell(i);
            if (cell.IsUnderwater)
            {
                continue;
            }
            ClimateData climateData = grid.GetClimateData(i); // climate[i];
            float weight =
                climateData.moisture * (cell.Elevation - data.waterLevel) /
                (data.elevationMaximum - data.waterLevel);
            if (weight > 0.75f)
            {
                riverOrigins.Add(cell);
                riverOrigins.Add(cell);
            }
            if (weight > 0.5f)
            {
                riverOrigins.Add(cell);
            }
            if (weight > 0.25f)
            {
                riverOrigins.Add(cell);
            }
        }

        int riverBudget = Mathf.RoundToInt(data.landCells * data.riverPercentage * 0.01f);

        while (riverBudget > 0 && riverOrigins.Count > 0)
        {
            int index = Random.Range(0, riverOrigins.Count);
            int lastIndex = riverOrigins.Count - 1;
            HexCell origin = riverOrigins[index];
            riverOrigins[index] = riverOrigins[lastIndex];
            riverOrigins.RemoveAt(lastIndex);

            if (!origin.HasRiver())
            {
                // 周围一个内，有河流或是海，则不会创建河流
                bool isValidOrigin = true;
                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    HexCell neighbor = origin.GetNeighbor(d);
                    if (neighbor && (neighbor.HasRiver() || neighbor.IsUnderwater))
                    {
                        isValidOrigin = false;
                        break;
                    }
                }
                if (isValidOrigin)
                {
                    riverBudget -= CreateRiver(origin);
                }
            }
        }

        if (riverBudget > 0)
        {
            Debug.LogWarning("Failed to use up river budget.");
        }

        // 将池返回
        ListPool<HexCell>.Add(riverOrigins);
    }



    /// <summary>
    /// 创建一个河流
    /// </summary>
    /// <param name="origin">河流起始</param>
    /// <returns></returns>
    int CreateRiver(HexCell origin)
    {
        HexGrid grid = HexGrid.instance;
        HexRiver river = grid.GetNewRiver(origin);

        int length = 0;
        HexCell cell = origin;

        HexDirection direction = HexDirection.NE;
        while (!cell.IsUnderwater)
        {
            int minNeighborElevation = int.MaxValue;
            flowDirections.Clear();

            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = cell.GetNeighbor(d);
                if (!neighbor)
                {
                    continue;
                }

                // 找到最低高度
                if (neighbor.Elevation < minNeighborElevation)
                {
                    minNeighborElevation = neighbor.Elevation;
                }

                if (neighbor == origin)
                {
                    continue;
                }

                // 不会向上流
                int delta = neighbor.Elevation - cell.Elevation;
                if (delta > 0)
                {
                    continue;
                }

                // 不会再流回自己的河道
                if (river.isInRiver(neighbor))
                {
                    continue;
                }

                // 如果是湖泊/海，则直接流入
                if (neighbor.IsLakes())
                {
                    cell.SetOutgoingRiver(d);
                    length += 1;
                    river.AddOneRiver(neighbor, d);
                    return length;
                }

                // 合并，邻居是河流起点，且邻居不在高点
                if (neighbor.IsRiverOrigin())
                {
                    cell.SetOutgoingRiver(d);

                    river.AddOneRiver(neighbor, d);

                    HexRiver oldRiver = grid.GetHexRiverByOrigin(neighbor);
                    grid.MergeRiver(river, oldRiver);

                    return length;
                }

                // 更高的概率向下流
                if (delta < 0)
                {
                    flowDirections.Add(d);
                    flowDirections.Add(d);
                    flowDirections.Add(d);
                }

                // 非急转弯,方向概率增加
                if (length == 1 ||
                    (d != direction.Next2() && d != direction.Previous2())
                )
                {
                    flowDirections.Add(d);
                }

                flowDirections.Add(d);
            }

            // 找不到下一个流向
            if (flowDirections.Count == 0)
            {
                // 只有一个出水口
                if (length == 1)
                {
                    return 0;
                }

                // 将当前格子变为湖泊
                if (minNeighborElevation >= cell.Elevation)
                {
                    cell.LakesLevel = minNeighborElevation;
                    if (minNeighborElevation == cell.Elevation)
                    {
                        cell.Elevation = minNeighborElevation - 1;
                    }
                }
                break;
            }

            direction = flowDirections[Random.Range(0, flowDirections.Count)];
            
            HexCell nextNeighbor = cell.GetNeighbor(direction);

            // 流到另一条河流
            if (nextNeighbor.HasRiver())
            {
                cell.SetOutgoingRiver(direction);
                length += 1;
                river.AddOneRiver(cell, direction);
                return length;
            }


            cell.SetOutgoingRiver(direction);
            length += 1;

            if (minNeighborElevation >= cell.Elevation &&
                Random.value < data.extraLakeProbability)
            {
                cell.LakesLevel = cell.Elevation;
                cell.Elevation -= 1;
            }

            cell = nextNeighbor;

            river.AddOneRiver(cell, direction);
        }
        return length;
    }
}
