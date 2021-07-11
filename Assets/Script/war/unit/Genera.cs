using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genera : Soldier
{
    public string genera_name;

    /// <summary>
    /// 血量
    /// </summary>
    public int blood_exp;

    /// <summary>
    /// 攻击力
    /// </summary>
    public int ATK_exp;

    /// <summary>
    /// 护甲
    /// </summary>
    public int armor_exp;

    /// <summary>
    /// 闪避 100
    /// </summary>
    public int dodge_exp;

    /// <summary>
    /// 格挡 100
    /// </summary>
    public int parry_exp;

    /// <summary>
    /// 冲锋
    /// </summary>
    public int charge_exp;

    /// <summary>
    /// 士气
    /// </summary>
    public int morale_exp;

    public int l_ATK_exp;

    /// <summary>
    /// 冲锋
    /// </summary>
    public int accurate_exp;

    public int ATKSpeed_exp;

    public int shootingSpeed_exp;

    public void SetGeneraData(GeneraData data)
    {
        base.SetSoliderData(data);

        this.genera_name = data.genera_name;

        /// <summary>
        /// 血量
        /// </summary>
        this.blood_exp = data.blood_exp;

        /// <summary>
        /// 攻击力
        /// </summary>
        this.ATK_exp = data.ATK_exp;

        /// <summary>
        /// 护甲
        /// </summary>
        this.armor_exp = data.armor_exp;

        /// <summary>
        /// 闪避 100
        /// </summary>
        this.dodge_exp = data.dodge_exp;

        /// <summary>
        /// 格挡 100
        /// </summary>
        this.parry_exp = data.parry_exp;

        /// <summary>
        /// 冲锋
        /// </summary>
        this.charge_exp = data.charge_exp;

        /// <summary>
        /// 士气
        /// </summary>
        this.morale_exp = data.morale_exp;

        this.l_ATK_exp = data.l_ATK_exp;

        /// <summary>
        /// 冲锋
        /// </summary>
        this.accurate_exp = data.accurate_exp;

        this.ATKSpeed_exp = data.ATKSpeed_exp;

        this.shootingSpeed_exp = data.shootingSpeed_exp;
    }
}
