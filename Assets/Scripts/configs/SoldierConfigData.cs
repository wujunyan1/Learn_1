using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierConfigData
{
    private static int _id = 0;
    public int id;

    public enum SOLDIER_TYPE
    {
        SWORD_SHIELD_CAVALRY,   // 剑 盾 骑兵
        SWORD_CAVALRY,           // 剑  骑兵
        SWORD_SOWRD_INFANTRY,   //双持剑士
        ARCHER,                 // 弓手
        SOWRD_INFANTRY,         // 持剑步兵
    }

    public static string SOLDIER_TYPE_NAME(SOLDIER_TYPE _type)
    {
        switch (_type) {
            case SOLDIER_TYPE.SWORD_SHIELD_CAVALRY:
                return "剑盾骑兵";
            case SOLDIER_TYPE.SWORD_CAVALRY:
                return "持剑骑兵";
            case SOLDIER_TYPE.SWORD_SOWRD_INFANTRY:
                return "双持剑士";
            case SOLDIER_TYPE.ARCHER:
                return "弓手";
            case SOLDIER_TYPE.SOWRD_INFANTRY:
                return "持剑步兵";
        }

        return "未知兵种";
    }

    public string key;
    public string KEY
    {
        get
        {
            return key;
        }
    }

    public string solider_name;

    /// <summary>
    /// 兵种类型，以后会影响装备类型限制
    /// </summary>
    public int solider_type;

    public SOLDIER_TYPE _TYPE
    {
        get
        {
            return (SOLDIER_TYPE)solider_type;
        }
    }

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
    public int l_ATKRange;

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

    /// <summary>
    /// 人物 人物体型类型 0 圆柱 1 立方体
    /// </summary>
    public int ph_type;

    /// <summary>
    /// 人物 长 Z
    /// </summary>
    public float ph_z;

    /// <summary>
    /// 人物 宽 x
    /// </summary>
    public float ph_x;

    /// <summary>
    /// 人物 人物 高 y
    /// </summary>
    public float ph_y;

    /// <summary>
    /// 人物 半径
    /// </summary>
    public float bradius;

    /// <summary>
    /// 重量
    /// </summary>
    public int m_dMass;

    /// <summary>
    /// 最大速度
    /// </summary>
    public int m_dMaxSpeed;

    /// <summary>
    /// 最大力
    /// </summary>
    public int m_dMaxForce;

    /// <summary>
    /// 最大转向
    /// </summary>
    public int m_dMaxTurnRate;

    public float m_dMaxTurnRateDeltaTime;

    public SoldierConfigData(bool isSaveData)
    {
        if (isSaveData)
        {
            id = _id++;
        }
        else
        {
            id = _id;
        }
    }
    

    public SoldierConfigData Clone(bool isNew = false)
    {
        SoldierConfigData clone = new SoldierConfigData(isNew);
        if (!isNew)
        {
            clone.id = id;
        }
        clone.key = key;
        clone.solider_name = solider_name;
        clone.solider_type = solider_type;
        clone.blood = blood;

        clone.ATK = ATK;

        clone.power = power;

        clone.armor = armor;

        clone.dodge = dodge;

        clone.parry = parry;

        clone.charge = charge;

        clone.morale = morale;

        clone.ATKRange = ATKRange;

        clone.l_ATKRange = l_ATKRange;

        clone.l_power = l_power;

        clone.accurate = accurate;

        clone.ATKSpeed = ATKSpeed;

        clone.shootingSpeed = shootingSpeed;

        clone.ammo = ammo;

        clone.magic = magic;

        clone.head = head;

        clone.model = model;

        clone.ph_type = ph_type;

        clone.ph_x = ph_x;

        clone.ph_y = ph_y;

        clone.ph_z = ph_z;

        clone.bradius = bradius;

        clone.m_dMass = m_dMass;

        clone.m_dMaxSpeed = m_dMaxSpeed;

        clone.m_dMaxForce = m_dMaxForce;

        clone.m_dMaxTurnRate = m_dMaxTurnRate;

        clone.m_dMaxTurnRateDeltaTime = m_dMaxTurnRate * BattleConstant.deltaTime;

        return clone;
    }

    public override string ToString()
    {
        string str = "";
        str += "key = " + key;
        str += "; solider_name = "+solider_name;
        str += "; blood = " + blood;

        str += "; ATK = " + ATK;

        str += "; power = " + power;

        str += "; armor = " + armor;

        str += "; dodge = " + dodge;

        str += "; parry = " + parry;

        str += "; charge = " + charge;

        str += "; morale = " + morale;

        str += "; ATKRange = " + ATKRange;

        str += "; l_ATKRange = " + l_ATKRange;

        str += "; l_power = " + l_power;

        str += "; accurate = " + accurate;

        str += "; ATKSpeed = " + ATKSpeed;

        str += "; shootingSpeed = " + shootingSpeed;

        str += "; ammo = " + ammo;

        str += "; magic = " + magic;

        str += "; head = " + head;

        str += "; model = " + model;

        str += "; ph_type = " + ph_type;

        str += "; ph_x = " + ph_x;

        str += "; ph_y = " + ph_y;

        str += "; ph_z = " + ph_z;

        str += "; bradius = " + bradius;

        str += "; m_dMass = " + m_dMass;

        str += "; m_dMaxSpeed = " + m_dMaxSpeed;

        str += "; m_dMaxForce = " + m_dMaxForce;

        str += "; m_dMaxTurnRate = " + m_dMaxTurnRate;

        return str;
    }
}
