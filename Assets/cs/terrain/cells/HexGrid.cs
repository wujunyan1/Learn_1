using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

// 网格，整个六边形地图
public class HexGrid : MonoBehaviour, SaveLoadInterface, RoundObject
{
    public static HexGrid instance;

    public int cellCountX = 6;
    public int cellCountZ = 6;

    int chunkCountX = 1, chunkCountZ = 1;

    public HexCell cellPrefab;
    public HexGridChunk chunkPrefab;

    HexGridChunk[] chunks;
    HexCell[] cells;
    List<ClimateData> climateDatas;
    List<ClimateData> nextClimate;

    // 用于循环的  HexGridChunk以列的方式放进这里，
    Transform[] columns;

    // 位于摄像机中心的列索引
    int currentCenterColumnIndex = -1;

    public Transform mapPlane;
    public Transform objPlane;

    // 显示信息
    public Text cellLabelPrefab;

    public Texture2D noiseSource;

    HexCellShaderData cellShaderData;

    public Color clickHighlight;

    bool wrapping = false;
    public float GridWidth { get; set; }
    // 画布
    //Canvas gridCanvas;

    // 网格
    //HexMesh hexMesh;

    // 变化
    //[Range(1, 8)]
    public int octaves = 8;//1普通，8烟雾状
    
    //	[Range(0f, 1f)]
    public float persistence = 0.5f;

    public int mapSeed = 45646;

    public GameOptionData gameData;

    // 所有湖泊
    public List<Dictionary<int, HexCell>> lakes;

    public List<HexRiver> rivers;
    
    float[][] hei;

    bool isLoadMap = false;

    public void Save(BinaryWriter writer)
    {
        // 每个格子
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Save(writer);
        }
        
        // 每个格子的 气候数据
        for (int i = 0; i < climateDatas.Count; i++)
        {
            climateDatas[i].Save(writer);
        }

        // 河流数据
        writer.Write(rivers.Count);
        for (int i = 0; i < rivers.Count; i++)
        {
            rivers[i].Save(writer);
        }
    }

    public IEnumerator Load(BinaryReader reader)
    {
        NewGameData data = GameCenter.instance.gameData;
        IEnumerator tor = CreateMap(data);
        while (tor.MoveNext())
        {
            yield return null;
        }

        if (!isLoadMap)
        {
            Debug.Log("xxxxxxxxxxxxxx");
            //return;

            yield break;
        }

        // 加载地图时，可见性不用变化
        bool originalImmediateMode = cellShaderData.ImmediateMode;
        cellShaderData.ImmediateMode = true;

        // 格子
        for (int i = 0; i < cells.Length; i++)
        {
            tor = cells[i].Load(reader);
            while (tor.MoveNext())
            {
                yield return null;
            }
        }


        // 气候
        yield return null;
        for (int i = 0; i < climateDatas.Count; i++)
        {
            tor = climateDatas[i].Load(reader);
            while (tor.MoveNext())
            {
                yield return null;
            }
        }

        // 河流
        yield return null;
        int riverCount = reader.ReadInt32();
        for (int i = 0; i < riverCount; i++)
        {
            HexRiver river = new HexRiver();
            tor = river.Load(reader);
            while (tor.MoveNext())
            {
                yield return null;
            }
            rivers.Add(river);
        }

        yield return null;
        cellShaderData.ImmediateMode = originalImmediateMode;
    }
    

    // 初始化地图
    public void Awake()
    {
        HexMapCamera.instance.grid = this;

        instance = this;
        HexMetrics.noiseSource = noiseSource;
        cellShaderData = gameObject.AddComponent<HexCellShaderData>();
        cellShaderData.Grid = this;
        cellShaderData.Initialize(cellCountX, cellCountZ);

        //CreateMap(cellCountX, cellCountZ);
    }

    private void Start()
    {
        // 添加事件侦听
        ObjectEventDispatcher.dispatcher.addEventListener(EventTypeName.UPDATE_CHUNK_MESH, this.UpdateChunkMesh);
    }

    public static HexGrid GetInstance()
    {
        return instance;
    }

    public IEnumerator CreateMap(NewGameData data)
    {
        ClearPath();

        isLoadMap = false;
        currentCenterColumnIndex = -1;

        int x = data.X;
        int z = data.Z;
        int mapSeed = data.mapSeed;
        if (x <= 0 || x % HexMetrics.chunkSizeX != 0 || z <= 0 || z % HexMetrics.chunkSizeZ != 0 )
        {
            Debug.LogError("Unsupported map size.");
            yield break;
        }

        // 清除原来的数据
        if (columns != null)
        {
            for (int i = 0; i < columns.Length; i++)
            {
                Destroy(columns[i].gameObject);
            }
        }
        

        cellCountX = x;
        cellCountZ = z;
        chunkCountX = cellCountX / HexMetrics.chunkSizeX;
        chunkCountZ = cellCountZ / HexMetrics.chunkSizeZ;

        wrapping = data.wrapping;
        HexMetrics.wrapSize = data.wrapping ? cellCountX : 0;
        GridWidth = chunkCountX * (HexMetrics.innerDiameter * HexMetrics.chunkSizeX);


        // 修改shaderData 大小
        cellShaderData.Initialize(cellCountX, cellCountZ);
        
        lakes = new List<Dictionary<int, HexCell>>();

        yield return null;
        CreateChunks();

        yield return null;
        CreateCells();
        yield return null;

        CreateClimateDatas();

        rivers = new List<HexRiver>();
        
        isLoadMap = true;
        yield return null;
    }


    public void CenterMap(float xPosition)
    {
        int centerColumnIndex = (int)
            (xPosition / (HexMetrics.innerDiameter * HexMetrics.chunkSizeX));

        if (centerColumnIndex == currentCenterColumnIndex)
        {
            return;
        }
        currentCenterColumnIndex = centerColumnIndex;

        int minColumnIndex = centerColumnIndex - chunkCountX / 2;
        int maxColumnIndex = centerColumnIndex + chunkCountX / 2;

        // 重写设置坐标
        Vector3 position;
        position.y = position.z = 0f;
        for (int i = 0; i < columns.Length; i++)
        {
            if (i < minColumnIndex)
            {
                position.x = GridWidth;
            }
            else if (i > maxColumnIndex)
            {
                position.x = -GridWidth;
            }
            else
            {
                position.x = 0f;
            }
            columns[i].localPosition = position;
        }
    }

    void CreateChunks()
    {
        columns = new Transform[chunkCountX];
        for (int x = 0; x < chunkCountX; x++)
        {
            columns[x] = new GameObject("Column").transform;
            columns[x].SetParent(mapPlane, false);
        }

        chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        int i = 0;
        for (int z = 0; z < chunkCountZ; z++)
        {
            for (int x = 0; x < chunkCountX; x++)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(columns[x], false);
            }
        }
    }

    void CreateCells()
    {
        cells = new HexCell[cellCountZ * cellCountX];

        for (int z = 0, i = 0; z < cellCountZ; z++)
        {
            for (int x = 0; x < cellCountX; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    void CreateClimateDatas()
    {
        climateDatas = new List<ClimateData>();
        nextClimate = new List<ClimateData>();

        for (int i = 0; i < cellCountX * cellCountZ; i++)
        {
            ClimateData initialData = new ClimateData();
            initialData.moisture = GameCenter.instance.gameData.startingMoisture;
            climateDatas.Add(initialData);

            ClimateData clearData = new ClimateData();
            nextClimate.Add(clearData);
        }
    }

    // 创建每个单元格
    public void CreateCell(int x, int z, int index)
    {
        Vector3 position;

        // 每上一行都偏移半个间距， 每间隔一行则不用  z/2是整数
        position.x = (x + z * 0.5f - z / 2) * HexMetrics.innerDiameter;
        position.z = z * (HexMetrics.outerRadius * 1.5f);
        position.y = 0f; //0f;

        HexCell cell = cells[index] = Instantiate<HexCell>(cellPrefab, transform, false);

        //cell.transform.SetParent(transform, false);
        
        cell.index = index;
        cell.ColumnIndex = x / HexMetrics.chunkSizeX;
        cell.coordinates = new HexCoordinates(x , z); // HexCoordinates.FromOffsetCoordinates(x, z);
        cell.Vector = HexVector.FromOffsetCoordinates(x, z);
        cell.transform.localPosition = cell.Vector.GetGamePoint();

        cell.ShaderData = cellShaderData;

        // 判断是否是地图边缘
        if (wrapping)
        {
            cell.Explorable = z > 0 && z < cellCountZ - 1;
        }
        else
        {
            cell.Explorable =
            x > 0 && z > 0 && x < cellCountX - 1 && z < cellCountZ - 1;
        }

        //cell.terrainType = HexTerrainType.Ridge;
        //Debug.Log(cell.terrainType);

        // 设置邻居
        // 左右
        if (x > 0)
        {
            cell.SetNeighbor(HexDirection.W, cells[index - 1]);

            if (wrapping && x == cellCountX - 1)
            {
                cell.SetNeighbor(HexDirection.E, cells[index - x]);
            }
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
                else if (wrapping)
                {
                    cell.SetNeighbor(HexDirection.SW, cells[index - 1]);
                }
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[index - cellCountX]);
                if (x < cellCountX - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[index - cellCountX + 1]);
                }
                else if (wrapping)
                {
                    cell.SetNeighbor(
                        HexDirection.SE, cells[index - cellCountX * 2 + 1]
                    );
                }
            }
        }

        Text label = Instantiate<Text>(cellLabelPrefab);
        //label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition =
            new Vector2(position.x, position.z);

        string str = ""; // cell.Vector.GetIndex().ToString()+",[" + x.ToString() + "," + z.ToString() + "] \n" + cell.Vector.X +"," + cell.Vector.Z;
        label.text = str; // ""; // cell.coordinates.ToStringOnSeparateLines(); //x.ToString() + "\n" + z.ToString();

        // 设置UI位置
        cell.uiRect = label.rectTransform;
        cell.label = label;

        //hei[x][z]; // 
        cell.Height = 0; //GetCellHeight(x, z); // (PerlinNoise(position.x + random, position.z + random, frequency, octaves, lacunarity, persistence) + 0f) * HexMetrics.maxElevation;

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

    void UpdateAllChunkMesh()
    {
        if (chunks != null)
        {
            for (int i = 0; i < chunks.Length; i++)
            {
                chunks[i].Refresh();
            }
        }
    }

    public int GetChunkIndex(HexVector vector)
    {
        int index = vector.GetIndex();
        NewGameData data = GameCenter.instance.gameData;

        int x = index % data.width;
        int z = index / data.width;

        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;

        int chunkIndex = chunkX + chunkZ * chunkCountX;
        return chunkIndex;
    }

    // 事件更新某个Cell 
    void UpdateChunkMesh(UEvent uEvent)
    {
        int index = (int)uEvent.eventParams;
        NewGameData data = GameCenter.instance.gameData;
        
        int x = index % data.width;
        int z = index / data.width;

        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;

        int chunkIndex = chunkX + chunkZ * chunkCountX;
        HexGridChunk chunk = chunks[chunkIndex];
        chunk.Refresh();

        bool isLeft = false;
        bool isRight = false;

        // 在X轴 大格子的边界上
        if (x != 0 && x % HexMetrics.chunkSizeX == 0)
        {
            isLeft = true;
            chunks[chunkIndex - 1].Refresh();
        }
        else if (x != data.width - 1 && x % HexMetrics.chunkSizeX == HexMetrics.chunkSizeX - 1)
        {
            isRight = true;
            chunks[chunkIndex + 1].Refresh();
        }

        // 在Z轴 大格子的边界上
        if (z != 0 && z % HexMetrics.chunkSizeZ == 0)
        {
            chunks[chunkIndex - chunkCountX].Refresh();

            // 对角上的也要修改
            if (isLeft)
            {
                chunks[chunkIndex - chunkCountX - 1].Refresh();
            }
            else if (isRight)
            {
                chunks[chunkIndex - chunkCountX + 1].Refresh();
            }
        }
        else if (z != data.height - 1 && z % HexMetrics.chunkSizeZ == HexMetrics.chunkSizeZ - 1)
        {
            chunks[chunkIndex + chunkCountX].Refresh();

            if (isLeft)
            {
                chunks[chunkIndex + chunkCountX - 1].Refresh();
            }
            else if (isRight)
            {
                chunks[chunkIndex + chunkCountX + 1].Refresh();
            }
        }
    }

    //public void Start()
    //{
    //    //hexMesh.Triangulate(cells);
    //}

    public Vector2 GetMapSize()
    {
        float x = cellCountX * HexMetrics.innerRadius * 2f;
        float y = cellCountZ * HexMetrics.outerRadius * 1.5f;
        return new Vector2(x, y);
    }


    // 为某个位置染色
    public void ColorCell(Vector3 position, Color color)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        HexCell cell = cells[index];
        //cell.color = color;
        //hexMesh.Triangulate(cells);
    }

    // 获取坐标的格子
    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        if(index >= cellCountX * cellCountZ || index < 0)
        {
            return null;
        }
        return cells[index];
    }

    public HexCell GetCell(int xOffset, int zOffset)
    {
        return cells[xOffset + zOffset * cellCountX];
    }

    public HexCell GetCell(HexCoordinates point)
    {
        return GetCell(point.X, point.Z);
    }

    // 这个坐标是斜着的
    public HexCell GetCell(HexVector vector )
    {
        int index = vector.GetIndex();
        if (index >= 0 && index < cells.Length)
        {
            return cells[index];
        }
        return null;
    }

    // 获取坐标的格子
    public HexCell GetCell(int cellIndex)
    {
        return cells[cellIndex];
    }

    public ClimateData GetClimateData(int cellIndex)
    {
        return climateDatas[cellIndex];
    }

    public ClimateData GetNextClimateData(int cellIndex)
    {
        return nextClimate[cellIndex];
    }

    public void ReplaceClimateData()
    {
        List<ClimateData> swap = climateDatas;
        climateDatas = nextClimate;
        nextClimate = swap;

        for (int i = 0; i < nextClimate.Count; i++)
        {
            nextClimate[i] = new ClimateData();
        }
    }

    public void StartGenerateMap()
    {
        for (int i = 0; i < chunks.Length; i++)
        {
            chunks[i].StartGenerateMap();
        }

        for (int i = 0; i < chunks.Length; i++)
        {
            chunks[i].RefreshBoundary();
        }
    }

    public void Refresh()
    {
        //hexMesh.Triangulate(cells);
        for(int i = 0; i < chunks.Length; i++)
        {
            chunks[i].Refresh();
        }
    }

    public void MakeChildOfColumn(Transform child, int columnIndex)
    {
        child.SetParent(columns[columnIndex], false);
    }

    public void TroopUpdatePostion(Transform troop, HexCell cell)
    {
        MakeChildOfColumn(troop, cell.ColumnIndex);
    }


    // 搜索队列
    HexCellPriorityQueue searchFrontier;

    // 表示第几次搜寻，cell的搜寻数据在SearchPhase小于这个是无视
    int searchFrontierPhase;

    HexCell currentPathFrom, currentPathTo;
    bool currentPathExists;

    public List<HexCell> FindPath(HexCell fromCell, HexCell toCell, int speed)
    {
        //StopAllCoroutines();
        //StartCoroutine(Search(fromCell, toCell, speed));
        // 计时器
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        ClearPath();
        currentPathFrom = fromCell;
        currentPathTo = toCell;
        currentPathExists = Search(fromCell, toCell, speed);
        List<HexCell> path = GetPath();
        ShowPath(fromCell, path, speed);

        sw.Stop();
        Debug.Log(sw.ElapsedMilliseconds);

        return path;
    }

    public bool Search(HexCell fromCell, HexCell toCell, int speed)
    {
        searchFrontierPhase += 2;

        if (searchFrontier == null)
        {
            searchFrontier = new HexCellPriorityQueue();
        }
        else
        {
            searchFrontier.Clear();
        }

        //for (int i = 0; i < cells.Length; i++)
        //{
        //    //cells[i].Distance = int.MaxValue;
        //    cells[i].SetLabel(null);
        //    cells[i].DisableHighlight();
        //}

        //fromCell.EnableHighlight(Color.blue);
        //toCell.EnableHighlight(Color.red);

        // 所有搜寻过的位置
        //List<HexCell> frontier = new List<HexCell>();
        fromCell.SearchPhase = searchFrontierPhase;
        fromCell.Distance = 0;
        //frontier.Add(fromCell);
        searchFrontier.Enqueue(fromCell);

        while (searchFrontier.Count > 0)
        {
            HexCell current = searchFrontier.Dequeue();
            current.SearchPhase += 1;
            //frontier.RemoveAt(0);

            if (current == toCell)
            {
                return true;
            }

            int currentTurn = current.Distance / speed;

            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);
                if (neighbor == null || neighbor.SearchPhase > searchFrontierPhase)
                {
                    continue;
                }
                //int distance = current.Distance;

                int moveCost = current.GetDistanceCost(neighbor);
                if (moveCost < 0)
                {
                    continue;
                }

                int distance = current.Distance + moveCost;
                int turn = distance / speed;

                // 额外增加了一个回合
                if (turn > currentTurn)
                {
                    // 距离就是 前一个回合数走的距离 + 这个格子的距离
                    distance = turn * speed + moveCost;
                }

                // 没有设置过距离的
                if (neighbor.SearchPhase < searchFrontierPhase)
                {
                    neighbor.SearchPhase = searchFrontierPhase;
                    neighbor.Distance = distance;
                    neighbor.PathFrom = current;
                    neighbor.SearchHeuristic =
                        neighbor.coordinates.DistanceTo(toCell.coordinates) * 5;

                    //neighbor.SetLabel(turn.ToString());
                    searchFrontier.Enqueue(neighbor);
                    //frontier.Add(neighbor);
                }
                // 有更近的距离的
                else if (distance < neighbor.Distance)
                {
                    int oldPriority = neighbor.SearchPriority;
                    neighbor.Distance = distance;
                    neighbor.PathFrom = current;
                    //neighbor.SetLabel(turn.ToString());
                    searchFrontier.Change(neighbor, oldPriority);
                }

                //frontier.Sort(
                //    (x, y) => x.SearchPriority.CompareTo(y.SearchPriority)
                //);
            }

        }
        return false;
    }

    List<HexCell> GetPath()
    {
        List<HexCell> path = new List<HexCell>();

        if (currentPathExists)
        {
            HexCell current = currentPathTo;
            while (current != currentPathFrom)
            {
                path.Add(current);

                //int turn = current.Distance / speed;
                //current.SetLabel(turn.ToString());
                //string s = string.Format("{0} {1} {2}", current.index, current.coordinates.X, current.coordinates.Z);
                //current.SetLabel(s);
                //current.EnableHighlight(Color.white);
                current = current.PathFrom;
            }
        }
        //currentPathFrom.EnableHighlight(Color.blue);
        //currentPathTo.EnableHighlight(Color.red);

        return path;
    }
    
    public void ShowPath(HexCell start, List<HexCell> path, int speed)
    {
        //List<HexCell> path = new List<HexCell>();
        int cost = 0;

        HexCell currCell = start;

        for ( int i = path.Count - 1; i >= 0; i--)
        {
            HexCell nextCell = path[i];

            cost += currCell.GetDistanceCost(nextCell);

            int turn = cost / speed;
            nextCell.SetLabel(turn.ToString());
            //string s = string.Format("{0} {1} {2}", nextCell.index, nextCell.coordinates.X, nextCell.coordinates.Z);
            //nextCell.SetLabel(s);
            nextCell.EnableHighlight(Color.white);

            currCell = nextCell;
        }

        start.EnableHighlight(Color.blue);
        if (path.Count > 0)
        {
            path[0].EnableHighlight(Color.red);
        }
    }



    void ClearCellDistance()
    {

    }

    public void ShowPath(List<HexCell> path)
    {
        foreach (var item in path)
        {
            item.EnableHighlight(Color.white);
        }
    }

    public List<HexCell> Search(HexCell fromCell, int speed)
    {
        searchFrontierPhase += 2;

        if (searchFrontier == null)
        {
            searchFrontier = new HexCellPriorityQueue();
        }
        else
        {
            searchFrontier.Clear();
        }

        List<HexCell> frontier = new List<HexCell>();

        fromCell.SearchPhase = searchFrontierPhase;
        fromCell.Distance = 0;
        // frontier.Add(fromCell);
        searchFrontier.Enqueue(fromCell);

        while (searchFrontier.Count > 0)
        {
            HexCell current = searchFrontier.Dequeue();
            current.SearchPhase += 1;

            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);
                if (neighbor == null || neighbor.SearchPhase > searchFrontierPhase)
                {
                    continue;
                }
                //int distance = current.Distance;

                int moveCost = current.GetDistanceCost(neighbor);
                if (moveCost < 0)
                {
                    continue;
                }

                int distance = current.Distance + moveCost;
                int turn = speed > 0 ? distance / speed : 0;

                // 额外增加了一个回合
                if (distance > speed)
                {
                    continue;
                }

                // 没有设置过距离的
                if (neighbor.SearchPhase < searchFrontierPhase)
                {
                    neighbor.SearchPhase = searchFrontierPhase;
                    neighbor.Distance = distance;
                    neighbor.PathFrom = current;
                    neighbor.SearchHeuristic = speed;

                    //neighbor.SetLabel(turn.ToString());
                    searchFrontier.Enqueue(neighbor);
                    frontier.Add(neighbor);
                }
                // 有更近的距离的
                else if (distance < neighbor.Distance)
                {
                    int oldPriority = neighbor.SearchPriority;
                    neighbor.Distance = distance;
                    neighbor.PathFrom = current;
                    //neighbor.SetLabel(turn.ToString());
                    searchFrontier.Change(neighbor, oldPriority);
                }
                
            }

        }
        return frontier;
    }

    public List<HexCell> ShowCanMoveCell(HexCell start, int speed)
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        List<HexCell> path = Search(start, speed);
        foreach (var item in path)
        {
            if (item.Explorable)
            {
                item.EnableHighlight(Color.white);
            }
        }

        sw.Stop();
        return path;
    }


    void ClearPath()
    {
        if (currentPathExists)
        {
            HexCell current = currentPathTo;
            while (current != currentPathFrom)
            {
                current.SetLabel(null);
                current.DisableHighlight();
                current = current.PathFrom;
            }
            current.DisableHighlight();
            currentPathExists = false;
        }
        else if (currentPathFrom)
        {
            currentPathFrom.DisableHighlight();
            currentPathTo.DisableHighlight();
        }

        currentPathFrom = currentPathTo = null;
    }

    public void ClearShowPath(HexCell path)
    {
        path.DisableHighlight();
        path.SetLabel(null);
    }

    public void ClearShowPath(List<HexCell> path)
    {
        foreach(HexCell cell in path)
        {
            ClearShowPath(cell);
        }
    }

    List<HexCell> GetVisibleCells(HexCell fromCell, int range)
    {
        List<HexCell> visibleCells = ListPool<HexCell>.Get();

        searchFrontierPhase += 2;
        if (searchFrontier == null)
        {
            searchFrontier = new HexCellPriorityQueue();
        }
        else
        {
            searchFrontier.Clear();
        }

        range += fromCell.ViewElevation;
        fromCell.SearchPhase = searchFrontierPhase;
        fromCell.Distance = 0;
        searchFrontier.Enqueue(fromCell);

        HexCoordinates fromCoordinates = fromCell.coordinates;

        while (searchFrontier.Count > 0)
        {
            HexCell current = searchFrontier.Dequeue();
            current.SearchPhase += 1;
            visibleCells.Add(current);
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                // 不存在，已被搜索，不可被探索
                HexCell neighbor = current.GetNeighbor(d);
                if (
                    neighbor == null ||
                    neighbor.SearchPhase > searchFrontierPhase ||
                    !neighbor.Explorable
                )
                {
                    continue;
                }
                int distance = current.Distance + 1;
                
                // 每高一格则限制一格的视觉范围
                if (distance + neighbor.ViewElevation > range) // ||
                    //distance > fromCoordinates.DistanceTo(neighbor.coordinates))
                {
                    continue;
                }
                if (neighbor.SearchPhase < searchFrontierPhase)
                {
                    neighbor.SearchPhase = searchFrontierPhase;
                    neighbor.Distance = distance;
                    neighbor.SearchHeuristic = 0;
                    searchFrontier.Enqueue(neighbor);
                }
                else if (distance < neighbor.Distance)
                {
                    int oldPriority = neighbor.SearchPriority;
                    neighbor.Distance = distance;
                    searchFrontier.Change(neighbor, oldPriority);
                }
            }
        }
        return visibleCells;
    }

    public void IncreaseVisibility(HexCell fromCell, int range)
    {
        List<HexCell> cells = GetVisibleCells(fromCell, range);
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].IncreaseVisibility();
        }
        ListPool<HexCell>.Add(cells);
    }

    public void DecreaseVisibility(HexCell fromCell, int range)
    {
        List<HexCell> cells = GetVisibleCells(fromCell, range);
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].DecreaseVisibility();
        }
        ListPool<HexCell>.Add(cells);
    }

    public void ResetVisibility()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].ResetVisibility();
        }

        GameCenter gameCenter = GameCenter.instance;
        Camp camp = gameCenter.GetCamp(gameCenter.PlayerCampId);
        camp.ResetTroopObserveVisibility();


        //
        //for (int i = 0; i < units.Count; i++)
        //{
        //    HexUnit unit = units[i];
        //    IncreaseVisibility(unit.Location, unit.VisionRange);
        //}
    }

    public void SetCellMapDataShader(bool isShow)
    {
        foreach (var item in chunks)
        {
            item.SetCellMapDataShader(isShow);
        }
    }



    public HexRiver GetNewRiver(HexCell origin)
    {
        HexRiver river = new HexRiver(origin);
        rivers.Add(river);

        return river;
    }

    public HexRiver GetHexRiverByOrigin(HexCell origin)
    {
        foreach (var item in rivers)
        {
            if (item.isOrigin(origin))
            {
                return item;
            }
        }

        return null;
    }

    // 合并河流
    public void MergeRiver(HexRiver river, HexRiver oldRiver)
    {
        if(river == null || oldRiver == null)
        {
            return;
        }

        river.AddHexRiverr(oldRiver);

        rivers.Remove(oldRiver);
    }



   

    /// <summary>
    /// 随机获得一块陆地
    /// </summary>
    /// <returns></returns>
    public HexCell GetRandomLand()
    {
        while (true)
        {
            int x = Random.Range(0, cellCountX * cellCountZ);
            HexCell cell = GetCell(x);

            if (!cell.IsUnderwater)
            {
                return cell;
            }
        }
    }

    /// <summary>
    /// 距离范围内的所有格子
    /// </summary>
    /// <param name="cell">中心</param>
    /// <param name="distance">距离</param>
    /// <returns></returns>
    public List<HexCell> GetNearCell(HexCell cell, int distance)
    {
        List<HexCell> cells = ListPool<HexCell>.Get();

        HexVector vector = cell.Vector;

        int centerX = vector.X;
        int centerZ = vector.Z;

        for (int r = 0, z = centerZ - distance; z <= centerZ; z++, r++)
        {
            for (int x = centerX - r; x <= centerX + distance; x++)
            {
                cells.Add(GetCell(new HexVector(x, z)));
            }
        }
        for (int r = 0, z = centerZ + distance; z > centerZ; z--, r++)
        {
            for (int x = centerX - distance; x <= centerX + r; x++)
            {
                cells.Add(GetCell(new HexVector(x, z)));
            }
        }
        
        return cells;
    }

    public void NextRound()
    {
        foreach (var cell in cells)
        {
            cell.NextRound();
        }
    }

    public void LaterNextRound()
    {
        foreach (var cell in cells)
        {
            cell.LaterNextRound();
        }
    }





    public void UpdateAllBoundary()
    {
        if (chunks != null)
        {
            for (int i = 0; i < chunks.Length; i++)
            {
                chunks[i].RefreshBoundary();
            }
        }
    }

    public void UpdateBoundary(List<HexCell> cells)
    {
        
    }



}
