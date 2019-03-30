using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise
{

    static float Noise1(int x, int y, int terrain1, int terrain2)
    {
        x = x % 25;
        y = y % 25;
        int n = x + y * 57;
        n = (n << 13) ^ n;
        return (1.0f - ((n * ((n * n * terrain1) + terrain2) + 1376312589) & 0x7fffffff) / 1073741824.0f);
    }

    public static float Interpolate(float a, float b, float x)
    {
        float ft = x * 3.1415927f;
        float f = (1 - Mathf.Cos(ft)) * 0.5f;
        return a * (1 - f) + b * f;
    }

    public static float SmoothNoise_1(int x, int y, int terrain1, int terrain2)
    {
        float corners = (Noise1(x - 1, y - 1, terrain1, terrain2) + Noise1(x + 1, y - 1, terrain1, terrain2) 
            + Noise1(x - 1, y + 1, terrain1, terrain2) + Noise1(x + 1, y + 1, terrain1, terrain2)) / 16;
        float sides = (Noise1(x - 1, y, terrain1, terrain2) + Noise1(x + 1, y, terrain1, terrain2) + Noise1(x, y - 1, terrain1, terrain2) + Noise1(x, y + 1, terrain1, terrain2)) / 8;
        float center = Noise1(x, y, terrain1, terrain2) / 4;
        return corners + sides + center;
    }

    public static float InterpolatedNoise_1(float x, float y, int terrain1, int terrain2)
    {
        int integer_X = (int)x;
        float fractional_X = x - integer_X;
        int integer_Y = (int)y;
        float fractional_Y = y - integer_Y;
        float v1 = SmoothNoise_1(integer_X, integer_Y, terrain1, terrain2);
        float v2 = SmoothNoise_1(integer_X + 1, integer_Y, terrain1, terrain2);
        float v3 = SmoothNoise_1(integer_X, integer_Y + 1, terrain1, terrain2);
        float v4 = SmoothNoise_1(integer_X + 1, integer_Y + 1, terrain1, terrain2);
        float i1 = Interpolate(v1, v2, fractional_X);
        float i2 = Interpolate(v3, v4, fractional_X);
        return Interpolate(i1, i2, fractional_Y);
    }

    public static float PerlinNoise2D(float x, float y, int terrain1, int terrain2, int octaves, float persistence)
    {
        float total = 0;
        float p = persistence;
        float n = octaves - 1;

        //Debug.Log(string.Format("{0} {1} {2} {3}", x,  y,  octaves,  persistence));

        for (int i = 0; i < n; i++)
        {
            float frequency = Mathf.Pow(2.0f,i);
            float amplitude = Mathf.Pow(p, i);
            total = total + InterpolatedNoise_1(x * frequency, y * frequency, terrain1, terrain2) * amplitude;
        }
        return total;
    }
}
