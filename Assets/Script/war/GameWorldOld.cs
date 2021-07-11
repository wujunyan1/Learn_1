using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏世界
/// </summary>
public class GameWorldOLD :  MonoBehaviour
{


    public WarMap map;

    // 判断 包围半径
    public void TagObstaclesWithInViewRange(MovingEntity entity, float length)
    {
        Vector2 m_pos = entity.GetPos();

        List<BaseGameEntity> objstacles = map.getObstacles();

        foreach(BaseGameEntity e in objstacles)
        {
            // 距离 小于 包围盒距离 + 半径
            if (Vector2.Distance(e.GetPos(), m_pos) < length + e.GetRadius())
            {
                e.SetTag(true);
            }
        }
    }

    public void ResetTag()
    {
        List<BaseGameEntity> objstacles = map.getObstacles();

        foreach (BaseGameEntity e in objstacles)
        {
            e.SetTag(false);
        }
    }

    public BPolygon GetBoundary()
    {
        return map.GetBoundary();
    }
}
