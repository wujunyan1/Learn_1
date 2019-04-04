using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjGenerate : MonoBehaviour
{
    public CreaterControl createrPrefab;

    public PersonControlView personUIShow;

    public FunctionGenerate functionGenerate;

    public void GenerateCreater(Creater creater)
    {
        CreaterControl item = Instantiate(createrPrefab);
        item.SetPerson(creater);
        item.controlView = personUIShow;
        item.AddFunction(new MoveFunction());

        HexGrid grid = HexGrid.instance;
        HexCell cell = grid.GetCell(creater.point);
        item.transform.localPosition = cell.transform.localPosition;
        item.transform.SetParent(grid.objPlane, false);

        cell.person = item;
    }


}
