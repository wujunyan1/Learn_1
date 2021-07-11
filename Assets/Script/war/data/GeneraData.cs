using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneraData : SoldierData
{
    public string genera_name;

    /// <summary>
    /// 血量
    /// </summary>
    [SerializeField]
    public int blood_exp;

    /// <summary>
    /// 攻击力
    /// </summary>
    [SerializeField]
    public int ATK_exp;

    /// <summary>
    /// 护甲
    /// </summary>
    [SerializeField]
    public int armor_exp;

    /// <summary>
    /// 闪避 100
    /// </summary>
    [SerializeField]
    public int dodge_exp;

    /// <summary>
    /// 格挡 100
    /// </summary>
    [SerializeField]
    public int parry_exp;

    /// <summary>
    /// 冲锋
    /// </summary>
    [SerializeField]
    public int charge_exp;

    /// <summary>
    /// 士气
    /// </summary>
    [SerializeField]
    public int morale_exp;
    

    [SerializeField]
    public int l_ATK_exp;

    /// <summary>
    /// 冲锋
    /// </summary>
    [SerializeField]
    public int accurate_exp;

    [SerializeField]
    public int ATKSpeed_exp;

    [SerializeField]
    public int shootingSpeed_exp;


}
