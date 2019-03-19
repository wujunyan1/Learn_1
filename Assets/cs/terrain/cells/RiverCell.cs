using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RiverDirection
{
    Incoming,
    Outgoing
}

public class River
{
    public RiverDirection direction;
    public float flow;

    public River(RiverDirection direction, float flow)
    {
        this.direction = direction;
        this.flow = flow;
    }
}

public class RiverCell
{
    River[] rivers;

    public RiverCell()
    {
        rivers = new River[HexMetrics.HexTrianglesNum];
    }

    // 是否后河流
    public bool HasRiver()
    {
        for(HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++)
        {
            if(GetRiver(dir) != null)
            {
                return true;
            }
        }
        return false;
    }

    // 添加流出河流
    public void SetOutgoingRiver(HexDirection direction, float flow)
    {
        //Debug.Log("------SetOutgoingRiver---------");
        // 有原来的河流 则合并水流量流出
        float baseFlow = 0f;
        if(rivers[(int)direction] != null)
        {
            baseFlow = rivers[(int)direction].flow;
        }
        rivers[(int)direction] = new River(RiverDirection.Outgoing, flow);
    }

    // 添加流入河流
    public void SetIncomingRiver(HexDirection direction, float flow)
    {
        rivers[(int)direction] = new River(RiverDirection.Incoming, flow);
    }

    public River GetRiver(HexDirection direction)
    {
        return rivers[(int)direction];
    }

    // 获取所有流入的河流的总流量
    public float getAllIncomingFlow()
    {
        float flow = 0f;
        for (HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++)
        {
            River river = GetRiver(dir);
            if (river != null && river.direction == RiverDirection.Incoming)
            {
                flow += river.flow;
            }
        }

        return flow;
    }

    // 判断是河流的开始和结束
    public bool HasRiverBeginOrEnd()
    {
        bool hasIn = false;
        bool hasOut = false;
        for (HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++)
        {
            River river = GetRiver(dir);
            if (river != null)
            {
                if(river.direction == RiverDirection.Incoming)
                {
                    hasIn = true;
                }
                else if(river.direction == RiverDirection.Outgoing)
                {
                    hasOut = true;
                }
            }

            if(hasIn && hasOut)
            {
                return false;
            }
        }

        //Debug.Log(string.Format(" hasin {0}",hasIn));
        //Debug.Log(string.Format(" hasOut {0}", hasOut));

        //Debug.Log(string.Format(" hasOut {0}", hasOut));

        if (!hasIn && !hasOut)
        {
            return false;
        }

        return true;
    }

    // 获取河流数量
    public int GetRiverCount()
    {
        int count = 0;
        for (HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++)
        {
            River river = GetRiver(dir);
            if (river != null)
            {
                count++;
            }
        }

        return count;
    }

    // 获取某个流向的所有方向河流
    public List<HexDirection> GetRiverDirections(RiverDirection direction)
    {
        List<HexDirection> dirs = new List<HexDirection>();

        for (HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++)
        {
            River river = GetRiver(dir);
            if (river != null && river.direction == direction)
            {
                dirs.Add(dir);
            }
        }

        return dirs;
    }

    public void ClearRiver(HexDirection direction)
    {
        rivers[(int)direction] = null;
    }
}
