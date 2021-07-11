using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUI : EventComponent
{
    public RectTransform baseUI;
    public BattleGroupView battleGroupView;

    // 保持全局只会有一个
    private SoldierMessageView openSoldierMessageView = null;

    // Start is called before the first frame update
    void Start()
    {
        this.RegisterEvent("OpenBattleGroupView", OpenBattleGroupView);

        this.RegisterEvent("OpenSoldierMessageView", OpenSoldierMessageView);
        this.RegisterEvent("CloseSoldierMessageView", CloseSoldierMessageView);

        this.RegisterEvent("SHOW_MSG", ShowMsg);

        this.RegisterEvent("OpenBattleScene", OpenBattleScene);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenUI(string key)
    {
        this.FireEvent(key);
    }

    public void ShowMsg(UEvent org)
    {
        MsgUI prefab = Resources.Load<MsgUI>("prefabs/Base/MsgPrefab");
        MsgUI.MsgData data = (MsgUI.MsgData)org.eventParams;

        MsgUI msgUI = GameObject.Instantiate<MsgUI>(prefab);
        msgUI.SetMsg(data);

        msgUI.transform.SetParent(baseUI, false);
    }

    public void OpenBattleGroupView(UEvent org)
    {
        BattleGroupView openUI = GameObject.Instantiate<BattleGroupView>(battleGroupView, baseUI);
    }

    public void OpenSoldierMessageView(UEvent org)
    {
        BattleSoldierData data = (BattleSoldierData)org.eventParams;

        SoldierMessageView openUI;
        if (openSoldierMessageView != null)
        {
            openUI = openSoldierMessageView;
        }
        else
        {
            SoldierMessageView prefab = Resources.Load<SoldierMessageView>("prefabs/Base/SoldierMessagePanel");
            openUI = GameObject.Instantiate<SoldierMessageView>(prefab, baseUI);
        }
        openUI.SetBattleSoldierData(data);
        openSoldierMessageView = openUI;
    }

    public void CloseSoldierMessageView(UEvent org)
    {
        if (openSoldierMessageView)
        {
            GameObject.Destroy(openSoldierMessageView.gameObject);
            openSoldierMessageView = null;
        }
    }

    public void OpenBattleScene(UEvent org)
    {
        SceneManager.LoadScene("Scenes/BattleScene");
    }
}
