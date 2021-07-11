using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

// 一个六边形 类
public class HexCell : MonoBehaviour, SaveLoadInterface, RoundObject
{
    public int index;
    public int chunkIndex;
    public int ColumnIndex { get; set; }

    // 坐标
    public HexCoordinates coordinates;

    public HexVector Vector;

    // 高度层级
    int elevation = int.MinValue;
    public int Elevation
    {
        get
        {
            return elevation;
        }
        set
        {
            if (elevation == value)
            {
                return;
            }
            int originalViewElevation = ViewElevation;
            
            int old_elevation = elevation;
            elevation = value;

            if (ViewElevation != originalViewElevation)
            {
                ShaderData.ViewElevationChanged();
            }

            // 修改垂直坐标
            RefreshPosition();
            
            for (int i = 0; i < roads.Length; i++)
            {
                if (roads[i] && GetElevationDifference((HexDirection)i) > 1)
                {
                    SetRoad(i, false);
                }
            }

            // 并非初始赋值时
            if (old_elevation != int.MinValue)
            {
                //Refresh();
            }
        }
    }

    // 高度 数据非显示高度
    int height;
    public int Height
    {
        get
        {
            return height;
        }
        set
        {
            height = value;
            Elevation = HexMetrics.GetElevationHierarchy(height);
        }
    }

    // 旁边的
    [SerializeField]
    HexCell[] neighbors;

    // 坐标UI
    public RectTransform uiRect;
    public Text label;

    // 地形类型
    HexTerrainType terrainType = HexTerrainType.Land;
    public HexTerrainType TerrainType
    {
        get
        {
            return terrainType;
        }
        set
        {
            terrainType = value;
        }
    }

    public HexFeatureType FeatureType = HexFeatureType.NULL;
    public int FeatureLevel = 0;
    public List<GameObject> FeaturePos = null;

    // 建筑物
    MapBuild build = null;
    public MapBuild Build
    {
        get
        {
            return build;
        }
        set
        {
            build = value;
        }
    }

    // 属于的城市地块
    City _city;
    public City city
    {
        get
        {
            return _city;
        }
        set
        {
            _city = value;

            if(_city != null)
            {
                ShaderData.UpdateCellCampData(this, _city.camp.GetId());
            }
        }
    }

    // 人物
    public PersonControl Person { get; set; }

    public Troop Troop { get; set; }

    public HexGridChunk chunk;
    

    // 湖泊高度 / 水面高度
    int lakesLevel;
    public int LakesLevel
    {
        get
        {
            return lakesLevel;
        }
        set
        {
            if (lakesLevel == value)
            {
                return;
            }
            int originalViewElevation = ViewElevation;
            lakesLevel = value;
            if (ViewElevation != originalViewElevation)
            {
                ShaderData.ViewElevationChanged();
            }
            // Refresh();
        }
    }

    // 临时数据 
    public float lakesHeight;

    // 存储量
    int store;
    public int Store
    {
        get
        {
            return store;
        }
        set
        {
            store = value;
        }
    }

    // 河流
    RiverDirection[] rivers;

    // 道路
    [SerializeField]
    bool[] roads;

    /// <summary>
    /// 距离
    /// </summary>
    int distance;
    public int Distance
    {
        get
        {
            return distance;
        }
        set
        {
            distance = value;
        }
    }

    [SerializeField]
    int walls;

    // 是否可被探索，用于地图边缘
    public bool Explorable { get; set; }

    bool explored;

    // 是否已被探索
    public bool IsExplored {
        get
        {
            return explored && Explorable;
        }
        private set
        {
            explored = value;
        }
    }

    /// <summary>
    /// 显示路径
    /// </summary>
    public HexCell PathFrom { get; set; }

    public int SearchHeuristic { get; set; }

    public int SearchPhase { get; set; }

    // 预估距离
    public int SearchPriority
    {
        get
        {
            return distance + SearchHeuristic;
        }
    }

    public HexCell NextWithSamePriority { get; set; }

    public HexCellShaderData ShaderData { get; set; }

    // 是否亮
    public bool IsVisible
    {
        get
        {
            return visibility > 0 && Explorable;
        }
    }
    int visibility;
    

    // 视角高度，
    public int ViewElevation
    {
        get
        {
            return elevation >= lakesLevel ? elevation : lakesLevel;
        }
    }

    public bool IsUnderwater
    {
        get
        {
            return lakesLevel > elevation;
        }
    }

    public void Awake()
    {
        rivers = new RiverDirection[HexMetrics.HexTrianglesNum];

        roads = new bool[HexMetrics.HexTrianglesNum];
        //DisableHighlight();

        walls = 0;
    }

    public void Start()
    {
        DisableHighlight();
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write((byte)terrainType);
        writer.Write(elevation);
        writer.Write((byte)height);
        writer.Write((byte)lakesLevel);
        writer.Write(store);
        writer.Write(explored);
        writer.Write((byte)FeatureType);
        writer.Write((byte)FeatureLevel);


        for (int i = 0; i < rivers.Length; i++)
        {
            writer.Write((byte)rivers[i]);
        }

        for (int i = 0; i < roads.Length; i++)
        {
            writer.Write(roads[i]);
        }

        writer.Write(walls);
        

        //MapBuildFactory.Save(build, writer);
    }

    public IEnumerator Load(BinaryReader reader)
    {
        terrainType = (HexTerrainType)reader.ReadByte();
        elevation = reader.ReadInt32();
        RefreshPosition();
        height = reader.ReadByte();
        lakesLevel = reader.ReadByte();
        store = reader.ReadInt32();
        explored = reader.ReadBoolean();
        FeatureType = (HexFeatureType)reader.ReadByte();
        FeatureLevel = reader.ReadByte();


        for (int i = 0; i < rivers.Length; i++)
        {
            rivers[i] = (RiverDirection)reader.ReadByte();
        }

        for (int i = 0; i < roads.Length; i++)
        {
            roads[i] = reader.ReadBoolean();
        }

        walls = reader.ReadInt32();

        yield return null;
        //build = MapBuildFactory.Load(reader);
    }

    void RefreshPosition()
    {
        Vector3 position = transform.localPosition;
        position.y = elevation * HexMetrics.elevationStep;
        transform.localPosition = position;

        Vector3 uiPosition = uiRect.localPosition;
        uiPosition.z = -position.y;
        uiRect.localPosition = uiPosition;
    }

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    public HexDirection Direction( HexCell neighbor)
    {
        int center = this.index;
        int nei = neighbor.index;

        int s = nei - center;
        int cellCountX = HexGrid.instance.cellCountX;

        if (s == 1)
        {
            return HexDirection.E;
        }

        if (s == -1)
        {
            return HexDirection.W;
        }

        int diff = 1;

        if((coordinates.Z & 1) == 0)
        {
            diff = 0;
        }

        s -= diff;

        if (s == cellCountX )
        {
            return HexDirection.NE;
        }

        if (s == cellCountX - 1)
        {
            return HexDirection.NW;
        }

        if (s == -cellCountX - 1)
        {
            return HexDirection.SW;
        }

        if (s == -cellCountX)
        {
            return HexDirection.SE;
        }


        return HexDirection.E;
    }




    // 获取自己和邻居的连接地形
    public HexEdgeType GetEdgeType(HexDirection direction)
    {
        return HexMetrics.GetEdgeType(
            elevation, neighbors[(int)direction].elevation
        );
    }

    // 获取自己和邻居的连接地形
    public HexEdgeType GetEdgeType(HexCell otherCell)
    {
        return HexMetrics.GetEdgeType(
            elevation, otherCell.elevation
        );
    }

    public Vector3 Position
    {
        get
        {
            return transform.localPosition;
        }
        set
        {
            transform.localPosition = value;
        }
    }

    // 待修改， 只需要更新周围的 6格就好了
    public void Refresh()
    {
        if (chunk)
        {
            chunk.Refresh();

            if (Person)
            {
                Person.ValidateLocation();
            }

        }

    }

    // 
    public void RefreshOnlySelf()
    {
        chunk.Refresh( this );
        if (Person)
        {
            Person.ValidateLocation();
        }
    }

    

    /**
     *   河流
     * 
     * 
     */

    public void RemoveRiver()
    {
        rivers = new RiverDirection[HexMetrics.HexTrianglesNum];
    }

    // 增加一条流出的河流
    public void SetOutgoingRiver(HexDirection direction, float flow = 100)
    {
        
        HexCell neighbor = GetNeighbor(direction);

        // 设置流出河流
        rivers[(int)direction] = RiverDirection.Outgoing;
        if (neighbor)
        {
            neighbor.SetIncomingRiver(HexDirectionExtensions.Opposite(direction), flow);
        }

        SetRoad((int)direction, false);
    }
 
    // 增加一条流入的河流
    public void SetIncomingRiver(HexDirection direction, float flow = 100)
    {
        // 设置流入河流
        rivers[(int)direction] = RiverDirection.Incoming;
    }

    public RiverDirection GetRiverDirection(HexDirection direction)
    {
        return rivers[(int)direction];
    }

    public bool HasRiver()
    {
        for (HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++)
        {
            if (GetRiverDirection(dir) != RiverDirection.Null)
            {
                return true;
            }
        }
        return false;
    }

    // 判断指定的边是否有河流
    public bool HasRiverThroughEdge(HexDirection direction)
    {
        return GetRiverDirection(direction) != RiverDirection.Null;
    }

    public bool HasRiverBeginOrEnd()
    {
        // 如果是湖泊，只要有河流就是开头和结束
        bool hasIn = false;
        bool hasOut = false;
        for (HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++)
        {
            RiverDirection river = GetRiverDirection(dir);
            
            if (river == RiverDirection.Incoming)
            {
                hasIn = true;
            }
            else if (river == RiverDirection.Outgoing)
            {
                hasOut = true;
            }

            if (hasIn && hasOut)
            {
                return false;
            }
        }

        if (!hasIn && !hasOut)
        {
            return false;
        }

        return true;
    }

    public HexDirection RiverBeginOrEndDirection()
    {
        for (HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++)
        {
            RiverDirection river = GetRiverDirection(dir);
            if (river != RiverDirection.Null)
            {
                return dir;
            }
        }

        return HexDirection.NE;
    }


    public bool IsRiverOrigin()
    {
        int num = 0;
        for (HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++)
        {
            RiverDirection river = GetRiverDirection(dir);
            if (river != RiverDirection.Outgoing)
            {
                return false;
            }
            if(river != RiverDirection.Null)
            {
                num++;
                if(num > 1)
                {
                    return false;
                }
            }
        }

        return true;
    }

    // 获取显示河床高度
    public float StreamBedY
    {
        get
        {
            return
                (elevation + HexMetrics.streamBedElevationOffset) *
                HexMetrics.elevationStep;
        }
    }

    public int GetRiverCount()
    {
        int count = 0;
        for (HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++)
        {
            RiverDirection river = GetRiverDirection(dir);
            if (river != RiverDirection.Null)
            {
                count++;
            }
        }

        return count;
    }

    // 获取某个流向的所有方向河流
    public List<HexDirection> GetRiverDirections(RiverDirection riverDirection)
    {
        List<HexDirection> dirs = new List<HexDirection>();

        for (HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++)
        {
            RiverDirection river = GetRiverDirection(dir);
            if (river == riverDirection)
            {
                dirs.Add(dir);
            }
        }

        return dirs;
    }

    // 河流表面的垂直位置
    public float RiverSurfaceY
    {
        get
        {
            return
                (elevation + HexMetrics.waterElevationOffset) *
                HexMetrics.elevationStep;
        }
    }

    // 湖泊高度
    public float LakesSurfaceY
    {
        get
        {
            return
                (lakesLevel + HexMetrics.waterElevationOffset) *
                HexMetrics.elevationStep;
        }
    }

    // 判断指定的边是否有河流
    public bool IsInComingRiverDirection(HexDirection direction)
    {
        RiverDirection river = GetRiverDirection(direction);
        return river == RiverDirection.Incoming;
    }

    // 判断指定的边是否有河流
    public bool IsOutgoingRiverDirection(HexDirection direction)
    {
        RiverDirection river = GetRiverDirection(direction);
        return river == RiverDirection.Outgoing;
    }

    // 取消某方向河流
    public void ClearRiver(HexDirection direction)
    {
        // 清除自身的河流
        rivers[(int)direction] = RiverDirection.Null;

        // 清除邻居相应的河流
        HexCell neighbor = GetNeighbor(direction);
        if (neighbor)
        {
            neighbor.rivers[(int)direction.Opposite()] = RiverDirection.Null;
        }
    }

    //public River GetRiver(HexDirection direction)
    //{
    //    if (riverCell != null)
    //    {
    //        return riverCell.GetRiver(direction);
    //    }

    //    return null;
    //}


    public bool IsLakes()
    {
        return IsUnderwater; //  terrainType == HexTerrainType.Water;
    }

    bool IsValidRiverDestination(HexCell neighbor)
    {
        return neighbor && (
            elevation >= neighbor.elevation || lakesLevel == neighbor.elevation
        );
    }


    /***
     * 
     * 
     *  道路
     * 
     * 
     */


    // 判断是否有路
    public bool HasRoadThroughEdge(HexDirection direction)
    {
        return roads[(int)direction];
    }

    public bool HasRoads
    {
        get
        {
            for (int i = 0; i < roads.Length; i++)
            {
                if (roads[i])
                {
                    return true;
                }
            }
            return false;
        }
    }

    

    public void AddRoad(HexDirection direction)
    {
        // 没有路， 不是河流，且不是悬崖
        if (!roads[(int)direction] && !HasRiverThroughEdge(direction) &&
            GetElevationDifference(direction) <= 1)
        {
            SetRoad((int)direction, true);
        }
    }

    public void RemoveRoads()
    {
        for (int i = 0; i < neighbors.Length; i++)
        {
            if (roads[i])
            {
                SetRoad(i, false);
            }
        }
    }

    void SetRoad(int index, bool state)
    {
        roads[index] = state;
        neighbors[index].roads[(int)((HexDirection)index).Opposite()] = state;

        // neighbors[index].RefreshOnlySelf();
        // RefreshOnlySelf();
    }



    // 获取高度差
    public int GetElevationDifference(HexDirection direction)
    {
        int difference = elevation - GetNeighbor(direction).elevation;
        return difference >= 0 ? difference : -difference;
    }


    /// <summary>
    /// 判断能否建筑这个建筑物
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool CanBuild(BuildType type)
    {
        bool res = false;
        switch (type)
        {
            case BuildType.City:
                res = CanBuildCity();
                break;
            case BuildType.Farm:
                res = CanBuildFarm();
                break;
        }

        return res;
    }

    bool CanBuildCity()
    {
        if(IsUnderwater || TerrainType == HexTerrainType.Ridge)
        {
            return false;
        }

        if(city != null)
        {
            return false;
        }

        for (HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++)
        {
            HexCell neighbor = GetNeighbor(dir);
            if (neighbor && neighbor.city != null)
            {
                return false;
            }
        }

        return true;
    }

    bool CanBuildFarm()
    {
        if (IsUnderwater || TerrainType == HexTerrainType.Ridge)
        {
            Debug.Log("11111111111111");
            return false;
        }

        if ( build != null)
        {
            Debug.Log("222222222222");
            return false;
        }

        // 不能在冰和沙漠上建造
        if(terrainType == HexTerrainType.Snow || terrainType == HexTerrainType.Desert)
        {
            Debug.Log("333333333333");
            return false;
        }

        return true;
    }

    LinkedList<Color> highLightColors = new LinkedList<Color>();

    /// <summary>
    /// 关闭高亮
    /// </summary>
    public void DisableHighlight()
    {
        if(highLightColors.Count > 0)
        {
            highLightColors.RemoveLast();
        }

        Image highlight = uiRect.GetChild(0).GetComponent<Image>();
        if (highLightColors.Count > 0)
        {
            highlight.color = highLightColors.Last.Value;
            highlight.enabled = true;
        }
        else
        {
            highlight.enabled = false;
        }
    }

    /// <summary>
    /// 打开高亮
    /// </summary>
    public void EnableHighlight(Color color)
    {
        highLightColors.AddLast(color);

        Image highlight = uiRect.GetChild(0).GetComponent<Image>();
        highlight.color = color;
        highlight.enabled = true;
    }

    public void SetLabel(string text)
    {
        UnityEngine.UI.Text label = uiRect.GetComponent<Text>();
        label.text = text;
    }

    public void SetLabelDir(float angle)
    {
        UnityEngine.UI.Text label = uiRect.GetComponent<Text>();
        //label.text = "个";

        label.rectTransform.localEulerAngles = new Vector3(0, 0, angle);

        //label.rectTransform.Rotate(new Vector3(0, 0, angle));
    }

    public int GetDistanceCost(HexCell neighbor)
    {
        HexEdgeType edgeType = GetEdgeType(neighbor);
        if (edgeType == HexEdgeType.Cliff)
        {
            return -1;
        }

        // 不能入海
        if (neighbor.IsUnderwater)
        {
            return -1;
        }

        int moveCost = 0;
        {
            moveCost += edgeType == HexEdgeType.Flat ? 5 : 10;

            // 不同类型地块额外增加消耗
            moveCost += neighbor.TerrainType.Distance();

            // 有河流在额外加3
            moveCost += neighbor.HasRiver() ? 3 : 0;

        }

        return moveCost;
    }

    // 判断地块显示还是暗
    public void IncreaseVisibility()
    {
        visibility += 1;
        if (visibility == 1)
        {
            IsExplored = true;
            ShaderData.RefreshVisibility(this);
        }

    }

    public void DecreaseVisibility()
    {
        visibility -= 1;
        if (visibility == 0)
        {
            ShaderData.RefreshVisibility(this);
        }

    }

    public void ResetVisibility()
    {
        if (visibility > 0)
        {
            visibility = 0;
            ShaderData.RefreshVisibility(this);
        }
    }


    public bool HasWall(HexDirection dir)
    {
        int i = (int)dir;
        short b = (short)Mathf.Pow(2, i);
        
        return (walls & b) != 0;
    }

    public void SetWalls(bool[] v)
    {
        walls = 0;
        for (int i = 0; i < v.Length; i++)
        {
            walls = walls << 1;
            if (v[i] == true)
            {
                walls = walls | 1;
            }
        }

        Debug.Log(walls);
    }


    public void SetMapData(float data)
    {
        ShaderData.SetMapData(this, data);
    }



    // 地面变化
    public void NextRound()
    {
        // 其上的建筑变化
        if(build != null)
        {
            switch (build.BuildType)
            {
                case BuildType.City:
                    ((City)build).NextRound();
                    break;
                case BuildType.Farm:
                    ((Farm)build).NextRound();
                    break;
                default:
                    break;
            }
        }
    }

    public void LaterNextRound()
    {
       
    }


    public Vector3 GetBoundaryCenterPos()
    {
        if (IsUnderwater)
        {
            Vector3 v = transform.localPosition;
            v.y = LakesSurfaceY;
            return v;
        }
        else
        {
            return transform.localPosition;
        }
    }

    public bool IsCityBoundary(HexDirection dir, out float width)
    {
        HexCell nei = GetNeighbor(dir);

        if(city != null)
        {
            // 不属于同一个城市
            if(nei == null || city != nei.city)
            {
                width = 1f;

                // 属于同一个阵营
                if (nei != null && nei.city != null
                    && nei.city.camp.GetId() == city.camp.GetId())
                {
                    width = width / 2;
                }

                return true;
            }
        }

        width = 0;
        return false;
    }
}
