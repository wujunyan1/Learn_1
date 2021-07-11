using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionPH
{
    public CollisionType collisionType;
    public Vector3 pos;
    public Coordinate2 coordinate;
    public float briefnessBrradius;
}

// 碰撞立方体
public class CollisionCube : CollisionPH
{
    public float Length;
    public float Width;
    public float Height;

    public CollisionCube(float length, float width, float height)
    {
        collisionType = CollisionType.CUBE;
        Length = length;
        Width = width;
        Height = height;

        briefnessBrradius = Mathf.Sqrt((Width * Width) + (Length * Length));
    }
}

// 碰撞圆柱体
public class CollisionCylinder : CollisionPH
{
    public float Bradius;
    public float Height;

    public CollisionCylinder(float bradius, float height)
    {
        collisionType = CollisionType.CYLINDER;
        Bradius = bradius;
        Height = height;

        briefnessBrradius = bradius;
    }
}

// 碰撞点
public class CollisionPoint : CollisionPH
{
    public CollisionPoint()
    {
        collisionType = CollisionType.POINT;
        briefnessBrradius = 0f;
    }
}

public class CollisionDetection
{
    protected string name = "";
    public void PrintName()
    {
        Debug.Log(name);
    }

    /// <summary>
    /// 判断线段是否与矩形相交
    /// </summary>
    /// <param name="line"></param>
    /// <param name="one"></param>
    /// <returns></returns>
    public static bool IsLineToCubeCollision(Line line, CollisionCube one, out Vector2 pos)
    {
        Coordinate2 coordinate2 = one.coordinate;
        Vector2 start = Vector2Tool.PointToLocalSpace(line.GetFirst(), coordinate2);
        Vector2 end = Vector2Tool.PointToLocalSpace(line.GetSecond(), coordinate2);

        Line n_line = new Line(start, end);

        float o_l = one.Length;
        float o_w = one.Width;

        // 点在矩形里面则直接返回， 碰撞点为他的中心
        if (start.x <= o_l && start.x >= -o_l
            && start.y <= o_w && start.y >= -o_w)
        {
            pos = line.GetFirst();
            return true;
        }

        if (end.x <= o_l && end.x >= -o_l
            && end.y <= o_w && end.y >= -o_w)
        {
            pos = line.GetSecond();
            return true;
        }

        // one的4个角坐标
        Vector2 o_lu = new Vector2(-one.Length, one.Width);
        Vector2 o_ru = new Vector2(one.Length, one.Width);
        Vector2 o_ld = new Vector2(-one.Length, -one.Width);
        Vector2 o_rd = new Vector2(one.Length, -one.Width);

        // 0,1,2,3 上下左右
        Line[] lines = new Line[4];

        // 只检查右方线段即可
        if (start.x > 0)
        {
            // 检查右边
            lines[3] = new Line(o_rd, o_ru);
        }
        else
        {
            lines[2] = new Line(o_ld, o_lu);
        }

        // 下方
        if (start.y < 0)
        {
            lines[1] = new Line(o_ld, o_rd);
        }
        else
        {
            lines[0] = new Line(o_rd, o_rd);
        }

        // 只检查右方线段即可
        if (end.x > 0)
        {
            // 检查右边
            lines[3] = new Line(o_rd, o_ru);
        }
        else
        {
            lines[2] = new Line(o_ld, o_lu);
        }

        // 下方
        if (end.y < 0)
        {
            lines[1] = new Line(o_ld, o_rd);
        }
        else
        {
            lines[0] = new Line(o_rd, o_rd);
        }

        bool isInter = false;
        pos = Vector2.zero;
        Vector2 local_pos;
        float min_dis = float.MaxValue;
        // 判断线段相交
        for (int i = 0; i < 4; i++)
        {
            Line l = lines[i];
            if (l != null)
            {
                if (Line.LineIntersection2D(n_line, l, out local_pos))
                {
                    if (local_pos.magnitude < min_dis)
                    {
                        min_dis = local_pos.magnitude;
                        pos = local_pos;
                        isInter = true;
                    }
                }
            }
        }

        return isInter;
    }


    /// <summary>
    /// 判断点是否在矩形内
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="one"></param>
    /// <returns></returns>
    public static bool IsPointToCubeCollision(Vector3 pos, CollisionCube one)
    {
        Coordinate2 coordinate = one.coordinate;
        Vector2 v2_pos = Vector3Tool.ToVector2(pos);

        Vector2 local_v2_pos = Vector2Tool.PointToLocalSpace(v2_pos, coordinate);

        if (local_v2_pos.x <= one.Length && local_v2_pos.x >= -one.Length
            && local_v2_pos.y <= one.Width && local_v2_pos.y >= -one.Width)
        {
            return true;
        }

        return false;

    }



    public virtual bool Detection(CollisionPH one, CollisionPH other, out Vector3 pos)
    {
        pos = Vector3.zero;
        return false;
    }
}

/// <summary>
/// 立方体 立方体
/// </summary>
public class CollisionDetectionCubeTCube : CollisionDetection
{
    public CollisionDetectionCubeTCube()
    {
        name = "CollisionDetectionCubeTCube";
    }

    /// <summary>
    /// 立方体和立方体的碰撞 one 去检测 other
    /// </summary>
    /// <param name="one"></param>
    /// <param name="other"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public override bool Detection(CollisionPH one_ph, CollisionPH other_ph, out Vector3 pos)
    {
        CollisionCube one = (CollisionCube)one_ph;
        CollisionCube other = (CollisionCube)other_ph;

        Coordinate2 coordinate2 = one.coordinate;
        Vector2 otherWorldPos = Vector3Tool.ToVector2(other.pos);

        // other 在 one 的 局部坐标系里
        Vector2 otherPos = Vector2Tool.PointToLocalSpace(otherWorldPos, coordinate2);
        

        float o_l = one.Length;
        float o_w = one.Width;

        // 中心点在矩形里面则直接返回， 碰撞点为他的中心
        if (otherPos.x <= o_l && otherPos.x >= -o_l
            && otherPos.y <= o_w && otherPos.y >= -o_w)
        {
            pos = other.pos;
            return true;
        }

        // other 在 one的后方，则直接不检测 不对自己的后方检测，
        // 这个交给 other 检测 one的时候
        if (otherPos.x < -o_l)
        {
            pos = Vector3.zero;
            return false;
        }

        // one的4个角转化为世界坐标
        Vector2 o_lu = Vector2Tool.PointToWorldSpace(new Vector2(-one.Length, one.Width), coordinate2);
        Vector2 o_ru = Vector2Tool.PointToWorldSpace(new Vector2(one.Length, one.Width), coordinate2);
        Vector2 o_ld = Vector2Tool.PointToWorldSpace(new Vector2(-one.Length, -one.Width), coordinate2);
        Vector2 o_rd = Vector2Tool.PointToWorldSpace(new Vector2(one.Length, -one.Width), coordinate2);

        // 2个one上靠近other的线段
        Line one1 = otherPos.y > 0 ? new Line(o_lu, o_ru) : new Line(o_ld, o_rd);
        Line one2 = otherPos.x > 0 ? new Line(o_rd, o_ru) : new Line(o_ld, o_lu);

        // 分别判断这2个线段是否与矩形相交就行
        pos = Vector3.zero;
        bool isCollision = false;
        Vector2 out1 = Vector2.zero;
        if (IsLineToCubeCollision(one1, other, out out1))
        {
            pos = (pos + Vector2Tool.ToVector3(out1)) / 2;
            isCollision = true;
        }

        Vector2 out2 = Vector2.zero;
        if (IsLineToCubeCollision(one2, other, out out2))
        {
            pos = (pos + Vector2Tool.ToVector3(out2)) / 2;
            isCollision = true;
        }

        return isCollision;
    }
}

/// <summary>
/// 立方体 圆柱 
/// </summary>
public class CollisionDetectionCubeTCylinder : CollisionDetection
{
    public CollisionDetectionCubeTCylinder()
    {
        name = "CollisionDetectionCubeTCylinder";
    }

    /// <summary>
    /// 立方体和圆柱的碰撞
    /// 参考 https://www.cnblogs.com/lijiajia/p/6718812.html
    /// </summary>
    /// <param name="one"></param>
    /// <param name="other"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public override bool Detection(CollisionPH one_ph, CollisionPH other_ph, out Vector3 pos)
    {
        CollisionCube cube;
        CollisionCylinder cylinder;

        // 如果one 是圆柱的话
        if (one_ph.collisionType == CollisionType.CYLINDER)
        {
            cube = (CollisionCube)other_ph;
            cylinder = (CollisionCylinder)one_ph;
        }
        else
        {
            cube = (CollisionCube)one_ph;
            cylinder = (CollisionCylinder)other_ph;
        }

        Vector3 targetDir = other_ph.pos - one_ph.pos;
        //Vector3 forward = one.transform.TransformDirection(Vector3.forward);

        Vector2 v2_targetDir = Vector3Tool.ToVector2(targetDir);

        Coordinate2 one_ph_coordinate = one_ph.coordinate;
        Vector2 v2_forward = one_ph_coordinate.X; // Vector3Tool.ToVector2(forward);

        // * 大于0 则是正面  -90 ` 90
        float result = Vector2.Dot(v2_forward, v2_targetDir);

        // other 在 one的后方，则直接不检测 不对自己的后方检测，
        // 这个交给 other 检测 one的时候
        if (result < 0)
        {
            pos = Vector3.zero;

            Debug.Log("false 1111111111");
            return false;
        }
        

        Debug.Log("-------------");
        Debug.Log(string.Format("{0} {1} {2}", cube.Length, cube.Width, cube.Height));
        Debug.Log(string.Format("{0}", cylinder.Bradius));

        Coordinate2 coordinate2 = cube.coordinate;
        Debug.Log(coordinate2.ToString());

        Vector2 otherWorldPos = Vector3Tool.ToVector2(cylinder.pos);

        Debug.Log(otherWorldPos);
        Debug.Log(cube.pos);

        // other 在 one 的 局部坐标系里
        Vector2 otherPos = Vector2Tool.PointToLocalSpace(otherWorldPos, coordinate2);

        Debug.Log(otherPos);

        // 因为是矩形转化为正值是一样的
        float p_x = otherPos.x < 0 ? -otherPos.x : otherPos.x;
        float p_y = otherPos.y < 0 ? -otherPos.y : otherPos.y;

        Vector2 v = new Vector2(p_x, p_y);
        Vector2 h = new Vector2(cube.Length, cube.Width);

        Vector2 _u = v - h;
        _u.x = _u.x < 0 ? 0 : _u.x;
        _u.y = _u.y < 0 ? 0 : _u.y;

        Vector2 intel = new Vector2(cube.Length, cube.Width);

        // 有交点
        if (_u.magnitude <= cylinder.Bradius)
        {
            // 交点的建议计算
            if (p_x < cube.Length)
            {
                intel = (v * (cube.Width / cube.Width + cylinder.Bradius));
            }

            if (p_y < cube.Width)
            {
                intel = (v * (cube.Length / cube.Length + cylinder.Bradius));
            }

            intel.x = otherPos.x < 0 ? -intel.x : intel.x;
            intel.y = otherPos.y < 0 ? -intel.y : intel.y;

            Vector2 p = Vector2Tool.PointToWorldSpace(intel, coordinate2);
            pos = Vector2Tool.ToVector3(p);

            Debug.Log("true");
            Debug.Log(pos);
            return true;
        }

        Debug.Log("false 222222222222222");
        pos = Vector3.zero;
        return false;
    }
}

/// <summary>
/// 圆柱 圆柱 
/// </summary>
public class CollisionDetectionCylinderTCylinder : CollisionDetection
{
    public CollisionDetectionCylinderTCylinder()
    {
        name = "CollisionDetectionCylinderTCylinder";
    }

    /// <summary>
    /// 圆柱和圆柱的碰撞
    /// 简易计算就是2个圆
    /// </summary>
    /// <param name="one"></param>
    /// <param name="other"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public override bool Detection(CollisionPH one_ph, CollisionPH other_ph, out Vector3 pos)
    {
        CollisionCylinder one = (CollisionCylinder)one_ph;
        CollisionCylinder other = (CollisionCylinder)other_ph;

        Vector3 v = other.pos - one.pos;

        v = v * (one.Bradius / (one.Bradius + other.Bradius));

        pos = one.pos + v;

        Debug.Log(pos);

        return true;
    }
}

/// <summary>
/// 圆柱 点 
/// </summary>
public class CollisionDetectionCylinderTPoint : CollisionDetection
{
    public CollisionDetectionCylinderTPoint()
    {
        name = "CollisionDetectionCylinderTPoint";
    }

    public override bool Detection(CollisionPH one_ph, CollisionPH other_ph, out Vector3 pos)
    {
        CollisionCylinder cylinder;
        CollisionPoint point;
        if(one_ph.collisionType == CollisionType.CYLINDER)
        {
            cylinder = (CollisionCylinder)one_ph;
            point = (CollisionPoint)other_ph;
        }
        else
        {
            cylinder = (CollisionCylinder)other_ph;
            point = (CollisionPoint)one_ph;
        }

        Vector3 v = point.pos - cylinder.pos;

        if(v.y > cylinder.Height)
        {
            pos = Vector3.zero;
            return false;
        }

        if (v.y < 0)
        {
            pos = Vector3.zero;
            return false;
        }

        pos = point.pos;
        return true;
    }
}

/// <summary>
/// 立方体 点
/// </summary>
public class CollisionDetectionCubeTPoint : CollisionDetection
{
    public CollisionDetectionCubeTPoint()
    {
        name = "CollisionDetectionCubeTPoint";
    }

    public override bool Detection(CollisionPH one_ph, CollisionPH other_ph, out Vector3 pos)
    {
        CollisionCube cube;
        CollisionPoint point;
        if (one_ph.collisionType == CollisionType.CUBE)
        {
            cube = (CollisionCube)one_ph;
            point = (CollisionPoint)other_ph;
        }
        else
        {
            cube = (CollisionCube)other_ph;
            point = (CollisionPoint)one_ph;
        }

        Vector3 v = point.pos - cube.pos;
        if (v.y > cube.Height)
        {
            pos = Vector3.zero;
            return false;
        }

        if (v.y < 0)
        {
            pos = Vector3.zero;
            return false;
        }
        
        Coordinate2 coordinate = cube.coordinate;
        Vector2 v2_pos = Vector3Tool.ToVector2(point.pos);

        Vector2 local_v2_pos = Vector2Tool.PointToLocalSpace(v2_pos, coordinate);

        if (local_v2_pos.x <= cube.Length && local_v2_pos.x >= -cube.Length
            && local_v2_pos.y <= cube.Width && local_v2_pos.y >= -cube.Width)
        {
            pos = point.pos;
            return true;
        }

        pos = Vector3.zero;
        return false;
    }
}

/// <summary>
/// 点 点 
/// </summary>
public class CollisionDetectionPointTPoint : CollisionDetection
{
    public CollisionDetectionPointTPoint()
    {
        name = "CollisionDetectionPointTPoint";
    }

    public override bool Detection(CollisionPH one, CollisionPH other, out Vector3 pos)
    {
        Vector3 v = other.pos - one.pos;

        if(v.magnitude < 0.0001f)
        {
            pos = other.pos;
            return true;
        }
        pos = Vector3.zero;
        return false;
    }
}
