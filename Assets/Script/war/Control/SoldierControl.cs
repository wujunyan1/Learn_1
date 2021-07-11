using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BASE_BATTLE_STATUS
{
    NORMAL = 1,         //正常
    ESCAPE = 2,         //逃跑
}

public enum BASE_MOVE_STATUS
{
    STOP = 0,         //停止
    WALK = 1,         //走
    RUN = 2,         //跑
}

public class SoldierControl : MovingEntity
{
    public BattleSoldierData data;

    // 移动组件
    public SteeringBehaviors m_pSteering;

    // 碰撞组件
    public CollisionObj collisionObj;

    // 当前状态
    public BASE_BATTLE_STATUS BBstatus;

    public BASE_MOVE_STATUS BMstatus;

    public List<SoldierOrderStatus> orderList;

    private AIControl ai;

    public AIControl aiControl
    {
        get
        {
            return ai;
        }
    }

    private bool life = true;

    private GameObject select_image;

    public SoldierControl()
    {
        orderList = new List<SoldierOrderStatus>();

        ai = new AIControl(this);

    }

    private void Start()
    {
        select_image = this.FindChild("mod/Canvas").gameObject;
        select_image.SetActive(false);
        
    }

    public void SetData(BattleSoldierData _data)
    {
        data = _data;
        this.m_vVelocity = Vector2.zero;

        float angle = this.transform.rotation.eulerAngles.y;

        SetHeading(new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)));
        m_vSide = Vector2Tool.Perp(Heading());

        m_dMass = data.GetConfig().m_dMass;

        m_dMaxSpeed = data.GetConfig().m_dMaxSpeed;

        m_dMaxForce = data.GetConfig().m_dMaxForce;

        m_dMaxTurnRate = data.GetConfig().m_dMaxTurnRate;

        bradius = 1;

        BBstatus = BASE_BATTLE_STATUS.NORMAL;
        BMstatus = BASE_MOVE_STATUS.STOP;
    }

    public void CheckLife()
    {
        life = data.blood > 0;
    }

    // 是否死亡
    public bool isLife()
    {
        return life;
    }

    public int GetId()
    {
        return data.GetConfig().id;
    }

    public void ShowSelect()
    {
        select_image.SetActive(true);
    }

    public void HideSelect()
    {
        select_image.SetActive(false);
    }

    public bool IsSelect()
    {
        return select_image.activeSelf;
    }

    public void Stop()
    {

    }

    public SoldierOrderStatus GetCurrOrder()
    {
        if(orderList.Count == 0)
        {
            return new SoldierOrderStatus();
        }

        return orderList[0];
    }

    public void UpdateBMstatus()
    {
        SoldierOrderStatus order = GetCurrOrder();

        Vector2 targetPos;
        BMstatus = order.GetMoveStatus(out targetPos);
        
        if(BMstatus == BASE_MOVE_STATUS.STOP)
        {
            m_vVelocity = Vector2.zero;
        }
    }

    // 避让
    public void AvoidTo(int targetId, Vector2 target)
    {
        SoldierAvoidOrderStatus order = new SoldierAvoidOrderStatus(targetId, target);

        orderList.Add(order);

        UpdateBMstatus();
    }

    // 攻击
    public void AttackTo(int targetId)
    {
        //SoldierAttackOrderStatus order = new SoldierAttackOrderStatus(targetId);
        //orderList.AddFirst(new LinkedListNode<SoldierOrderStatus>(order));
        //UpdateBMstatus();

        Debug.Log("------22222222---------");
        ai.AddEvent("AttackTo", targetId);
    }

    // 追逐
    public SoldierChaseOrderStatus ChaseTo(int targetId)
    {
        SoldierChaseOrderStatus order = new SoldierChaseOrderStatus(targetId);
        orderList.Add(order);
        UpdateBMstatus();

        return order;
    }

    public SoldierRotaOrderStatus RotaTo(Vector2 heading)
    {
        SoldierRotaOrderStatus order = new SoldierRotaOrderStatus(heading);
        orderList.Add(order);
        UpdateBMstatus();

        return order;
    }

    public SoldierRotaObjOrderStatus RotaTo(int id)
    {
        SoldierRotaObjOrderStatus order = new SoldierRotaObjOrderStatus(id);
        orderList.Add(order);
        UpdateBMstatus();

        return order;
    }

    public void RemoveOrder(SoldierOrderStatus order)
    {
        orderList.Remove(order);
        UpdateBMstatus();
    }

    public Vector2 GetTarget()
    {
        SoldierOrderStatus order = GetCurrOrder();

        Vector2 targetPos;
        order.GetMoveStatus(out targetPos);
        
        return targetPos;
    }

    public void MoveTo(Vector2 target)
    {
        SoldierMoveOrderStatus order = new SoldierMoveOrderStatus(target);
        orderList.Add(order);
        UpdateBMstatus();
    }

    public void ClearOrderList()
    {
        orderList.Clear();
    }

    public void ArriveTarget()
    {
        orderList.RemoveAt(0);
        UpdateBMstatus();
    }

    // 使力不会超过最大值
    private Vector2 GetForce(Vector2 force)
    {
        if(force.magnitude <= 0.001f)
        {
            return Vector2.zero;
        }

        if(force.magnitude > this.m_dMaxForce)
        {
            return force.normalized * this.m_dMaxForce;
        }

        return force;
    }

    private Vector2 MovingUpdate()
    {
        Vector2 force = m_pSteering.TryWallAvoidance();
        

        return force;
    }

    public Vector2 GetCalculate()
    {
        SoldierOrderStatus status = GetCurrOrder();
        return status.Calculate(this);
    }

    // 移动 上一帧到这一帧的移动
    public override void MoveUpdate()
    {

        //更新朝向
        //if (m_vVelocity.magnitude > 0.0001 && BMstatus != BASE_MOVE_STATUS.STOP)
        //{
        //    this.SetHeading(m_vVelocity.normalized);
        //    m_vSide = Vector2Tool.Perp(Heading());
        //}

        //if ((Vector2Tool.ToVector3(GetTarget()) - this.transform.localPosition).magnitude
        //    <= movePos.magnitude)
        //{
        //    Debug.Log("-------------------");
        //    this.transform.localPosition = Vector2Tool.ToVector3(GetTarget());
        //}
        //else
        //{
        //    this.transform.localPosition += movePos;
        //}


        // 计算合力
        Vector2 force = this.collisionObj.nextAddForce;
        this.collisionObj.nextAddForce = Vector2.zero;

        // 命令的添加力
        force += GetCalculate(); // m_pSteering.Calculate();
        // 移动时
        if (m_vVelocity.magnitude > 0)
        {
            force += MovingUpdate();
        }

        // 力方向
        force = GetForce(force);

        
        float addV = 0f;

        if (force.magnitude == 0)
        {
            
        }
        else
        {
            Vector3 v3_force = Vector2Tool.ToVector3(force);
            Vector3 forward_dir = this.transform.TransformDirection(Vector3.forward);

            float cos = Vector3.Dot(forward_dir.normalized, v3_force.normalized);
            float forceNum = cos * v3_force.magnitude;

            float forceDirV = cos * velocity;
            

            // 角度
            float angle = Mathf.Acos(cos) * Mathf.Rad2Deg;

            if (float.IsNaN(angle))
            {
                angle = 0;
            }

            bool isOK = true;
            if (angle > data.GetConfig().m_dMaxTurnRateDeltaTime)
            {
                isOK = false;
                angle = data.GetConfig().m_dMaxTurnRateDeltaTime;
            }

            // 右手法则
            // y 小于0 则在左边   180 -- 360
            // 叉积
            // 小于0 则 后面的是前面的逆时针方向
            // 大于0 则 后面的是前面的顺时针方向
            Vector3 c = Vector3.Cross(forward_dir.normalized, v3_force.normalized);
            if (c.y < 0)
            {
                angle = 360 - angle;
            }

            // 转向角度
            this.Rotation(angle);

            Vector3 off = v3_force / m_dMass * BattleConstant.deltaTime;

            Vector3 n_v = forward_dir.normalized * velocity + off;

            // 速度变化
            if (isOK)
            {
                addV = v3_force.magnitude / m_dMass * BattleConstant.deltaTime;
            }
            else
            {
                if (forceNum < 0)
                {
                    addV = forceNum / m_dMass * BattleConstant.deltaTime;
                }
                else
                {
                    float will_v = velocity * Mathf.Cos(angle * Mathf.Deg2Rad);
                    float max_add_v = m_dMaxForce / m_dMass * BattleConstant.deltaTime;

                    addV = will_v - velocity;
                    if( Mathf.Abs(addV) > max_add_v)
                    {
                        addV = -max_add_v;
                    }
                    
                }
            }


            // 速度变化
            //velocity = n_v.magnitude;
            velocity += addV;

            velocity = velocity < 0 ? 0 : velocity;

            // 保证最大速度
            float maxSpeed = m_dMaxSpeed * BattleConstant.moveSpeedCorrection;
            velocity = velocity > maxSpeed ? maxSpeed : velocity;
        }


        Vector3 forward = this.transform.TransformDirection(Vector3.forward);
        //更新位置
        Vector3 movePos = forward.normalized * velocity * BattleConstant.deltaTime;
        //this.transform.localPosition += movePos;

        //Vector3 movePos = Vector2Tool.ToVector3(m_vVelocity) * BattleConstant.deltaTime;
        if ((Vector2Tool.ToVector3(GetTarget()) - this.transform.localPosition).magnitude
            <= movePos.magnitude)
        {
            this.transform.localPosition = Vector2Tool.ToVector3(GetTarget());
        }
        else
        {
            this.transform.localPosition += movePos;
        }

        // 被迫 位移
        this.transform.localPosition += Vector2Tool.ToVector3(collisionObj.nextAddPos);
        this.transform.localPosition = BattleWorld.battleMap.GetPos(this.transform.localPosition);

        collisionObj.nextAddPos = Vector2.zero;

        ////  加速度
        //Vector2 acceleration = force / m_dMass;

        ////Debug.Log(acceleration);

        //// 速度变化
        //Vector2 addV = acceleration * BattleConstant.deltaTime;

        ////Debug.Log(addV);
        //addV += collisionObj.nextAddVelocity;
        //collisionObj.nextAddVelocity = Vector2.zero;

        //m_vVelocity += addV;

        //// 停止状态 尝试停止
        //if (BMstatus == BASE_MOVE_STATUS.STOP)
        //{
        //    // 希望停止移动，得到一个速度相反方向的减速度
        //    float add_v = this.m_dMaxForce / m_dMass;
        //    if (m_vVelocity.magnitude < add_v)
        //    {
        //        addV = -m_vVelocity;
        //    }
        //    else
        //    {
        //        addV += (-m_vVelocity.normalized * add_v);
        //    }
        //}



        // 更新位置
        //Vector3 movePos = Vector2Tool.ToVector3(m_vVelocity) * BattleConstant.deltaTime;

        //if ((Vector2Tool.ToVector3(GetTarget()) - this.transform.localPosition).magnitude
        //    <= movePos.magnitude)
        //{
        //    Debug.Log("-------------------");
        //    this.transform.localPosition = Vector2Tool.ToVector3(GetTarget());
        //}
        //else
        //{
        //    this.transform.localPosition += movePos;
        //}

        // this.transform.localPosition += Vector2Tool.ToVector3(collisionObj.nextAddPos);
        // collisionObj.nextAddPos = Vector2.zero;



    }

    public override void LogicUpdate()
    {
        SoldierOrderStatus status = GetCurrOrder();
        // Debug.Log(status.ToString());

        if (status.CheckFininal(this))
        {
            ArriveTarget();
        }

        ai.LogicAI();
    }
}
