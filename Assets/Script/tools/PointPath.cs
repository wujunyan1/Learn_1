using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointPath
{
    List<Vector2> path;
    int currPoint = 0;
    bool isMoveEnd;

    bool isLoop;

    public PointPath(bool isLoop = false)
    {
        path = new List<Vector2>();
        currPoint = 0;
        this.isLoop = isLoop;
        isMoveEnd = false;
    }

    public void AddNextPoint(Vector2 v)
    {
        path.Add(v);
    }

    public bool IsFinished()
    {
        return isMoveEnd;
    }

    public Vector2 GetCurrWayPoint()
    {
        if (!this.IsFinished())
        {
            return path[currPoint];
        }
        return path[path.Count - 1];
    }

    public void SetNextWayPoint()
    {
        currPoint++;
        // 移动到最后
        if(currPoint >= path.Count)
        {
            //是循环路径
            if (isLoop)
            {
                // 返回到初始位置
                currPoint = 0;
            }
            else
            {
                isMoveEnd = true;
            }
        }
    }
}
