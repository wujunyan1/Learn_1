using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMapSetting : MonoBehaviour
{
    public Color[] terrainTypeColor;

    private void Awake()
    {
        HexMetrics.terrainTypeColor = terrainTypeColor;
    }
}
