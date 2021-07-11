using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Soldier : MovingEntity
{
    protected string soliderKey;

    public string soliderName;

    /// <summary>
    /// 血量
    /// </summary>
    public int blood;

    /// <summary>
    /// 攻击力
    /// </summary>
    public int ATK;

    /// <summary>
    /// 护甲
    /// </summary>
    public int armor;

    /// <summary>
    /// 闪避 100
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

    public float l_ATKRange;

    public int l_ATK;

    public float accurate;

    public float ATKSpeed;

    public float shootingSpeed;

    public virtual void SetSoliderData(SoldierData data)
    {
        this.soliderKey = data.key;
        this.soliderName = data.solider_name;
        this.bradius = data.bradius;
        this.m_dMass = data.m_dMass;
        this.m_dMaxSpeed = data.m_dMaxSpeed;
        this.m_dMaxForce = data.m_dMaxForce;
        this.m_dMaxTurnRate = data.m_dMaxTurnRate;
        this.blood = data.blood;
        this.ATK = data.ATK;
        this.armor = data.armor;
        this.dodge = data.dodge;
        this.parry = data.parry;
        this.charge = data.charge;
        this.morale = data.morale;
        this.ATKRange = data.ATKRange;
        this.l_ATKRange = data.l_ATKRange;
        this.l_ATK = data.l_ATK;
        this.accurate = data.accurate;
        this.ATKSpeed = data.ATKSpeed;
        this.shootingSpeed = data.shootingSpeed;
    }

    /***
     * 
     * 
     * 
     * 
     * 
     * 
     */

    public SteeringBehaviors m_pSteering;
    public GameWorldOLD world;

    // 自己在队伍中的位置
    protected int index;

    // 自己的队伍
    public TeamControl control;
    // 队伍中的位置
    public Vector2Int centerOffset;

    protected Soldier leader;

    // 攻击的敌方队伍
    public TeamControl attackTarget;

    // 要到达的位置
    public Vector2 targetPos;

    public BASE_BATTLE_STATUS BBstatus = BASE_BATTLE_STATUS.NORMAL;

    public void SetIndex(int index)
    {
        this.index = index;
    }

    public int GetIndex()
    {
        return this.index;
    }

    public void SetLeader(Soldier soldier)
    {
        this.leader = soldier;
    }

    public Soldier GetLeader()
    {
        return this.leader;
    }

    public override void LogicUpdate()
    {
        // 计算合力
        Vector2 force = m_pSteering.Calculate();

        //  加速度
        Vector2 acceleration = force / m_dMass;

        // 速度变化
        m_vVelocity += acceleration * BattleConstant.deltaTime;

        // 保证最大速度
        m_vVelocity = Vector2Tool.Truncate(m_vVelocity, m_dMaxSpeed);

        // 更新位置
        //this.transform.localPosition += Vector2Tool.ToVector3(m_vVelocity) * Time.fixedDeltaTime;

        // 更新朝向
        if(m_vVelocity.magnitude > 0.0001)
        {
            this.SetHeading(m_vVelocity.normalized);
            m_vSide = Vector2Tool.Perp(Heading());
        }
    }

    public void MoveTo(Vector3 target)
    {
        Vector2 toVector2 = Vector3Tool.ToVector2(target);
    }
}
