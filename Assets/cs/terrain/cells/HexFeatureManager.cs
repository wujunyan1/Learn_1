using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexFeatureManager : MonoBehaviour
{
    public Transform featurePrefab;

    // 树林
    public Transform[] treePrefab;
    // 草地
    public Transform[] grassPrefab;
    // 石头
    public Transform[] quarryPrefab;
    // 铁
    public Transform[] ironPrefab;
    // 金
    public Transform[] goldPrefab;

    Transform[] containers;

    void Awake()
    {
        containers = new Transform[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
    }

    public void Clear()
    {
        //container = new GameObject("Features Container").transform;
        //container.SetParent(transform, false);
        Debug.Log(containers.Length);
        for (int index = 0; index < containers.Length; index++)
        {
            Transform container = containers[index];
            if (container != null)
            {
                Destroy(container.gameObject);
            }
        }

        containers = new Transform[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
    }

    public void Apply()
    {
        //containers = new Transform[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
    }

    Transform GetFeatureTransform(HexCell cell)
    {
        int cellIndex = cell.chunkIndex;

        if (containers[cellIndex])
        {
            return containers[cellIndex];
        }
        Transform container = new GameObject("Features Container").transform;
        //container.localPosition = cell.Position;
        container.SetParent(transform, false);
        containers[cellIndex] = container;

        return container;
    }

    // 根据不同类型增加不同的物品
    public void AddFeature(HexCell cell)
    {
        AddFeature(cell, cell.Position, cell.Position, cell.Position);
    }

    public void AddFeature(HexCell cell, Vector3 A, Vector3 B, Vector3 C, Vector3 D)
    {
        switch (cell.TerrainType)
        {
            case HexTerrainType.Wood:
                AddTreeFeature(cell, A, B, C);
                AddTreeFeature(cell, B, C, D);
                break;
            case HexTerrainType.Grassplot:
                AddGrassFeature(cell, A, B, C);
                AddGrassFeature(cell, B, C, D);
                break;
            case HexTerrainType.Quarry:
                AddQuarryFeature(cell, A, B, C);
                AddQuarryFeature(cell, B, C, D);
                break;
            case HexTerrainType.Iron:
                AddIronFeature(cell, A, B, C);
                AddIronFeature(cell, B, C, D);
                break;
            case HexTerrainType.Gold:
                AddGoldFeature(cell, A, B, C);
                AddGoldFeature(cell, B, C, D);
                break;
        }
    }

    public void AddFeature(HexCell cell, Vector3 A, Vector3 B, Vector3 C)
    {
        switch (cell.TerrainType)
        {
            case HexTerrainType.Wood:
                AddTreeFeature(cell, A, B, C);
                break;
            case HexTerrainType.Grassplot:
                AddGrassFeature(cell, A, B, C);
                break;
            case HexTerrainType.Quarry:
                AddQuarryFeature(cell, A, B, C);
                break;
            case HexTerrainType.Iron:
                AddIronFeature(cell, A, B, C);
                break;
            case HexTerrainType.Gold:
                AddGoldFeature(cell, A, B, C);
                break;
        }
    }

    void AddFeatureRes(HexCell cell, Vector3 A, Vector3 B, Vector3 C, float Coefficient, Transform[] prefabs)
    {
        float density = cell.Store * Coefficient;
        Vector3[] positions = HexMetrics.RandomTriangleVectors(A, B, C, density);
        Transform container = GetFeatureTransform(cell);

        foreach (Vector3 position in positions)
        {
            if (position != null)
            {
                int index = Random.Range(0, prefabs.Length);
                Transform instance = Instantiate(prefabs[index]);
                instance.localPosition = position;
                instance.localRotation = Quaternion.Euler(0f, 360f * Random.value, 0f);

                instance.SetParent(container, false);
            }
        }
    }

    void AddTreeFeature(HexCell cell, Vector3 A, Vector3 B, Vector3 C)
    {
        AddFeatureRes(cell, A, B, C, 0.0001f, treePrefab);
    }

    void AddQuarryFeature(HexCell cell, Vector3 A, Vector3 B, Vector3 C)
    {
        AddFeatureRes(cell, A, B, C, 0.0001f, quarryPrefab);
    }

    void AddIronFeature(HexCell cell, Vector3 A, Vector3 B, Vector3 C)
    {
        AddFeatureRes(cell, A, B, C, 0.0001f, ironPrefab);
    }

    void AddGoldFeature(HexCell cell, Vector3 A, Vector3 B, Vector3 C)
    {
        AddFeatureRes(cell, A, B, C, 0.000001f, goldPrefab);
    }

    void AddGrassFeature(HexCell cell, Vector3 A, Vector3 B, Vector3 C)
    {
        Transform container = GetFeatureTransform(cell);
        for (int i = 0; i< 10; i++)
        {
            int index = Random.Range(0, grassPrefab.Length);

            Transform instance = Instantiate(grassPrefab[index]);
            // 扰乱位置
            instance.localPosition = HexMetrics.RandomTriangleVector3(A, B, C);
            instance.localRotation = Quaternion.Euler(0f, 360f * Random.value, 0f);

            instance.SetParent(container, false);
        }
    }
}
