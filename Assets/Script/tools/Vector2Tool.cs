using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector2Tool
{
    public const float PRECISION = 0.0001f;
    public const int AMPLIFICATION = 1000;
    public const int AMPLIFICATION2 = 1000000;

    public static Vector2 Normalized(Vector2 v)
    {
        Vector2 r = Vector2.zero;

        float l = Magnitude(v);

        if (Mathf.Abs(l) < PRECISION)
        {
            return r;
        }

        int x = Mathf.CeilToInt(v.x * AMPLIFICATION);
        int y = Mathf.CeilToInt(v.y * AMPLIFICATION);

        int L = Mathf.CeilToInt(l * AMPLIFICATION);

        r.x = x * 1.0f / L;
        r.y = y * 1.0f / L;

        return r;
    }

    public static float Magnitude(Vector2 v)
    {
        float n = v.x * v.x * AMPLIFICATION2 + v.y * v.y * AMPLIFICATION2;

        if (Mathf.Abs(n) < 1)
        {
            n = 0.0f;
        }

        return Mathf.Sqrt(n) / AMPLIFICATION;
    }

    public static Vector3 ToVector3(Vector2 v)
    {
        return new Vector3(v.x, 0, v.y);
    }

    /// <summary>
    /// 使 v 的长度不会超过 num
    /// </summary>
    /// <param name="v"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    public static Vector2 Truncate(Vector2 v,  float num)
    {
        if (v.magnitude > num)
        {
            return Normalized(v) * num;
        }

        return v;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static Vector2 Perp(Vector2 v)
    {
        return new Vector2(-v.y, v.x);
    }

    /// <summary>
    /// 将局部坐标系转为世界坐标系
    /// </summary>
    /// <param name="targetLocal">局部坐标系位置</param>
    /// <param name="x">局部坐标系X</param>
    /// <param name="y">局部坐标系Y</param>
    /// <returns></returns>
    public static Vector2 VectorToWorldSpace(Vector2 targetLocal, Vector2 x, Vector2 y)
    {
        return x.normalized * targetLocal.x + y.normalized * targetLocal.y;
    }

    public static Vector2 PointToWorldSpace(Vector2 targetLocal, Vector2 x, Vector2 y, Vector2 pos)
    {
        return VectorToWorldSpace(targetLocal, x, y) + pos;
    }

    public static Vector2 PointToWorldSpace(Vector2 targetLocal, Coordinate2 coordinate)
    {
        return VectorToWorldSpace(targetLocal, coordinate.X, coordinate.Y) + coordinate.getLocalPoint();
    }

    /// <summary>
    /// 将世界坐标系转为局部坐标系
    /// </summary>
    /// <param name="targetWorld">需要转化的坐标</param>
    /// <param name="x">局部坐标x方向</param>
    /// <param name="y">局部坐标y方向</param>
    /// <param name="pos">局部坐标原点</param>
    /// <returns></returns>
    public static Vector2 PointToLocalSpace(Vector2 targetWorld, Vector2 x, Vector2 y, Vector2 pos)
    {
        Vector2 targetPos = targetWorld - pos;

        //Vector2 nx = targetPos / x.normalized;
        //Vector2 ny = targetPos / y.normalized;

        float localX = Vector2.Dot(targetPos, x) / x.magnitude;
        float localY = Vector2.Dot(targetPos, y) / y.magnitude;

        return new Vector2(localX, localY);
    }

    public static Vector2 PointToLocalSpace(Vector2 targetWorld, Coordinate2 coordinate)
    {
        return PointToLocalSpace(targetWorld, coordinate.X, coordinate.Y, coordinate.getLocalPoint());
    }

    
    public static Vector2 DirToLocalSpace(Vector2 targetDir, Coordinate2 coordinate)
    {
        Vector2 x = coordinate.X;
        Vector2 y = coordinate.Y;

        float localX = Vector2.Dot(targetDir, x) / x.magnitude;
        float localY = Vector2.Dot(targetDir, y) / y.magnitude;

        return new Vector2(localX, localY);

    }

    /// <summary>
    /// 坐标系夹角60度
    /// x 在 y的顺时针
    /// x,y 都为单位向量
    /// </summary>
    /// <param name="targetDir"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static Vector2 DirToLocalSpace60(Vector2 targetDir, Vector2 x, Vector2 y)
    {
        // 制作成 x _y的坐标系
        Vector2 _y = Vector2.Perpendicular(x);

        Vector2 _t = PointToLocalSpace(targetDir, x, _y, Vector2.zero);

        float n = _t.y / 0.15425f;
        float m = _t.x - n * 0.5f;

        return new Vector2(n, m);
    }



    public static Vector2 DirToLocalSpaceNo90(Vector2 targetDir, Vector2 x, Vector2 y)
    {
        float Xa = x.x;
        float Ya = x.y;

        float Xb = y.x;
        float Yb = y.y;

        float X = targetDir.x;
        float Y = targetDir.y;

        float fm_1 = (Yb * Xa - Xb * Ya);
        if(fm_1 == 0)
        {

        }


        float n = (Xa * Y - Ya * X) / (Yb * Xa - Xb * Ya);
        float m = (X - n * Xa) / Xb;

        return new Vector2(n, m);

    }

    /// <summary>
    /// V 投影到 dir方向上的长度
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public static float DirLengthByV(Vector2 dir, Vector2 v)
    {
        return Vector2.Dot(dir, v) * v.magnitude;
    }


    public static float Angle(Vector2 from, Vector2 to, bool isClockwise = true)
    {
        float angle = Vector2.SignedAngle(from, to);
        
        if(angle < 0)
        {
            angle = -angle;
        }
        else
        {
            angle = 360 - angle;
        }

        if (!isClockwise)
        {
            angle = 360 - angle;
        }

        angle = angle < 0 ? angle + 360 : angle;
        angle = angle >= 360 ? angle - 360 : angle;

        return angle;
    }
}
