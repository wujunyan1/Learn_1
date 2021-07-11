using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControlOLD : EventComponent
{
    // Start is called before the first frame update
    public MsgUI MsgPrefabs;
    public RectTransform UIBase;

    void Start()
    {
        this.RegisterEvent("StartGame", StartGame);

        this.RegisterEvent("SHOW_MSG", ShowMsg);
    }

    void StartGame(UEvent org)
    {
        Debug.Log("start game");
    }

    void ShowMsg(UEvent org)
    {
        MsgUI.MsgData data = (MsgUI.MsgData)org.eventParams;

        MsgUI msgUI = GameObject.Instantiate<MsgUI>(MsgPrefabs);
        msgUI.SetMsg(data);

        msgUI.transform.SetParent(UIBase, false);
    }
}
