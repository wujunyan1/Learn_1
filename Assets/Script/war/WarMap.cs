using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 战斗地图
/// </summary>
[Serializable]
public class WarMap
{
    /// <summary>
    /// 地图上的圆形障碍物
    /// </summary>
    [SerializeField]
    List<BaseGameEntity> obstacles;

    /// <summary>
    /// 地图上的多边形
    /// </summary>
    [SerializeField]
    List<BPolygon> bPolygons;

    [SerializeField]
    BPolygon boundary;

    public WarMap()
    {
        obstacles = new List<BaseGameEntity>();
    }

    public List<BaseGameEntity> getObstacles()
    {
        return obstacles;
    }

    public BPolygon GetBoundary()
    {
        return boundary;
    }
}
