using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public enum RiverDirection
{
    Null,
    Incoming,
    Outgoing
}

// 一个六边形 类
public class HexCell : MonoBehaviour
{
    public int index;
    public int chunkIndex;

    // 坐标
    public HexCoordinates coordinates;

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
            //value = 1;
            int old_elevation = elevation;
            elevation = value;
            //elevation = 1;

            // 修改垂直坐标
            RefreshPosition();
            
            if (HexMetrics.elevationHierarchy[elevation] <= HexMetrics.coastline)
            {
                //Debug.Log("++++++++++++");
                terrainType = HexTerrainType.Water;
                LakesLevel = elevation + 1;
            }
            else
            {
                //terrainType = HexTerrainType.Land;
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
    public City city;

    // 人物
    public PersonControl Person { get; set; }

    public HexGridChunk chunk;

    // 降雨量
    public int rain = 0;
    public int Rain
    {
        get
        {
            return rain;
        }
        set
        {
            float addRain = value;
            rain = rain + value;

            pondage = pondage + value;

            foreach (HexCell cell in neighbors)
            {
                if(cell)
                {
                    cell.rain += (int)(addRain * 0.3f);
                    cell.pondage += (int)(addRain * 0.3f);
                }
            }

            
        }
    }

    // 蓄水量
    public int pondage = 0;
    public int Pondage
    {
        get
        {
            return pondage;
        }
        set
        {
            pondage = value;
        }
    }
    
    // 湖泊高度
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
            lakesLevel = value;
            Refresh();
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

    public void Awake()
    {
        rivers = new RiverDirection[HexMetrics.HexTrianglesNum];

        //DisableHighlight();
    }

    public void Start()
    {
        DisableHighlight();
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write((byte)terrainType);
        writer.Write((byte)elevation);
        writer.Write((byte)height);
        writer.Write((byte)rain);
        writer.Write((byte)pondage);
        writer.Write((byte)lakesLevel);
        writer.Write(store);

        for (int i = 0; i < rivers.Length; i++)
        {
            writer.Write((byte)rivers[i]);
        }

        //MapBuildFactory.Save(build, writer);
    }

    public void Load(BinaryReader reader)
    {
        terrainType = (HexTerrainType)reader.ReadByte();
        elevation = reader.ReadByte();
        RefreshPosition();
        height = reader.ReadByte();
        rain = reader.ReadByte();
        pondage = reader.ReadByte();
        lakesLevel = reader.ReadByte();
        store = reader.ReadInt32();

        for (int i = 0; i < rivers.Length; i++)
        {
            rivers[i] = (RiverDirection)reader.ReadByte();
        }

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
    void Refresh()
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
    void RefreshOnlySelf()
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
     
    // 增加一条流出的河流
    public void SetOutgoingRiver(HexDirection direction, float flow)
    {
        
        HexCell neighbor = GetNeighbor(direction);

        // 设置流出河流
        rivers[(int)direction] = RiverDirection.Outgoing;
        if (neighbor)
        {
            neighbor.SetIncomingRiver(HexDirectionExtensions.Opposite(direction), flow);
        }
    }

    // 增加一条流入的河流
    public void SetIncomingRiver(HexDirection direction, float flow)
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
            if (river != RiverDirection.Null)
            {
                if (river == RiverDirection.Incoming)
                {
                    hasIn = true;
                }
                else if (river == RiverDirection.Outgoing)
                {
                    hasOut = true;
                }
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
        return terrainType == HexTerrainType.Water;
    }

    bool IsValidRiverDestination(HexCell neighbor)
    {
        return neighbor && (
            elevation >= neighbor.elevation || lakesLevel == neighbor.elevation
        );
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
        }

        return res;
    }

    bool CanBuildCity()
    {
        if(TerrainType == HexTerrainType.Water || TerrainType == HexTerrainType.Ridge)
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

    /// <summary>
    /// 是否高亮
    /// </summary>
    public void DisableHighlight()
    {
        Image highlight = uiRect.GetChild(0).GetComponent<Image>();
        highlight.enabled = false;
    }

    /// <summary>
    /// 是否高亮
    /// </summary>
    public void EnableHighlight(Color color)
    {
        Image highlight = uiRect.GetChild(0).GetComponent<Image>();
        highlight.color = color;
        highlight.enabled = true;
    }

    public void SetLabel(string text)
    {
        UnityEngine.UI.Text label = uiRect.GetComponent<Text>();
        label.text = text;
    }
    

    public int GetDistanceCost(HexCell neighbor)
    {
        HexEdgeType edgeType = GetEdgeType(neighbor);
        if (edgeType == HexEdgeType.Cliff)
        {
            return int.MaxValue;
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
}
