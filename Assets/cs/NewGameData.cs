using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGameData
{
    int x, z;
    public int X
    {
        get
        {
            return x;
        }
    }

    public int Z
    {
        get
        {
            return z;
        }
    }

    public int mapSeed;

    // 后续未加入
    // 湿度
    // 降雨度
    // 高度
    // 平整度

    int cellCount = 0;
    public int CellCount
    {
        get
        {
            return cellCount;
        }
    }

    public NewGameData(int x, int z)
    {
        this.x = x;
        this.z = z;
        cellCount = x + z;
    }

    public int campNum;
}
