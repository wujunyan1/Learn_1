using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnUseRandomLakers
{
    /***
    // 创建降雨图
    void CreateRainMap()
    {
        // GameOptionData gameData = GameOptionData.getInstance();

        // 计算每个格子的降雨量
        for (int index = 0; index < cells.Length; index++)
        {
            HexCell cell = cells[index];
            float height = cell.Height;
            int x = cell.coordinates.X;
            int y = cell.coordinates.Y;


            int rain = (int)(height * gameData.humidity * (Noise.PerlinNoise2D(x, y, rainTerrain1, rainTerrain2, octaves, persistence) + 0.5f));
            if (height > HexMetrics.maxRainElevation)
            {
                rain = (int)((HexMetrics.maxRainElevation - (height - HexMetrics.maxRainElevation)) * gameData.humidity * Mathf.PerlinNoise(x + mapRainTendency, y + mapRainTendency));
            }

            cell.Rain = rain;
        }


    }

    int count = 0;
    int co = 0;

    // 创建河流
    void CreateAllRiver()
    {
        //Debug.Log("|||||||||||||");
        count++;
        co = 0;
        if (count > 50)
        {
            //Debug.Log("|||||||||||||");
            //return;
        }
        // 按降雨量排序
        List<HexCell> rains = new List<HexCell>();
        //Debug.Log(cells.Length);
        for (int index = 0; index < cells.Length; index++)
        {
            HexCell cell = cells[index];
            float pondage = cell.Pondage;

            //Debug.Log(pondage);
            //Debug.Log(!cell.HasRiver());
            //Debug.Log(cell.terrainType);
            // 超过最大蓄水量才会下流 并且该地块不是河流
            if (pondage > HexMetrics.cellMaxPondage && !cell.HasRiver() && cell.TerrainType != HexTerrainType.Water)
            {
                //Debug.Log("rains+++++1");
                //Debug.Log(string.Format("add rains {0}", cell.index));
                rains.Add(cell);
            }
        }

        rains.Sort(
            (left, right) => {
                return (int)((right.Pondage - left.Pondage) * 100);
            });

        if (rains.Count < 1)
        {
            return;
        }

        HexCell next = rains[0];

        //Debug.Log(next.Pondage);

        if (next.Pondage > HexMetrics.cellMaxPondage)
        {
            CreateOneRiver(next, 0f);
            //CreateRiver(next);
        }

    }

    float RiverChangeLandPondage(HexCell cell)
    {
        float _flow = cell.Pondage * 0.3f;

        // 河流里面增加自己这块的多余水量
        if (cell.Pondage > HexMetrics.cellWaterStore)
        {
            int flow = cell.Pondage - _flow < HexMetrics.cellWaterStore ? cell.Pondage - HexMetrics.cellWaterStore : (int)_flow;
            cell.Pondage = cell.Pondage - flow;

            return _flow;
        }
        else
        {
            int flow = cell.Pondage + _flow > HexMetrics.cellWaterStore ? HexMetrics.cellWaterStore - cell.Pondage : (int)_flow;
            cell.Pondage = cell.Pondage + flow;

            return -_flow;
        }
    }

    // 创建一条从顶点开始的河流
    void CreateOneRiver(HexCell riverTop, float addFlow)
    {
        // 当前位置
        HexCell currStep = riverTop;
        bool isEnd = false;

        // 河流当前水量
        //float addFlow = 0f;

        //int co = 0;
        //while (!isEnd)
        //{
        co++;
        if (co > 50)
        {
            Debug.Log(string.Format("error {0}", currStep.coordinates.ToString()));
            return;
        }

        // 找到周围邻居最低的格子
        HexCell downCell = null;
        HexDirection nextDir = HexDirection.E;
        for (HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++)
        {
            HexCell cell = currStep.GetNeighbor(dir);

            if (cell)
            {
                if (downCell == null)
                {
                    downCell = cell;
                    nextDir = dir;
                }

                // 如果是湖泊，则和水高度比较
                if (cell.TerrainType == HexTerrainType.Water)
                {
                    if (cell.lakesHeight < downCell.Height)
                    {
                        downCell = cell;
                        nextDir = dir;
                    }
                }
                else
                {
                    if (cell.Height < downCell.Height)
                    {
                        downCell = cell;
                        nextDir = dir;
                    }
                }

                // 如果是湖泊
                if (cell.TerrainType == HexTerrainType.Water)
                {

                }
                // 周围的有过多的水量也会被河流带走
                else
                {
                    addFlow += RiverChangeLandPondage(cell);
                }

            }
        }

        // 周围存在邻居
        if (downCell)
        {
            // 周围的格子比自己低
            if (downCell.Height < currStep.Height)
            {
                CreateCellRiver(currStep, downCell, nextDir, addFlow);
            }
            else //创建一个湖泊
            {
                //CreateCellLakes(currStep, downCell, nextDir, addFlow);
                CreateOneLakes(currStep, downCell, nextDir, addFlow);
            }
        }
        //}
    }

    // 创建该地图块的一条河流
    void CreateCellRiver(HexCell currStep, HexCell nextStep, HexDirection nextDir, float addFlow)
    {

        // 河流里面增加自己这块的多余水量
        addFlow += RiverChangeLandPondage(currStep);

        // 向该方向添加河流
        currStep.SetOutgoingRiver(nextDir, addFlow);

        // 继续往下创建
        CreateOneRiver(nextStep, addFlow);
    }

    // 增加一个位置到湖泊中
    void AddLakesCell(Dictionary<int, HexCell> lake, HexCell cell)
    {
        HexCell oldCell;
        if (!lake.TryGetValue(cell.index, out oldCell))
        {
            cell.TerrainType = HexTerrainType.Water;
            //cell.color = touchedColor;
            lake.Add(cell.index, cell);
        }

        //Debug.Log(string.Format("add lake cell {0}", cell.index));
    }

    void removeLakesCell(Dictionary<int, HexCell> lake, HexCell cell)
    {
        cell.TerrainType = HexTerrainType.Land;
        //cell.color = defaultColor;
        lake.Remove(cell.index);
        cell.lakesHeight = 0f;

        //Debug.Log(string.Format("remove lake cell {0}", cell.index));
    }

    // 创建湖泊
    void CreateOneLakes(HexCell currStep, HexCell minHeightNeighbor, HexDirection nextDir, float addFlow)
    {
        // 一个湖泊
        Dictionary<int, HexCell> lake = new Dictionary<int, HexCell>();
        lakes.Add(lake);

        AddLakesCell(lake, currStep);
        currStep.lakesHeight = currStep.Height;
        bool isMerge = CheckNeighborLakes(lake, currStep);

        HexCell lastCell = currStep;
        if (isMerge)
        {
            HexCell outRiverCell = GetLakesOutgoingCell(lake);
            if (outRiverCell)
            {
                lastCell = outRiverCell;
            }
        }

        // 河水全部流入
        currStep.Pondage = currStep.Pondage + (int)addFlow;

        // 这个高度差下，能装的水量
        float heightDiff = minHeightNeighbor.Height - currStep.Height;
        float waterNum = heightDiff * HexMetrics.waterCellMaxPondage;

        // 水多了
        if (currStep.Pondage > waterNum + HexMetrics.cellMaxPondage)
        {
            SetLakesHeight(lake, minHeightNeighbor.Height);
            CreateCellLakes(lake, lastCell, minHeightNeighbor.Height, minHeightNeighbor, nextDir, addFlow);
        }
        // 这个湖泊就是终点了，而且只有一个格子
        else
        {
            CreateAllRiver();
        }
    }

    // 当前湖泊
    
     *  lake 湖泊
     *  lastCell 上一个添加的格子
     *  waterHeight 当前湖水高度
     *  addFlow 多余水量
     
    void CreateCellLakes(Dictionary<int, HexCell> lake, HexCell lastCell, float waterHeight, HexCell lastNearCell, HexDirection lastNearDir, float addFlow)
    {
        // 最低的湖泊邻居
        HexCell lowCell = null;

        // 与这个格子相邻的湖泊格子
        HexCell nearCell = null;
        HexDirection nearDir = HexDirection.E;

        // 找到所有邻居中最低的格子
        // 遍历所有湖泊格子
        foreach (var item in lake)
        {
            HexCell currStep = item.Value;

            // 遍历所有邻居
            for (HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++)
            {
                HexCell neighbor = currStep.GetNeighbor(dir);
                // 存在邻居，并且邻居不是这个湖泊的水格子
                if (neighbor && !lake.ContainsKey(neighbor.index))
                {
                    if (lowCell == null)
                    {
                        lowCell = neighbor;
                        nearCell = currStep;
                        nearDir = dir;
                    }
                    if (lowCell.Height > neighbor.Height)
                    {
                        lowCell = neighbor;
                        nearCell = currStep;
                        nearDir = dir;
                    }
                }
            }
        }

        if (lowCell == null)
        {
            return;
        }
        //Debug.Log(string.Format("find next lake {0}", lowCell.index));

        bool isMge = true;
        // 如果有河流，判断这个格子是否是这个湖泊流出到这个格子的
        if (lowCell.TerrainType != HexTerrainType.Water && lowCell.HasRiver())
        {
            for (HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++)
            {
                HexCell neighbor = lowCell.GetNeighbor(dir);
                HexCell value;
                // 有邻居，是湖泊，并且是当前的湖泊
                if (neighbor && neighbor.TerrainType == HexTerrainType.Water && lake.TryGetValue(neighbor.index, out value))
                {
                    // 暂时不做其他处理
                    isMge = false;
                }
            }
        }

        // 如果这个邻居够矮，则上一个格子不会变为湖泊，而是河流
        if (lowCell.Height < waterHeight && isMge)
        {

            // 将上一个添加的格子移除
            removeLakesCell(lake, nearCell);

            // 会多的水量
            float heightDiff = waterHeight - lowCell.Height;
            int cellNum = lake.Count;
            float waterNum = heightDiff * HexMetrics.waterCellMaxPondage * cellNum;
            addFlow += waterNum;

            // 设置一条流出到这个格子的河流
            lastNearCell.SetOutgoingRiver(lastNearDir, addFlow);
            nearCell.SetOutgoingRiver(nearDir, addFlow);

            CreateOneRiver(lowCell, addFlow);
        }
        else
        {
            // 如果他也是湖泊
            if (lowCell.TerrainType == HexTerrainType.Water)
            {
                LakeMerge(lake, lowCell);
            }

            // 这个高度差下，能装的水量
            float heightDiff = lowCell.Height - waterHeight;
            int cellNum = lake.Count;
            float waterNum = heightDiff * HexMetrics.waterCellMaxPondage * cellNum + HexMetrics.cellMaxPondage;

            AddLakesCell(lake, lowCell);
            lowCell.lakesHeight = lowCell.Height;
            bool isMerge = CheckNeighborLakes(lake, lowCell);

            foreach (var item in lake)
            {
                HexCell cell = item.Value;
                cell.Pondage = HexMetrics.cellMaxPondage + ((int)(lowCell.Height - cell.Height) * HexMetrics.waterCellMaxPondage);
                cell.lakesHeight = lowCell.Height;
            }

            // 还有多的水
            if (addFlow > waterNum)
            {
                addFlow -= waterNum;
                // 继续添加湖泊
                CreateCellLakes(lake, lowCell, lowCell.Height, nearCell, nearDir, addFlow);
            }

            else
            {
                CreateAllRiver();
            }
        }
    }

    // 判断这个邻居是否是其他的湖泊
    bool CheckNeighborLakes(Dictionary<int, HexCell> lake, HexCell lowCell)
    {
        bool isMerge = false;
        for (HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++)
        {
            HexCell neighbor = lowCell.GetNeighbor(dir);
            HexCell value;

            // 判断这个邻居 是湖泊类型，而且不是属于这个湖泊
            if (neighbor && neighbor.TerrainType == HexTerrainType.Water && !lake.TryGetValue(neighbor.index, out value))
            {
                LakeMerge(lake, neighbor);
                isMerge = true;
            }
        }
        return isMerge;
    }

    // 湖水合并
    void LakeMerge(Dictionary<int, HexCell> lake, HexCell conCell)
    {

        HexCell baseCell = null;
        foreach (var i in lake)
        {
            baseCell = i.Value;
            break;
        }

        float newLakesHeight = (baseCell.lakesHeight + conCell.lakesHeight) * 0.5f;

        int lakeIndex = -1;
        // 循环所有湖泊
        for (int index = 0; index < lakes.Count; index++)
        {
            Dictionary<int, HexCell> itemList = lakes[index];
            // 判断该湖泊中是否有该格子
            HexCell cell;
            // 存在，则2个湖泊进行合并
            if (itemList.TryGetValue(conCell.index, out cell))
            {
                lakeIndex = index;
                foreach (var item in itemList)
                {
                    AddLakesCell(lake, item.Value);
                }
                break;
            }
        }
        if (lakeIndex != -1)
        {
            lakes.RemoveAt(lakeIndex);
        }

        SetLakesHeight(lake, newLakesHeight);
    }

    // 设置河流高度
    void SetLakesHeight(Dictionary<int, HexCell> lakes, float height)
    {
        // 循环湖泊中的所有格子
        foreach (var item in lakes)
        {
            HexCell cell = item.Value;
            cell.lakesHeight = height;
            //cell.Elevation = 0;
        }
    }

    HexCell GetLakesOutgoingCell(Dictionary<int, HexCell> lakes)
    {
        // 循环湖泊中的所有格子
        foreach (var item in lakes)
        {
            HexCell cell = item.Value;

            for (HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++)
            {
                if (cell.IsOutgoingRiverDirection(dir))
                {
                    return cell;
                }
            }
        }

        return null;
    }

    // 检查湖泊里多余的河流
    void CheckLakeRiver()
    {
        // 循环所有湖泊
        foreach (Dictionary<int, HexCell> itemList in lakes)
        {
            int minLandElevation = HexMetrics.hierarchyNum;

            // 循环湖泊中的所有格子
            foreach (var item in itemList)
            {
                HexCell cell = item.Value;

                // 所有湖泊降低半个高度
                //Vector3 center = cell.transform.localPosition;
                //center.y = (cell.Elevation + HexMetrics.riverSurfaceElevationOffset) *  HexMetrics.elevationStep;
                //cell.transform.localPosition = center;

                //List<HexDirection> outgoings = cell.GetRiverDirections(RiverDirection.Outgoing);

                // 循环所有的邻居
                for (HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++)
                {
                    RiverDirection river = cell.GetRiverDirection(dir);
                    HexCell neighbor = cell.GetNeighbor(dir);
                    // 这个边有河流，并且是流出的河流
                    if (river == RiverDirection.Outgoing)
                    {
                        // 如果流出也是湖泊则，这个河路径取消
                        if (neighbor && neighbor.TerrainType == HexTerrainType.Water)
                        {
                            cell.ClearRiver(dir);
                        }
                    }

                    // 如果湖泊与周围地块高度相同，则降低一个地块
                    if (neighbor && neighbor.TerrainType != HexTerrainType.Water)
                    {
                        if (neighbor.Elevation < minLandElevation)
                        {
                            minLandElevation = neighbor.Elevation;
                        }
                        //cell.Elevation = neighbor.Elevation - 1;
                    }
                }
            }


            // 循环湖泊中的所有格子
            foreach (var item in itemList)
            {
                HexCell cell = item.Value;
                if (cell.Elevation >= minLandElevation)
                {
                    cell.Elevation = minLandElevation - 1;
                }

                cell.LakesLevel = minLandElevation;
                //cell.Elevation = 0;
            }
        }
    }

    // 格子资源初始化处理
    void CheckCellTerrainType()
    {
        foreach (HexCell cell in cells)
        {
            HexTerrain.SetTerrainType(cell);
            HexTerrain.SetCellStore(cell);
        }
    }

    **/

}
