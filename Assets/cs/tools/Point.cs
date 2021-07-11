using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 坐标
public class Point
{
    public int x;
    public int z;

    public Point()
    {
        x = z = 0;
    }

    public Point(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public Vector3 GetPosition()
    {
        //HexCell cell = HexGrid.instance.GetCell(this);
        return Vector3.zero;
    }

    /// <summary>
    /// 算 x-z 平面上 2射线交点
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v1Dir"></param>
    /// <param name="v3"></param>
    /// <param name="v3Dir"></param>
    /// <returns></returns>
    public static Vector3 GetRayIntersection(Vector3 v1, Vector3 v1Dir, Vector3 v3, Vector3 v3Dir)
    {
        //Vector2 v1t2 = Vector3Tool.ToVector2(v1);
        //Vector2 v1Dt2 = Vector3Tool.ToVector2(v1Dir);
        //Vector2 v3t2 = Vector3Tool.ToVector2(v3);
        //Vector2 v3Dt2 = Vector3Tool.ToVector2(v3Dir);
        //Coordinate2 coordinate1 = new Coordinate2(v1t2, v1t2 + v1Dt2);

        //Vector2 v3t2_inV1co = Vector2Tool.PointToLocalSpace(v3t2, coordinate1);
        //Vector2 v3Dt2_inV1co = Vector2Tool.DirToLocalSpace(v3Dt2, coordinate1);

        //if(v3Dt2_inV1co.y > -0.001f && v3Dt2_inV1co.y < 0.001f)
        //{
        //    if(v3t2_inV1co.y > -0.001f && v3t2_inV1co.y < 0.001f)
        //    {
        //        Vector2 _intV1co = new Vector2(v3t2_inV1co.x / 2, 0);
        //        Vector2 _inter = Vector2Tool.PointToWorldSpace(_intV1co, coordinate1);
        //        return new Vector3(_inter.x, v1.y, _inter.y);
        //    }
        //}

        //float _x = v3t2_inV1co.y * v3Dt2_inV1co.x / v3Dt2_inV1co.y;
        //Vector2 intV1co = new Vector2(_x + v3t2_inV1co.x, 0);

        //Vector2 inter = Vector2Tool.PointToWorldSpace(intV1co, coordinate1);

        //Debug.Log("--------------------------");
        //Debug.Log(inter);
        //return new Vector3(inter.x, v1.y, inter.y);

        //Debug.Log("--------------------------");
        //Debug.Log(v1);
        //Debug.Log(v1Dir);
        //Debug.Log(v3);
        //Debug.Log(v3Dir);

        ////float n = v1Dir.x / v1Dir.z;
        ////float m = v3Dir.x / v3Dir.z;

        ////Debug.Log(n);
        ////Debug.Log(m);

        ////float z = (v1.x - n * v1.z - v3.x + m * v3.z) / (m - n);
        ////float x = v1.x - n * (v1.z - z);

        //// return new Vector3(x, v1.y, z);

        float A = v1.x;
        float B = v1.z;
        float C = v1Dir.x;
        float D = v1Dir.z;

        float X = v3.x;
        float Y = v3.z;
        float Z = v3Dir.x;
        float G = v3Dir.z;

        float ks = (D * Z - C * G);
        if (ks > -0.001f && ks < 0.001f)
        {
            return new Vector3((A + X) / 2, v1.y, (B + Y) / 2);
        }

        float m = (C * Y - C * B - D * X + D * A) / ks;

        float px = X + Z * m;
        float pz = Y + G * m;
        return new Vector3(px, v1.y, pz);
    }
}
