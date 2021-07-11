using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : View, MapClickBase
{
    GameObject editorPanel;
    bool isDrag
    {
        get
        {
            return HexMapEditorData.GetInstance().isDrag;
        }
        set
        {
            HexMapEditorData.GetInstance().isDrag = value;
        }
    }
    HexDirection dragDirection
    {
        get
        {
            return HexMapEditorData.GetInstance().dragDirection;
        }
        set
        {
            HexMapEditorData.GetInstance().dragDirection = value;
        }
    }

    protected string maskName;

    protected int targetMask;

    protected HexCell selectCell;

    int unTargetMask;

    public HexMapEditor()
    {
        this.Init("Map");

    }
    protected void Init(string maskName)
    {
        this.maskName = maskName;
    }

    private void Awake()
    {
        targetMask = LayerMask.GetMask(maskName);
        unTargetMask = LayerMask.GetMask("UI");
    }

    //点击事件
    void Update()
    {
        
        if (Input.GetMouseButton(0))
        {
            HandleInput();
        }
        else
        {
            HexCellUnClick();
        }
    }

    protected void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        RaycastHit uihit;

        // 没有点击UI
        if (!Physics.Raycast(inputRay, out uihit, 200f, unTargetMask) &&
            Physics.Raycast(inputRay, out hit, 200f, targetMask))
        {
            HexGrid grid = HexGrid.GetInstance();

            Vector3 position = hit.point;
            position = grid.transform.InverseTransformPoint(position);

            HexVector vector = HexCoordinates.GameToHexCoordinate(position);
            HexCell newSelectCell = HexGrid.GetInstance().GetCell(vector);
            HexCell previousCell = HexMapEditorData.GetInstance().GetPreHexCell();

            if(selectCell && selectCell == newSelectCell)
            {
                return;
            }

            if (previousCell && previousCell != newSelectCell)
            {
                ValidateDrag(newSelectCell);
            }
            else
            {
                isDrag = false;
            }


            selectCell = newSelectCell;
            HexCellClick();
            HexMapEditorData.GetInstance().SetPreHexCell(newSelectCell);
            return;
        }
        else
        {
            HexCellUnClick();
        }
    }

    void ValidateDrag(HexCell currentCell)
    {
        HexCell previousCell = HexMapEditorData.GetInstance().GetPreHexCell();
        for (
            dragDirection = HexDirection.NE;
            dragDirection <= HexDirection.NW;
            dragDirection++
        )
        {
            if (previousCell.GetNeighbor(dragDirection) == currentCell)
            {
                isDrag = true;
                return;
            }
        }
        isDrag = false;
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
        Debug.Log("HexCellClick");
        Debug.Log(selectCell);
        HexMapEditorData.GetInstance().SelectHexCell(selectCell);
    }

    public void HexCellUnClick()
    {
        HexMapEditorData.GetInstance().SetPreHexCell(null);
    }


    // 加载编辑UI
    private void OnEnable()
    {
        Shader.EnableKeyword("HEX_MAP_EDIT_MODE");
    }

    private void OnDestroy()
    {
        Shader.DisableKeyword("HEX_MAP_EDIT_MODE");
    }

    public void UpdateSelectCellColor(Color color)
    {

    }
}
