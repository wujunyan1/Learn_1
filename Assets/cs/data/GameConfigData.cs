using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfigData
{
    private static GameConfigData instance;
    

    HexTerrainType[] colors = {
        HexTerrainType.Grassplot,
        HexTerrainType.Ridge,
        HexTerrainType.Desert,
        HexTerrainType.Land,
        HexTerrainType.Snow,
    };

    private GameConfigData(){
        
    }

    public static GameConfigData GetInstance(){
        if(instance == null)
        {
            instance = new GameConfigData();
        }

        return instance;
    }
    

    public HexTerrainType GetColor(int index = 0)
    {
        return this.colors[index];
    }

    public HexTerrainType[] GetColors()
    {
        return this.colors;
    }
}
