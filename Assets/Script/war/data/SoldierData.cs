using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[SerializeField]
public class SoldierData : MonoBehaviour
{
    [SerializeField]
    public string key;

    // 
    [SerializeField]
    public string solider_name;

    /// <summary>
    /// 半径
    /// </summary>
    [SerializeField]
    public float bradius;

    // 质量
    [SerializeField]
    public int m_dMass;

    /// <summary>
    /// 速度
    /// </summary>
    [SerializeField]
    public float m_dMaxSpeed;

    /// <summary>
    /// 最大力
    /// </summary>
    [SerializeField]
    public float m_dMaxForce;

    /// <summary>
    /// 转向速度
    /// </summary>
    [SerializeField]
    public float m_dMaxTurnRate;

    /// <summary>
    /// 血量
    /// </summary>
    [SerializeField]
    public int blood;

    /// <summary>
    /// 攻击力
    /// </summary>
    [SerializeField]
    public int ATK;

    /// <summary>
    /// 护甲
    /// </summary>
    [SerializeField]
    public int armor;

    /// <summary>
    /// 闪避 100
    /// </summary>
    [SerializeField]
    public int dodge;

    /// <summary>
    /// 格挡 100
    /// </summary>
    [SerializeField]
    public int parry;

    /// <summary>
    /// 冲锋
    /// </summary>
    [SerializeField]
    public int charge;

    /// <summary>
    /// 士气
    /// </summary>
    [SerializeField]
    public int morale;

    /// <summary>
    /// 攻击距离
    /// </summary>
    [SerializeField]
    public float ATKRange;

    [SerializeField]
    public float l_ATKRange;

    [SerializeField]
    public int l_ATK;

    /// <summary>
    /// 精准
    /// </summary>
    [SerializeField]
    public float accurate;

    [SerializeField]
    public float ATKSpeed;

    [SerializeField]
    public float shootingSpeed;

    public void SetData(string dataKey, string data)
    {
        switch (dataKey)
        {
            case "key":
                this.key = data;
                break;
            case "solider_name":
                this.solider_name = data;
                break;
            case "bradius":
                this.bradius = float.Parse(data);
                break;
            case "m_dMass":
                this.m_dMass = int.Parse(data);
                break;
            case "m_dMaxSpeed":
                this.m_dMaxSpeed = float.Parse(data);
                break;
            case "m_dMaxForce":
                this.m_dMaxForce = float.Parse(data);
                break;
            case "m_dMaxTurnRate":
                this.m_dMaxTurnRate = float.Parse(data);
                break;
            case "blood":
                this.blood = int.Parse(data);
                break;
            case "ATK":
                this.ATK = int.Parse(data);
                break;
            case "armor":
                this.armor = int.Parse(data);
                break;
            case "dodge":
                this.dodge = int.Parse(data);
                break;
            case "parry":
                this.parry = int.Parse(data);
                break;
            case "charge":
                this.charge = int.Parse(data);
                break;
            case "morale":
                this.morale = int.Parse(data);
                break;
            case "ATKRange":
                this.ATKRange = float.Parse(data);
                break;
            case "l_ATKRange":
                this.l_ATKRange = float.Parse(data);
                break;
            case "l_ATK":
                this.l_ATK = int.Parse(data);
                break;
            case "accurate":
                this.accurate = float.Parse(data);
                break;
            case "ATKSpeed":
                this.ATKSpeed = float.Parse(data);
                break;
            case "shootingSpeed":
                this.shootingSpeed = float.Parse(data);
                break;
        }
    }


    public override string ToString()
    {
        return base.ToString();
    }
}
