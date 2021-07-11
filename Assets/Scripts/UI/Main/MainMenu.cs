using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : View
{
    RectTransform rectTransform;
    VerticalLayoutGroup layoutGroup;
    public int startBtnNum;
    public RectTransform startBattleSectTransform;

    private void Start()
    {
        this.rectTransform = GetComponent<RectTransform>();
        layoutGroup = GetComponent<VerticalLayoutGroup>();
    }

    public void OnBtnStartBattle()
    {
        Vector2 sizeDelta = this.startBattleSectTransform.sizeDelta;
        
        if(sizeDelta.y <= 31)
        {
            this.startBattleSectTransform.sizeDelta = new Vector2(sizeDelta.x, startBtnNum * 30);
        }
        else
        {
            this.startBattleSectTransform.sizeDelta = new Vector2(sizeDelta.x, 30);
        }

        // 更新layout 框大小
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }


    public void OnBtnEditorBattle()
    {
        View view;
        ViewManager.Instance.OpenView("ReadlyEditorMapView", out view);
    }

    public void OnBtnNewBattle()
    {
        NewMapMenu newMapMenu;
        ViewManager.Instance.OpenView<NewMapMenu>("NewMapMenu", out newMapMenu);
    }

    public void OnBtnLoadBattle()
    {
        SaveLoadMenu menu;
        UObject o = new UObject();
        o.Set("saveMode", false);
        ViewManager.Instance.OpenView<SaveLoadMenu>("SaveLoadMenu", out menu, o);
    }
}
