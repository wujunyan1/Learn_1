using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayerControl : EventObject
{
    private static BattlePlayerControl instance;
    private static object locker = new object();

    public static BattlePlayerControl GetInstance()
    {
        if (instance == null)
        {
            lock (locker)
            {
                if (instance == null)
                {
                    instance = new BattlePlayerControl();
                }
            }
        }
        return instance;
    }


    int campId;

    Troop selectTroop;
    City selectCity;
    HexCell selectCell;
    ClickCellView clickCellView;

    private BattlePlayerControl()
    {
        this.RegisterEvent("LeftMouseButton", LeftMouseButton);
        this.RegisterEvent("RightMouseButton", RightMouseButton);
        this.RegisterEvent("LeftMouseButtonBlank", LeftMouseButtonBlank);

        this.RegisterEvent("SelectCity", LeftMouseButtonCity);
    }

    public void ClearShow()
    {
        SelectTroop(null);
        ShowCell = null;
    }

    public void SelectTroop(Troop troop)
    {
        selectTroop = troop;
        if(selectTroop != null)
        {
            ShowTroopSearchPath();
            OpenTroopView();
        }
        else
        {
            if (showCanMoveCells != null) {
                HexGrid.instance.ClearShowPath(showCanMoveCells);
                showCanMoveCells = null;
            }
            CloseTroopView();
        }
    }

    public void SelectCity(City city)
    {
        selectCity = city;
        if (selectCity != null)
        {
            OpenCityView();
        }
        else
        {
            CloseCityView();
        }
    }

    public void SelectCell(HexCell cell)
    {
        if(selectCell != null)
        {
            selectCell.DisableHighlight();
            if (cell == null)
            {
                CloseClickCellView();
            }
            selectCell = null;
        }
        selectCell = cell;
        if(selectCell != null)
        {
            selectCell.EnableHighlight(HexGrid.instance.clickHighlight);
            //OpenClickCellView(selectCell);
        }
    }

    public HexCell GetSelectCell()
    {
        return selectCell;
    }

    public HexCell ShowCell { get; set; }

    public void LeftMouseButton(UEvent e)
    {
        HexCell cell = (HexCell)e.eventParams;

        SelectTroop(null);
        SelectCell(cell);
    }

    public void RightMouseButton(UEvent e)
    {
        HexCell cell = (HexCell)e.eventParams;

        // 选择了某个部队
        if(selectTroop != null)
        {
            // 是玩家阵营，进行操作， 移动/攻击
            if(selectTroop.camp.GetId() == GameCenter.instance.PlayerCampId)
            {
                PlayerTroopSelectHexCell(cell);
            }
        }
    }

    public void LeftMouseButtonBlank(UEvent e)
    {
        SelectCell(null);
        SelectTroop(null);
        SelectCity(null);
    }


    public void LeftMouseButtonCity(UEvent e)
    {
        City city = (City)e.eventParams;

        SelectTroop(null);
        SelectCity(city);
    }

    List<HexCell> showCanMoveCells = null;
    private void ShowTroopSearchPath()
    {
        showCanMoveCells = HexGrid.instance.ShowCanMoveCell(selectTroop.control.Location, selectTroop.data.speed);
    }

    /// <summary>
    /// 选中的队伍，指向cell
    /// </summary>
    void PlayerTroopSelectHexCell(HexCell cell)
    {
        // 目标点有部队
        if (cell.Troop != null)
        {
            // 目标点的部队是自己的部队
            if(cell.Troop.camp.GetId() == GameCenter.instance.PlayerCampId)
            {

            }
            else
            {

            }
        }
        else if(cell.Build != null)
        {

        }
        else
        {
            Troop t = selectTroop;
            selectTroop.MoveToHexCell(cell, ()=> {
                // 如果还是null 则选中这个移动的队伍
                if(selectCell == null)
                {
                    SelectCell(t.control.Location);
                }
            });

            ShowCell = null;
            SelectTroop(null);
            SelectCell(null);
        }
    }

    void OpenClickCellView(HexCell cell)
    {
        if (clickCellView == null)
        {
            ViewManager.Instance.OpenView<ClickCellView>("ClickCellView", out clickCellView);
        }

        clickCellView.UpdateClickCell(cell);
    }

    void CloseClickCellView()
    {
        if(clickCellView != null)
        {
            ViewManager.Instance.RemoveView(clickCellView);
            clickCellView = null;
        }
    }



    TroopView troopView;
    void OpenTroopView()
    {
        UObject o = UObjectPool.Get();
        o.Set("Troop", selectTroop);
        ViewManager.Instance.OpenView("TroopView", out troopView, o);
    }

    void CloseTroopView()
    {
        if(troopView != null)
        {
            ViewManager.Instance.RemoveView(troopView);
            troopView = null;
        }
    }


    CityView cityView;
    void OpenCityView()
    {
        UObject o = UObjectPool.Get();
        o.Set("City", selectCity);
        ViewManager.Instance.OpenView("CityView", out cityView, o);
    }

    void CloseCityView()
    {
        if (cityView != null)
        {
            ViewManager.Instance.RemoveView(cityView);
            cityView = null;
        }
    }
    


}
