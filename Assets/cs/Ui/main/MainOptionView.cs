using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainOptionView : View
{
    // Start is called before the first frame update
    void Start()
    {
        this.RegisterEvent("CREATER_NEW_GAME", CloseView);
    }

    private void OnEnable()
    {
        CameraMove.Locked = true;
    }

    private void OnDisable()
    {
        CameraMove.Locked = false;
    }

    /*
    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (this.gameObject.activeSelf)
            {
                Close();
            }
            else
            {
                Open();
            }
        }
    }
    */

    public void OnBtnSave()
    {
        UObject o = UObjectPool.Get();
        o.Set("UI_NAME", "SaveOrLoadUI");
        o.Set("saveMode", true);

        this.FireEvent("OPEN_UI", o);
    }

    public void OnBtnLoad()
    {
        UObject o = UObjectPool.Get();
        o.Set("UI_NAME", "SaveOrLoadUI");
        o.Set("saveMode", false);

        this.FireEvent("OPEN_UI", o);
    }

    public void OnBtnNew()
    {
        UObject o = UObjectPool.Get();
        o.Set("UI_NAME", "CreateNewGameUI");
        o.Set("saveMode", false);

        this.FireEvent("OPEN_UI", o);
    }

    public override void Close()
    {
        base.Close();
        this.gameObject.SetActive(false);
    }

    public override void Open(UObject o)
    {
        base.Open(o);
        gameObject.SetActive(true);
    }

    public void CloseView(UEvent uEvent)
    {
        Close();
    }
}
