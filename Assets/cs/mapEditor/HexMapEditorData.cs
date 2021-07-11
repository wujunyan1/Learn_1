using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum OptionalToggle
{
    Ignore, Yes, No
}

public class HexMapEditorData
{
    HexCell preCell;
    HexCell selectCell;
    HexTerrainType terrainType;

    bool updateHeight;
    int height;

    int brushSize = 0;

    public bool isDrag;
    public HexDirection dragDirection;

    OptionalToggle riverMode, roadMode;

    bool[] walls;

    bool addPerson = false;

    private static HexMapEditorData instance;

    private Dictionary<int, HexVector> meshs;

    private HexMapEditorData()
    {
        terrainType = HexTerrainType.Land;
        meshs = new Dictionary<int, HexVector>();

        walls = new bool[HexMetrics.HexDirectionNum];
    }

    public static HexMapEditorData GetInstance()
    {
        if(instance == null)
        {
            instance = new HexMapEditorData();
        }
        return instance;
    }

    void EditCell(HexCell cell)
    {
        if(cell != null) { 
            Debug.Log(cell.Vector);
            Debug.Log(cell.Vector.GetIndex());

            cell.TerrainType = terrainType;//  GameConfigData.GetInstance().GetColor(1);

            //if(terrainType == HexTerrainType.Water)
            //{
            //    cell.LakesLevel = cell.Height + 1;
            //}
            

            if (this.updateHeight)
            {
                // cell.height = height;
                cell.Elevation = height;
            }
            if (roadMode == OptionalToggle.No)
            {
                cell.RemoveRoads();
            }
            if (riverMode == OptionalToggle.No)
            {
                cell.RemoveRiver();
            }
            else if (isDrag)
            {
                HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
                if (otherCell)
                {
                    if(roadMode == OptionalToggle.Yes)
                    {
                        otherCell.AddRoad(dragDirection);
                    }
                    if(riverMode == OptionalToggle.Yes)
                    {
                        otherCell.SetOutgoingRiver(dragDirection);
                    }
                    
                }
            }

            cell.SetWalls(walls);
            if (addPerson)
            {
                AddPerson(cell);
            }

            int index = HexGrid.GetInstance().GetChunkIndex(cell.Vector);

            // 没有则加入
            HexVector res;
            if (!meshs.TryGetValue(index, out res))
            {
                meshs.Add(index, cell.Vector);
            }
        }
    }


    void AddPerson(HexCell cell)
    {
        HexCoordinates coordinates = cell.coordinates;
        GameCenter center = GameCenter.instance;

        Camp camp = center.GetCamp(center.PlayerCampId);
        // camp.AddCreater(camp.CreatCreater(coordinates));

        Troop troop = new Troop();
        troop.IsNew();
        camp.AddTroop(troop);

        //Creater creater = new Creater();
        //camp.AddPerson(creater);

        TroopControl troopControl = GameCenter.instance.GenerateTroopModel(troop);
        //creater.SetPointAndUpdateToModel(coordinates);
        troopControl.Location = cell;
        cell.Troop = troop;
    }


    void UpdateCell()
    {
        meshs.Clear();
        Debug.Log("111111111111111111111111111111111");
        Debug.Log(selectCell);
        if (selectCell != null)
        {
            HexVector vector = selectCell.Vector;

            int centerX = vector.X;
            int centerZ = vector.Z;

            HexGrid ground = HexGrid.GetInstance();
            for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
            {
                for (int x = centerX - r; x <= centerX + brushSize; x++)
                {
                    EditCell(ground.GetCell(new HexVector(x, z)));
                }
            }
            for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++)
            {
                for (int x = centerX - brushSize; x <= centerX + r; x++)
                {
                    EditCell(ground.GetCell(new HexVector(x, z)));
                }
            }

            UpdateMesh();
        }
    }

    void UpdateMesh()
    {
        Debug.Log("-------------------");
        var indexs = meshs.Keys;
        foreach(int index in indexs)
        {
            Debug.Log(index);
            HexVector vector;
            if(meshs.TryGetValue(index, out vector))
            {
                // 触发事件
                ObjectEventDispatcher.dispatcher.dispatchEvent(new UEvent(EventTypeName.UPDATE_CHUNK_MESH, vector.GetIndex()), this);
            }
        }
    }
    
    public HexCell GetPreHexCell()
    {
        return preCell;
    }

    public void SetPreHexCell(HexCell cell)
    {
        preCell = cell;
    }

    public void SelectHexCell(HexCell cell)
    {
        selectCell = cell;
        UpdateCell();
    }

    public void UpdateColor(HexTerrainType color)
    {
        this.terrainType = color;
        UpdateCell();
    }

    public void SetUpdateHeight(bool select)
    {
        this.updateHeight = select;
    }

    public void UpdateHeight(int height)
    {
        this.height = height;
        if (this.updateHeight)
        { 
            UpdateCell();
        }
    }

    public void UpdateBrushSize(int size)
    {
        this.brushSize = size;
    }

    public void UpdateRiver(int mode)
    {
        this.riverMode = (OptionalToggle)mode;
    }

    public void SetRoadMode(int mode)
    {
        roadMode = (OptionalToggle)mode;
    }

    public void SetWallDir(int dir, bool isWall)
    {
        walls[dir] = isWall;
    }

    public void SetAddPerson(bool toggle)
    {
        addPerson = toggle;
    }
}
