using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapGenerator : MonoBehaviour
{
    public HexGrid grid;
    NewGameData data;

    HexCellPriorityQueue searchFrontier;
    int searchFrontierPhase;

    public void GenerateMap(NewGameData data)
    {
        this.data = data;
        grid.CreateMap(data);
        //RaiseTerrain(7);
    }

    void RaiseTerrain(int chunkSize)
    {
        for (int i = 0; i < chunkSize; i++)
        {
            GetRandomCell().TerrainType = HexTerrainType.Grassplot;
        }
    }

    HexCell GetRandomCell()
    {
        return grid.GetCell(Random.Range(0, data.CellCount));
    }
}
