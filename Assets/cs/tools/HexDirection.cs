using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum HexDirection
{
    /// <summary>
    /// 北东 0
    /// </summary>
    NE,
    /// <summary>
    /// 东 1
    /// </summary>
    E,
    /// <summary>
    /// 南东 2
    /// </summary>
    SE,
    /// <summary>
    /// 南西 3
    /// </summary>
    SW,
    /// <summary>
    /// 西 4
    /// </summary>
    W,
    /// <summary>
    /// 北西 5
    /// </summary>
    NW
}

public static class HexDirectionExtensions
{
    /// <summary>
    /// 对边
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static HexDirection Opposite(this HexDirection direction)
    {
        return (int)direction < 3 ? (direction + 3) : (direction - 3);
    }
    
    /// <summary>
    /// 上一个邻居
    /// </summary>
    public static HexDirection Previous(this HexDirection direction)
    {
        return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
    }

    /// <summary>
    /// 下一个邻居
    /// </summary>
    public static HexDirection Next(this HexDirection direction)
    {
        return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
    }


    /// <summary>
    /// 上2个邻居
    /// </summary>
    public static HexDirection Previous2(this HexDirection direction)
    {
        return direction.Previous().Previous();
    }

    /// <summary>
    /// 下2个邻居
    /// </summary>
    public static HexDirection Next2(this HexDirection direction)
    {
        return direction.Next().Next();
    }

}

