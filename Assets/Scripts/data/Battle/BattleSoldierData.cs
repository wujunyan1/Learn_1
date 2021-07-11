using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗时的 一个兵的数据
/// </summary>
public class BattleSoldierData
{
    public enum ATTACK_TYPE
    {
        COMBAT,
        SHOOT,
    }

    /// <summary>
    /// 基础数据
    /// </summary>
    protected SoldierConfigData configData;

    public string KEY
    {
        get
        {
            return configData.KEY;
        }
    }

    public string solider_name
    {
        get
        {
            return configData.solider_name;
        }
    }

    /// <summary>
    /// 兵种类型，以后会影响装备类型限制
    /// </summary>
    public SoldierConfigData.SOLDIER_TYPE solider_type
    {
        get
        {
            return configData._TYPE;
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
    public int ATKSpeed;

    /// <summary>
    /// 射速
    /// </summary>
    public int shootingSpeed;

    /// <summary>
    /// 弹药
    /// </summary>
    public int ammo;

    /// <summary>
    /// 魔力
    /// </summary>
    public int magic;

    public int move_speed;

    public int camp;
    
    public ATTACK_TYPE AttackType
    {
        get
        {
            if (this.ammo > 0)
            {
                return ATTACK_TYPE.SHOOT;
            }
            return ATTACK_TYPE.COMBAT;
        }
    }

    public void SetConfigData(SoldierConfigData configData)
    {
        this.configData = configData;

        this.blood = configData.blood;

        this.ATK = configData.ATK;

        this.power = configData.power;

        this.armor = configData.armor;

        this.dodge = configData.dodge;

        this.parry = configData.parry;

        this.charge = configData.charge;

        this.morale = configData.morale;

        this.ATKRange = configData.ATKRange;

        this.l_ATKRange = configData.l_ATKRange;

        this.l_power = configData.l_power;

        this.accurate = configData.accurate;

        this.ATKSpeed = Mathf.FloorToInt(configData.ATKSpeed / BattleConstant.deltaTime);

        this.shootingSpeed = Mathf.FloorToInt(configData.shootingSpeed / BattleConstant.deltaTime);

        this.ammo = configData.ammo;

        this.magic = configData.magic;

        this.move_speed = configData.m_dMaxSpeed;
    }

    public SoldierConfigData GetConfig()
    {
        return configData;
    }

    ///////////////////////////////////////////////////////////////////
    ///

    bool moveShoot = false;
    public bool isCanMoveShoot()
    {
        return moveShoot;
    }

    public void SetCanMoveShoot( bool moveShoot )
    {
        this.moveShoot = moveShoot;
    }
}
