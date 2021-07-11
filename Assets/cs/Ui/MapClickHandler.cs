using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapClickHandler : EventComponent, MapClickBase
{
    public MapClickHandler()
    {
        this.Init("ground");
    }

    protected string maskName;

    protected int targetMask;

    protected HexCell selectCell;

    protected void Init(string maskName)
    {
        this.maskName = maskName;
    }

    private void Awake()
    {
        targetMask = LayerMask.GetMask(maskName);
    }

    //点击事件
    void Update()
    {
        //if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        if (Input.GetMouseButtonDown(0))
        {
            HandleInput();
        }
    }

    protected virtual void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit, 200f, targetMask))
        {
            TouchCell(hit.point);
            return;
        }
        else
        {
            HexCellUnClick();
        }
    }

    protected void TouchCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);

        HexVector vector = HexCoordinates.GameToHexCoordinate(position);
        HexCell newSelectCell = HexGrid.GetInstance().GetCell(vector);

        Debug.Log("selectCell at " + vector.X + " " + vector.Z);
        // Debug.Log("selectCell at " + selectCell);

        if (newSelectCell != null)
        {
            selectCell = newSelectCell;
            this.HexCellClick();
        }
    }

    public void HexCellClick()
    {

    }

    public void HexCellUnClick()
    {

    }
}
