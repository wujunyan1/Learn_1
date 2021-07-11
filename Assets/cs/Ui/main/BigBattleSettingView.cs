using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBattleSettingView : LockCameraView
{
    BtnListMenuView btnListMenu;

    private void Start()
    {
        btnListMenu = GetComponent<BtnListMenuView>();
        
        btnListMenu.AddButton("返回游戏", OnBtnBackGame);
        btnListMenu.AddButton("游戏设置", OnBtnSettingGame);
        btnListMenu.AddButton("保存游戏", OnBtnSaveGame);
        btnListMenu.AddButton("加载游戏", OnBtnLoadGame);
        btnListMenu.AddButton("返回主界面", OnBtnReturnMainScene);
        btnListMenu.AddButton("退出游戏", OnBtnExitGame);
    }

    public override void Open(UObject o)
    {
        base.Open(o);

        gameObject.SetActive(true);
    }

    public override void Close()
    {
        base.Close();
    }

    public override void UpdateView()
    {
        base.UpdateView();
    }

    public override void Show()
    {
        base.Show();
        gameObject.SetActive(true);
    }

    private void HideView()
    {
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// 创建新游戏
    /// </summary>
    public void OnBtnCreaterNewGame()
    {

    }

    /// <summary>
    /// 保存新游戏
    /// </summary>
    public void OnBtnSaveGame()
    {
        SaveLoadMenu menu;
        UObject o = new UObject();
        o.Set("saveMode", true);
        ViewManager.Instance.OpenView<SaveLoadMenu>("SaveLoadMenu", out menu, o);

        HideView();
    }

    /// <summary>
    /// 加载新游戏
    /// </summary>
    public void OnBtnLoadGame()
    {
        SaveLoadMenu menu;
        UObject o = new UObject();
        o.Set("saveMode", false);
        ViewManager.Instance.OpenView<SaveLoadMenu>("SaveLoadMenu", out menu, o);

        HideView();
    }

    /// <summary>
    /// 返回主界面
    /// </summary>
    public void OnBtnReturnMainScene()
    {

    }

    /// <summary>
    /// 游戏设置
    /// </summary>
    public void OnBtnSettingGame()
    {

    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void OnBtnExitGame()
    {

    }

    /// <summary>
    /// 返回游戏
    /// </summary>
    public void OnBtnBackGame()
    {
        this.Close();
    }
}
