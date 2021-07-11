using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BigBattleUI : EventComponent
{
    /// <summary>
    /// 玩家选中的人物（开荒，部队，粮车，商队）
    /// </summary>
    Person selectedPerson;

    public Toggle[] specialMaps;

    //HexCell selectCell;

    bool isDrag;
    HexDirection dragDirection;
    int targetMask;
    int unTargetMask;
    int unTargetLayer;


    public Transform testCheat;
    public InputFieldSubmit testCheatInput;

    private void Awake()
    {
        targetMask = LayerMask.GetMask("Map");
        unTargetMask = LayerMask.GetMask("UI");
        unTargetLayer = LayerMask.NameToLayer("UI");
    }

    private void Start()
    {
        //testCheatInput.OnSubmit();

        testCheatInput.onSubmit.AddListener(_subTestCheat);
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) 
            && !IsPointerOverGameObject(Input.mousePosition))
        {
            HexCell cell = HandleInput();
            if(cell == null)
            {
                this.FireEvent("LeftMouseButtonBlank");
            }
            // 没变化
            else if(cell == BattlePlayerControl.GetInstance().GetSelectCell())
            {

            }
            else
            {
                this.FireEvent("LeftMouseButton", cell);
            }
        }
        //右键点击
        else if (Input.GetMouseButtonUp(1) 
            && !IsPointerOverGameObject(Input.mousePosition))
        {
            
            HexCell cell = HandleInput();

            if (cell == null)
            {
                
            }
            // 没变化
            else if (cell == BattlePlayerControl.GetInstance().GetSelectCell())
            {

            }
            else
            {
                this.FireEvent("RightMouseButton", cell);
            }
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            OnBtnSetting();
        }


        if (GameVersions.Debug)
        {
            if (Input.GetKeyUp(KeyCode.T))
            {
                testCheat.gameObject.SetActive(true);
            }
        }
    }

    HexCell HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        RaycastHit uihit;

        BattlePlayerControl control = BattlePlayerControl.GetInstance();
        HexCell selectCell = control.GetSelectCell();

        

        // 没有点击UI
        //if (!Physics.Raycast(inputRay, out uihit, 2000f, unTargetMask) &&
        if( Physics.Raycast(inputRay, out hit, 2000f, targetMask))
        {
            HexGrid grid = HexGrid.GetInstance();

            Vector3 position = hit.point;
            position = grid.transform.InverseTransformPoint(position);

            if (GameCenter.instance.gameData.wrapping)
            {
                if (position.x > grid.GridWidth)
                {
                    position.x -= grid.GridWidth;
                }
                if(position.x < 0)
                {
                    position.x += grid.GridWidth;
                }
            }

            HexVector vector = HexCoordinates.GameToHexCoordinate(position);
            HexCell newSelectCell = HexGrid.GetInstance().GetCell(vector);
            // HexCell previousCell = HexMapEditorData.GetInstance().GetPreHexCell();

            Debug.Log("11111111111111111111");
            Debug.Log(position);

            string s = "";
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                s += d + ":" + newSelectCell.GetRiverDirection(d) + "  ";
                
            }
            Debug.Log(s);

            newSelectCell.RefreshOnlySelf();

            //ClimateData climateData = grid.GetClimateData(newSelectCell.index);
            //ObjBaseTool.PrintObj("climateData", climateData);
            //ObjBaseTool.PrintObj("nextClimateData", grid.GetNextClimateData(newSelectCell.index));
            Debug.Log(newSelectCell.Elevation);

            if (selectCell && selectCell == newSelectCell)
            {
                return newSelectCell;
            }

            // 属于隐藏图块
            if (!newSelectCell.IsExplored)
            {
                //HexCellUnClick();
                return null;
            }

            // 暂时没拖拽
            //if (previousCell && previousCell != newSelectCell)
            //{
            //    ValidateDrag(newSelectCell);
            //}
            //else
            //{
            //    isDrag = false;
            //}
            
            //HexCellClick(newSelectCell);
            return newSelectCell;
        }
        else
        {
            //HexCellUnClick();
            return null;
        }
    }

    private bool IsPointerOverGameObject(Vector2 mousePosition)
    {
        //创建一个点击事件
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();

        //向点击位置发射一条射线，检测是否点击UI
        EventSystem.current.RaycastAll(eventData, raycastResults);
        if (raycastResults.Count > 0)
        {
            foreach (var item in raycastResults)
            {
                if (unTargetLayer == item.gameObject.layer)
                {
                    return true;
                }
            }
            return false;
        }
        else
        {
            return false;
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

    void HexCellClick(HexCell selectCell)
    {
        this.FireEvent("LeftMouseButton", selectCell);
    }

    void HexCellUnClick()
    {
        this.FireEvent("LeftMouseButtonBlank");
    }

    public void NextRound()
    {
        Debug.Log("NextRound");
        GameCenter.instance.NextRound();
    }
    
    // 显示地图信息
    public void ShowSpecialMap(int index)
    {
        Toggle t = specialMaps[index];
        
        if (t.isOn)
        {
            GameCenter.instance.ShowMapData(index);
        }
        else
        {
            bool hasOn = false;
            foreach (var item in specialMaps)
            {
                if (item.isOn)
                {
                    hasOn = true;
                    break;
                }
            }

            if (!hasOn)
            {
                GameCenter.instance.HideMapData();
            }
        }
    }
    
    public void OnBtnSetting()
    {
        string key = "BigBattleSettingView";
        ViewManager viewManager = ViewManager.Instance;
        BigBattleSettingView view = viewManager.GetShowView<BigBattleSettingView>(key);

        if(view == null)
        {
            viewManager.OpenView<BigBattleSettingView>(key, out view);
        }
        else
        {
            //view.Close();

            viewManager.CloseView();
        }
    }




    void _subTestCheat(string arg)
    {
        if (GameVersions.Debug)
        {
            Debug.Log(arg);

            CmdExecute.Execute(arg);
        }

        testCheat.gameObject.SetActive(false);
    }
}
