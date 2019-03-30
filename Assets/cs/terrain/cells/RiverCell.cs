using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


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
    RiverDirection[] rivers;

    public RiverCell()
    {
        rivers = new RiverDirection[HexMetrics.HexTrianglesNum];
    }

    public void Save()
    {

    }

    public void Load()
    {

    }

    // 是否后河流
    public bool HasRiver()
    {
        for(HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++)
        {
            if(GetRiverDirection(dir) != RiverDirection.Null)
            {
                return true;
            }
        }
        return false;
    }

    // 添加流出河流
    public void SetOutgoingRiver(HexDirection direction)
    {
        rivers[(int)direction] = RiverDirection.Outgoing;
    }

    // 添加流入河流
    public void SetIncomingRiver(HexDirection direction)
    {
        rivers[(int)direction] = RiverDirection.Incoming;
    }

    public RiverDirection GetRiverDirection(HexDirection direction)
    {
        return rivers[(int)direction];
    }

    // 获取所有流入的河流的总流量
    public float getAllIncomingFlow()
    {
        float flow = 0f;
        //for (HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++)
        //{
        //    RiverDirection river = GetRiverDirection(dir);
        //    if (river != null && river == RiverDirection.Incoming)
        //    {
                
        //    }
        //}

        return flow;
    }

    // 判断是河流的开始和结束
    public bool HasRiverBeginOrEnd()
    {
        bool hasIn = false;
        bool hasOut = false;
        for (HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++)
        {
            RiverDirection river = GetRiverDirection(dir);
            if (river != RiverDirection.Null)
            {
                if(river == RiverDirection.Incoming)
                {
                    hasIn = true;
                }
                else if(river == RiverDirection.Outgoing)
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
            RiverDirection river = GetRiverDirection(dir);
            if (river != RiverDirection.Null)
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
            RiverDirection river = GetRiverDirection(dir);
            if (river == direction)
            {
                dirs.Add(dir);
            }
        }

        return dirs;
    }

    public void ClearRiver(HexDirection direction)
    {
        rivers[(int)direction] = RiverDirection.Null;
    }
}
