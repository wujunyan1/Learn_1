using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector3Tool
{
    public const float PRECISION = 0.0001f;
    public const int AMPLIFICATION = 1000;
    public const int AMPLIFICATION2 = 1000000;

    public static Vector3 Normalized(Vector3 v)
    {
        Vector3 r = Vector3.zero;

        float l = Magnitude(v);

        if (Mathf.Abs(l) < PRECISION)
        {
            return r;
        }

        int x = Mathf.CeilToInt(v.x * AMPLIFICATION);
        int y = Mathf.CeilToInt(v.y * AMPLIFICATION);
        int z = Mathf.CeilToInt(v.z * AMPLIFICATION);

        int L = Mathf.CeilToInt(l * AMPLIFICATION);

        r.x = x * 1.0f / L;
        r.y = y * 1.0f / L;
        r.z = z * 1.0f / L;

        return r;
    }

    public static float Magnitude(Vector3 v)
    {
        float n = v.x * v.x * AMPLIFICATION2 + v.y * v.y * AMPLIFICATION2 + v.z * v.z * AMPLIFICATION2;

        if( Mathf.Abs(n) < 1)
        {
            n = 0.0f;
        }

        return Mathf.Sqrt(n) / AMPLIFICATION;
    }

    public static Vector2 ToVector2(Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }
}
