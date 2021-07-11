using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridChunk : MonoBehaviour
{
    HexCell[] cells;

    // 画布
    Canvas gridCanvas;

    // 网格
    public HexMesh terrain;

    // 河流
    public HexMesh rivers;

    public HexMesh roads;

    // 湖泊
    public HexMesh lakes;

    // 湖泊与陆地接壤
    public HexMesh lakesShore;

    // 河口
    public HexMesh estuaries;

    // 边界线
    public HexMesh boundary;


    public HexFeatureManager features;

    public Color[] colors;

    static Color weights1 = new Color(1f, 0f, 0f);
    static Color weights2 = new Color(0f, 1f, 0f);
    static Color weights3 = new Color(0f, 0f, 1f);

    public CollionClick click;
    int targetMask;

    Material terrainMaterial;

    // 初始化地图
    public void Awake()
    {
        targetMask = LayerMask.GetMask("Map");

        gridCanvas = GetComponentInChildren<Canvas>();
        //hexMesh = GetComponentInChildren<HexMesh>();

        terrainMaterial = terrain.GetComponent<MeshRenderer>().material;

        CreateCells();
    }

    void CreateCells()
    {
        cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
    }

    public void AddCell(int index, HexCell cell)
    {
        cells[index] = cell;
        cell.chunkIndex = index;
        cell.transform.SetParent(transform, false);
        cell.uiRect.SetParent(gridCanvas.transform, false);
        cell.chunk = this;
    }

    public void StartGenerateMap()
    {
        Triangulate(cells);
    }

    public void Refresh()
    {
        Triangulate(cells);
    }

    public void Refresh(HexCell cell)
    {
        Triangulate(cell);
    }

    public void ClearMesh()
    {
        // 清楚渲染
        terrain.Clear();
        roads.Clear();
        rivers.Clear();
        lakes.Clear();
        lakesShore.Clear();
        estuaries.Clear();
        features.Clear();
    }

    public void ApplyMesh()
    {
        //渲染
        terrain.Apply();
        roads.Apply();
        rivers.Apply();
        lakes.Apply();
        lakesShore.Apply();
        estuaries.Apply();
        features.Apply();
    }

    // 渲染所以六边形
    public void Triangulate(HexCell[] cells)
    {
        // 清楚渲染
        ClearMesh();

        for (int i = 0; i < cells.Length; i++)
        {
            // 渲染某个六边形
            Triangulate(cells[i]);
        }

        // 战争迷雾
        for (int i = 0; i < cells.Length; i++)
        {
            HexCell cell = cells[i];
            cell.ShaderData.RefreshTerrain(cell);
        }

        ApplyMesh();
    }

    // 渲染某个六边形
    public void Triangulate(HexCell cell)
    {
        features.ClearFeature(cell);

        Vector3 center = cell.transform.localPosition;
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            Triangulate(d, cell);
            //AddTriangleColor(cell.color);
        }

        // 处理河流连接点 有河流且不是开始和结束
        if (cell.HasRiver() && !cell.HasRiverBeginOrEnd())
        {
            TriangulateWithRiverConnection(cell);
        }

        // 处理道路连接点 
        if (cell.HasRoads)
        {
            TriangulateRoadAdjacentToRiver(cell);
            //TriangulateWithRiverConnection(cell);
        }

        //features.AddFeature(cell);
        //if (cell.Build != null)
        //{
        //    features.AddBuildFeature(cell);
        //}
    }

    // 渲染某个方向的三角形
    void Triangulate(HexDirection direction, HexCell cell)
    {
        Vector3 center = cell.transform.localPosition;
        EdgeVertices e = new EdgeVertices(
            center + HexMetrics.GetFirstSolidCorner(direction),
            center + HexMetrics.GetSecondSolidCorner(direction)
        );

        if (cell.IsLakes())
        {
            TriangulateWater(direction, cell, center);
        }


        //Debug.Log("-----------------");
        //  三角形区域
        if (cell.HasRiver())
        {
            TriangulateAdjacentToRiver(direction, cell, center, e);
        }
        else
        {
            // TriangulateEdgeFan(center, e, cell.index);
            // features.AddFeature(cell, center, e.v1, e.v5);

            TriangulateWithoutRiver(direction, cell, center, e);
        }

        // 三角形外边区域 两个块之间的连接区域
        if (direction <= HexDirection.SE)
        {
            TriangulateConnection(direction, cell, e);
        }
    }

    void TriangulateAdjacentToRiver(
        HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
    )
    {
        //Debug.Log(cell.HasRiver());
        // 这条边是否有河流
        if (cell.HasRiverThroughEdge(direction))
        {
            e.v3.y = cell.StreamBedY;
            // 是湖泊
            if (cell.IsLakes())
            {

            }

            //Debug.Log(cell.HasRiverBeginOrEnd());
            // 只有流入或者流出
            if (cell.HasRiverBeginOrEnd())
            {
                TriangulateWithRiverBeginOrEnd(direction, cell, center, e);
            }
            else
            {
                TriangulateWithRiver(direction, cell, center, e);
            }
        }
        else
        {
            // 这边是河流的就不画路了
            if (cell.HasRoads)
            {
                //TriangulateRoadAdjacentToRiver(direction, cell, center, e);
            }

            if (cell.HasRiverBeginOrEnd())
            {
                TriangulateEdgeFan(center, e, cell.index);
            }
            else
            {
                TriangulateWithNoRiver(direction, cell, center, e);
            }
            //TriangulateEdgeFan(center, e, cell.color);

            // 添加地上部分
            Vector3 v1 = Vector3.Lerp(center, e.v1, 0.35f);
            Vector3 v5 = Vector3.Lerp(center, e.v5, 0.35f);
            features.AddFeature(cell, v1, v5, e.v1, e.v5);
        }
    }

    // 没有河流的六边形的某边三角形
    void TriangulateWithoutRiver(
        HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
    )
    {
        TriangulateEdgeFan(center, e, cell.index);
        features.AddFeature(cell, center, e.v1, e.v5);

        //if (cell.HasRoads)
        //{
        //    Vector2 interpolators = GetRoadInterpolators(direction, cell);
        //    TriangulateRoad(
        //        center,
        //        Vector3.Lerp(center, e.v1, interpolators.x),
        //        Vector3.Lerp(center, e.v5, interpolators.y),
        //        e, cell.HasRoadThroughEdge(direction)
        //    );
        //}
    }

    void TriangulateConnection(HexDirection direction, HexCell cell, EdgeVertices e1)
    {
        HexCell neighbor = cell.GetNeighbor(direction);
        if (neighbor == null)
        {
            return;
        }

        Vector3 bridge = HexMetrics.GetBridge(direction);
        bridge.y = neighbor.transform.localPosition.y - cell.transform.localPosition.y;
        EdgeVertices e2 = new EdgeVertices(e1.v1 + bridge, e1.v5 + bridge);

        bool hasRiver = cell.HasRiverThroughEdge(direction);
        bool hasRoad = cell.HasRoadThroughEdge(direction);

        // 这个方向有河流
        if (hasRiver)
        {
            e1.v3.y = cell.StreamBedY;
            e2.v3.y = neighbor.StreamBedY;

            Vector3 indices;
            indices.x = indices.z = cell.index;
            indices.y = neighbor.index;


            // 自己不是湖泊
            if (!cell.IsLakes())
            {
                // 邻居不是湖泊，则正常
                if (!neighbor.IsLakes())
                {
                    bool reversed = cell.IsInComingRiverDirection(direction);
                    TriangulateRiverQuad(e1.v1, e1.v5, e2.v1, e2.v5, cell.RiverSurfaceY, neighbor.RiverSurfaceY, 0.6f, reversed, indices);
                }
                // 邻居是湖泊
                else if (neighbor.IsLakes())
                {
                    TriangulateWaterfallInWater(
                        e1.v2, e1.v4, e2.v2, e2.v4,
                        cell.RiverSurfaceY, neighbor.RiverSurfaceY,
                        neighbor.LakesSurfaceY, indices
                    );
                }

            }
            // 自己是湖泊 邻居不是
            else if ( !neighbor.IsLakes() && neighbor.Elevation > cell.LakesLevel )
            {
                TriangulateWaterfallInWater(
                    e2.v4, e2.v2, e1.v4, e1.v2,
                    neighbor.RiverSurfaceY, cell.RiverSurfaceY,
                    cell.LakesSurfaceY, indices
                );
            }
        }

        HexEdgeType _type = cell.GetEdgeType(direction);
        if (_type == HexEdgeType.Slope)
        {
            TriangulateEdgeTerraces(e1, cell, e2, neighbor, hasRoad);
        }
        else
        {
            TriangulateEdgeStrip(e1, weights1, cell.index, e2, weights2, neighbor.index, hasRoad);

            // 平坦的并且2个区域类型相同
            if(_type == HexEdgeType.Flat && cell.TerrainType == neighbor.TerrainType)
            {
                features.AddFeature(cell, e1.v1, e1.v5, e2.v1, e2.v5);
            }
        }

        // AddQuad(v1, v2, v3, v4);
        // AddQuadColor(cell.color, neighbor.color);

        features.AddWall(e1, cell, e2, neighbor, direction,
            hasRiver, hasRoad
            );


        // 三角形部分
        // 获得这个 方向上的邻居 以及周围2个
        HexCell nextNeighbor = cell.GetNeighbor(direction.Next()) ?? cell;  // 方向上的下一个顺序的
        if (direction <= HexDirection.E && nextNeighbor != null)
        {
            Vector3 v5 = e1.v5 + HexMetrics.GetBridge(direction.Next());
            v5.y = nextNeighbor.Position.y;

            if (cell.Elevation <= neighbor.Elevation)
            {
                if (cell.Elevation <= nextNeighbor.Elevation)
                {
                    TriangulateCorner(
                        e1.v5, cell, e2.v5, neighbor, v5, nextNeighbor
                    );
                }
                else
                {
                    TriangulateCorner(
                        v5, nextNeighbor, e1.v5, cell, e2.v5, neighbor
                    );
                }
            }
            else if (neighbor.Elevation <= nextNeighbor.Elevation)
            {
                TriangulateCorner(
                    e2.v5, neighbor, v5, nextNeighbor, e1.v5, cell
                );
            }
            else
            {
                TriangulateCorner(
                    v5, nextNeighbor, e1.v5, cell, e2.v5, neighbor
                );
            }

            features.AddWall(e1.v5, cell, e2.v5, neighbor, direction, v5, nextNeighbor, direction.Next());
        }
    }

    // 渲染连接四边形及其颜色
    //  Vector3 beginLeft, Vector3 beginRight, HexCell beginCell, Vector3 endLeft, Vector3 endRight, HexCell endCell
    void TriangulateEdgeTerraces(EdgeVertices begin, HexCell beginCell, EdgeVertices end, HexCell endCell, bool hasRoad)
    {
        EdgeVertices e2 = EdgeVertices.TerraceLerp(begin, end, 1);
        Color w2 = HexMetrics.TerraceLerp(weights1, weights2, 1);
        float i1 = beginCell.index;
        float i2 = endCell.index;


        TriangulateEdgeStrip(begin, weights1, i1, e2, w2, i2, hasRoad);

        for (int i = 2; i < HexMetrics.terraceSteps; i++)
        {
            EdgeVertices e1 = e2;
            Color w1 = w2;
            e2 = EdgeVertices.TerraceLerp(begin, end, i);
            w2 = HexMetrics.TerraceLerp(weights1, weights2, i);
            TriangulateEdgeStrip(e1, w1, i1, e2, w2, i2, hasRoad);
        }

        TriangulateEdgeStrip(e2, w2, i1, end, weights2, i2, hasRoad);


        //Vector3 v3 = HexMetrics.TerraceLerp(beginLeft, endLeft, 1);
        //Vector3 v4 = HexMetrics.TerraceLerp(beginRight, endRight, 1);
        //Color c2 = HexMetrics.TerraceLerp(beginCell.Color, endCell.Color, 1);

        //AddQuad(beginLeft, beginRight, v3, v4);
        //AddQuadColor(beginCell.Color, c2);

        //for (int i = 2; i < HexMetrics.terraceSteps; i++)
        //{
        //    Vector3 v1 = v3;
        //    Vector3 v2 = v4;
        //    Color c1 = c2;
        //    v3 = HexMetrics.TerraceLerp(beginLeft, endLeft, i);
        //    v4 = HexMetrics.TerraceLerp(beginRight, endRight, i);
        //    c2 = HexMetrics.TerraceLerp(beginCell.Color, endCell.Color, i);
        //    AddQuad(v1, v2, v3, v4);
        //    AddQuadColor(c1, c2);
        //}

        //AddQuad(v3, v4, endLeft, endRight);
        //AddQuadColor(c2, endCell.Color);
    }

    // 渲染连接三边形及其颜色
    void TriangulateCorner(Vector3 bottom, HexCell bottomCell, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
    {
        HexEdgeType leftEdgeType = bottomCell.GetEdgeType(leftCell);
        HexEdgeType rightEdgeType = bottomCell.GetEdgeType(rightCell);

        if (leftEdgeType == HexEdgeType.Slope)
        {
            if (rightEdgeType == HexEdgeType.Slope)
            {
                TriangulateCornerTerraces(
                    bottom, bottomCell, left, leftCell, right, rightCell
                );
            }
            else if (rightEdgeType == HexEdgeType.Flat)
            {
                TriangulateCornerTerraces(
                    left, leftCell, right, rightCell, bottom, bottomCell
                );
            }
            else
            {
                TriangulateCornerTerracesCliff(
                    bottom, bottomCell, left, leftCell, right, rightCell
                );
            }
        }
        else if (rightEdgeType == HexEdgeType.Slope)
        {
            if (leftEdgeType == HexEdgeType.Flat)
            {
                TriangulateCornerTerraces(
                    right, rightCell, bottom, bottomCell, left, leftCell
                );
            }
            else
            {
                TriangulateCornerCliffTerraces(
                    bottom, bottomCell, left, leftCell, right, rightCell
                );
            }
        }
        else if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
        {
            if (leftCell.Elevation < rightCell.Elevation)
            {
                TriangulateCornerCliffTerraces(
                    right, rightCell, bottom, bottomCell, left, leftCell
                );
            }
            else
            {
                TriangulateCornerTerracesCliff(
                    left, leftCell, right, rightCell, bottom, bottomCell
                );
            }
        }
        else
        {
            terrain.AddTriangle(bottom, left, right);
            Vector3 indices;
            indices.x = bottomCell.index;
            indices.y = leftCell.index;
            indices.z = rightCell.index;
            terrain.AddTriangleCellData(indices, weights1, weights2, weights3);

            //terrain.AddTriangleColor(color1, color2, color3);
            //Vector3 types;
            //types.x = (int)bottomCell.TerrainType;
            //types.y = (int)leftCell.TerrainType;
            //types.z = (int)rightCell.TerrainType;
            //terrain.AddTriangleTerrainTypes(types);

        }
    }

    // 渲染连接三边形及其颜色
    void TriangulateCornerTerraces(Vector3 begin, HexCell beginCell, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
    {
        Vector3 v3 = HexMetrics.TerraceLerp(begin, left, 1);
        Vector3 v4 = HexMetrics.TerraceLerp(begin, right, 1);
        Color w3 = HexMetrics.TerraceLerp(weights1, weights2, 1);
        Color w4 = HexMetrics.TerraceLerp(weights1, weights3, 1);

        Vector3 indices;
        indices.x = beginCell.index;
        indices.y = leftCell.index;
        indices.z = rightCell.index;


        terrain.AddTriangle(begin, v3, v4);
        terrain.AddTriangleCellData(indices, weights1, w3, w4);


        for (int i = 2; i < HexMetrics.terraceSteps; i++)
        {
            Vector3 v1 = v3;
            Vector3 v2 = v4;
            Color w1 = w3;
            Color w2 = w4;
            v3 = HexMetrics.TerraceLerp(begin, left, i);
            v4 = HexMetrics.TerraceLerp(begin, right, i);
            w3 = HexMetrics.TerraceLerp(weights1, weights2, i);
            w4 = HexMetrics.TerraceLerp(weights1, weights3, i);
            terrain.AddQuad(v1, v2, v3, v4);
            terrain.AddQuadCellData(indices, w1, w2, w3, w4);
        }

        terrain.AddQuad(v3, v4, left, right);
        terrain.AddQuadCellData(indices, w3, w4, weights2, weights3);

    }

    // 渲染连接三边形及其颜色 悬崖+2个斜面
    void TriangulateCornerTerracesCliff(Vector3 begin, HexCell beginCell, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
    {
        float b = 1f / (rightCell.Elevation - beginCell.Elevation);
        Vector3 boundary = Vector3.Lerp(begin, right, b);
        Color boundaryWeights = Color.Lerp(weights1, weights3, b);

        Vector3 indices;
        indices.x = beginCell.index;
        indices.y = leftCell.index;
        indices.z = rightCell.index;

        TriangulateBoundaryTriangle(
            begin, weights1, left, weights3, boundary, boundaryWeights, indices
        );

        if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
        {
            TriangulateBoundaryTriangle(
                left, weights2, right, weights3, boundary, boundaryWeights, indices
            );
        }
        else
        {
            terrain.AddTriangle(left, right, boundary);
            terrain.AddTriangleCellData(
                indices, weights2, weights3, boundaryWeights
            );
        }
    }

    void TriangulateBoundaryTriangle(Vector3 begin, Color beginWeights, Vector3 left, Color leftWeights, Vector3 boundary, Color boundaryWeights, Vector3 indices)
    {
        Vector3 v2 = HexMetrics.TerraceLerp(begin, left, 1);
        Color w2 = HexMetrics.TerraceLerp(beginWeights, leftWeights, 1);

        terrain.AddTriangle(begin, v2, boundary);
        terrain.AddTriangleCellData(indices, beginWeights, w2, boundaryWeights);

        for (int i = 2; i < HexMetrics.terraceSteps; i++)
        {
            Vector3 v1 = v2;
            Color w1 = w2;
            v2 = HexMetrics.TerraceLerp(begin, left, i);
            w2 = HexMetrics.TerraceLerp(beginWeights, leftWeights, i);
            terrain.AddTriangle(v1, v2, boundary);
            terrain.AddTriangleCellData(indices, w1, w2, boundaryWeights);
        }

        terrain.AddTriangle(v2, left, boundary);
        terrain.AddTriangleCellData(indices, w2, leftWeights, boundaryWeights);

    }

    void TriangulateCornerCliffTerraces(Vector3 begin, HexCell beginCell, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
    {
        float b = 1f / (leftCell.Elevation - beginCell.Elevation);
        if (b < 0)
        {
            b = -b;
        }

        Vector3 boundary = Vector3.Lerp(begin, left, b);
        Color boundaryWeights = Color.Lerp(weights1, weights2, b);

        Vector3 indices;
        indices.x = beginCell.index;
        indices.y = leftCell.index;
        indices.z = rightCell.index;

        TriangulateBoundaryTriangle(
            right, weights3, begin, weights1, boundary, boundaryWeights, indices
        );

        if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
        {
            TriangulateBoundaryTriangle(
                left, weights2, right, weights3, boundary, boundaryWeights, indices
            );
        }
        else
        {
            terrain.AddTriangle(left, right, boundary);
            terrain.AddTriangleCellData(indices, weights2, weights3, boundaryWeights);
        }
    }

    //三角锥形
    void Triangulatecone(Vector3 bottom, HexCell bottomCell)
    {
    }

    // 画没有河道的边
    void TriangulateWithNoRiver(HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e)
    {
        Vector3 centerL = (center + e.v1) * 0.5f;
        Vector3 centerR = (center + e.v5) * 0.5f;
        Vector3 centerDown = (centerL + centerR) * 0.5f;

        //Vector3 river_centerL = center + HexMetrics.GetFirstSolidCorner(direction.Previous()) * 0.25f;
        //Vector3 river_centerR = center + HexMetrics.GetSecondSolidCorner(direction.Next()) * 0.25f;

        EdgeVertices m = new EdgeVertices(Vector3.Lerp(center, e.v1, 0.5f), Vector3.Lerp(center, e.v5, 0.5f));
        m.v3.y = e.v3.y;
        
        //TriangulateEdgeStrip(m, cell.Color, e, cell.Color);

        terrain.AddTriangle(e.v1, e.v2, m.v1);
        terrain.AddQuad(e.v2, m.v1, e.v3, m.v3);
        terrain.AddQuad(e.v3, m.v3, e.v4, m.v5);
        terrain.AddTriangle(e.v4, e.v5, m.v5);

        Vector3 indices;
        indices.x = indices.y = indices.z = cell.index;
        terrain.AddTriangleCellData(indices, weights1);
        terrain.AddQuadCellData(indices, weights1);
        terrain.AddQuadCellData(indices, weights1);
        terrain.AddTriangleCellData(indices, weights1);
    }


    // 画河道
    void TriangulateWithRiver(HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e)
    {
        EdgeVertices m = new EdgeVertices(Vector3.Lerp(center, e.v1, 0.5f), Vector3.Lerp(center, e.v5, 0.5f));
        m.v3.y = e.v3.y;

        Vector3 dir = e.v2 - m.v1;


        TriangulateWithNoRiver(direction, cell, center, e);

        if (!cell.IsLakes())
        {
            Vector3 indices;
            indices.x = indices.y = indices.z = cell.index;

            bool reversed = cell.IsInComingRiverDirection(direction);
            float v = 0.4f;
            if (!reversed)
            {
                v = 0.6f;
            }
            TriangulateRiverQuad(e.v1 - dir, e.v5 - dir, e.v1, e.v5, cell.RiverSurfaceY, cell.RiverSurfaceY, v, reversed, indices);
        }

    }

    // 画河道开始和结束
    void TriangulateWithRiverBeginOrEnd(HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e)
    {
        //Debug.Log("TriangulateWithRiverBeginOrEnd");

        EdgeVertices m = new EdgeVertices(Vector3.Lerp(center, e.v1, 0.5f), Vector3.Lerp(center, e.v5, 0.5f));
        m.v3.y = e.v3.y;

        TriangulateEdgeStrip(m, weights1, cell.index, e, weights1, cell.index);
        TriangulateEdgeFan(center, m, cell.index);

        if (!cell.IsLakes())
        {
            //Debug.Log("!cell.IsLakes");
            bool reversed = cell.IsInComingRiverDirection(direction);

            Vector3 indices;
            indices.x = indices.y = indices.z = cell.index;

            e.v1.y = e.v5.y = cell.RiverSurfaceY;
            rivers.AddTriangle(center, e.v1, e.v5);
            
            if (reversed)
            {
                rivers.AddTriangleUV(
                    new Vector2(0.5f, 0.4f),
                    new Vector2(1f, 0.2f), new Vector2(0f, 0.2f)
                );
            }
            else
            {
                rivers.AddTriangleUV(
                    new Vector2(0.5f, 0.4f),
                    new Vector2(0f, 0.6f), new Vector2(1f, 0.6f)
                );
            }
            rivers.AddTriangleCellData(indices, weights1);
        }

        //TriangulateRiverQuad(
        //    m.v2, m.v4, e.v2, e.v4, cell.RiverSurfaceY, cell.RiverSurfaceY, 0f, reversed
        //);
    }

    // 需要修改，有问题
    // 渲染格子中心多段河流连接处
    void TriangulateWithRiverConnection(HexCell cell)
    {
        int riverCount = cell.GetRiverCount();
        if (riverCount <= 1)
        {
            return;
        }
        // 交点
        Vector3[] intersections = new Vector3[riverCount + 1];
        Vector2[] intersectionRiver = new Vector2[riverCount + 1];

        Vector3 center = cell.transform.localPosition;
        Vector3 centerD = new Vector3(center.x, cell.StreamBedY, center.z);

        HexDirection firstRiverDir = HexDirection.NE;
        Vector3 leftPoint = center + HexMetrics.GetFirstSolidCorner(firstRiverDir) * 0.5f;
        Vector3 rightPoint = center + HexMetrics.GetSecondSolidCorner(firstRiverDir) * 0.5f;
        Vector3 briDir = HexMetrics.GetBridge(firstRiverDir);

        // 当前河流方向中间低位置
        Vector3 riverDPoint = (leftPoint + rightPoint) / 2;
        riverDPoint.y = centerD.y;

        Vector3 riverCenter = new Vector3(center.x, cell.RiverSurfaceY, center.z);

        List<HexDirection> riverDir = new List<HexDirection>();
        

        // 先找一条河流
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            RiverDirection dir = cell.GetRiverDirection(d);
            if(dir != RiverDirection.Null)
            {
                firstRiverDir = d;
                leftPoint = center + HexMetrics.GetFirstSolidCorner(d) * 0.5f;
                rightPoint = center + HexMetrics.GetSecondSolidCorner(d) * 0.5f;

                riverDPoint = (leftPoint + rightPoint) / 2;
                riverDPoint.y = centerD.y;

                briDir = HexMetrics.GetBridge(d);
                briDir = briDir;

                riverDir.Add(d);
                break;
            }
        }

        // shader使用数据
        Vector3 indices;
        indices.x = indices.y = indices.z = cell.index;

        Vector3 lastPoint = leftPoint;
        List<Vector3> points = new List<Vector3>();
        points.Add(rightPoint);

        List<Vector3> pos = new List<Vector3>();
        pos.Add(riverDPoint);
        pos.Add(rightPoint);

        int intersection_index = 1;
        List<Vector3> riverPoint = new List<Vector3>();
        List<Vector3> riverPointUV = new List<Vector3>();
        //riverPoint.Add(leftPoint);


        //  以第一条河流为起始，循环一圈
        HexDirection nextDir = firstRiverDir;
        RiverDirection beforRiverDir = cell.GetRiverDirection(nextDir);
        for (int i = 0; i < 6; i++)
        {
            nextDir = nextDir.Next();

            // 找到下一个河流
            RiverDirection nextRiverDir = cell.GetRiverDirection(nextDir);
            if (nextRiverDir != RiverDirection.Null)
            {
                Vector3 nextLeftPoint = center + HexMetrics.GetFirstSolidCorner(nextDir) * 0.5f;
                Vector3 nextRightPoint = center + HexMetrics.GetSecondSolidCorner(nextDir) * 0.5f;
                Vector3 nextDPoint = (nextLeftPoint + nextRightPoint) / 2;
                nextDPoint.y = centerD.y;

                Vector3 nextBriDir = HexMetrics.GetBridge(nextDir);
                nextBriDir = nextBriDir;

                Vector3 Intersection = Point.GetRayIntersection(rightPoint, briDir, nextLeftPoint, nextBriDir);
                intersections[intersection_index] = Intersection;

                // 表示河流流向和速度
                if (beforRiverDir == RiverDirection.Incoming || nextRiverDir == RiverDirection.Incoming)
                {
                    if(beforRiverDir == RiverDirection.Outgoing || nextRiverDir == RiverDirection.Outgoing)
                    {
                        intersectionRiver[intersection_index] = new Vector2(0.5f, 0.5f);
                    }
                    else
                    {
                        intersectionRiver[intersection_index] = new Vector2(0.4f, 0.4f);
                    }
                }
                else
                {
                    intersectionRiver[intersection_index] = new Vector2(0.6f, 0.6f);
                }
                //intersectionRiver[intersection_index] = new Vector2(
                //    beforRiverDir == RiverDirection.Incoming ? 0.4f : 0.6f,
                //    nextRiverDir == RiverDirection.Incoming ? 0.4f : 0.6f
                //    );

                intersection_index++;


                riverPoint.Add(riverCenter);
                riverPoint.Add(rightPoint);
                riverPoint.Add(Intersection);

                if (beforRiverDir == RiverDirection.Incoming)
                {
                    riverPointUV.Add(new Vector2(0.6f, 0.6f));
                    riverPointUV.Add(new Vector2(0.4f, 0.4f));
                    riverPointUV.Add(new Vector2(0.6f, 0.6f));
                }
                else
                {
                    riverPointUV.Add(new Vector2(0.2f, 0.2f));
                    riverPointUV.Add(new Vector2(0.4f, 0.4f));
                    riverPointUV.Add(new Vector2(0.2f, 0.2f));
                }


                leftPoint = nextLeftPoint;
                rightPoint = nextRightPoint;

                //riverPoint.Add(nextLeftPoint);
                //riverPoint.Add(nextRightPoint);

                // points.Add(rightPoint);

                // 画平面上的地
                for (int j = 0; j < points.Count - 1; j++)
                {
                    Vector3 v1 = points[j];
                    Vector3 v2 = Intersection;
                    Vector3 v3 = points[j + 1];

                    // 汇合处顶部多余部分
                    terrain.AddTriangle(v1, v3, v2);
                    terrain.AddTriangleCellData(indices, weights1);
                }

                points.Clear();
                points.Add(rightPoint);


                pos.Add(Intersection);
                pos.Add(nextLeftPoint);
                pos.Add(nextDPoint);

                // 画河道两边的坡
                for (int j = 0; j < pos.Count - 1; j++)
                {
                    Vector3 v1 = centerD;
                    Vector3 v2 = pos[j];
                    Vector3 v3 = pos[j + 1];

                    // 汇合处顶部多余部分
                    terrain.AddTriangle(v1, v2, v3);
                    terrain.AddTriangleCellData(indices, weights1);
                }

                pos.Clear();
                pos.Add(nextDPoint);
                pos.Add(rightPoint);


                briDir = nextBriDir;

                riverDir.Add(nextDir);

                beforRiverDir = nextRiverDir;



                Vector3 riverV2 = new Vector3(nextLeftPoint.x, cell.RiverSurfaceY, nextLeftPoint.z);
                Vector3 riverV3 = new Vector3(rightPoint.x, cell.RiverSurfaceY, rightPoint.z);

                Vector2 UV = new Vector2(0.4f, 0.4f);
                Vector2 EndUV = new Vector2(0.2f, 0.2f);
                if (nextRiverDir == RiverDirection.Incoming)
                {
                    UV = new Vector2(0.4f, 0.4f);
                    EndUV = new Vector2(0.6f, 0.6f);
                }

                if (!cell.IsUnderwater)
                {
                    rivers.AddTriangle(riverCenter, riverV2, riverV3);
                    rivers.AddTriangleUV(EndUV, UV, UV);
                    rivers.AddTriangleCellData(indices, weights1);
                }
                

                riverPoint.Add(riverCenter);
                riverPoint.Add(Intersection);
                riverPoint.Add(nextLeftPoint);

                riverPointUV.Add(EndUV);
                riverPointUV.Add(EndUV);
                riverPointUV.Add(UV);
            }
            else
            {
                Vector3 nextLeftPoint = center + HexMetrics.GetFirstSolidCorner(nextDir) * 0.5f;
                Vector3 nextRightPoint = center + HexMetrics.GetSecondSolidCorner(nextDir) * 0.5f;

                points.Add(nextRightPoint);
            }
        }
        
        //terrain.AddTriangle(centerD, pos[0], pos[1]);
        //terrain.AddTriangleCellData(indices, weights1);

        intersections[0] = intersections[intersection_index - 1];
        intersectionRiver[0] = intersectionRiver[intersection_index - 1];


        for (int i = 0; i < riverPoint.Count - 2;)
        {
            Vector3 v1 = riverPoint[i];
            Vector3 v2 = riverPoint[i + 1];
            Vector3 v3 = riverPoint[i + 2];

            if (!cell.IsUnderwater) { 
                rivers.AddTriangle(v1, new Vector3(v2.x, cell.RiverSurfaceY, v2.z),
                    new Vector3(v3.x, cell.RiverSurfaceY, v3.z));
                rivers.AddTriangleUV(riverPointUV[i], riverPointUV[i + 1], riverPointUV[i + 2]);
                rivers.AddTriangleCellData(indices, weights1);
            }

            i = i + 3;
        }

        //for (int i = 0; i < cell.GetRiverCount(); i++)
        //{
        //    Vector3 v1 = riverPoint[i * 2];
        //    Vector3 v2 = riverPoint[i * 2 + 1];

        //    Vector3 v3 = intersections[i];
        //    Vector3 v4 = intersections[i + 1];

        //    // v1.y = v2.y = v3.y = v4.y = cell.RiverSurfaceY;

        //    HexDirection dir = riverDir[i];
        //    bool reversed = cell.IsInComingRiverDirection(dir);
        //    TriangulateRiverQuad(v3, v4, v1, v2, cell.RiverSurfaceY, cell.RiverSurfaceY, 0.6f, reversed, indices);

        //}

        //if (intersections.Length > 3)
        //{
        //    for (int i = 0; i < intersections.Length - 2; i++)
        //    {
        //        Vector3 v1 = intersections[i];
        //        Vector3 v2 = intersections[i + 1];

        //        Vector3 v3 = intersections[i + 2];

        //        v1.y = v2.y = v3.y = cell.RiverSurfaceY;

        //        rivers.AddTriangle(v1, v2, v3);
        //        rivers.AddTriangleUV(intersectionRiver[i], intersectionRiver[i + 1], intersectionRiver[i + 2]);
        //        rivers.AddTriangleCellData(indices, weights1);
        //    }
        //}

        return;
        /**
        //Debug.Log("========================");
        Vector3 center = cell.transform.localPosition;
        List<HexDirection> incoming_rivers = cell.GetRiverDirections(RiverDirection.Incoming);
        List<HexDirection> outgoing_rivers = cell.GetRiverDirections(RiverDirection.Outgoing);

        HexDirection outgoing_dir = outgoing_rivers[0];

        // 有2个出水口，则一定是间隔一个的
        if(outgoing_rivers.Count == 2)
        {
            TriangulateWithThreeRiverTwoOutConnection(cell, incoming_rivers[0], outgoing_rivers);
        }
        // 2条河流，必定是 一条流入，一条流出
        else if (cell.GetRiverCount() == 2)
        {
            TriangulateWithTwoRiverConnection(cell, incoming_rivers, outgoing_dir);
        }
        else
        {
            // 寻找唯一没有入水口的方向 出水口周围2个方向不可能是进水口，剩下3个有2个以上的入水口
            HexDirection landDir = outgoing_dir;
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                // 不是出水口邻边
                if (d != outgoing_dir.Previous() && d != outgoing_dir.Next() && d != outgoing_dir)
                {
                    // 且不是入水口
                    bool isInRiver = false;
                    foreach (HexDirection dir in incoming_rivers)
                    {
                        if (dir == d)
                        {
                            isInRiver = true;
                            break;
                        }
                    }

                    if (!isInRiver)
                    {
                        landDir = d;
                    }
                }
            }

            //Debug.Log(string.Format(" landDir ({0}) out ({1}) ", landDir, outgoing_dir));

            // 有不是入水口的位置
            if (landDir != outgoing_dir && incoming_rivers.Count == 2)
            {
                Debug.Log(string.Format("^^^^^^{0}", incoming_rivers.Count));
                TriangulateWithThreeRiverConnection(cell, incoming_rivers, outgoing_dir, landDir);
            }
            else if(incoming_rivers.Count == 3) // 总共4条河流，3个入水口
            {
                TriangulateWithFourRiverConnection(cell, outgoing_dir);
            }
        }
        */
    }

    // 渲染格子中心3段河流2条流出连接处
    void TriangulateWithThreeRiverTwoOutConnection(HexCell cell, HexDirection incoming_river, List<HexDirection> outgoing_dirs)
    {
        Debug.Log("==================");
        Vector3 center = cell.transform.localPosition;
        EdgeVertices inEdge = new EdgeVertices(center + HexMetrics.GetFirstSolidCorner(incoming_river) * 0.5f, center + HexMetrics.GetSecondSolidCorner(incoming_river) * 0.5f);
        inEdge.v3.y = cell.StreamBedY;

        //EdgeVertices e = new EdgeVertices(center + HexMetrics.GetFirstSolidCorner(landDir) * 0.5f, center + HexMetrics.GetSecondSolidCorner(landDir) * 0.5f);

        Vector3 center_d = new Vector3(center.x, cell.StreamBedY, center.z);
        // d1,d2使按序列的 d1 一定比 d2小
        HexDirection d1 = outgoing_dirs[0];
        HexDirection d2 = outgoing_dirs[1];

        EdgeVertices d1e = new EdgeVertices(center + HexMetrics.GetFirstSolidCorner(d1) * 0.5f, center + HexMetrics.GetSecondSolidCorner(d1) * 0.5f);
        d1e.v3.y = cell.StreamBedY;

        EdgeVertices d2e = new EdgeVertices(center + HexMetrics.GetFirstSolidCorner(d2) * 0.5f, center + HexMetrics.GetSecondSolidCorner(d2) * 0.5f);
        d2e.v3.y = cell.StreamBedY;

        if (incoming_river.Next().Next() == d2)
        {
            HexDirection _d = d1;
            d1 = d2;
            d2 = _d;

            EdgeVertices _de = d1e;
            d1e = d2e;
            d2e = _de;
        }

        Vector3 v1 = Vector3.Lerp(d1e.v5, d2e.v1, 0.5f);
        Vector3 inJoinV = Vector3.Lerp(v1, center, 0.25f);

        Vector3 indices;
        indices.x = indices.y = indices.z = cell.index;

        // 汇合处顶部多余部分
        terrain.AddTriangle(inJoinV, d1e.v5, d2e.v1);
        terrain.AddTriangleCellData(indices, weights1);

        terrain.AddQuad(d1e.v5, d1e.v3, inJoinV, center_d);
        terrain.AddQuadCellData(indices, weights1);
        terrain.AddQuad(inJoinV, center_d, d2e.v1, d2e.v3);
        terrain.AddQuadCellData(indices, weights1);

        terrain.AddTriangle(center_d, inEdge.v3, d1e.v3);
        terrain.AddTriangleCellData(indices, weights1);
        terrain.AddTriangle(center_d, d2e.v3, inEdge.v3);
        terrain.AddTriangleCellData(indices, weights1);

        terrain.AddQuad(d2e.v5, d2e.v3, inEdge.v1, inEdge.v3);
        terrain.AddQuadCellData(indices, weights1);
        terrain.AddQuad(inEdge.v5, inEdge.v3, d1e.v1, d1e.v3);
        terrain.AddQuadCellData(indices, weights1);

        if( !cell.IsUnderwater)
        {
            //河流
            //TriangulateRiverTriangle()
            //rivers.AddTriangle(inJoinV, outEdge.v5, d1e.v1);
            Vector3 inJoinVH = Vector3.Lerp(inJoinV, center_d, 0.5f);
            TriangulateRiverQuad(inEdge.v4, inJoinVH, d1e.v1, d1e.v5, cell.RiverSurfaceY, cell.RiverSurfaceY, 0.4f, false, indices);
            TriangulateRiverQuad(inJoinVH, inEdge.v2, d2e.v1, d2e.v5, cell.RiverSurfaceY, cell.RiverSurfaceY, 0.4f, false, indices);

            rivers.AddTriangle(inJoinVH,
                new Vector3(inEdge.v2.x, cell.RiverSurfaceY, inEdge.v2.z),
                new Vector3(inEdge.v4.x, cell.RiverSurfaceY, inEdge.v4.z));
            rivers.AddTriangleUV(new Vector2(0.6f, 0.6f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            rivers.AddTriangleCellData(indices, weights1);
        }
    }

    // 渲染格子中心2段河流连接处
    void TriangulateWithTwoRiverConnection(HexCell cell, List<HexDirection> incoming_rivers, HexDirection outgoing_dir)
    {
        Vector3 center = cell.transform.localPosition;

        EdgeVertices outEdge = new EdgeVertices(center + HexMetrics.GetFirstSolidCorner(outgoing_dir) * 0.5f, center + HexMetrics.GetSecondSolidCorner(outgoing_dir) * 0.5f);
        outEdge.v3.y = cell.StreamBedY;

        HexDirection incoming_dir = incoming_rivers[0];
        EdgeVertices inEdge = new EdgeVertices(center + HexMetrics.GetFirstSolidCorner(incoming_dir) * 0.5f, center + HexMetrics.GetSecondSolidCorner(incoming_dir) * 0.5f);

        inEdge.v3.y = outEdge.v3.y = cell.StreamBedY;

        Vector3 indices;
        indices.x = indices.y = indices.z = cell.index;

        // 2侧河道
        terrain.AddQuad(inEdge.v1, outEdge.v5, inEdge.v3, outEdge.v3);
        terrain.AddQuadCellData(indices, weights1);
        terrain.AddQuad(inEdge.v3, outEdge.v3, inEdge.v5, outEdge.v1);
        terrain.AddQuadCellData(indices, weights1);;

        if ( !cell.IsUnderwater )
        {
            //河流
            TriangulateRiverQuad(inEdge.v5, inEdge.v1, outEdge.v1, outEdge.v5, cell.RiverSurfaceY, cell.RiverSurfaceY, 0.4f, false, indices);
        }

        // 如果是对边
        if (incoming_dir.Opposite() == outgoing_dir)
        {
            Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(incoming_dir.Previous()) * 0.5f;
            Vector3 v2 = center + HexMetrics.GetFirstSolidCorner(outgoing_dir.Previous()) * 0.5f;

            // 补足非河道的高地
            terrain.AddTriangle(v1, inEdge.v1, outEdge.v5);
            terrain.AddTriangleCellData(indices, weights1);

            terrain.AddTriangle(inEdge.v5, v2, outEdge.v1);
            terrain.AddTriangleCellData(indices, weights1);
        }
        else
        {
            EdgeVertices e1 = new EdgeVertices(center + HexMetrics.GetFirstSolidCorner(incoming_dir.Opposite()) * 0.5f, center + HexMetrics.GetSecondSolidCorner(incoming_dir.Opposite()) * 0.5f);
            EdgeVertices e2 = new EdgeVertices(center + HexMetrics.GetFirstSolidCorner(outgoing_dir.Opposite()) * 0.5f, center + HexMetrics.GetSecondSolidCorner(outgoing_dir.Opposite()) * 0.5f);

            // 补足非河道的高地
            terrain.AddQuad(e1.v1, e2.v5, e1.v5, e2.v1);
            terrain.AddQuadCellData(indices, weights1);
        }
    }

    // 渲染格子中心3段河流连接处
    void TriangulateWithThreeRiverConnection(HexCell cell, List<HexDirection> incoming_rivers, HexDirection outgoing_dir, HexDirection landDir)
    {
        Vector3 center = cell.transform.localPosition;
        EdgeVertices outEdge = new EdgeVertices(center + HexMetrics.GetFirstSolidCorner(outgoing_dir) * 0.5f, center + HexMetrics.GetSecondSolidCorner(outgoing_dir) * 0.5f);
        outEdge.v3.y = cell.StreamBedY;

        EdgeVertices e = new EdgeVertices(center + HexMetrics.GetFirstSolidCorner(landDir) * 0.5f, center + HexMetrics.GetSecondSolidCorner(landDir) * 0.5f);

        Vector3 center_d = new Vector3(center.x, cell.StreamBedY, center.z);
        // d1,d2使按序列的 d1 一定比 d2小
        HexDirection d1 = incoming_rivers[0];
        HexDirection d2 = incoming_rivers[1];

        EdgeVertices d1e = new EdgeVertices(center + HexMetrics.GetFirstSolidCorner(d1) * 0.5f, center + HexMetrics.GetSecondSolidCorner(d1) * 0.5f);
        d1e.v3.y = cell.StreamBedY;

        EdgeVertices d2e = new EdgeVertices(center + HexMetrics.GetFirstSolidCorner(d2) * 0.5f, center + HexMetrics.GetSecondSolidCorner(d2) * 0.5f);
        d2e.v3.y = cell.StreamBedY;

        Vector3 indices;
        indices.x = indices.y = indices.z = cell.index;

        // 在对边
        if (landDir == outgoing_dir.Opposite())
        {
            if (landDir.Previous() == d2)
            {
                HexDirection _d = d1;
                d1 = d2;
                d2 = _d;

                EdgeVertices _de = d1e;
                d1e = d2e;
                d2e = _de;
            }

            Vector3 v1 = Vector3.Lerp(d1e.v5, d2e.v1, 0.5f);
            Vector3 inJoinV = Vector3.Lerp(v1, center, 0.25f);

            // 汇合处顶部多余部分
            terrain.AddTriangle(inJoinV, d1e.v5, d2e.v1);
            terrain.AddTriangleCellData(indices, weights1);

            terrain.AddQuad(d1e.v5, d1e.v3, inJoinV, center_d);
            terrain.AddQuadCellData(indices, weights1);
            terrain.AddQuad(inJoinV, center_d, d2e.v1, d2e.v3);
            terrain.AddQuadCellData(indices, weights1);

            terrain.AddTriangle(center_d, outEdge.v3, d1e.v3);
            terrain.AddTriangleCellData(indices, weights1);
            terrain.AddTriangle(center_d, d2e.v3, outEdge.v3);
            terrain.AddTriangleCellData(indices, weights1);

            terrain.AddQuad(d2e.v5, d2e.v3, outEdge.v1, outEdge.v3);
            terrain.AddQuadCellData(indices, weights1);
            terrain.AddQuad(outEdge.v5, outEdge.v3, d1e.v1, d1e.v3);
            terrain.AddQuadCellData(indices, weights1);

            if ( !cell.IsUnderwater )
            {
                //河流
                //TriangulateRiverTriangle()
                //rivers.AddTriangle(inJoinV, outEdge.v5, d1e.v1);
                Vector3 inJoinVH = Vector3.Lerp(inJoinV, center_d, 0.5f);
                TriangulateRiverQuad(d1e.v5, d1e.v1, inJoinVH, outEdge.v4, cell.RiverSurfaceY, cell.RiverSurfaceY, 0.4f, false, indices);
                TriangulateRiverQuad(d2e.v5, d2e.v1, outEdge.v2, inJoinVH, cell.RiverSurfaceY, cell.RiverSurfaceY, 0.4f, false, indices);

                rivers.AddTriangle(inJoinVH,
                    new Vector3(outEdge.v2.x, cell.RiverSurfaceY, outEdge.v2.z),
                    new Vector3(outEdge.v4.x, cell.RiverSurfaceY, outEdge.v4.z));
                rivers.AddTriangleUV(new Vector2(0.5f, 0.5f), new Vector2(0.6f, 0.6f), new Vector2(0.6f, 0.6f));
                rivers.AddTriangleCellData(indices, weights1);
            }
        }
        else
        {
            if (d2.Next() == d1)
            {
                HexDirection _d = d1;
                d1 = d2;
                d2 = _d;

                EdgeVertices _de = d1e;
                d1e = d2e;
                d2e = _de;
            }

            Vector3 v = outEdge.v1;
            // 如果是在出水口的下2个，则坐标用v1
            if (outgoing_dir.Next().Next() == landDir)
            {
                v = outEdge.v5;

                terrain.AddTriangle(center_d, d2e.v3, outEdge.v3);
                terrain.AddTriangleCellData(indices, weights1);

                terrain.AddQuad(d2e.v3, outEdge.v3, d2e.v5, outEdge.v1);
                terrain.AddQuadCellData(indices, weights1);
                terrain.AddQuad(d1e.v1, outEdge.v5, d1e.v3, outEdge.v3);
                terrain.AddQuadCellData(indices, weights1);

                if ( !cell.IsUnderwater )
                {
                    //河流 d1为对边
                    Vector3 v1 = Vector3.Lerp(d1e.v5, d2e.v1, 0.5f);
                    Vector3 inJoinV = Vector3.Lerp(v1, center, 0.25f);
                    Vector3 inJoinVH = Vector3.Lerp(inJoinV, center_d, 0.5f);

                    TriangulateRiverQuad(d1e.v4, d1e.v2, outEdge.v2, outEdge.v4, cell.RiverSurfaceY, cell.RiverSurfaceY, 0.4f, false, indices);
                    TriangulateRiverQuad(d2e.v4, d2e.v2, outEdge.v2, inJoinVH, cell.RiverSurfaceY, cell.RiverSurfaceY, 0.4f, false, indices);
                }

            }
            else
            {
                terrain.AddTriangle(center_d, outEdge.v3, d1e.v3);
                terrain.AddTriangleCellData(indices, weights1);

                terrain.AddQuad(d2e.v3, outEdge.v3, d2e.v5, outEdge.v1);
                terrain.AddQuadCellData(indices, weights1);
                terrain.AddQuad(d1e.v1, outEdge.v5, d1e.v3, outEdge.v3);
                terrain.AddQuadCellData(indices, weights1);

                if ( !cell.IsUnderwater )
                {
                    //河流
                    Vector3 inJoinVH = Vector3.Lerp(d1e.v5, center_d, 0.5f);

                    TriangulateRiverQuad(d2e.v4, d2e.v2, outEdge.v2, outEdge.v4, cell.RiverSurfaceY, cell.RiverSurfaceY, 0.4f, false, indices);
                    TriangulateRiverQuad(d1e.v4, d1e.v2, inJoinVH, outEdge.v4, cell.RiverSurfaceY, cell.RiverSurfaceY, 0.4f, false, indices);
                }
            }

            terrain.AddTriangle(e.v1, e.v5, v);
            terrain.AddTriangleCellData(indices, weights1);

            //terrain.AddQuad(d2e.v3, outEdge.v3, d2e.v5, outEdge.v1);
            //terrain.AddQuadColor(cell.Color);
            //terrain.AddQuad(d1e.v1, outEdge.v5, d1e.v3, outEdge.v3);
            //terrain.AddQuadColor(cell.Color);

            terrain.AddTriangle(center_d, d1e.v3, d1e.v5);
            terrain.AddTriangleCellData(indices, weights1);
            terrain.AddTriangle(center_d, d2e.v1, d2e.v3);
            terrain.AddTriangleCellData(indices, weights1);

            // 不上的三角
            //terrain.AddTriangle(center_d, d.v3, outEdge.v3);
            //terrain.AddTriangleColor(cell.Color);


        }
    }

    // 渲染格子中心4段河流连接处
    void TriangulateWithFourRiverConnection(HexCell cell, HexDirection outgoing_dir)
    {
        Vector3 center = cell.transform.localPosition;
        Vector3 center_d = new Vector3(center.x, cell.StreamBedY, center.z);

        HexDirection d1 = outgoing_dir.Next().Next();
        HexDirection d2 = d1.Next();
        HexDirection d3 = d2.Next();

        EdgeVertices d1e = new EdgeVertices(center + HexMetrics.GetFirstSolidCorner(d1) * 0.5f, center + HexMetrics.GetSecondSolidCorner(d1) * 0.5f);
        EdgeVertices d2e = new EdgeVertices(center + HexMetrics.GetFirstSolidCorner(d2) * 0.5f, center + HexMetrics.GetSecondSolidCorner(d2) * 0.5f);
        EdgeVertices d3e = new EdgeVertices(center + HexMetrics.GetFirstSolidCorner(d3) * 0.5f, center + HexMetrics.GetSecondSolidCorner(d3) * 0.5f);
        EdgeVertices outEdge = new EdgeVertices(center + HexMetrics.GetFirstSolidCorner(outgoing_dir) * 0.5f, center + HexMetrics.GetSecondSolidCorner(outgoing_dir) * 0.5f);
        outEdge.v3.y = d3e.v3.y = d2e.v3.y = d1e.v3.y = cell.StreamBedY;

        Vector3 indices;
        indices.x = indices.y = indices.z = cell.index;

        terrain.AddQuad(outEdge.v5, outEdge.v3, d1e.v1, d1e.v3);
        terrain.AddQuadCellData(indices, weights1);
        //terrain.AddTriangle(d1e.v1, d1e.v3, center_d);
        //terrain.AddTriangleColor(cell.Color);
        terrain.AddTriangle(d1e.v3, d1e.v5, center_d);
        terrain.AddTriangleCellData(indices, weights1);

        terrain.AddTriangle(d2e.v1, d2e.v3, center_d);
        terrain.AddTriangleCellData(indices, weights1);
        terrain.AddTriangle(d2e.v3, d2e.v5, center_d);
        terrain.AddTriangleCellData(indices, weights1);

        terrain.AddTriangle(d3e.v1, d3e.v3, center_d);
        terrain.AddTriangleCellData(indices, weights1);
        //terrain.AddTriangle(d3e.v3, d3e.v5, center_d);
        //terrain.AddTriangleColor(cell.Color);

        terrain.AddQuad(d3e.v5, d3e.v3, outEdge.v1, outEdge.v3);
        terrain.AddQuadCellData(indices, weights1);

        terrain.AddTriangle(center_d, d3e.v3, outEdge.v3);
        terrain.AddTriangleCellData(indices, weights1);

        terrain.AddTriangle(center_d, outEdge.v3, d1e.v3);
        terrain.AddTriangleCellData(indices, weights1);

        if ( !cell.IsUnderwater )
        {
            //河流
            TriangulateRiverQuad(d1e.v4, d1e.v2, Vector3.Lerp(d1e.v5, center_d, 0.5f), outEdge.v4, cell.RiverSurfaceY, cell.RiverSurfaceY, 0.4f, false, indices);
            TriangulateRiverQuad(d3e.v4, d3e.v2, outEdge.v2, Vector3.Lerp(d3e.v1, center_d, 0.5f), cell.RiverSurfaceY, cell.RiverSurfaceY, 0.4f, false, indices);
            TriangulateRiverQuad(d2e.v4, d2e.v2, outEdge.v2, outEdge.v4, cell.RiverSurfaceY, cell.RiverSurfaceY, 0.4f, false, indices);
        }

    }

    // 三角化方法以便在一个单元的中心和一条边之间创建一个三角扇形
    // 某方向的边  插入 4个4边行
    void TriangulateEdgeFan(Vector3 center, EdgeVertices edge, float index)
    {
        terrain.AddTriangle(center, edge.v1, edge.v2);
        terrain.AddTriangle(center, edge.v2, edge.v3);
        terrain.AddTriangle(center, edge.v3, edge.v4);
        terrain.AddTriangle(center, edge.v4, edge.v5);

        Vector3 indices;
        indices.x = indices.y = indices.z = index;
        terrain.AddTriangleCellData(indices, weights1);
        terrain.AddTriangleCellData(indices, weights1);
        terrain.AddTriangleCellData(indices, weights1);
        terrain.AddTriangleCellData(indices, weights1);

    }

    // 对两条边之间一条四边形进行三角化的方法
    void TriangulateEdgeStrip(EdgeVertices e1, Color w1, float index1, EdgeVertices e2, Color w2, float index2, bool hasRoad = false)
    {
        terrain.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
        terrain.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
        terrain.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
        terrain.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);

        Vector3 indices;
        indices.x = indices.z = index1;
        indices.y = index2;
        terrain.AddQuadCellData(indices, w1, w2);
        terrain.AddQuadCellData(indices, w1, w2);
        terrain.AddQuadCellData(indices, w1, w2);
        terrain.AddQuadCellData(indices, w1, w2);

        if (hasRoad)
        {
            TriangulateRoadSegment(e1.v2, e1.v3, e1.v4, e2.v2, e2.v3, e2.v4);
        }
    }

    void TriangulateRiverQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Vector3 indices)
    {
        rivers.AddQuad(v1, v2, v3, v4);
        rivers.AddQuadUV(0f, 1f, 0f, 1f);
        rivers.AddQuadCellData(indices, weights1, weights2);
    }

    void TriangulateRiverQuad( Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, float y1, float y2, float v, bool reversed, Vector3 indices)
    {
        v1.y = v2.y = y1;
        v3.y = v4.y = y2;
        rivers.AddQuad(v1, v2, v3, v4);
        if (reversed)
        {
            rivers.AddQuadUV(1f, 0f, 0.8f - v, 0.6f - v);
        }
        else
        {
            rivers.AddQuadUV(0f, 1f, v, v + 0.2f);
        }
        rivers.AddQuadCellData(indices, weights1, weights2);
    }

    void TriangulateRiverTriangle(Vector3 v1, Vector3 v2, Vector3 v3, float y, Vector3 indices)
    {
        v1.y = v2.y = v3.y = y;
        rivers.AddTriangle(v1, v2, v3);
        //rivers.AddQuadColor(Color);
        rivers.AddTriangleUV(new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector3(0f, 1f));
        rivers.AddTriangleCellData(indices, weights1);
    }


    // 渲染湖泊
    void TriangulateWater(HexDirection direction, HexCell cell, Vector3 center)
    {
        // 中心
        center.y = cell.LakesSurfaceY;

        HexCell neighbor = cell.GetNeighbor(direction);
        if (neighbor != null && !neighbor.IsLakes())
        {
            TriangulateWaterShore(direction, cell, neighbor, center);
        }
        else
        {
            TriangulateOpenWater(direction, cell, neighbor, center);
        }
        
    }

    // 湖泊与湖泊的相邻边
    void TriangulateOpenWater(HexDirection direction, HexCell cell, HexCell neighbor, Vector3 center)
    {
        Vector3 c1 = center + HexMetrics.GetFirstLakesCorner(direction);
        Vector3 c2 = center + HexMetrics.GetSecondLakesCorner(direction);

        lakes.AddTriangle(center, c1, c2);
        Vector3 indices;
        indices.x = indices.y = indices.z = cell.index;
        lakes.AddTriangleCellData(indices, weights1);


        // 2个湖泊的连接处
        if (direction <= HexDirection.SE)
        {
            if (neighbor == null || !neighbor.IsLakes())
            {
                return;
            }

            Vector3 bridge = HexMetrics.GetLakesBridge(direction);
            Vector3 e1 = c1 + bridge;
            Vector3 e2 = c2 + bridge;

            lakes.AddQuad(c1, c2, e1, e2);
            indices.y = neighbor.index;
            lakes.AddQuadCellData(indices, weights1, weights2);

            // 多个格子的 三角形连接处
            HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
            if (direction <= HexDirection.E && nextNeighbor && nextNeighbor.IsLakes())
            {
                lakes.AddTriangle(
                    c2, e2, c2 + HexMetrics.GetLakesBridge(direction.Next())
                );
                indices.z = nextNeighbor.index;
                lakes.AddTriangleCellData(
                    indices, weights1, weights2, weights3
                );

            }
        }

    }

    // 湖泊与陆地相邻边
    void TriangulateWaterShore( HexDirection direction, HexCell cell, HexCell neighbor, Vector3 center )
    {
        EdgeVertices e1 = new EdgeVertices(
            center + HexMetrics.GetFirstLakesCorner(direction),
            center + HexMetrics.GetSecondLakesCorner(direction)
        );
        lakes.AddTriangle(center, e1.v1, e1.v2);
        lakes.AddTriangle(center, e1.v2, e1.v3);
        lakes.AddTriangle(center, e1.v3, e1.v4);
        lakes.AddTriangle(center, e1.v4, e1.v5);
        Vector3 indices;
        indices.x = indices.z = cell.index;
        indices.y = neighbor.index;

        lakes.AddTriangleCellData(indices, weights1);
        lakes.AddTriangleCellData(indices, weights1);
        lakes.AddTriangleCellData(indices, weights1);
        lakes.AddTriangleCellData(indices, weights1);

        // 连接处
        Vector3 center2 = neighbor.Position;
        if (neighbor.ColumnIndex < cell.ColumnIndex - 1)
        {
            center2.x += HexMetrics.wrapSize * HexMetrics.innerDiameter;
        }
        else if (neighbor.ColumnIndex > cell.ColumnIndex + 1)
        {
            center2.x -= HexMetrics.wrapSize * HexMetrics.innerDiameter;
        }

        center2.y = center.y;
        EdgeVertices e2 = new EdgeVertices(
            center2 + HexMetrics.GetSecondSolidCorner(direction.Opposite()),
            center2 + HexMetrics.GetFirstSolidCorner(direction.Opposite())
        );

        // 这条边是否有河流
        if (cell.HasRiverThroughEdge(direction))
        {
            TriangulateEstuary(e1, e2, cell.IsInComingRiverDirection(direction), indices);
        }
        else
        {

            lakesShore.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
            lakesShore.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
            lakesShore.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
            lakesShore.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);
            lakesShore.AddQuadUV(0f, 0f, 0f, 1f);
            lakesShore.AddQuadUV(0f, 0f, 0f, 1f);
            lakesShore.AddQuadUV(0f, 0f, 0f, 1f);
            lakesShore.AddQuadUV(0f, 0f, 0f, 1f);
            lakesShore.AddQuadCellData(indices, weights1, weights2);
            lakesShore.AddQuadCellData(indices, weights1, weights2);
            lakesShore.AddQuadCellData(indices, weights1, weights2);
            lakesShore.AddQuadCellData(indices, weights1, weights2);

        }

        // 多个格子的 三角形连接处
        HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
        if (nextNeighbor != null)
        {
            Vector3 center3 = nextNeighbor.Position;
            if (nextNeighbor.ColumnIndex < cell.ColumnIndex - 1)
            {
                center3.x += HexMetrics.wrapSize * HexMetrics.innerDiameter;
            }
            else if (nextNeighbor.ColumnIndex > cell.ColumnIndex + 1)
            {
                center3.x -= HexMetrics.wrapSize * HexMetrics.innerDiameter;
            }

            Vector3 v3 = center3 + (nextNeighbor.IsUnderwater ?
                HexMetrics.GetFirstLakesCorner(direction.Previous()) :
                HexMetrics.GetFirstSolidCorner(direction.Previous()));
            v3.y = center.y;

            lakesShore.AddTriangle(
                e1.v5, e2.v5, v3
            );

            lakesShore.AddTriangleUV(
                new Vector2(0f, 0f),
                new Vector2(0f, 1f),
                new Vector2(0f, nextNeighbor.IsLakes() ? 0f : 1f)
            );
            indices.z = nextNeighbor.index;
            lakesShore.AddTriangleCellData(
                indices, weights1, weights2, weights3
            );

        }

    }

    // 河水与湖泊入口处
    void TriangulateWaterfallInWater(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, float y1, float y2, float waterY, Vector3 indices)
    {
        v1.y = v2.y = y1;
        v3.y = v4.y = y2;
        //v1 = HexMetrics.Perturb(v1);
        //v2 = HexMetrics.Perturb(v2);
        //v3 = HexMetrics.Perturb(v3);
        //v4 = HexMetrics.Perturb(v4);
        float t = (waterY - y2) / (y1 - y2);
        v3 = Vector3.Lerp(v3, v1, t);
        v4 = Vector3.Lerp(v4, v2, t);
        rivers.AddQuadUnperturbed(v1, v2, v3, v4);
        rivers.AddQuadUV(0f, 1f, 0.8f, 1f);
        rivers.AddQuadCellData(indices, weights1);
    }

    // 河口
    void TriangulateEstuary(EdgeVertices e1, EdgeVertices e2, bool incomingRiver, Vector3 indices)
    {
        // 补足缺口
        lakesShore.AddTriangle(e2.v1, e1.v2, e1.v1);
        lakesShore.AddTriangle(e2.v5, e1.v5, e1.v4);
        lakesShore.AddTriangleUV( new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(0f, 0f) );
        lakesShore.AddTriangleUV( new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(0f, 0f) );
        lakesShore.AddTriangleCellData(indices, weights2, weights1, weights1);
        lakesShore.AddTriangleCellData(indices, weights2, weights1, weights1);

        // 河口
        estuaries.AddQuad(e2.v1, e1.v2, e2.v2, e1.v3);
        estuaries.AddTriangle(e1.v3, e2.v2, e2.v4);
        estuaries.AddQuad(e1.v3, e1.v4, e2.v4, e2.v5);

        estuaries.AddQuadUV(
            new Vector2(0f, 1f), new Vector2(0f, 0f),
            new Vector2(1f, 1f), new Vector2(0f, 0f)
        );
        estuaries.AddTriangleUV(
            new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(1f, 1f)
        );
        estuaries.AddQuadUV(
            new Vector2(0f, 0f), new Vector2(0f, 0f),
            new Vector2(1f, 1f), new Vector2(0f, 1f)
        );
        estuaries.AddQuadCellData(
            indices, weights2, weights1, weights2, weights1
        );
        estuaries.AddTriangleCellData(indices, weights1, weights2, weights2);
        estuaries.AddQuadCellData(indices, weights1, weights2);

        // 冲击水流
        if (incomingRiver)
        {
            estuaries.AddQuadUV2(
                new Vector2(1.5f, 1f), new Vector2(0.7f, 1.15f),
                new Vector2(1f, 0.8f), new Vector2(0.5f, 1.1f)
            );
            estuaries.AddTriangleUV2(
                new Vector2(0.5f, 1.1f),
                new Vector2(1f, 0.8f),
                new Vector2(0f, 0.8f)
            );
            estuaries.AddQuadUV2(
                new Vector2(0.5f, 1.1f), new Vector2(0.3f, 1.15f),
                new Vector2(0f, 0.8f), new Vector2(-0.5f, 1f)
            );
        }
        else
        {
            estuaries.AddQuadUV2(
                new Vector2(-0.5f, -0.2f), new Vector2(0.3f, -0.35f),
                new Vector2(0f, 0f), new Vector2(0.5f, -0.3f)
            );
            estuaries.AddTriangleUV2(
                new Vector2(0.5f, -0.3f),
                new Vector2(0f, 0f),
                new Vector2(1f, 0f)
            );
            estuaries.AddQuadUV2(
                new Vector2(0.5f, -0.3f), new Vector2(0.7f, -0.35f),
                new Vector2(1f, 0f), new Vector2(1.5f, -0.2f)
            );
        }
    }



    void TriangulateRoad(
        Vector3 center, Vector3 mL, Vector3 mR, EdgeVertices e, bool hasRoadThroughCellEdge
    )
    {
        if (hasRoadThroughCellEdge)
        {
            Vector3 mC = Vector3.Lerp(mL, mR, 0.5f);
            TriangulateRoadSegment(mL, mC, mR, e.v2, e.v3, e.v4);

            roads.AddTriangle(center, mL, mC);
            roads.AddTriangle(center, mC, mR);

            roads.AddTriangleUV(
                new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(1f, 0f)
            );
            roads.AddTriangleUV(
                new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f)
            );
        }
        else
        {
            TriangulateRoadEdge(center, mL, mR);
        }
    }

    void TriangulateRoadEdge(Vector3 center, Vector3 mL, Vector3 mR)
    {
        roads.AddTriangle(center, mL, mR);
        roads.AddTriangleUV(
            new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f)
        );
    }

    void TriangulateRoadSegment(
        Vector3 v1, Vector3 v2, Vector3 v3,
        Vector3 v4, Vector3 v5, Vector3 v6
    )
    {
        roads.AddQuad(v1, v2, v4, v5);
        roads.AddQuad(v2, v3, v5, v6);
        roads.AddQuadUV(0f, 1f, 0f, 0f);
        roads.AddQuadUV(1f, 0f, 0f, 0f);
    }
    
    // 备份
    void TriangulateRoadAdjacentToRiver(
        HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
    )
    {
        bool hasRoadThroughEdge = cell.HasRoadThroughEdge(direction);
        bool previousHasRiver = cell.HasRiverThroughEdge(direction.Previous());
        bool nextHasRiver = cell.HasRiverThroughEdge(direction.Next());

        Vector2 interpolators = GetRoadInterpolators(direction, cell);
        Vector3 roadCenter = center;

        // 河流初始或结束
        if (cell.HasRiverBeginOrEnd())
        {
            roadCenter += HexMetrics.GetSolidEdgeMiddle(
                cell.RiverBeginOrEndDirection().Opposite()
            ) * (1f / 3f);

            Vector3 mL = Vector3.Lerp(roadCenter, e.v1, interpolators.x);
            Vector3 mR = Vector3.Lerp(roadCenter, e.v5, interpolators.y);
            TriangulateRoad(roadCenter, mL, mR, e, hasRoadThroughEdge);

            return;
        }

        if (cell.GetRiverCount() <= 1)
        {
            return;
        }
        // 多段河流

        // 交点列表
        Vector3[] intersections = new Vector3[cell.GetRiverCount() + 1];

        Vector3 centerD = new Vector3(center.x, cell.StreamBedY, center.z);

        HexDirection firstRiverDir = HexDirection.NE;
        Vector3 leftPoint = center + HexMetrics.GetFirstSolidCorner(firstRiverDir) * 0.5f;
        Vector3 rightPoint = center + HexMetrics.GetSecondSolidCorner(firstRiverDir) * 0.5f;
        Vector3 briDir = HexMetrics.GetBridge(firstRiverDir);

        Vector3 riverDPoint = (leftPoint + rightPoint) / 2;
        riverDPoint.y = centerD.y;


        // 先找一条河流
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            RiverDirection dir = cell.GetRiverDirection(d);
            if (dir != RiverDirection.Null)
            {
                firstRiverDir = d;
                leftPoint = center + HexMetrics.GetFirstSolidCorner(d) * 0.5f;
                rightPoint = center + HexMetrics.GetSecondSolidCorner(d) * 0.5f;

                briDir = HexMetrics.GetBridge(d);

                break;
            }
        }

        Vector3 indices;
        indices.x = indices.y = indices.z = cell.index;

        Vector3 lastPoint = leftPoint;
        List<Vector3> points = new List<Vector3>();
        points.Add(rightPoint);

        int intersection_index = 1;
        bool hasRoad = false;

        //  以第一条河流为起始，循环一圈
        HexDirection nextDir = firstRiverDir;
        RiverDirection beforRiverDir = cell.GetRiverDirection(nextDir);
        for (int i = 0; i < 6; i++)
        {
            nextDir = nextDir.Next();

            hasRoad = cell.HasRoadThroughEdge(nextDir) ? true : hasRoad;

            RiverDirection nextRiverDir = cell.GetRiverDirection(nextDir);
            if (nextRiverDir != RiverDirection.Null)
            {
                Vector3 nextLeftPoint = center + HexMetrics.GetFirstSolidCorner(nextDir) * 0.5f;
                Vector3 nextRightPoint = center + HexMetrics.GetSecondSolidCorner(nextDir) * 0.5f;
                Vector3 nextDPoint = (nextLeftPoint + nextRightPoint) / 2;
                nextDPoint.y = centerD.y;

                Vector3 nextBriDir = HexMetrics.GetBridge(nextDir);

                Vector3 Intersection = Point.GetRayIntersection(rightPoint, briDir, nextLeftPoint, nextBriDir);
                intersections[intersection_index] = Intersection;
                intersection_index++;

                Debug.Log(Intersection);

                leftPoint = nextLeftPoint;
                rightPoint = nextRightPoint;

                // points.Add(rightPoint);

                if (hasRoad)
                {
                    //TriangulateRoadSegment(mL, mC, mR, e.v2, e.v3, e.v4);

                    // 画平面上的地
                    for (int j = 0; j < points.Count - 1; j++)
                    {
                        Vector3 v1 = points[j];
                        Vector3 v2 = Intersection;
                        Vector3 v3 = points[j + 1];

                        // 汇合处顶部多余部分
                        TriangulateRoadEdge(v2, v1, v3);
                    }

                    hasRoad = false;
                }

                points.Clear();
                points.Add(rightPoint);

                briDir = nextBriDir;

                beforRiverDir = nextRiverDir;
            }
            else
            {
                Vector3 nextLeftPoint = center + HexMetrics.GetFirstSolidCorner(nextDir) * 0.5f;
                Vector3 nextRightPoint = center + HexMetrics.GetSecondSolidCorner(nextDir) * 0.5f;

                points.Add(nextRightPoint);
            }
        }

        intersections[0] = intersections[intersection_index - 1];
    }

    // 路的中心位置
    void TriangulateRoadAdjacentToRiver(HexCell cell)
    {
        //Vector2 interpolators = GetRoadInterpolators(direction, cell);

        Vector3 center = cell.transform.localPosition;

        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            if (cell.HasRoadThroughEdge(d))
            {
                EdgeVertices e = new EdgeVertices(
                    center + HexMetrics.GetFirstSolidCorner(d),
                    center + HexMetrics.GetSecondSolidCorner(d)
                );

                Vector3 mL = center + HexMetrics.GetFirstSolidCorner(d) * 0.5f;
                Vector3 mR = center + HexMetrics.GetSecondSolidCorner(d) * 0.5f;
                
                Vector3 mC = Vector3.Lerp(mL, mR, 0.5f);

                TriangulateRoadSegment(mL, mC, mR, e.v2, e.v3, e.v4);
            }
        }

        if (cell.GetRiverCount() <= 1)
        {
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                if (!cell.HasRiverThroughEdge(d))
                {
                    EdgeVertices e = new EdgeVertices(
                        center + HexMetrics.GetFirstSolidCorner(d),
                        center + HexMetrics.GetSecondSolidCorner(d)
                    );
                    Vector2 interpolators = GetRoadInterpolators(d, cell);
                    TriangulateRoad(
                        center,
                        Vector3.Lerp(center, e.v1, interpolators.x),
                        Vector3.Lerp(center, e.v5, interpolators.y),
                        e, cell.HasRoadThroughEdge(d)
                    );
                }
            }
            return;
        }
        // 多段河流

        // 交点列表
        Vector3[] intersections = new Vector3[cell.GetRiverCount() + 1];

        Vector3 centerD = new Vector3(center.x, cell.StreamBedY, center.z);

        HexDirection firstRiverDir = HexDirection.NE;
        Vector3 leftPoint = center + HexMetrics.GetFirstSolidCorner(firstRiverDir) * 0.5f;
        Vector3 rightPoint = center + HexMetrics.GetSecondSolidCorner(firstRiverDir) * 0.5f;
        Vector3 briDir = HexMetrics.GetBridge(firstRiverDir);

        Vector3 riverDPoint = (leftPoint + rightPoint) / 2;
        riverDPoint.y = centerD.y;


        // 先找一条河流
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            RiverDirection dir = cell.GetRiverDirection(d);
            if (dir != RiverDirection.Null)
            {
                firstRiverDir = d;
                leftPoint = center + HexMetrics.GetFirstSolidCorner(d) * 0.5f;
                rightPoint = center + HexMetrics.GetSecondSolidCorner(d) * 0.5f;

                riverDPoint = (leftPoint + rightPoint) / 2;
                riverDPoint.y = centerD.y;

                briDir = HexMetrics.GetBridge(d);

                break;
            }
        }

        Vector3 indices;
        indices.x = indices.y = indices.z = cell.index;

        Vector3 lastPoint = leftPoint;
        List<Vector3> points = new List<Vector3>();
        points.Add(rightPoint);

        int intersection_index = 1;
        bool hasRoad = false;

        //  以第一条河流为起始，循环一圈
        HexDirection nextDir = firstRiverDir;
        RiverDirection beforRiverDir = cell.GetRiverDirection(nextDir);
        for (int i = 0; i < 6; i++)
        {
            nextDir = nextDir.Next();

            hasRoad = cell.HasRoadThroughEdge(nextDir) ? true : hasRoad;

            RiverDirection nextRiverDir = cell.GetRiverDirection(nextDir);
            if (nextRiverDir != RiverDirection.Null)
            {
                Vector3 nextLeftPoint = center + HexMetrics.GetFirstSolidCorner(nextDir) * 0.5f;
                Vector3 nextRightPoint = center + HexMetrics.GetSecondSolidCorner(nextDir) * 0.5f;
                Vector3 nextDPoint = (nextLeftPoint + nextRightPoint) / 2;
                nextDPoint.y = centerD.y;

                Vector3 nextBriDir = HexMetrics.GetBridge(nextDir);

                Vector3 Intersection = Point.GetRayIntersection(rightPoint, briDir, nextLeftPoint, nextBriDir);
                intersections[intersection_index] = Intersection;
                intersection_index++;

                Debug.Log(Intersection);

                leftPoint = nextLeftPoint;
                rightPoint = nextRightPoint;

                // points.Add(rightPoint);

                if (hasRoad)
                {
                    // 画平面上的地
                    for (int j = 0; j < points.Count - 1; j++)
                    {
                        Vector3 v1 = points[j];
                        Vector3 v2 = Intersection;
                        Vector3 v3 = points[j + 1];

                        // 汇合处顶部多余部分
                        TriangulateRoadEdge(v2, v1, v3);
                    }

                    hasRoad = false;
                }

                points.Clear();
                points.Add(rightPoint);

                briDir = nextBriDir;

                beforRiverDir = nextRiverDir;
            }
            else
            {
                Vector3 nextLeftPoint = center + HexMetrics.GetFirstSolidCorner(nextDir) * 0.5f;
                Vector3 nextRightPoint = center + HexMetrics.GetSecondSolidCorner(nextDir) * 0.5f;

                points.Add(nextRightPoint);
            }
        }

        intersections[0] = intersections[intersection_index - 1];
    }

    Vector2 GetRoadInterpolators(HexDirection direction, HexCell cell)
    {
        Vector2 interpolators;
        if (cell.HasRoadThroughEdge(direction))
        {
            interpolators.x = interpolators.y = 0.5f;
        }
        else
        {
            interpolators.x =
                cell.HasRoadThroughEdge(direction.Previous()) ? 0.5f : 0.25f;
            interpolators.y =
                cell.HasRoadThroughEdge(direction.Next()) ? 0.5f : 0.25f;
        }
        return interpolators;
    }



    ///////////////////////////////////////////////////////////////////////////////
    //////////////////////////////城墙/////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////


    public void AddWall(
        EdgeVertices near, HexCell nearCell,
        EdgeVertices far, HexCell farCell
    )
    {
    }



    // 添加建筑
    public void AddBuild(HexCell cell, BuildType type)
    {
        MapBuildFactory.CreateMapBuild(cell, type);

        Refresh(cell);
        //features.AddBuildFeature(cell);
    }


    HexCell ChooesCell { get; set; }

    HexCell GetCurrMouseCell()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit, 200f, targetMask))
        {
            return HexGrid.instance.GetCell(hit.point);
        }

        return null;
    }

    public void OnClickCell()
    {
        HexCell cell = GetCurrMouseCell();
        if(cell != null)
        {
            HexCell chooseCell = ChooesCell;

            // 移动了
            if(chooseCell == null || chooseCell.index != cell.index)
            {

                ChooesCell = cell;
            }
        }
    }


    public void SetCellMapDataShader(bool isShow)
    {
        //terrainMaterial.SetFloat("_ShowMapData", isShow ? 1f : 0f);

        if (isShow)
        {
            terrainMaterial.EnableKeyword("SHOW_MAP_DATA");
        }
        else
        {
            terrainMaterial.DisableKeyword("SHOW_MAP_DATA");
        }
    }




    ///////////////////////边界线//////////////////////////////
    

    public void RefreshBoundary()
    {
        // 清楚渲染
        boundary.Clear();

        // 渲染所有六边形
        for (int i = 0; i < cells.Length; i++)
        {
            // 渲染某个六边形
            boundarySide(cells[i]);
        }

        boundary.Apply();
    }


    void boundarySide(HexCell cell)
    {
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            boundarySide(d, cell);
        }
    }

    // 渲染某个方向的边
    void boundarySide(HexDirection direction, HexCell cell)
    {
        float width = 0f;
        if(cell.IsCityBoundary(direction, out width))
        {
            HexCell nei = cell.GetNeighbor(direction);

            Debug.Log("IsCityBoundary"+ width);

            Vector3 center = cell.GetBoundaryCenterPos();

            EdgeVertices e = new EdgeVertices(
                center + HexMetrics.GetFirstSolidCorner(direction),
                center + HexMetrics.GetSecondSolidCorner(direction)
            );

            Vector3 bridge = HexMetrics.GetBridge(direction);

            EdgeVertices nei_e = new EdgeVertices(
                e.v1 + bridge,
                e.v5 + bridge
            );
            if (nei != null)
            {
                Vector3 nei_center = nei.GetBoundaryCenterPos();

                nei_e = new EdgeVertices(
                    nei_center + HexMetrics.GetSecondSolidCorner(direction.Opposite()),
                    nei_center + HexMetrics.GetFirstSolidCorner(direction.Opposite())
                );
            }

            EdgeVertices boundary_line = new EdgeVertices(
                    Vector3.Lerp(e.v1, nei_e.v1, 0.5f),
                    Vector3.Lerp(e.v5, nei_e.v5, 0.5f)
                );

            EdgeVertices insit_boundary_line = new EdgeVertices(
                    Vector3.Lerp(boundary_line.v1, e.v1, width),
                    Vector3.Lerp(boundary_line.v5, e.v5, width)
                );

            boundary.AddQuad(insit_boundary_line.v1, insit_boundary_line.v5,
                boundary_line.v1, boundary_line.v5);

            Vector3 indices = new Vector3(cell.index, cell.index, cell.index);
            boundary.AddQuadCellData(indices, weights1);

            boundaryTriangulate(direction, cell, width, true);

            // 如果上一边不是边界
            float preWidth;
            if (!cell.IsCityBoundary(direction.Previous(), out preWidth))
            {
                boundaryTriangulate(direction.Previous(), cell, width, false);
            }
        }
    }


    void boundaryTriangulate(HexDirection direction, HexCell cell, float width, bool isbound)
    {
        Vector3 v1, v2, v3, v4;
        boundaryRightPoint(cell, direction, width, out v1, out v2);

        Vector3 indices = new Vector3(cell.index, cell.index, cell.index);

        float nextWidth;
        if(isbound && cell.IsCityBoundary(direction.Next(), out nextWidth))
        {
            boundaryLeftPoint(cell, direction.Next(), nextWidth, out v3, out v4);

            boundary.AddQuad(v1, v3, v2, v4);
            boundary.AddQuadCellData(indices, weights1);
        }
        else
        {
            Vector3 v5, v6;

            Vector3 center = cell.GetBoundaryCenterPos();

            HexCell nei = cell.GetNeighbor(direction);
            nei = nei == null ? cell : nei;
            Vector3 neiCenter = nei.GetBoundaryCenterPos();

            HexCell nextNei = cell.GetNeighbor(direction.Next());
            nextNei = nextNei == null ? cell : nextNei;
            Vector3 nextNeiCenter = nextNei.GetBoundaryCenterPos();

            v6 = center + HexMetrics.GetSecondCorner(direction);
            v6.y = (center.y + neiCenter.y + nextNeiCenter.y) / 3;

            Vector3 v = center + HexMetrics.GetSecondSolidCorner(direction);
            v5 = Vector3.Lerp(v6, v, width);

            boundary.AddQuad(v1, v5, v2, v6);
            boundary.AddQuadCellData(indices, weights1);

            boundaryLeftPoint(cell, direction.Next(), width, out v3, out v4);

            boundary.AddQuad(v5, v3, v6, v4);
            boundary.AddQuadCellData(indices, weights1);
        }
    }

    void boundaryRightPoint(HexCell cell, HexDirection dir, float width, out Vector3 v1, out Vector3 v2)
    {
        Vector3 center = cell.GetBoundaryCenterPos();

        Vector3 v = center + HexMetrics.GetSecondSolidCorner(dir);

        HexCell nei = cell.GetNeighbor(dir);

        Vector3 _v;
        if (nei != null)
        {
            Vector3 nei_center = nei.GetBoundaryCenterPos();
            _v = nei_center + HexMetrics.GetFirstSolidCorner(dir.Opposite());
        }
        else
        {
            _v = v + HexMetrics.GetBridge(dir);
        }

        v2 = Vector3.Lerp(v, _v, 0.5f);
        v1 = Vector3.Lerp(v2, v, width);
    }

    void boundaryLeftPoint(HexCell cell, HexDirection dir, float width, out Vector3 v1, out Vector3 v2)
    {
        Vector3 center = cell.GetBoundaryCenterPos();

        Vector3 v = center + HexMetrics.GetFirstSolidCorner(dir);

        HexCell nei = cell.GetNeighbor(dir);

        Vector3 _v;
        if (nei != null)
        {
            Vector3 nei_center = nei.GetBoundaryCenterPos();
            _v = nei_center + HexMetrics.GetSecondSolidCorner(dir.Opposite());
        }
        else
        {
            _v = v + HexMetrics.GetBridge(dir);
        }

        v2 = Vector3.Lerp(v, _v, 0.5f);
        v1 = Vector3.Lerp(v2, v, width);
    }
}
