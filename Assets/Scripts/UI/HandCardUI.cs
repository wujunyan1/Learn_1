using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCardUI : EventComponent
{
    public CardUI cardPrefabs;

    public Transform handParent;

    // Start is called before the first frame update
    void Start()
    {
        this.RegisterEvent("CardGroupView_Add_Hand_Card_Ani", AddHandCard);
        this.RegisterEvent("Close_UI_HandCardUI", CloseUI);
        this.RegisterEvent("Open_UI_HandCardUI", OpenUI);
    }

    void AddHandCard(UEvent org)
    {
        this.UpdateHandCard();
    }

    void UpdateHandCard()
    {
        CardUI[] children = handParent.GetComponentsInChildren<CardUI>();

        Camp camp = BattleData.GetInstance().GetSelfCamp();

        
    }

    public void CloseView()
    {
        this.gameObject.SetActive(false);
    }

    public void CloseUI(UEvent org)
    {
        this.CloseView();
    }

    public void OpenUI(UEvent org)
    {
        this.gameObject.SetActive(true);
    }

    public void OnClickOtherArea()
    {
        this.CloseView();
    }
}
