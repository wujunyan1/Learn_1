using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjGenerate : MonoBehaviour
{
    public CreaterControl createrPrefab;
    public City cityPrefab;

    public PersonInspectorView personUIShow;

    public FunctionGenerate functionGenerate;

    bool isf = true;

    public static ObjGenerate instance;

    private void Awake()
    {
        instance = this;
    }

    public void GenerateCreater(Creater creater)
    {
        CreaterControl item = Instantiate(createrPrefab);
        item.SetPerson(creater);
        item.controlView = personUIShow;
        //item.AddFunction(new MoveFunction());

        HexGrid grid = HexGrid.instance;
        //HexCell cell = grid.GetCell(creater.Point);

        //item.transform.localPosition = cell.transform.localPosition;
        item.Direction = (HexDirection)Random.Range(0, HexMetrics.HexDirectionNum);

        if (isf)
        {
            //new Vector3(0, 10, 0); //
            //item.transform.localPosition = new Vector3(0, 10, 0);
            isf = false;
        }
        item.transform.SetParent(grid.objPlane, false);

        //cell.Person = item;
    }

    public City CreateCity()
    {
        City item = Instantiate(cityPrefab);

        HexGrid grid = HexGrid.instance;
        item.transform.SetParent(grid.objPlane, false);
        return item;
    }

    public City CreateCity(HexCoordinates coordinates)
    {
        City item = Instantiate(cityPrefab);

        HexGrid grid = HexGrid.instance;
        item.transform.SetParent(grid.objPlane, false);

        item.BuildNewCity(coordinates);

        return item;
    }
}
