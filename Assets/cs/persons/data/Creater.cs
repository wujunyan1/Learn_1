using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 建造者
/// </summary>
public class Creater : Person
{
    // 坐标
    public Point point;

    public Camp camp;

    public Creater(Point point)
    {
        personName = "建造者";
        this.point = point;
    }
}
