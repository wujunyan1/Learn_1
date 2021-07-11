using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WarType
{
    /// <summary>
    /// 战前
    /// </summary>
    PREWAR,

    /// <summary>
    /// 中
    /// </summary>
    WAR,

    /// <summary>
    /// 战后
    /// </summary>
    POSTWAR,
}

public class War : MonoBehaviour
{

    // 当前状态
    static WarType warType;
    public static WarType CurrWarType
    {
        get
        {
            return warType;
        }
    }

    // 60帧 逻辑
    int fps = 60;

    public GameObject startBattleBtn;

    public List<CampControl> camps;

    private void Awake()
    {
        warType = WarType.PREWAR;
        startBattleBtn.SetActive(true);
    }
    
    private void CreateCampControl()
    {
        GameObject node = new GameObject();
        node.name = "camp_2";
        node.transform.SetParent(transform, false);
        node.transform.localScale = new Vector3(1, 1, 1);
        node.transform.localPosition = new Vector3(0, 0, 0);
        node.transform.localRotation = Quaternion.identity;
        //CampControl control = node.AddComponent<CampControl>();
        //camps.Add(control);
    }

    public void StartBattle()
    {
        warType = WarType.WAR;
        startBattleBtn.SetActive(false);

        //ParamLoader.GetInstance().LoadSoliderCSV();
    }

    public void BattleEnd()
    {
        warType = WarType.POSTWAR;
    }

    private void FixedUpdate()
    {
        foreach(CampControl control in camps)
        {
            control.LogicUpdate();
        }
    }
}
