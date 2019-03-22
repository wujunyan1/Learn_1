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

    // 湖泊
    public HexMesh lakes;

    // 湖泊与陆地接壤
    public HexMesh lakesShore;

    public Color[] colors;

    // 初始化地图
    public void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        //hexMesh = GetComponentInChildren<HexMesh>();

        //Random.InitState(mapSeed);
        //int random = Random.Range(0, 1000);

        CreateCells();
    }

    void CreateCells()
    {
        cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];

        for (int z = 0, i = 0; z < HexMetrics.chunkSizeZ; z++)
        {
            for (int x = 0; x < HexMetrics.chunkSizeX; x++)
            {

            }
        }
    }

    public void AddCell(int index, HexCell cell)
    {
        //Debug.Log(index);

        cells[index] = cell;
        cell.transform.SetParent(transform, false);
        cell.uiRect.SetParent(gridCanvas.transform, false);
        cell.chunk = this;
    }

    void Start()
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



    // 渲染所以六边形
    public void Triangulate(HexCell[] cells)
    {
        // 清楚渲染
        terrain.Clear();
        rivers.Clear();
        lakes.Clear();
        lakesShore.Clear();

        for (int i = 0; i < cells.Length; i++)
        {
            // 渲染某个六边形
            Triangulate(cells[i]);
        }

        terrain.Apply();
        rivers.Apply();
        lakes.Apply();
        lakesShore.Apply();
    }

    // 渲染某个六边形
    public void Triangulate(HexCell cell)
    {
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

        cell.label.text = string.Format("{0}\n{1}\n{2}", cell.index, cell.Elevation, (int)cell.TerrainType);
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
            //Debug.Log(cell.HasRiver());
            // 这条边是否有河流
            if (cell.HasRiverThroughEdge(direction))
            {
                e.v3.y = cell.StreamBedY;

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
                if (cell.HasRiverBeginOrEnd())
                {
                    TriangulateEdgeFan(center, e, cell.color);
                }
                else
                {
                    TriangulateWithNoRiver(direction, cell, center, e);
                }
                //TriangulateEdgeFan(center, e, cell.color);
            }
        }
        else
        {
            TriangulateEdgeFan(center, e, cell.color);
        }

        // 三角形外边区域
        if (direction <= HexDirection.SE)
        {
            TriangulateConnection(direction, cell, e);
        }
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

        // 这个方向有河流
        if (cell.HasRiverThroughEdge(direction))
        {
            e2.v3.y = neighbor.StreamBedY;

            // 自己不是湖泊
            if (!cell.IsLakes())
            {
                // 邻居不是湖泊，则正常
                if (!neighbor.IsLakes())
                {
                    bool reversed = cell.IsInComingRiverDirection(direction);
                    TriangulateRiverQuad(e1.v2, e1.v4, e2.v2, e2.v4, cell.RiverSurfaceY, neighbor.RiverSurfaceY, 0.6f, reversed);
                }
                // 邻居是湖泊
                else if (neighbor.IsLakes())
                {
                    TriangulateWaterfallInWater(
                        e1.v2, e1.v4, e2.v2, e2.v4,
                        cell.RiverSurfaceY, neighbor.RiverSurfaceY,
                        neighbor.LakesSurfaceY
                    );
                }

            }
        }
        

        if (cell.GetEdgeType(direction) == HexEdgeType.Slope)
        {
            TriangulateEdgeTerraces(e1, cell, e2, neighbor);
        }
        else
        {
            TriangulateEdgeStrip(e1, cell.color, e2, neighbor.color);
        }

        // AddQuad(v1, v2, v3, v4);
        // AddQuadColor(cell.color, neighbor.color);


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
        }
    }

    // 渲染连接四边形及其颜色
    //  Vector3 beginLeft, Vector3 beginRight, HexCell beginCell, Vector3 endLeft, Vector3 endRight, HexCell endCell
    void TriangulateEdgeTerraces(EdgeVertices begin, HexCell beginCell, EdgeVertices end, HexCell endCell)
    {
        EdgeVertices e2 = EdgeVertices.TerraceLerp(begin, end, 1);
        Color c2 = HexMetrics.TerraceLerp(beginCell.color, endCell.color, 1);

        TriangulateEdgeStrip(begin, beginCell.color, e2, c2);

        for (int i = 2; i < HexMetrics.terraceSteps; i++)
        {
            EdgeVertices e1 = e2;
            Color c1 = c2;
            e2 = EdgeVertices.TerraceLerp(begin, end, i);
            c2 = HexMetrics.TerraceLerp(beginCell.color, endCell.color, i);
            TriangulateEdgeStrip(e1, c1, e2, c2);
        }

        TriangulateEdgeStrip(e2, c2, end, endCell.color);


        //Vector3 v3 = HexMetrics.TerraceLerp(beginLeft, endLeft, 1);
        //Vector3 v4 = HexMetrics.TerraceLerp(beginRight, endRight, 1);
        //Color c2 = HexMetrics.TerraceLerp(beginCell.color, endCell.color, 1);

        //AddQuad(beginLeft, beginRight, v3, v4);
        //AddQuadColor(beginCell.color, c2);

        //for (int i = 2; i < HexMetrics.terraceSteps; i++)
        //{
        //    Vector3 v1 = v3;
        //    Vector3 v2 = v4;
        //    Color c1 = c2;
        //    v3 = HexMetrics.TerraceLerp(beginLeft, endLeft, i);
        //    v4 = HexMetrics.TerraceLerp(beginRight, endRight, i);
        //    c2 = HexMetrics.TerraceLerp(beginCell.color, endCell.color, i);
        //    AddQuad(v1, v2, v3, v4);
        //    AddQuadColor(c1, c2);
        //}

        //AddQuad(v3, v4, endLeft, endRight);
        //AddQuadColor(c2, endCell.color);
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
            terrain.AddTriangleColor(bottomCell.color, leftCell.color, rightCell.color);
        }
    }

    // 渲染连接三边形及其颜色
    void TriangulateCornerTerraces(Vector3 begin, HexCell beginCell, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
    {
        Vector3 v3 = HexMetrics.TerraceLerp(begin, left, 1);
        Vector3 v4 = HexMetrics.TerraceLerp(begin, right, 1);
        Color c3 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, 1);
        Color c4 = HexMetrics.TerraceLerp(beginCell.color, rightCell.color, 1);

        terrain.AddTriangle(begin, v3, v4);
        terrain.AddTriangleColor(beginCell.color, c3, c4);

        for (int i = 2; i < HexMetrics.terraceSteps; i++)
        {
            Vector3 v1 = v3;
            Vector3 v2 = v4;
            Color c1 = c3;
            Color c2 = c4;
            v3 = HexMetrics.TerraceLerp(begin, left, i);
            v4 = HexMetrics.TerraceLerp(begin, right, i);
            c3 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, i);
            c4 = HexMetrics.TerraceLerp(beginCell.color, rightCell.color, i);
            terrain.AddQuad(v1, v2, v3, v4);
            terrain.AddQuadColor(c1, c2, c3, c4);
        }

        terrain.AddQuad(v3, v4, left, right);
        terrain.AddQuadColor(c3, c4, leftCell.color, rightCell.color);
    }

    // 渲染连接三边形及其颜色 悬崖+2个斜面
    void TriangulateCornerTerracesCliff(Vector3 begin, HexCell beginCell, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
    {
        float b = 1f / (rightCell.Elevation - beginCell.Elevation);
        Vector3 boundary = Vector3.Lerp(begin, right, b);
        Color boundaryColor = Color.Lerp(beginCell.color, rightCell.color, b);

        TriangulateBoundaryTriangle(
            begin, beginCell, left, leftCell, boundary, boundaryColor
        );

        if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
        {
            TriangulateBoundaryTriangle(
                left, leftCell, right, rightCell, boundary, boundaryColor
            );
        }
        else
        {
            terrain.AddTriangle(left, right, boundary);
            terrain.AddTriangleColor(leftCell.color, rightCell.color, boundaryColor);
        }
    }

    void TriangulateBoundaryTriangle(Vector3 begin, HexCell beginCell, Vector3 left, HexCell leftCell, Vector3 boundary, Color boundaryColor)
    {
        Vector3 v2 = HexMetrics.TerraceLerp(begin, left, 1);
        Color c2 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, 1);

        terrain.AddTriangle(begin, v2, boundary);
        terrain.AddTriangleColor(beginCell.color, c2, boundaryColor);

        for (int i = 2; i < HexMetrics.terraceSteps; i++)
        {
            Vector3 v1 = v2;
            Color c1 = c2;
            v2 = HexMetrics.TerraceLerp(begin, left, i);
            c2 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, i);
            terrain.AddTriangle(v1, v2, boundary);
            terrain.AddTriangleColor(c1, c2, boundaryColor);
        }

        terrain.AddTriangle(v2, left, boundary);
        terrain.AddTriangleColor(c2, leftCell.color, boundaryColor);
    }

    void TriangulateCornerCliffTerraces(Vector3 begin, HexCell beginCell, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
    {
        float b = 1f / (leftCell.Elevation - beginCell.Elevation);
        if (b < 0)
        {
            b = -b;
        }

        Vector3 boundary = Vector3.Lerp(begin, left, b);
        Color boundaryColor = Color.Lerp(beginCell.color, leftCell.color, b);

        TriangulateBoundaryTriangle(
            right, rightCell, begin, beginCell, boundary, boundaryColor
        );

        if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
        {
            TriangulateBoundaryTriangle(
                left, leftCell, right, rightCell, boundary, boundaryColor
            );
        }
        else
        {
            terrain.AddTriangle(left, right, boundary);
            terrain.AddTriangleColor(leftCell.color, rightCell.color, boundaryColor);
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


        //TriangulateEdgeStrip(m, cell.color, e, cell.color);

        terrain.AddTriangle(e.v1, e.v2, m.v1);
        terrain.AddTriangleColor(cell.color);

        terrain.AddQuad(e.v2, m.v1, e.v3, m.v3);
        terrain.AddQuadColor(cell.color);
        terrain.AddQuad(e.v3, m.v3, e.v4, m.v5);
        terrain.AddQuadColor(cell.color);
        terrain.AddTriangle(e.v4, e.v5, m.v5);
        terrain.AddTriangleColor(cell.color);
    }


    // 画河道
    void TriangulateWithRiver(HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e)
    {

        //Vector3 river_centerL = center + HexMetrics.GetFirstSolidCorner(direction.Previous()) * 0.25f;
        //Vector3 river_centerR = center + HexMetrics.GetSecondSolidCorner(direction.Next()) * 0.25f;

        EdgeVertices m = new EdgeVertices(Vector3.Lerp(center, e.v1, 0.5f), Vector3.Lerp(center, e.v5, 0.5f));
        m.v3.y = e.v3.y;


        TriangulateWithNoRiver(direction, cell, center, e);

        if (!cell.IsLakes())
        {
            bool reversed = cell.IsInComingRiverDirection(direction);
            float v = 0.4f;
            if (!reversed)
            {
                v = 0.6f;
            }
            TriangulateRiverQuad(m.v1, m.v5, e.v2, e.v4, cell.RiverSurfaceY, cell.RiverSurfaceY, v, reversed);
        }

    }

    // 画河道开始和结束
    void TriangulateWithRiverBeginOrEnd(HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e)
    {
        EdgeVertices m = new EdgeVertices(Vector3.Lerp(center, e.v1, 0.5f), Vector3.Lerp(center, e.v5, 0.5f));
        m.v3.y = e.v3.y;

        TriangulateEdgeStrip(m, cell.color, e, cell.color);
        TriangulateEdgeFan(center, m, cell.color);

        if (!cell.IsLakes())
        {
            bool reversed = cell.IsInComingRiverDirection(direction);

            e.v2.y = e.v4.y = cell.RiverSurfaceY;
            rivers.AddTriangle(center, e.v2, e.v4);
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
        }

        //TriangulateRiverQuad(
        //    m.v2, m.v4, e.v2, e.v4, cell.RiverSurfaceY, cell.RiverSurfaceY, 0f, reversed
        //);
    }

    // 渲染格子中心多段河流连接处
    void TriangulateWithRiverConnection(HexCell cell)
    {
        if(cell.GetRiverCount() <= 1)
        {
            return;
        }
        //Debug.Log("========================");
        Vector3 center = cell.transform.localPosition;
        List<HexDirection> incoming_rivers = cell.GetRiverDirections(RiverDirection.Incoming);
        List<HexDirection> outgoing_rivers = cell.GetRiverDirections(RiverDirection.Outgoing);

        HexDirection outgoing_dir = outgoing_rivers[0];

        // 2条河流，必定是 一条流入，一条流出
        if (cell.GetRiverCount() == 2)
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
            if (landDir != outgoing_dir)
            {
                Debug.Log(string.Format("^^^^^^{0}", incoming_rivers.Count));
                TriangulateWithThreeRiverConnection(cell, incoming_rivers, outgoing_dir, landDir);
            }
            else // 总共4条河流，3个入水口
            {
                TriangulateWithFourRiverConnection(cell, outgoing_dir);
            }
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

        // 2侧河道
        terrain.AddQuad(inEdge.v1, outEdge.v5, inEdge.v3, outEdge.v3);
        terrain.AddQuadColor(cell.color);
        terrain.AddQuad(inEdge.v3, outEdge.v3, inEdge.v5, outEdge.v1);
        terrain.AddQuadColor(cell.color);

        //河流
        TriangulateRiverQuad(inEdge.v5, inEdge.v1, outEdge.v1, outEdge.v5, cell.RiverSurfaceY, cell.RiverSurfaceY, 0.4f, false);

        // 如果是对边
        if (incoming_dir.Opposite() == outgoing_dir)
        {
            Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(incoming_dir.Previous()) * 0.5f;
            Vector3 v2 = center + HexMetrics.GetFirstSolidCorner(outgoing_dir.Previous()) * 0.5f;

            // 补足非河道的高地
            terrain.AddTriangle(v1, inEdge.v1, outEdge.v5);
            terrain.AddTriangleColor(cell.color);

            terrain.AddTriangle(inEdge.v5, v2, outEdge.v1);
            terrain.AddTriangleColor(cell.color);
        }
        else
        {
            EdgeVertices e1 = new EdgeVertices(center + HexMetrics.GetFirstSolidCorner(incoming_dir.Opposite()) * 0.5f, center + HexMetrics.GetSecondSolidCorner(incoming_dir.Opposite()) * 0.5f);
            EdgeVertices e2 = new EdgeVertices(center + HexMetrics.GetFirstSolidCorner(outgoing_dir.Opposite()) * 0.5f, center + HexMetrics.GetSecondSolidCorner(outgoing_dir.Opposite()) * 0.5f);

            // 补足非河道的高地
            terrain.AddQuad(e1.v1, e2.v5, e1.v5, e2.v1);
            terrain.AddQuadColor(cell.color);
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
            terrain.AddTriangleColor(cell.color);

            terrain.AddQuad(d1e.v5, d1e.v3, inJoinV, center_d);
            terrain.AddQuadColor(cell.color);
            terrain.AddQuad(inJoinV, center_d, d2e.v1, d2e.v3);
            terrain.AddQuadColor(cell.color);

            terrain.AddTriangle(center_d, outEdge.v3, d1e.v3);
            terrain.AddTriangleColor(cell.color);
            terrain.AddTriangle(center_d, d2e.v3, outEdge.v3);
            terrain.AddTriangleColor(cell.color);

            terrain.AddQuad(d2e.v5, d2e.v3, outEdge.v1, outEdge.v3);
            terrain.AddQuadColor(cell.color);
            terrain.AddQuad(outEdge.v5, outEdge.v3, d1e.v1, d1e.v3);
            terrain.AddQuadColor(cell.color);

            //河流
            //TriangulateRiverTriangle()
            //rivers.AddTriangle(inJoinV, outEdge.v5, d1e.v1);
            Vector3 inJoinVH = Vector3.Lerp(inJoinV, center_d, 0.5f);
            TriangulateRiverQuad(d1e.v5, d1e.v1, inJoinVH, outEdge.v4, cell.RiverSurfaceY, cell.RiverSurfaceY, 0.4f, false);
            TriangulateRiverQuad(d2e.v5, d2e.v1, outEdge.v2, inJoinVH, cell.RiverSurfaceY, cell.RiverSurfaceY, 0.4f, false);

            rivers.AddTriangle(inJoinVH, 
                new Vector3(outEdge.v2.x, cell.RiverSurfaceY, outEdge.v2.z),
                new Vector3(outEdge.v4.x, cell.RiverSurfaceY, outEdge.v4.z));
            rivers.AddTriangleUV(new Vector2(0.5f, 0.5f), new Vector2(0.6f, 0.6f), new Vector2(0.6f, 0.6f));
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
                terrain.AddTriangleColor(cell.color);

                terrain.AddQuad(d2e.v3, outEdge.v3, d2e.v5, outEdge.v1);
                terrain.AddQuadColor(cell.color);
                terrain.AddQuad(d1e.v1, outEdge.v5, d1e.v3, outEdge.v3);
                terrain.AddQuadColor(cell.color);

                //河流 d1为对边
                Vector3 v1 = Vector3.Lerp(d1e.v5, d2e.v1, 0.5f);
                Vector3 inJoinV = Vector3.Lerp(v1, center, 0.25f);
                Vector3 inJoinVH = Vector3.Lerp(inJoinV, center_d, 0.5f);

                TriangulateRiverQuad(d1e.v4, d1e.v2, outEdge.v2, outEdge.v4, cell.RiverSurfaceY, cell.RiverSurfaceY, 0.4f, false);
                TriangulateRiverQuad(d2e.v4, d2e.v2, outEdge.v2, inJoinVH, cell.RiverSurfaceY, cell.RiverSurfaceY, 0.4f, false);

            }
            else
            {
                terrain.AddTriangle(center_d, outEdge.v3, d1e.v3);
                terrain.AddTriangleColor(cell.color);

                terrain.AddQuad(d2e.v3, outEdge.v3, d2e.v5, outEdge.v1);
                terrain.AddQuadColor(cell.color);
                terrain.AddQuad(d1e.v1, outEdge.v5, d1e.v3, outEdge.v3);
                terrain.AddQuadColor(cell.color);

                //河流
                Vector3 inJoinVH = Vector3.Lerp(d1e.v5, center_d, 0.5f);

                TriangulateRiverQuad(d2e.v4, d2e.v2, outEdge.v2, outEdge.v4, cell.RiverSurfaceY, cell.RiverSurfaceY, 0.4f, false);
                TriangulateRiverQuad(d1e.v4, d1e.v2, inJoinVH, outEdge.v4 , cell.RiverSurfaceY, cell.RiverSurfaceY, 0.4f, false);
            }

            terrain.AddTriangle(e.v1, e.v5, v);
            terrain.AddTriangleColor(cell.color);

            //terrain.AddQuad(d2e.v3, outEdge.v3, d2e.v5, outEdge.v1);
            //terrain.AddQuadColor(cell.color);
            //terrain.AddQuad(d1e.v1, outEdge.v5, d1e.v3, outEdge.v3);
            //terrain.AddQuadColor(cell.color);

            terrain.AddTriangle(center_d, d1e.v3, d1e.v5);
            terrain.AddTriangleColor(cell.color);
            terrain.AddTriangle(center_d, d2e.v1, d2e.v3);
            terrain.AddTriangleColor(cell.color);

            // 不上的三角
            //terrain.AddTriangle(center_d, d.v3, outEdge.v3);
            //terrain.AddTriangleColor(cell.color);

            
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


        terrain.AddQuad(outEdge.v5, outEdge.v3, d1e.v1, d1e.v3);
        terrain.AddQuadColor(cell.color);
        //terrain.AddTriangle(d1e.v1, d1e.v3, center_d);
        //terrain.AddTriangleColor(cell.color);
        terrain.AddTriangle(d1e.v3, d1e.v5, center_d);
        terrain.AddTriangleColor(cell.color);

        terrain.AddTriangle(d2e.v1, d2e.v3, center_d);
        terrain.AddTriangleColor(cell.color);
        terrain.AddTriangle(d2e.v3, d2e.v5, center_d);
        terrain.AddTriangleColor(cell.color);

        terrain.AddTriangle(d3e.v1, d3e.v3, center_d);
        terrain.AddTriangleColor(cell.color);
        //terrain.AddTriangle(d3e.v3, d3e.v5, center_d);
        //terrain.AddTriangleColor(cell.color);

        terrain.AddQuad(d3e.v5, d3e.v3, outEdge.v1, outEdge.v3);
        terrain.AddQuadColor(cell.color);

        terrain.AddTriangle(center_d, d3e.v3, outEdge.v3);
        terrain.AddTriangleColor(cell.color);

        terrain.AddTriangle(center_d, outEdge.v3, d1e.v3);
        terrain.AddTriangleColor(cell.color);

        //河流
        TriangulateRiverQuad(d1e.v4, d1e.v2, Vector3.Lerp(d1e.v5, center_d, 0.5f), outEdge.v4, cell.RiverSurfaceY, cell.RiverSurfaceY, 0.4f, false);
        TriangulateRiverQuad(d3e.v4, d3e.v2, outEdge.v2, Vector3.Lerp(d3e.v1, center_d, 0.5f), cell.RiverSurfaceY, cell.RiverSurfaceY, 0.4f, false);
        TriangulateRiverQuad(d2e.v4, d2e.v2, outEdge.v2, outEdge.v4, cell.RiverSurfaceY, cell.RiverSurfaceY, 0.4f, false);

    }

    // 三角化方法以便在一个单元的中心和一条边之间创建一个三角扇形
    // 某方向的边  插入 4个4边行
    void TriangulateEdgeFan(Vector3 center, EdgeVertices edge, Color color)
    {
        terrain.AddTriangle(center, edge.v1, edge.v2);
        terrain.AddTriangleColor(color);
        terrain.AddTriangle(center, edge.v2, edge.v3);
        terrain.AddTriangleColor(color);
        terrain.AddTriangle(center, edge.v3, edge.v4);
        terrain.AddTriangleColor(color);
        terrain.AddTriangle(center, edge.v4, edge.v5);
        terrain.AddTriangleColor(color);
    }

    // 对两条边之间一条四边形进行三角化的方法
    void TriangulateEdgeStrip(EdgeVertices e1, Color c1, EdgeVertices e2, Color c2)
    {
        terrain.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
        terrain.AddQuadColor(c1, c2);
        terrain.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
        terrain.AddQuadColor(c1, c2);
        terrain.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
        terrain.AddQuadColor(c1, c2);
        terrain.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);
        terrain.AddQuadColor(c1, c2);
    }

    void TriangulateRiverQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        rivers.AddQuad(v1, v2, v3, v4);
        rivers.AddQuadUV(0f, 1f, 0f, 1f);
    }

    void TriangulateRiverQuad( Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, float y1, float y2, float v, bool reversed)
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
    }

    void TriangulateRiverTriangle(Vector3 v1, Vector3 v2, Vector3 v3, float y)
    {
        v1.y = v2.y = v3.y = y;
        rivers.AddTriangle(v1, v2, v3);
        //rivers.AddQuadColor(color);
        rivers.AddTriangleUV(new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector3(0f, 1f));
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

            // 多个格子的 三角形连接处
            HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
            if (direction <= HexDirection.E && nextNeighbor && nextNeighbor.IsLakes())
            {
                lakes.AddTriangle(
                    c2, e2, c2 + HexMetrics.GetLakesBridge(direction.Next())
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

        // 连接处
        Vector3 center2 = neighbor.Position;
        center2.y = center.y;
        EdgeVertices e2 = new EdgeVertices(
            center2 + HexMetrics.GetSecondSolidCorner(direction.Opposite()),
            center2 + HexMetrics.GetFirstSolidCorner(direction.Opposite())
        );

        lakesShore.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
        lakesShore.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
        lakesShore.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
        lakesShore.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);
        lakesShore.AddQuadUV(0f, 0f, 0f, 1f);
        lakesShore.AddQuadUV(0f, 0f, 0f, 1f);
        lakesShore.AddQuadUV(0f, 0f, 0f, 1f);
        lakesShore.AddQuadUV(0f, 0f, 0f, 1f);


        // 多个格子的 三角形连接处
        HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
        if (nextNeighbor != null)
        {
            Vector3 v3 = nextNeighbor.Position + (nextNeighbor.IsLakes() ?
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

        }
    }

    // 河水与湖泊入口处
    void TriangulateWaterfallInWater(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, float y1, float y2, float waterY)
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
    }
}
