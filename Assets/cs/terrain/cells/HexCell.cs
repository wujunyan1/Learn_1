using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 一个六边形 类
public class HexCell : MonoBehaviour
{
    public int index;

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
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.elevationStep;
            transform.localPosition = position;

            // 修改UI位置
            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = elevation * -HexMetrics.elevationStep;
            uiRect.localPosition = uiPosition;

            if (HexMetrics.elevationHierarchy[elevation] <= HexMetrics.coastline)
            {
                //Debug.Log("++++++++++++");
                terrainType = HexTerrainType.Water;
            }
            else
            {
                terrainType = HexTerrainType.Land;
            }

            // 并非初始赋值时
            if (old_elevation != int.MinValue)
            {
                Refresh();
            }
        }
    }

    // 高度 数据非显示高度
    float height;
    public float Height
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

    // 颜色
    public Color color;

    // 旁边的
    [SerializeField]
    HexCell[] neighbors;

    // 坐标UI
    public RectTransform uiRect;
    public Text label;

    // 地形类型
    public HexTerrainType terrainType = HexTerrainType.Land;

    public HexGridChunk chunk;

    // 降雨量
    public float rain = 0f;

    public float Rain
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
                    cell.rain += (addRain * 0.3f);
                    cell.pondage += (addRain * 0.3f);
                }
            }

            label.text = string.Format("{0}\n{1}", (int)height, (int)rain);
        }
    }

    // 蓄水量
    public float pondage = 0f;
    public float Pondage
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


    // 河流
    public RiverCell riverCell;

    public void Awake()
    {
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
        //if(chunk)
        {
            chunk.Refresh();
        }
    }

    // 
    void RefreshOnlySelf()
    {
        chunk.Refresh( this );
    }





    /**
     *   河流
     * 
     * 
     */
     
    // 增加一条流出的河流
    public void SetOutgoingRiver(HexDirection direction, float flow)
    {
        if (riverCell == null)
        {
            riverCell = new RiverCell();
        }

        //Debug.Log("----rver---------");
        // 流出方向的块更高则不添加
        HexCell neighbor = GetNeighbor(direction);
        if (!neighbor || elevation < neighbor.elevation)
        {
            return;
        }

        //Debug.Log("----riverCell--SetOutgoingRiver---------");
        // 设置流出河流
        riverCell.SetOutgoingRiver(direction, flow);

        // 如果对方不是水域 则添加一条流入河流
        //if(neighbor.terrainType != HexTerrainType.Water)
        //{
            neighbor.SetIncomingRiver(HexDirectionExtensions.Opposite(direction), flow);
        //}
    }

    // 增加一条流入的河流
    public void SetIncomingRiver(HexDirection direction, float flow)
    {
        if (riverCell == null)
        {
            riverCell = new RiverCell();
        }
      
        // 设置流入河流
        riverCell.SetIncomingRiver(direction, flow);
    }

    public bool HasRiver()
    {
        if(riverCell != null)
        {
            return riverCell.HasRiver();
        }

        return false;
    }

    // 判断指定的边是否有河流
    public bool HasRiverThroughEdge(HexDirection direction)
    {
        return riverCell != null && riverCell.GetRiver(direction) != null;
    }

    public bool HasRiverBeginOrEnd()
    {
        // 如果是湖泊，只要有河流就是开头和结束
        if(terrainType == HexTerrainType.Water)
        {
            return riverCell != null && riverCell.GetRiverCount() > 0;
        }
        return riverCell != null && riverCell.HasRiverBeginOrEnd();
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
        if(riverCell != null)
        {
            return riverCell.GetRiverCount();
        }

        return 0;
    }

    public List<HexDirection> GetRiverDirections(RiverDirection riverDirection)
    {
        if (riverCell == null)
        {
            return new List<HexDirection>();
        }

        return riverCell.GetRiverDirections(riverDirection);
    }

    // 河流表面的垂直位置
    public float RiverSurfaceY
    {
        get
        {
            return
                (elevation + HexMetrics.riverSurfaceElevationOffset) *
                HexMetrics.elevationStep;
        }
    }

    // 判断指定的边是否有河流
    public bool IsInComingRiverDirection(HexDirection direction)
    {
        if (riverCell == null)
        {
            return false;
        }
        River river = riverCell.GetRiver(direction);
        if (river != null)
        {
            return river.direction == RiverDirection.Incoming;
        }
        return false;
    }

    // 取消某方向河流
    public void ClearRiver(HexDirection direction)
    {
        // 清除自身的河流
        riverCell.ClearRiver(direction);

        // 清除邻居相应的河流
        HexCell neighbor = GetNeighbor(direction);
        if (neighbor)
        {
            neighbor.riverCell.ClearRiver(direction.Opposite());
        }
    }
}
