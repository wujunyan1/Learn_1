using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickCellView : View
{
    public UIShow btns;

    public Button personBtn;
    public Button cellBtn;
    public Button buildBtn;

    private HexCell cell;
    

    public void UpdateClickCell(HexCell cell)
    {
        this.cell = cell;
        UpdateView();
        btns.potion = cell.Position;

        if(cell.Troop != null)
        {
            OnBtnPerson();
        }
        else if(cell.Build != null)
        {
            OnBtnBuild();
        }
        else
        {
            OnBtnCell();
        }
    }

    private void Awake()
    {
        personBtn.onClick.AddListener(OnBtnPerson);
        buildBtn.onClick.AddListener(OnBtnBuild);
        cellBtn.onClick.AddListener(OnBtnCell);
    }

    public override void UpdateView()
    {
        if(cell == null)
        {
            return;
        }

        if(cell.Troop != null)
        {
            personBtn.gameObject.SetActive(true);
        }
        else
        {
            personBtn.gameObject.SetActive(false);
        }

        if (cell.Build != null)
        {
            buildBtn.gameObject.SetActive(true);
        }
        else
        {
            buildBtn.gameObject.SetActive(false);
        }

        cellBtn.gameObject.SetActive(true);;
    }

    

    public void OnBtnPerson()
    {
        BattlePlayerControl control = BattlePlayerControl.GetInstance();
        control.ClearShow();
        control.SelectCity(null);
        control.SelectTroop(cell.Troop);
    }

    public void OnBtnCell()
    {
        BattlePlayerControl control = BattlePlayerControl.GetInstance();
        control.ClearShow();
        control.ShowCell = cell;
    }

    public void OnBtnBuild()
    {

    }
}
