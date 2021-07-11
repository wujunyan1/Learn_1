using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 近战攻击
/// </summary>
public class AttackAI : BaseAi
{
    /// <summary>
    /// 是否主动的
    /// </summary>
    public bool isInitiative = false;

    public int enemyId;
    public SoldierControl enemy;

    private int wait;

    public SoldierChaseOrderStatus orderStatus;
    

    public AttackAI()
    {
        enemyId = -1;
        enemy = null;
        orderStatus = null;

        listener = new Dictionary<string, UEventListener.EventListenerDelegate>();

        listener.Add("AttackTo", AttackTo);
    }

    // 攻击
    public void AttackTo(UEvent e)
    {
        int enemyId = (int)e.eventParams;
        AttackTo(enemyId, true);
    }

    public void AttackTo(int enemyId, bool isInitiative = false)
    {
        Debug.Log("AttackTo " + enemyId + isInitiative);

        this.isInitiative = isInitiative;
        this.enemyId = enemyId;
        enemy = BattleWorld.gameControl.GetSoldierControlById(this.enemyId);
    }

    public void ReSetAttackObj()
    {
        enemyId = -1;
        enemy = null;

        // 有追逐 则删除追逐
        if (orderStatus != null)
        {
            aiControl.GetGameObj().RemoveOrder(orderStatus);
            orderStatus = null;
        }
    }

    // 进行攻击
    private void Attack()
    {
        SoldierControl control = aiControl.GetGameObj();

        // cd 重置
        ReSetCD();


    }

    // cd 重置
    private void ReSetCD()
    {
        SoldierControl control = aiControl.GetGameObj();

        // cd 重置
        wait = control.data.ATKSpeed;
    }

    public bool IsCanAttack()
    {
        if (wait >= 0)
        {
            return false;
        }

        SoldierControl control = aiControl.GetGameObj();

        if (control.BMstatus != BASE_MOVE_STATUS.STOP)
        {
            return false;
        }

        return true;
    }

    protected override void StartAI()
    {
        // 下一帧就可以攻击
        wait = 0;
    }
    
    private void TryAttack()
    {
        SoldierControl control = aiControl.GetGameObj();
        // 对方死亡
        if (!enemy.isLife())
        {
            return;
        }

        // 判断是否在攻击范围内，不在范围内的话则为追逐
        bool isInRange = false;

        // 攻击范围
        float one_attack_range = control.data.ATKRange;

        // 攻击范围碰撞体
        CollisionCylinder one_ph = new CollisionCylinder(one_attack_range, control.data.GetConfig().ph_y);
        one_ph.pos = control.transform.localPosition;
        one_ph.coordinate = control.GetCoordinate2();

        // 敌人的体型
        CollisionPH other_ph = enemy.collisionObj.GetCollisionPH();

        // 判断是否发生碰撞
        float dis;
        if (CollisionManager.IsBriefnessCollision(one_ph, other_ph, out dis))
        {
            Vector3 pos;
            if (CollisionManager.IsCollision(one_ph, other_ph, out pos))
            {
                isInRange = true;
            }
        }

        // 如果不在范围内 且是主动攻击  则添加追逐
        if (!isInRange && isInitiative && orderStatus == null)
        {
            orderStatus = control.ChaseTo(enemyId);
        }

        // 如果不在范围内 且不是主动攻击
        if (!isInRange && !isInitiative)
        {
            // 停止进攻
            ReSetAttackObj();
            return;
        }

        // 在攻击范围内 有追逐 则删除追逐
        if (isInRange && orderStatus != null)
        {
            control.RemoveOrder(orderStatus);
            orderStatus = null;
        }

        //  现在是停止移动
        if (isInRange && control.BMstatus == BASE_MOVE_STATUS.STOP)
        {
            Attack();
        }
    }

    private void FindCanAttackObj()
    {
        SoldierControl control = aiControl.GetGameObj();
        List<CollisionObj> collisions = control.collisionObj.collisions;

        // 攻击范围
        float one_attack_range = control.data.ATKRange;

        // 攻击范围碰撞体
        CollisionCylinder one_ph = new CollisionCylinder(one_attack_range, control.data.GetConfig().ph_y);
        one_ph.pos = control.transform.localPosition;
        one_ph.coordinate = control.GetCoordinate2();

        float dirLength = 10f;
        CollisionObj obj = null;
        bool isCanAttack = false;

        foreach (var item in collisions)
        {
            // 敌人的体型
            CollisionPH other_ph = item.GetCollisionPH();
            bool isCan = false;

            // 判断是否发生碰撞
            float dis;
            if (CollisionManager.IsBriefnessCollision(one_ph, other_ph, out dis))
            {
                Vector3 pos;
                if (CollisionManager.IsCollision(one_ph, other_ph, out pos))
                {
                    isCan = true;
                }
            }

            // 最近的敌人
            float l = (one_ph.pos - other_ph.pos).magnitude;
            if (l < dirLength)
            {
                dirLength = l;
                obj = item;
                isCanAttack = isCan;
            }
        }

        if(obj != null)
        {
            // 可以攻击则攻它
            if (isCanAttack)
            {
                AttackTo(obj.GetId(), false);
            }

            // 面向它， 面向攻击者
            // control.RotaTo(obj.GetId());
        }
    }

    public override void LogicAI()
    {
        wait--;

        SoldierControl control = aiControl.GetGameObj();

        // 没有攻击 对象
        if (enemy == null)
        {
            // 当前状态是静止
            if (control.BMstatus == BASE_MOVE_STATUS.STOP)
            {
                FindCanAttackObj();
            }
            return;
        }
        
        TryAttack();
    }
}
