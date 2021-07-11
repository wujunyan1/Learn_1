using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmControl : MonoBehaviour
{
    public GameObject wheatPerfabs;

    public GameObject cropland;

    public HexMesh mesh;

    List<Vector3> meshArea;

    public void RefreshWheat()
    {
        
    }





    public void ClearMesh()
    {
        // 清楚渲染
        mesh.Clear();
    }

    public void RefreshGround(HexCell cell)
    {
        if(meshArea != null)
        {
            ListPool<Vector3>.Add(meshArea);
        }

        ClearMesh();

        Triangulate(cell);

        meshArea = mesh.GetVertices();

        ApplyMesh();
    }

    public void ApplyMesh()
    {
        //渲染
        mesh.Apply();
    }



    void Triangulate(HexCell cell)
    {
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            Triangulate(d, cell);
        }
    }

    void Triangulate(HexDirection direction, HexCell cell)
    {
        Vector3 center = new Vector3(0, 0.1f, 0);

        EdgeVertices e = new EdgeVertices(
            center + HexMetrics.GetFirstSolidCorner(direction),
            center + HexMetrics.GetSecondSolidCorner(direction)
        );

        mesh.AddTriangle(center, e.v1, e.v5);

        HexDirection nextDir = direction.Next();

        HexCell nei = cell.GetNeighbor(direction);
        HexCell nextNei = cell.GetNeighbor(nextDir);

        if (cell.GetEdgeType(direction) == HexEdgeType.Flat)
        {
            Vector3 bridge = HexMetrics.GetBridge(direction) / 2;
            EdgeVertices e2 = new EdgeVertices(
                e.v1 + bridge,
                e.v5 + bridge
            );

            if (nei == null || nei.Build == null || !(nei.Build is Farm))
            {
                e2.v1.y = e2.v5.y = 0;
            }

            mesh.AddQuad(e.v1, e.v5, e2.v1, e2.v5);

            // 那么三角形区域也要
            if(cell.GetEdgeType(nextDir) == HexEdgeType.Flat)
            {
                Vector3 nextBridge = HexMetrics.GetBridge(nextDir) / 2;

                Vector3 p3 = e.v5 + nextBridge;

                if (nextNei == null || nextNei.Build == null || !(nextNei.Build is Farm))
                {
                    p3.y = 0;
                }

                mesh.AddTriangle(e.v5, e2.v5, p3);
            }
        }
    }
}
