using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierConfigDatasss
{
    protected string soliderKey;
    public string KEY
    {
        get
        {
            return soliderKey;
        }
    }

    public string soliderName;

    /// <summary>
    /// 血量
    /// </summary>
    public int blood;

    /// <summary>
    /// 攻击 命中概率为  攻击力/敌方防御力 * 0.5
    /// </summary>
    public int ATK;

    public int power;

    /// <summary>
    /// 护甲
    /// </summary>
    public int armor;

    /// <summary>
    /// 防御 
    /// </summary>
    public int dodge;

    /// <summary>
    /// 格挡 100
    /// </summary>
    public int parry;

    /// <summary>
    /// 冲锋
    /// </summary>
    public int charge;

    /// <summary>
    /// 士气
    /// </summary>
    public int morale;

    /// <summary>
    /// 攻击距离
    /// </summary>
    public float ATKRange;

    /// <summary>
    /// 射程
    /// </summary>
    public float l_ATKRange;

    /// <summary>
    /// 远程威力
    /// </summary>
    public int l_power;

    /// <summary>
    /// 精准
    /// </summary>
    public float accurate;

    /// <summary>
    /// 攻速
    /// </summary>
    public float ATKSpeed;

    /// <summary>
    /// 射速
    /// </summary>
    public float shootingSpeed;
    
    /// <summary>
    /// 弹药
    /// </summary>
    public int ammo;

    /// <summary>
    /// 魔力
    /// </summary>
    public int magic;

    /// <summary>
    /// 头像
    /// </summary>
    public string head;

    /// <summary>
    /// 模板
    /// </summary>
    public string model;

}
