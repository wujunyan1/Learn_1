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

    public Transform[] buildsPrefab;

    Transform[] containers;

    // 城墙
    public HexMesh walls;


    void Awake()
    {
        containers = new Transform[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
    }

    public void Clear()
    {
        //container = new GameObject("Features Container").transform;
        //container.SetParent(transform, false);
        //Debug.Log(containers.Length);
        for (int index = 0; index < containers.Length; index++)
        {
            Transform container = containers[index];
            if (container != null)
            {
                Destroy(container.gameObject);
            }
        }

        containers = new Transform[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];

        walls.Clear();
    }

    public void Apply()
    {
        //containers = new Transform[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];

        walls.Apply();
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

    public void ClearFeature(HexCell cell)
    {
        if (cell.FeaturePos != null)
        {
            foreach (var item in cell.FeaturePos)
            {
                GameObject.Destroy(item);
            }
            cell.FeaturePos.Clear();
        }
    }

    // 根据不同类型增加不同的物品
    public void AddFeature(HexCell cell)
    {
        AddFeature(cell, cell.Position, cell.Position, cell.Position);
    }

    public void AddFeature(HexCell cell, Vector3 A, Vector3 B, Vector3 C, Vector3 D)
    {
        switch (cell.FeatureType)
        {
            case HexFeatureType.Wood:
                AddTreeFeature(cell, A, B, C);
                AddTreeFeature(cell, B, C, D);
                break;
            case HexFeatureType.Gold:
                AddGoldFeature(cell, A, B, C);
                AddGoldFeature(cell, B, C, D);
                break;
        }
    }

    public void AddFeature(HexCell cell, Vector3 A, Vector3 B, Vector3 C)
    {
        switch (cell.FeatureType)
        {
            case HexFeatureType.Wood:
                AddTreeFeature(cell, A, B, C);
                break;
            case HexFeatureType.Gold:
                AddGoldFeature(cell, A, B, C);
                break;
        }
    }

    void AddFeatureRes(HexCell cell, Vector3 A, Vector3 B, Vector3 C, float Coefficient, Transform[] prefabs)
    {
        float density = cell.FeatureLevel * Coefficient;
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

                if(cell.FeaturePos == null)
                {
                    cell.FeaturePos = new List<GameObject>();
                }

                cell.FeaturePos.Add(instance.gameObject);
            }
        }
    }

    void AddTreeFeature(HexCell cell, Vector3 A, Vector3 B, Vector3 C)
    {
        if (!cell.IsUnderwater)
        {
            AddFeatureRes(cell, A, B, C, 0.002f, treePrefab);
        }
    }

    void AddQuarryFeature(HexCell cell, Vector3 A, Vector3 B, Vector3 C)
    {
        AddFeatureRes(cell, A, B, C, 0.002f, quarryPrefab);
    }

    void AddIronFeature(HexCell cell, Vector3 A, Vector3 B, Vector3 C)
    {
        AddFeatureRes(cell, A, B, C, 0.002f, ironPrefab);
    }

    void AddGoldFeature(HexCell cell, Vector3 A, Vector3 B, Vector3 C)
    {
        AddFeatureRes(cell, A, B, C, 0.0002f, goldPrefab);
    }

    void AddGrassFeature(HexCell cell, Vector3 A, Vector3 B, Vector3 C)
    {
        return;
        Transform container = GetFeatureTransform(cell);
        for (int i = 0; i < 10; i++)
        {
            int index = Random.Range(0, grassPrefab.Length);

            Transform instance = Instantiate(grassPrefab[index]);
            // 扰乱位置
            instance.localPosition = HexMetrics.RandomTriangleVector3(A, B, C);
            instance.localRotation = Quaternion.Euler(0f, 360f * Random.value, 0f);

            instance.SetParent(container, false);
        }
    }

    // 添加建筑物
    public void AddBuildFeature(HexCell cell)
    {
        BuildType type = cell.Build.BuildType;
        Transform container = GetFeatureTransform(cell);

        Transform instance = Instantiate(buildsPrefab[(int)type]);
        instance.localPosition = cell.Position;
        instance.SetParent(container, false);
    }


    //////////////////////////////城墙//////////////////////////////////////////

    /// <summary>
    /// 三角形区域
    /// </summary>
    /// <param name="c1"></param>
    /// <param name="cell1"></param>
    /// <param name="c2"></param>
    /// <param name="cell2"></param>
    /// <param name="d2"></param>
    /// <param name="c3"></param>
    /// <param name="cell3"></param>
    /// <param name="d3"></param>
    public void AddWall(
        Vector3 c1, HexCell cell1,
        Vector3 c2, HexCell cell2, HexDirection d1,
        Vector3 c3, HexCell cell3, HexDirection d2
    )
    {
        bool leftD = cell1.HasWall(d1) || cell2.HasWall(d1.Opposite());
        bool rightD = cell1.HasWall(d2) || cell3.HasWall(d2.Opposite());

        HexDirection c2Tc3 = d1.Opposite().Previous();
        bool topD = cell2.HasWall(c2Tc3) || cell3.HasWall(c2Tc3.Opposite());


        if (leftD)
        {
            if (rightD)
            {
                if (topD)
                {
                    //三个都有
                    AddWallTriangle(c1, cell1, c2, cell2, c3, cell3);
                }
                else
                {
                    AddWallSegment(c1, cell1, c2, cell2, c3, cell3);
                }
            }
            else if (topD)
            {
                AddWallSegment(c2, cell2, c3, cell3, c1, cell1);
            }
            else
            {
                // 就一个
                AddWallCap(c1, c2);
            }
        }
        else if (rightD)
        {
            if (topD)
            {
                AddWallSegment(c3, cell3, c1, cell1, c2, cell2);
            }
            else
            {
                // 就一个
                AddWallCap(c3, c1);
            }
        }
        else if (topD)
        {
            // 就一个
            AddWallCap(c2, c3);
        }
    }


    /// <summary>
    /// 城墙 某2个格子之间的城墙
    /// </summary>
    /// <param name="near"></param>
    /// <param name="nearCell"> 本格 </param>
    /// <param name="far"></param>
    /// <param name="farCell"> 邻格 </param>
    public void AddWall(
        EdgeVertices near, HexCell nearCell,
        EdgeVertices far, HexCell farCell,
        HexDirection dir,
        bool hasRiver, bool hasRoad
    )
    {
        bool hasWall = nearCell.HasWall(dir) || farCell.HasWall(dir.Opposite());

        if (hasWall &&
            !nearCell.IsLakes() && !farCell.IsLakes() &&
            nearCell.GetEdgeType(farCell) != HexEdgeType.Cliff)
        {
            AddWallSegment(near.v1, far.v1, near.v2, far.v2);
            if (hasRiver || hasRoad)
            {
                // Leave a gap.
                AddWallCap(near.v2, far.v2);
                AddWallCap(far.v4, near.v4);
            }
            else
            {
                AddWallSegment(near.v2, far.v2, near.v3, far.v3);
                AddWallSegment(near.v3, far.v3, near.v4, far.v4);
            }
            AddWallSegment(near.v4, far.v4, near.v5, far.v5);
        }
    }

    /// <summary>
    /// 三角形部位 2个的
    /// </summary>
    /// <param name="pivot"></param>
    /// <param name="pivotCell"></param>
    /// <param name="left"></param>
    /// <param name="leftCell"></param>
    /// <param name="right"></param>
    /// <param name="rightCell"></param>
    void AddWallSegment(
        Vector3 pivot, HexCell pivotCell,
        Vector3 left, HexCell leftCell,
        Vector3 right, HexCell rightCell
    )
    {
        if (pivotCell.IsLakes())
        {
            return;
        }

        bool hasLeftWall = !leftCell.IsLakes() &&
            pivotCell.GetEdgeType(leftCell) != HexEdgeType.Cliff;
        bool hasRighWall = !rightCell.IsLakes() &&
            pivotCell.GetEdgeType(rightCell) != HexEdgeType.Cliff;

        if (hasLeftWall)
        {
            if (hasRighWall)
            {
                AddWallSegment(pivot, left, pivot, right);
            }
            else if (leftCell.Elevation < rightCell.Elevation)
            {
                AddWallWedge(pivot, left, right);
            }
            else
            {
                AddWallCap(pivot, left);
            }
        }
        else if (hasRighWall)
        {
            if (rightCell.Elevation < leftCell.Elevation)
            {
                AddWallWedge(right, pivot, left);
            }
            else
            {
                AddWallCap(right, pivot);
            }
        }
    }


    /// <summary>
    /// 创建一段城墙
    /// </summary>
    /// <param name="nearLeft"></param>
    /// <param name="farLeft"></param>
    /// <param name="nearRight"></param>
    /// <param name="farRight"></param>
    void AddWallSegment(
        Vector3 nearLeft, Vector3 farLeft, Vector3 nearRight, Vector3 farRight
    )
    {
        nearLeft = HexMetrics.Perturb(nearLeft);
        farLeft = HexMetrics.Perturb(farLeft);
        nearRight = HexMetrics.Perturb(nearRight);
        farRight = HexMetrics.Perturb(farRight);

        Vector3 left = HexMetrics.WallLerp(nearLeft, farLeft);
        Vector3 right = HexMetrics.WallLerp(nearRight, farRight);

        Vector3 leftThicknessOffset =
            HexMetrics.WallThicknessOffset(nearLeft, farLeft);
        Vector3 rightThicknessOffset =
            HexMetrics.WallThicknessOffset(nearRight, farRight);

        float leftTop = left.y + HexMetrics.wallHeight;
        float rightTop = right.y + HexMetrics.wallHeight;

        // 向上画四边形
        Vector3 v1, v2, v3, v4;
        v1 = v3 = left - leftThicknessOffset;
        v2 = v4 = right - rightThicknessOffset;
        v3.y = leftTop;
        v4.y = rightTop;
        walls.AddQuadUnperturbed(v1, v2, v3, v4);

        Vector3 t1 = v3, t2 = v4;

        //偏移顶部位置，增加厚度
        v1 = v3 = left + leftThicknessOffset;
        v2 = v4 = right + rightThicknessOffset;
        v3.y = leftTop;
        v4.y = rightTop;
        walls.AddQuadUnperturbed(v2, v1, v4, v3);

        // 画顶
        walls.AddQuadUnperturbed(t1, t2, v3, v4);
    }


    /// <summary>
    /// 三角形部位 3个的
    /// </summary>
    /// <param name="nearLeft"></param>
    /// <param name="farLeft"></param>
    /// <param name="nearRight"></param>
    /// <param name="farRight"></param>
    /// <param name="topLeft"></param>
    /// <param name="topRight"></param>
    void AddWallTriangle(
        Vector3 c1, HexCell cell1,
        Vector3 c2, HexCell cell2,
        Vector3 c3, HexCell cell3
    )
    {
        if (cell1.IsLakes() || cell1.GetEdgeType(cell2) == HexEdgeType.Cliff
            || cell2.IsLakes() || cell2.GetEdgeType(cell3) == HexEdgeType.Cliff
            || cell3.IsLakes() || cell3.GetEdgeType(cell1) == HexEdgeType.Cliff)
        {
            return;
        }
        

        c1 = HexMetrics.Perturb(c1);
        c2 = HexMetrics.Perturb(c2);
        c3 = HexMetrics.Perturb(c3);
        

        Vector3 left = HexMetrics.WallLerp(c1, c2);
        Vector3 right = HexMetrics.WallLerp(c1, c3);
        Vector3 top = HexMetrics.WallLerp(c2, c3);

        Vector3 leftThicknessOffset =
            HexMetrics.WallThicknessOffset(c1, c2);
        Vector3 rightThicknessOffset =
            HexMetrics.WallThicknessOffset(c1, c3);
        Vector3 topThicknessOffset =
            HexMetrics.WallThicknessOffset(c2, c3);

        Vector3 c1Left = left - leftThicknessOffset;
        Vector3 c1Right = right - rightThicknessOffset;

        Vector3 c2Left = left + leftThicknessOffset;
        Vector3 c3Right = right + rightThicknessOffset;

        Vector3 topLeft = top - topThicknessOffset;
        Vector3 topRight = top + topThicknessOffset;

        AddLineWall(c1Left, c1Right);
        AddLineWall(c1Right, c3Right);
        AddLineWall(c3Right, topRight);
        AddLineWall(topRight, topLeft);
        AddLineWall(topLeft, c2Left);
        AddLineWall(c2Left, c1Left);

        Vector3 topHCenter = Vector3.Lerp(left, right, 0.5f);
        topHCenter = Vector3.Lerp(topHCenter, top, 0.333f);
        topHCenter.y = topHCenter.y + HexMetrics.wallHeight;

        c1Left.y = c1Left.y + HexMetrics.wallHeight;
        c1Right.y = c1Right.y + HexMetrics.wallHeight;
        c3Right.y += HexMetrics.wallHeight;
        topRight.y += HexMetrics.wallHeight;
        topLeft.y += HexMetrics.wallHeight;
        c2Left.y += HexMetrics.wallHeight;

        walls.AddTriangle(topHCenter, c1Left, c2Left);
        walls.AddTriangle(topHCenter, c2Left, topLeft);
        walls.AddTriangle(topHCenter, topLeft, topRight);
        walls.AddTriangle(topHCenter, topRight, c3Right);
        walls.AddTriangle(topHCenter, c3Right, c1Right);
        walls.AddTriangle(topHCenter, c1Right, c1Left);
    }


    /// <summary>
    /// 画一段城墙 
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    void AddLineWall(Vector3 v1, Vector3 v2)
    {
        Vector3 v3 = v1;
        Vector3 v4 = v2;
        v3.y = v1.y + HexMetrics.wallHeight;
        v4.y = v2.y + HexMetrics.wallHeight;
        walls.AddQuadUnperturbed(v1, v2, v3, v4);
    }

    /// <summary>
    /// 墙结束 补足
    /// </summary>
    void AddWallCap(Vector3 near, Vector3 far)
    {
        near = HexMetrics.Perturb(near);
        far = HexMetrics.Perturb(far);

        Vector3 center = HexMetrics.WallLerp(near, far);
        Vector3 thickness = HexMetrics.WallThicknessOffset(near, far);

        Vector3 v1, v2, v3, v4;

        v1 = v3 = center - thickness;
        v2 = v4 = center + thickness;
        v3.y = v4.y = center.y + HexMetrics.wallHeight;
        walls.AddQuadUnperturbed(v1, v2, v3, v4);
    }

    void AddWallWedge(Vector3 near, Vector3 far, Vector3 point)
    {
        near = HexMetrics.Perturb(near);
        far = HexMetrics.Perturb(far);
        point = HexMetrics.Perturb(point);

        Vector3 center = HexMetrics.WallLerp(near, far);
        Vector3 thickness = HexMetrics.WallThicknessOffset(near, far);

        Vector3 v1, v2, v3, v4;
        Vector3 pointTop = point;
        point.y = center.y;

        v1 = v3 = center - thickness;
        v2 = v4 = center + thickness;
        v3.y = v4.y = pointTop.y = center.y + HexMetrics.wallHeight;

        //        walls.AddQuadUnperturbed(v1, v2, v3, v4);
        walls.AddQuadUnperturbed(v1, point, v3, pointTop);
        walls.AddQuadUnperturbed(point, v2, pointTop, v4);
        walls.AddTriangleUnperturbed(pointTop, v3, v4);
    }
}
