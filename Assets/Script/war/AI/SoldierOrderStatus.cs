using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierOrderStatus
{
    public enum TYPE
    {
        SoldierOrderStatus,
        SoldierMoveOrderStatus,
        SoldierAvoidOrderStatus,
        SoldierChaseOrderStatus,
        SoldierAttackOrderStatus,
        SoldierRotaOrderStatus,
        SoldierRotaObjOrderStatus
    }

    public TYPE _type = TYPE.SoldierOrderStatus;
    protected bool active = false;
    public bool IsActive()
    {
        return active;
    }

    public virtual BASE_MOVE_STATUS GetMoveStatus(out Vector2 targetPos)
    {
        targetPos = Vector2.zero;
        return BASE_MOVE_STATUS.STOP;
    }

    // 移动
    public virtual Vector2 Calculate(SoldierControl control)
    {
        if (control.velocity == 0)
        {
            return Vector2.zero;
        }

        // 希望停止移动，得到一个速度相反方向的减速度
        Vector2 dir = control.Heading();
        float add_v = control.m_dMaxForce / control.m_dMass;
        if (control.velocity < add_v)
        {
            return -dir.normalized * control.velocity / BattleConstant.deltaTime * control.m_dMass;
        }
        else
        {
            return -dir.normalized * control.m_dMaxForce;
        }
    }

    public virtual bool CheckFininal(SoldierControl control)
    {
        return false;
    }
}

// 移动
public class SoldierMoveOrderStatus : SoldierOrderStatus
{
    public Vector2 targetPos;

    public SoldierMoveOrderStatus(Vector2 targetPos)
    {
        this.targetPos = targetPos;
        _type = TYPE.SoldierMoveOrderStatus;
    }

    public override BASE_MOVE_STATUS GetMoveStatus(out Vector2 targetPos)
    {
        targetPos = this.targetPos;
        return BASE_MOVE_STATUS.RUN;
    }

    public override Vector2 Calculate(SoldierControl control)
    {
        Vector2 force = control.m_pSteering.Arrive(targetPos, Deceleration.FAST);
        return force;
    }

    public override bool CheckFininal(SoldierControl control)
    {
        if (
            (Vector3Tool.ToVector2(control.transform.localPosition) - targetPos).magnitude < 0.05f)
        {
            
            //transform.localPosition = Vector2Tool.ToVector3(targetPos);
            return true;
        }
        return false;
    }

    public override string ToString()
    {
        return string.Format("class {0} target {1}", "SoldierMoveOrderStatus", targetPos.ToString());
    }
}

// 避让
public class SoldierAvoidOrderStatus : SoldierOrderStatus
{
    public int avoidId;
    public Vector2 targetPos;

    public SoldierAvoidOrderStatus(int avoidId, Vector2 targetPos)
    {
        this.avoidId = avoidId;
        this.targetPos = targetPos;

        _type = TYPE.SoldierAvoidOrderStatus;
    }

    public override BASE_MOVE_STATUS GetMoveStatus(out Vector2 targetPos)
    {
        targetPos = this.targetPos;
        return BASE_MOVE_STATUS.RUN;
    }

    public override Vector2 Calculate(SoldierControl control)
    {
        return control.m_pSteering.Calculate();
    }

    public override bool CheckFininal(SoldierControl control)
    {
        Debug.Log((Vector3Tool.ToVector2(control.transform.localPosition)));
        Debug.Log(targetPos);

        Debug.Log((Vector3Tool.ToVector2(control.transform.localPosition) - targetPos).magnitude);
        Debug.Log(targetPos);
        if ((Vector3Tool.ToVector2(control.transform.localPosition) - targetPos).magnitude < 0.05f)
        {
            return true;
        }
        return false;
    }

    public override string ToString()
    {
        return string.Format("class {0} avoid{2} target {1}", "SoldierAvoidOrderStatus", targetPos.ToString(), avoidId);
    }
}

// 追逐
public class SoldierChaseOrderStatus : SoldierOrderStatus
{
    public int chaseId;
    public SoldierControl target;

    public SoldierChaseOrderStatus(int chaseId)
    {
        this.chaseId = chaseId;

        target = BattleWorld.gameControl.GetSoldierControlById(this.chaseId);

        _type = TYPE.SoldierChaseOrderStatus;
    }

    public override BASE_MOVE_STATUS GetMoveStatus(out Vector2 targetPos)
    {
        targetPos = target.GetPos();
        return BASE_MOVE_STATUS.RUN;
    }

    public override Vector2 Calculate(SoldierControl control)
    {
        Vector2 force = Vector2.zero;
        // 不是停止状态
        force += control.m_pSteering.Pursuit(target);
        return force;
    }

    public override bool CheckFininal(SoldierControl control)
    {
        // 对方死亡
        if (!target.isLife())
        {
            return true;
        }

        // 判断是否在攻击范围内，不在范围内的话则为追逐
        bool isCanAttack = false;
        CollisionPH one_ph = control.collisionObj.GetCollisionPH();
        CollisionPH other_ph = target.collisionObj.GetCollisionPH();

        // 战斗方式为射击，则判断是否在射击范围内
        if (control.data.AttackType == BattleSoldierData.ATTACK_TYPE.SHOOT)
        {
            // -2f 是为了让对方完全进入射程内
            CollisionCylinder new_one_ph = new CollisionCylinder(control.data.l_ATKRange - 2f, control.data.GetConfig().ph_y);
            new_one_ph.pos = one_ph.pos;
            new_one_ph.coordinate = one_ph.coordinate;
            one_ph = new_one_ph;
        }

        float dis;
        if (CollisionManager.IsBriefnessCollision(one_ph, other_ph, out dis))
        {
            Vector3 pos;
            if (CollisionManager.IsCollision(one_ph, other_ph, out pos))
            {
                isCanAttack = true;
            }
        }

        // 如果没碰触 则添加追逐
        if (!isCanAttack)
        {
            return false;
        }

        return true;
    }

    public override string ToString()
    {
        return string.Format("class {0} chase{1}", "SoldierChaseOrderStatus", chaseId);
    }
}

// 攻击
public class SoldierAttackOrderStatus : SoldierOrderStatus
{
    public int attackId;
    private SoldierControl target;

    public SoldierAttackOrderStatus(int attackId)
    {
        this.attackId = attackId;

        target = BattleWorld.gameControl.GetSoldierControlById(this.attackId);

        _type = TYPE.SoldierAttackOrderStatus;
    }

    public override BASE_MOVE_STATUS GetMoveStatus(out Vector2 targetPos)
    {
        targetPos = Vector2.zero;
        return BASE_MOVE_STATUS.STOP;
    }

    public override bool CheckFininal(SoldierControl control)
    {
        // 对方死亡
        if (!target.isLife())
        {
            return true;
        }

        // 判断是否在攻击范围内，不在范围内的话则为追逐
        bool isCanAttack = false;
        CollisionPH one_ph = control.collisionObj.GetCollisionPH();
        CollisionPH other_ph = target.collisionObj.GetCollisionPH();

        // 战斗方式为射击，则判断是否在射击范围内
        if(control.data.AttackType == BattleSoldierData.ATTACK_TYPE.SHOOT)
        {
            CollisionCylinder new_one_ph = new CollisionCylinder(control.data.l_ATKRange, control.data.GetConfig().ph_y);
            new_one_ph.pos = one_ph.pos;
            new_one_ph.coordinate = one_ph.coordinate;
            one_ph = new_one_ph;
        }

        float dis;
        if(CollisionManager.IsBriefnessCollision(one_ph, other_ph, out dis))
        {
            Vector3 pos;
            if(CollisionManager.IsCollision(one_ph, other_ph, out pos))
            {
                isCanAttack = true;
            }
        }

        // 如果没碰触 则添加追逐
        if (!isCanAttack)
        {
            control.ChaseTo(attackId);
        }

        return false;
    }

    public override string ToString()
    {
        return string.Format("class {0} attack{1}", "SoldierAttackOrderStatus", attackId);
    }
}

// 旋转
public class SoldierRotaOrderStatus : SoldierOrderStatus
{
    private Vector2 heading;

    public SoldierRotaOrderStatus(Vector2 heading)
    {
        this.heading = heading;
        _type = TYPE.SoldierRotaOrderStatus;
    }

    public override BASE_MOVE_STATUS GetMoveStatus(out Vector2 targetPos)
    {
        targetPos = Vector2.zero;
        return BASE_MOVE_STATUS.RUN;
    }

    public override Vector2 Calculate(SoldierControl control)
    {
        Vector2 force = heading.normalized;
        return force * 0.1f;
    }

    public override bool CheckFininal(SoldierControl control)
    {
        if ((control.Heading().normalized - heading.normalized).magnitude < 0.0001f)
        {
            return true;
        }
        return false;
    }

    public override string ToString()
    {
        return string.Format("class {0} rota{1}", "SoldierRotaOrderStatus", heading);
    }
}

// 旋转
public class SoldierRotaObjOrderStatus : SoldierOrderStatus
{
    private int id;
    private SoldierControl obj;

    public SoldierRotaObjOrderStatus(int id)
    {
        this.id = id;

        obj = BattleWorld.gameControl.GetSoldierControlById(this.id);
        _type = TYPE.SoldierRotaObjOrderStatus;
    }

    public override BASE_MOVE_STATUS GetMoveStatus(out Vector2 targetPos)
    {
        targetPos = Vector2.zero;
        return BASE_MOVE_STATUS.RUN;
    }

    public override Vector2 Calculate(SoldierControl control)
    {
        Vector2 force = this.obj.GetPos() - control.GetPos();
        return force.normalized * 0.1f;
    }

    public override bool CheckFininal(SoldierControl control)
    {
        Vector2 heading = this.obj.GetPos() - control.GetPos();

        if ((control.Heading().normalized - heading.normalized).magnitude < 0.0001f)
        {
            return true;
        }
        return false;
    }

    public override string ToString()
    {
        return string.Format("class {0} rota{1}", "SoldierRotaOrderStatus", obj.GetId());
    }
}