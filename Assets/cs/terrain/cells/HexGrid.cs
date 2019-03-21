using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public struct HexCellRain
{
    private int cellIndex;

    private float rain;

    public HexCellRain(int cellIndex, float rain)
    {
        this.cellIndex = cellIndex;
        this.rain = rain;
    }

    public int CellIndex
    {
        get
        {
            return cellIndex;
        }
    }

    public float Rain
    {
        get
        {
            return rain;
        }
    }
}

// 网格，整个六边形地图
public class HexGrid : MonoBehaviour
{
    private int cellCountX = 6;
    private int cellCountZ = 6;

    public int chunkCountX = 1, chunkCountZ = 1;

    public HexCell cellPrefab;
    public HexGridChunk chunkPrefab;

    HexGridChunk[] chunks;
    HexCell[] cells;

    // 显示信息
    public Text cellLabelPrefab;

    // 画布
    //Canvas gridCanvas;

    // 网格
    //HexMesh hexMesh;

    // 颜色
    public Color defaultColor = Color.white;
    public Color touchedColor = Color.magenta;

    // 整幅
    public float frequency = 1;

    // 变化
    //[Range(1, 8)]
    public int octaves = 8;//1普通，8烟雾状

    //	[Range(1f, 4f)]
    public float lacunarity = 2f;

    //	[Range(0f, 1f)]
    public float persistence = 0.5f;

    int mapSeed = 45646;

    public GameOptionData gameData;

    public List<Dictionary<int, HexCell>> lakes;

    float[][] hei;

    // 初始化地图
    public void Awake()
    {
        //gridCanvas = GetComponentInChildren<Canvas>();
        //hexMesh = GetComponentInChildren<HexMesh>();

        hei = new float[5][];
        float[] h0 = { 40f, 40f, 40f, 40f, 40f };
        float[] h1 = { 40f, 40f, 90f, 43f, 90f };
        float[] h2 = { 40f, 40f, 40f, 24f, 25f };
        float[] h3 = { 40f, 40f, 23f, 18f, 40f };
        float[] h4 = { 40f, 90f, 22f, 40f, 10f };

        hei[0] = h0;
        hei[1] = h1;
        hei[2] = h2;
        hei[3] = h3;
        hei[4] = h4;



        cellCountX = chunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

        System.TimeSpan ts = System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 0, 0, 0);
        long ret = System.Convert.ToInt64(ts.TotalSeconds);
        mapSeed = (int)(ret % 100000);

        mapSeed = 51608;
        //mapSeed = 53063; // 2条 2个入口1出口河 2入口相邻
        // mapSeed = 57532; // 2条 2个入口1出口河 1条各相隔1边 1条相邻
        Debug.Log(string.Format("seed ===== {0}", mapSeed));

        Random.InitState(mapSeed);
        int random = Random.Range(0, 1000);
        int rainRandom = Random.Range(0, 1000);

        lakes = new List<Dictionary<int, HexCell>>();

        CreateChunks();
        CreateCells(random);

        // 创建降雨图
        CreateRainMap(rainRandom);
        CreateAllRiver();
    }

    void CreateChunks()
    {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        int i = 0;
        for (int z = 0; z < chunkCountZ; z++)
        {
            for (int x = 0; x < chunkCountX; x++)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }
    }

    void CreateCells(int random)
    {
        cells = new HexCell[cellCountZ * cellCountX];

        for (int z = 0, i = 0; z < cellCountZ; z++)
        {
            for (int x = 0; x < cellCountX; x++)
            {
                CreateCell(x, z, i++, random);
            }
        }
    }

    // 创建每个单元格
    public void CreateCell(int x, int z, int index, int random)
    {
        Vector3 position;

        // 每上一行都偏移半个间距， 每间隔一行则不用  z/2是整数
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.z = z * (HexMetrics.outerRadius * 1.5f);
        position.y = 0f; //0f;

        HexCell cell = cells[index] = Instantiate<HexCell>(cellPrefab, transform, false);
        //cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.index = index;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.color = defaultColor;
        //cell.terrainType = HexTerrainType.Ridge;
        //Debug.Log(cell.terrainType);

        // 设置邻居
        // 左右
        if (x > 0)
        {
            cell.SetNeighbor(HexDirection.W, cells[index - 1]);
        }

        if (z > 0)
        {
            // 第1,3,5排 
            if ((z & 1) == 0)
            {
                // 右下左上
                cell.SetNeighbor(HexDirection.SE, cells[index - cellCountX]);

                // 左下右上
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, cells[index - cellCountX - 1]);
                }
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[index - cellCountX]);
                if (x < cellCountX - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[index - cellCountX + 1]);
                }
            }
        }

        Text label = Instantiate<Text>(cellLabelPrefab);
        //label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition =
            new Vector2(position.x, position.z);
        label.text = ""; // cell.coordinates.ToStringOnSeparateLines(); //x.ToString() + "\n" + z.ToString();

        // 设置UI位置
        cell.uiRect = label.rectTransform;
        cell.label = label;

        //hei[x][z]; // 
        cell.Height = (PerlinNoise(position.x + random, position.z + random, frequency, octaves, lacunarity, persistence) + 0f) * HexMetrics.maxElevation;

        if (cell.terrainType == HexTerrainType.Water)
        {
            cell.color = touchedColor;
        }

        AddCellToChunk(x, z, cell);

    }

    void AddCellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
    }

    //public void Start()
    //{
    //    //hexMesh.Triangulate(cells);
    //}

    // 为某个位置染色
    public void ColorCell(Vector3 position, Color color)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        HexCell cell = cells[index];
        cell.color = color;
        //hexMesh.Triangulate(cells);
    }

    // 获取坐标的格子
    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        return cells[index];
    }

    public void Refresh()
    {
        //hexMesh.Triangulate(cells);
        for(int i = 0; i < chunks.Length; i++)
        {
            chunks[i].Refresh();
        }
    }

    //public void Refresh(HexCell cell)
    //{
    //    //hexMesh.Triangulate(cell);
    //}

    public static float PerlinNoise(float x, float y, float frequency, int octaves, float lacunarity, float persistence )
    {
        float sum = Mathf.PerlinNoise(x * frequency, y * frequency);
        //      float sum = method(point, frequency);
        float amplitude = 1f;
        float range = 1f;
        for (int o = 1; o < octaves; o++)
        {
            frequency *= lacunarity;
            amplitude *= persistence;
            range += amplitude;
            sum += Mathf.PerlinNoise(x * frequency, y * frequency) * amplitude;            
        }

        //Debug.Log(string.Format("{0:N5} {1:N5} {2:N5}", sum, range, sum / range));

        return sum / range;
    }

    // 创建降雨图
    void CreateRainMap(int rainRandom)
    {
        // GameOptionData gameData = GameOptionData.getInstance();

        // 计算每个格子的降雨量
        for ( int index = 0; index < cells.Length; index++)
        {
            HexCell cell = cells[index];
            float height = cell.Height;
            int x = cell.coordinates.X;
            int y = cell.coordinates.Y;

            float rain = height * gameData.humidity * Mathf.PerlinNoise(x + rainRandom, y + rainRandom);
            if (height > HexMetrics.maxRainElevation)
            {
                rain = (HexMetrics.maxRainElevation - (height - HexMetrics.maxRainElevation)) * gameData.humidity * Mathf.PerlinNoise( x + rainRandom, y + rainRandom);
            }

            cell.Rain = rain;
            cell.label.text = string.Format("{0}\n{1}\n{2}", index, (int)height, (int)cell.Pondage);
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
            Debug.Log("|||||||||||||");
            return;
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
            if (pondage > HexMetrics.cellMaxPondage && !cell.HasRiver() && cell.terrainType != HexTerrainType.Water )
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

        if(rains.Count < 1)
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
                // 没有值或者
                if (downCell == null || cell.Height < downCell.Height)
                {
                    downCell = cell;
                    nextDir = dir;
                }

                // 如果是湖泊
                if(cell.terrainType == HexTerrainType.Water)
                {

                }
                // 周围的有过多的水量也会被河流带走
                else if (cell.Pondage > HexMetrics.cellMaxPondage)
                {
                    addFlow += cell.Pondage - HexMetrics.cellMaxPondage;
                    cell.Pondage = HexMetrics.cellMaxPondage;
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

    // 创建一条从顶点开始的河流
    void CreateRiver(HexCell riverTop)
    {
        HexCell next = riverTop;
        bool isEnd = false;

        float addFlow = 0f;

        int co = 0;

        while (!isEnd)
        {
            co++;
            if(co > 50)
            {
                Debug.Log(string.Format("error {0}", next.coordinates.ToString()));
                break;
            }
            //Debug.Log("------------------------");
            //Debug.Log(next.index);
            // 找到周围邻居最低的格子
            HexCell downCell = null;
            HexDirection nextDir = HexDirection.E;

            for (HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++)
            {
                HexCell cell = next.GetNeighbor(dir);

                // 这个 格子是湖泊，并且水满了
                if (cell)
                {
                    //Debug.Log(string.Format(" cell pondage {0} {1} {2}", cell.coordinates.ToString(), cell.terrainType, cell.Pondage));
                }
                if(cell && cell.terrainType == HexTerrainType.Water && cell.Pondage >= HexMetrics.waterCellMaxPondage)
                {

                }
                else
                {
                    if (cell && downCell == null)
                    {
                        downCell = cell;
                        nextDir = dir;
                    }

                    // 存在，并且更低，并且不是水域
                    if (cell && cell.Height < downCell.Height)
                    {
                        downCell = cell;
                        nextDir = dir;
                    }

                    // 周围的有过多的水量也会被河流带走
                    if (cell && cell.Pondage > HexMetrics.cellMaxPondage && cell.terrainType != HexTerrainType.Water)
                    {
                        addFlow += cell.Pondage - HexMetrics.cellMaxPondage;
                        cell.Pondage = HexMetrics.cellMaxPondage;
                    }
                }
            }

            if(downCell == null)
            {
                downCell = next;
                isEnd = true;
            }
            //Debug.Log(string.Format(" downCell {0} {1} {2} {3}", co, downCell.coordinates.ToString(), downCell.terrainType, downCell.Pondage));

            // 如果自己本身是湖泊
            if(next.terrainType == HexTerrainType.Water)
            {
                addFlow += next.Pondage - HexMetrics.waterCellMaxPondage;
                next.Pondage = HexMetrics.waterCellMaxPondage;

                // 下一个流出点是自己的流入点
                // 这个流入点变为河流并取消这个流入点
                if (next.IsInComingRiverDirection(nextDir))
                {
                    downCell.terrainType = HexTerrainType.Water;
                    downCell.color = touchedColor;
                    downCell.Pondage += addFlow;

                    next.ClearRiver(nextDir);
                }
                else
                {

                    // 向该方向添加河流
                    next.SetOutgoingRiver(nextDir, addFlow);
                    next = downCell;
                }
            }
            // 找到最低的邻居也比自身高则 ， 自身变为湖泊，再多余的水则往这个格子流
            //Debug.Log(next == downCell);
            else if (next.Height < downCell.Height)
            {
                // 自身变为水域
                next.terrainType = HexTerrainType.Water;
                next.color = touchedColor;
                next.Pondage = next.Pondage + addFlow;
                addFlow = 0;

                // 过多的水
                if (next.Pondage > HexMetrics.waterCellMaxPondage)
                {
                    //addFlow = next.Pondage - HexMetrics.waterCellMaxPondage;
                    //next.Pondage = HexMetrics.waterCellMaxPondage;
                    //next = downCell;
                }

                isEnd = true;

            }
            else
            {
                // 向该方向添加河流
                next.SetOutgoingRiver(nextDir, addFlow);
                next = downCell;
            }
            
        }

        CreateAllRiver();
    }

    // 创建该地图块的一条河流
    void CreateCellRiver(HexCell currStep, HexCell nextStep, HexDirection nextDir, float addFlow)
    {
        // 河流里面增加自己这块的多余水量
        if (currStep.Pondage > HexMetrics.cellMaxPondage)
        {
            addFlow += currStep.Pondage - HexMetrics.cellMaxPondage;
            currStep.Pondage = HexMetrics.cellMaxPondage;
        }
        else
        {
            addFlow += currStep.Pondage - HexMetrics.cellMaxPondage;
            currStep.Pondage = HexMetrics.cellMaxPondage;
        }
        // 向该方向添加河流
        currStep.SetOutgoingRiver(nextDir, addFlow);

        // 继续往下创建
        CreateOneRiver(nextStep, addFlow);
    }

    // 增加一个位置到湖泊中
    void AddLakes()
    {

    }

    void AddLakesCell(Dictionary<int, HexCell> lake, HexCell cell)
    {
        cell.terrainType = HexTerrainType.Water;
        cell.color = touchedColor;
        lake.Add(cell.index, cell);

        Debug.Log(string.Format("add lake cell {0}", cell.index));
    }

    void removeLakesCell(Dictionary<int, HexCell> lake, HexCell cell)
    {
        cell.terrainType = HexTerrainType.Land;
        cell.color = defaultColor;
        lake.Remove(cell.index);

        Debug.Log(string.Format("remove lake cell {0}", cell.index));
    }

    // 创建湖泊
    void CreateOneLakes(HexCell currStep, HexCell minHeightNeighbor, HexDirection nextDir, float addFlow)
    {
        // 一个湖泊
        Dictionary<int, HexCell> lake = new Dictionary<int, HexCell>();
        lakes.Add(lake);

        AddLakesCell(lake, currStep);

        // 河水全部流入
        currStep.Pondage = currStep.Pondage + addFlow;

        // 这个高度差下，能装的水量
        float heightDiff = minHeightNeighbor.Height - currStep.Height;
        float waterNum = heightDiff * HexMetrics.waterCellMaxPondage;

        // 水多了
        if(currStep.Pondage > waterNum + HexMetrics.cellMaxPondage)
        {
            CreateCellLakes(lake, currStep, currStep.Height, addFlow);
        }
        // 这个湖泊就是终点了，而且只有一个格子
        else
        {
            CreateAllRiver();
        }
    }

    // 当前湖泊
    /*
     *  lake 湖泊
     *  lastCell 上一个添加的格子
     *  waterHeight 当前湖水高度
     *  addFlow 多余水量
     */
    void CreateCellLakes(Dictionary<int, HexCell> lake, HexCell lastCell, float waterHeight, float addFlow)
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
                    if(lowCell.Height > neighbor.Height)
                    {
                        lowCell = neighbor;
                        nearCell = currStep;
                        nearDir = dir;
                    }
                }
            }
        }

        if(lowCell == null)
        {
            return;
        }
        Debug.Log(string.Format("find next lake {0}", lowCell.index));

        // 如果这个邻居够矮，则上一个格子不会变为湖泊，而是河流
        if(lowCell.Height < waterHeight)
        {
            // 将上一个添加的格子移除
            removeLakesCell(lake, lastCell);

            // 会多的水量
            float heightDiff = waterHeight - lowCell.Height;
            int cellNum = lake.Count;
            float waterNum = heightDiff * HexMetrics.waterCellMaxPondage * cellNum;
            addFlow += waterNum;

            // 设置一条流出到这个格子的河流
            nearCell.SetOutgoingRiver(nearDir, addFlow);
            Debug.Log(string.Format("------------{0}", nearCell.index));

            CreateOneRiver(lowCell, addFlow);
        }
        else
        {
            // 这个高度差下，能装的水量
            float heightDiff = lowCell.Height - waterHeight;
            int cellNum = lake.Count;
            float waterNum = heightDiff * HexMetrics.waterCellMaxPondage * cellNum + HexMetrics.cellMaxPondage;

            AddLakesCell(lake, lowCell);

            foreach (var item in lake)
            {
                HexCell cell = item.Value;
                cell.Pondage = HexMetrics.cellMaxPondage + ((int)(lowCell.Height - cell.Height) * HexMetrics.waterCellMaxPondage);
            }

            // 还有多的水
            if (addFlow > waterNum)
            {
                addFlow -= waterNum;
                // 继续添加湖泊
                CreateCellLakes(lake, lowCell, lowCell.Height, addFlow);
            }
            else
            {
                CreateAllRiver();
            }
        }
    }
}
