using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionObj : MonoBehaviour
{
    private CollisionPH ph;
    
    private BattleSoldierData data;


    public SteeringBehaviors m_pSteering;
    // 下一帧添加的力
    public Vector2 nextAddForce;
    // 下一帧施加的速度
    public Vector2 nextAddVelocity;
    // 下一帧施加的偏移坐标
    public Vector2 nextAddPos;

    // 待检测队列
    public List<CollisionObj> collisions;

    // 侦查半径， 距离这个以内的都加入待检测队列
    public float investigationBradius;

    //sleep 帧后全局搜索，刷新检测队列
    public int sleep;

    public CollisionObj()
    {
        collisions = new List<CollisionObj>();
        sleep = 0;

        nextAddForce = Vector2.zero;
        nextAddVelocity = Vector2.zero;
    }

    public int GetId()
    {
        return data.GetConfig().id;
    }

    public void SetBattleData(BattleSoldierData data)
    {
        this.data = data;

        SoldierConfigData config = data.GetConfig();
        CollisionType _type = (CollisionType)config.ph_type;

        switch(_type)
        {
            case CollisionType.CYLINDER:
                ph = new CollisionCylinder(config.bradius / 2, config.ph_y / 2);
                break;

            case CollisionType.CUBE:
                ph = new CollisionCube(config.ph_z / 2, config.ph_x / 2, config.ph_y / 2);
                break;
            default: /* 可选的 */
                ph = new CollisionPoint();
                break;
        }

        UpdateInvestigationBradius();
        sleep = 0;
    }

    public void UpdateInvestigationBradius()
    {
        float maxBradius = (data.move_speed + BattleConstant.maxMoveSpeed)
            * BattleConstant.moveSpeedCorrection;

        //SoldierControl control = this.GetComponent<SoldierControl>();
        //Vector2 speed = control.Velocity();
        //float n = speed.magnitude / (speed.magnitude + data.move_speed);

        investigationBradius = maxBradius;

        // 刷新评率高
        sleep = 30;
    }
    
    public void ClearCollisions()
    {
        collisions.Clear();
    }

    public void UpdateCoordinate2()
    {
        ph.pos = this.transform.localPosition;
        ph.coordinate = GetCoordinate2();
    }

    /// <summary>
    /// 获取 二维坐标系
    /// </summary>
    /// <returns></returns>
    public Coordinate2 GetCoordinate2()
    { 
        Vector3 forward = this.transform.TransformDirection(Vector3.forward);

        Vector2 start = Vector3Tool.ToVector2(this.transform.localPosition);
        Vector2 end = start + Vector3Tool.ToVector2(forward);
        return new Coordinate2(start, end);
    }

    public CollisionPH GetCollisionPH()
    {
        return ph;
    }

    /// <summary>
    /// 前进者行为
    /// </summary>
    /// <param name="other"></param>
    /// <param name="pos"></param>
    public void Advance(CollisionObj other, Vector3 pos)
    {

    }

    /// <summary>
    /// 避让者行为
    /// </summary>
    /// <param name="other"></param>
    /// <param name="pos"></param>
    public void Avoid(CollisionObj other, Vector3 pos)
    {
        //  避让
        CollisionObj avoid = this;
        Vector3 avoid_pos = avoid.transform.localPosition;
        Vector2 v2_avoid_pos = Vector3Tool.ToVector2(avoid_pos);

        // 前进
        CollisionObj advance = other;
        Vector2 advance_velocity = other.m_pSteering.vehicle.Velocity();
        Coordinate2 coordinate2 = other.GetCoordinate2();

        Vector2 local_avoid_pos = Vector2Tool.PointToLocalSpace(v2_avoid_pos, coordinate2);
        Vector2 local_collision_pos = Vector2Tool.PointToLocalSpace(pos, coordinate2);

        if (local_avoid_pos.x < 0)
        {
            return;
        }

        //Vector2 moveDir;
        Vector2 moveDir = new Vector2(0, 1);
        if (local_avoid_pos.y >= 0)
        {
            moveDir = new Vector2(0, 0.1f / local_avoid_pos.y);
        }
        else
        {
            moveDir = new Vector2(0, -0.1f / local_avoid_pos.y);
        }
        Vector2 world_speed = Vector2Tool.VectorToWorldSpace(moveDir, coordinate2.X, coordinate2.Y);

        if(world_speed.magnitude > 1)
        {
            world_speed = world_speed.normalized;
        }

        //float csin = Vector2.Dot(local_collision_pos, new Vector2(1, 0)) /
        //        local_collision_pos.magnitude;

        //float l = advance_velocity.magnitude;
        //float n = l * csin;
        //Vector2 p = local_collision_pos.normalized * n;
        //Vector2 speed = moveDir * p.y;

        //Vector2 world_speed = Vector2Tool.VectorToWorldSpace(speed, coordinate2.X, coordinate2.Y); 
        //float avoidDistance = (other.ph.briefnessBrradius + ph.briefnessBrradius + 0.5f);

        //Vector2 avoidPos = world_speed.normalized * avoidDistance + Vector3Tool.ToVector2(avoid.transform.localPosition);
        //avoid.m_pSteering.vehicle.AvoidTo(advance.data.GetConfig().id, avoidPos);

        //Vector3 dir = avoid.transform.localPosition - pos;
        Vector3 dir = avoid.transform.localPosition - advance.transform.localPosition;
        float dir_l = (avoid.GetCollisionPH().briefnessBrradius + advance.GetCollisionPH().briefnessBrradius) - dir.magnitude;
        

        avoid.nextAddPos += Vector3Tool.ToVector2(dir.normalized * dir_l);
        avoid.nextAddPos += world_speed * dir_l;

        // 前进者速度减值
        //advance.nextAddVelocity += -advance_velocity.normalized * n / 2;
        //advance.nextAddForce += -world_speed.normalized * avoidDistance;
    }

    // 发生了碰撞
    public void Collision(CollisionObj other, Vector3 pos)
    {
        Debug.Log("Collision");
        // other 移动
        Vector2 o_move_dir = other.m_pSteering.vehicle.Velocity();

        // 自己移动
        Vector2 velocity = this.m_pSteering.vehicle.Velocity();

        SoldierControl control = m_pSteering.vehicle;
        SoldierControl o_control = other.m_pSteering.vehicle;

        // 相同阵营，体重低的避让 则速度慢的避让避让对方的前进 方向
        if (data.camp == other.data.camp)
        {
            if(this.data.GetConfig().m_dMass > other.data.GetConfig().m_dMass)
            {
                other.Avoid(this, pos);
                Advance(other, pos);
                return;
            }
            else if(this.data.GetConfig().m_dMass < other.data.GetConfig().m_dMass)
            {
                Avoid(other, pos);
                other.Advance(this, pos);
                return;
            }
            
            if (o_move_dir.magnitude > velocity.magnitude)
            {
                Avoid(other, pos);
                other.Advance(this, pos);
            }
            else
            {
                other.Avoid(this, pos);
                Advance(other, pos);
            }
        }
        else
        {
        }
        
    }


}
