using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 速度
public enum Deceleration
{
    SLOW = 3,
    NORMAL = 2,
    FAST = 1,
}

public class SteeringBehaviors : MonoBehaviour
{
    public SoldierControl vehicle;

    Vector2 m_vWanderTarget;

    ParamLoader Prm;

    public SteeringBehaviors()
    {
        Prm = ParamLoader.GetInstance();
    }

    /// <summary>
    /// 靠近某目标点
    /// </summary>
    /// <param name="">目标</param>
    /// <returns></returns>
    public Vector2 Seek(Vector2 targetPos)
    {
        Vector2 des = targetPos - vehicle.GetPos();

        return des.normalized * vehicle.m_dMaxForce;
    }

    /// <summary>
    /// 离开
    /// </summary>
    /// <param name="targetPos">目标</param>
    /// <returns></returns>
    public Vector2 Flee(Vector2 targetPos)
    {
        const float panicDistance = 100.0f * 100.0f;

        if (Vector2.Distance(vehicle.GetPos(), targetPos) > panicDistance)
        {
            return Vector2.zero;
        }

        Vector2 des = (vehicle.GetPos() - targetPos) * vehicle.MaxSpeed();

        return des - vehicle.Velocity();
    }

    /// <summary>
    /// 到达目标点
    /// </summary>
    /// <param name="targetPos"></param>
    /// <param name="deceleration"></param>
    /// <returns></returns>
    public Vector2 Arrive(Vector2 targetPos, Deceleration deceleration)
    {
        Vector2 toTarget = targetPos - vehicle.GetPos();

        float dist = toTarget.magnitude;
        Vector2 velocity = vehicle.Heading() * vehicle.velocity;
        

        // 目标方向的 速度
        float currTV = vehicle.velocity * Vector2.Dot(velocity.normalized, toTarget.normalized);
        Vector2 v = velocity.normalized * currTV;
        float currV = v.magnitude;

        float maxSpeed = vehicle.MaxSpeed();

        //  加速度
        float addV = vehicle.m_dMaxForce / vehicle.m_dMass;
        float h_l = maxSpeed / addV * maxSpeed / 2;

        // 目标不在前进方向范围内
        if(vehicle.velocity > 0 && dist < h_l && currTV <= 0)
        {
            return (toTarget - velocity).normalized * vehicle.m_dMaxForce;
        }

        // 距离接近开始减速
        float lost_l = (currV * currV) / (addV * 2);
        if (dist <= lost_l)
        {
            return -toTarget.normalized * vehicle.m_dMaxForce;
        }

        return toTarget.normalized * vehicle.m_dMaxForce;
        /**
        if (Mathf.Abs(dist) > 0.001f)
        {
            // 因为枚举Deceleration是整数，这里提供倍率
            const float decelerationTweaker = 0.33f;

            // 给定预期减速度，计算到达目标位置所需要的时间
            float speed = dist / ((int)deceleration * decelerationTweaker);

            speed = Mathf.Min(speed, vehicle.MaxSpeed());

            // 这边的处理和seek一样
            Vector2 des = toTarget * speed / dist;
            
            return des - vehicle.Velocity();
        }

        return Vector2.zero;
        */
    }

    float TurnarountTime(SoldierControl pAgent, Vector2 targetPos)
    {
        // 确定到目标的标准化向量
        Vector2 toTarget = (targetPos - pAgent.GetPos()).normalized;
        float dot = Vector2.Dot(pAgent.Heading(), toTarget);

        //改变这个值得到预期行为
        //交通工具的最大转弯绿越高，这个值越大
        //如果交通工具正在朝向到目标位置的反方向
        //那么0.5这个值意味着这个函数返回1秒的时间以便让交通工具转弯
        const float coefficient = 0.5f;

        //如果目标直接在前面，那么点积为1
        //如果目标在后面，那么点积为-1
        //减去1，除以负的coefficient，得到一个正的值
        //且正比于交通工具和目标的转动角位移
        return (dot - 1.0f) * -coefficient;
    }

    /// <summary>
    /// 追逐
    /// </summary>
    /// <param name="evader"></param>
    /// <returns></returns>
    public Vector2 Pursuit(SoldierControl evader)
    {
        //如果逃避者在前面，且面对智能体，那么我们只要靠近逃避者的当前位置
        Vector2 toEvader = evader.GetPos() - vehicle.GetPos();

        float relativeHeading = Vector2.Dot(vehicle.Heading(), evader.Heading());

        // acos(0.95) = 18
        if( Vector2.Dot(toEvader, evader.Heading()) > 0 && relativeHeading < -0.95)
        {
            return Seek(evader.GetPos());
        }

        // 预测逃避者位置

        float lookAheadTime = toEvader.magnitude / (vehicle.MaxSpeed() + evader.Speed());

        // 预测转动角所需的时间
        lookAheadTime += TurnarountTime(vehicle, evader.GetPos());

        // 现在靠近逃避者的预测位置
        return Seek(evader.GetPos() + evader.Velocity() * lookAheadTime);
    }

    /// <summary>
    /// 逃离
    /// </summary>
    /// <param name="pursuer"></param>
    /// <returns></returns>
    public Vector2 Evade(Soldier pursuer)
    {
        // 没有必要检查面向方向
        Vector2 toPursuer = pursuer.GetPos() - vehicle.GetPos();

        //  预测的时间正比于追逐者于逃避者的距离，反比与智能体的速度
        float lookAheadTime = toPursuer.magnitude / (vehicle.MaxSpeed() + pursuer.Speed());

        // 现在逃离追逐者预测的位置
        return Flee(pursuer.GetPos() + pursuer.Velocity() * lookAheadTime);
    }

    // wander的半径
    float m_dWanderRadius;

    // wander在智能体前的距离
    float m_dWanderDistance;

    // 每秒加到目标的随机位移的最大值
    float m_dWanderJitter;

    /// <summary>
    /// 徘徊 一定范围内的随机移动
    /// </summary>
    /// <returns></returns>
    public Vector2 Wander()
    {
        m_vWanderTarget += new Vector2(Random.Range(-1, 1) * m_dWanderJitter, Random.Range(-1, 1) * m_dWanderJitter);

        m_vWanderTarget.Normalize();
        m_vWanderTarget *= m_dWanderRadius;

        Vector2 targetLocal = m_vWanderTarget + new Vector2(m_dWanderDistance, 0);

        Vector2 targetWorld = Vector2Tool.PointToWorldSpace(targetLocal, vehicle.Heading(), vehicle.Side(), vehicle.GetPos());

        return targetWorld - vehicle.GetPos();
    }

    public Vector2 ObstacleAvoidance(List<BaseGameEntity> obstacles)
    {
        // 包围盒长度
        float m_dDBoxLength = Prm.MinDetectionBoxLength + (vehicle.Speed() / vehicle.MaxSpeed()) * Prm.MinDetectionBoxLength;

        // 标记包围盒内的节点
        BattleWorld.battleCenter.GetGameControl().TagObstaclesWithInViewRange(vehicle, m_dDBoxLength);

        //  跟踪最近 的相交的障碍物 CIB
        BaseGameEntity closestIntersectingObstacle = null;

        // 跟踪到CIB的距离
        float distToClosestIP = float.MaxValue;

        // 记录CIB被转化的局部坐标
        Vector2 localPosOfClosestObstacle = Vector2.zero;

        foreach(BaseGameEntity entity in obstacles)
        {
            if (entity.IsTag())
            {
                entity.SetTag(false);
                Vector2 localPos = Vector2Tool.PointToLocalSpace(entity.GetPos(), vehicle.Heading(), vehicle.Side(), vehicle.GetPos());

                // 如果局部空间位置有个负的X值
                // 它在后方，可以被忽视
                if(localPos.x >= 0)
                {
                    // 如果物品到x轴的距离小于 它的半径+碰撞盒宽度的一半则可能碰撞
                    float expandedRadius = entity.GetRadius() + vehicle.GetRadius();

                    if(Mathf.Abs(localPos.y) < expandedRadius)
                    {
                        // 现在做线/圆周相交测试。圆周的中心是

                        float cx = localPos.x;
                        float cy = localPos.y;

                        // 相交线段的一半长度
                        float sqrtPart = Mathf.Sqrt(expandedRadius * expandedRadius - cy * cy);

                        float ip = cx - sqrtPart;
                        if( ip < 0 )
                        {
                            ip = cx + sqrtPart;
                        }

                        // 判断这个是否是最近的
                        if( ip < distToClosestIP)
                        {
                            distToClosestIP = ip;
                            closestIntersectingObstacle = entity;
                            localPosOfClosestObstacle = localPos;
                        }
                    }
                }
            }
        }

        //vehicle.world.ResetTag();

        // 计算力
        // 远离它的控制力
        Vector2 steeringForce = Vector2.zero;
        if (closestIntersectingObstacle != null)
        {
            // 离碰撞物体越近，力就越强
            float multiplier = 1.0f + (m_dDBoxLength - localPosOfClosestObstacle.x) / m_dDBoxLength;

            // 计算侧向力
            steeringForce.y = (closestIntersectingObstacle.GetRadius() - localPosOfClosestObstacle.y) * multiplier;

            // 施加一个制动力，它正比于障碍物到交通工具的距离
            float brakingWeight = 0.2f;

            steeringForce.x = (closestIntersectingObstacle.GetRadius() - localPosOfClosestObstacle.y) * brakingWeight;

            // 最后，把操纵向量转化为世界向量
            return Vector2Tool.VectorToWorldSpace(steeringForce, vehicle.Heading(), vehicle.Side());
        }

        return Vector2.zero;
    }

    /// <summary>
    /// 避开墙
    /// </summary>
    /// <param name="walls"></param>
    /// <returns></returns>
    public Vector2 WallAvoidance(List<BPolygon> walls)
    {
        float minLength = float.MaxValue;
        Line minLine = null;

        // 包围盒长度
        float m_dDBoxLength = Prm.MinDetectionBoxLength + (vehicle.Speed() / vehicle.MaxSpeed()) * Prm.MinDetectionBoxLength + vehicle.GetRadius();
        Vector2 end = vehicle.Heading() * m_dDBoxLength + vehicle.GetPos();

        Line l = new Line(vehicle.GetPos(), end);

        foreach (BPolygon polygon in walls)
        {
            foreach(Line line in polygon.lines)
            {
                Vector2 intersect;
                if(Line.LineIntersection2D(line, l, out intersect))
                {
                    float len = (intersect - vehicle.GetPos()).magnitude;
                    if(len < minLength)
                    {
                        minLength = len;
                        minLine = line;
                    }
                }
            }
        }

        if(minLine != null)
        {
            return minLine.GetCoordinate2().Y * (m_dDBoxLength - minLength) / m_dDBoxLength * vehicle.m_dMaxForce;
        }
        return Vector2.zero;
    }

    /// <summary>
    /// 插入2个的之间
    /// </summary>
    /// <param name="agentA"></param>
    /// <param name="agentB"></param>
    /// <returns></returns>
    public Vector2 Interpose(Soldier agentA, Soldier agentB)
    {
        //首先，计算未来预测的时间T。
        Vector2 midPoint = (agentA.GetPos() + agentB.GetPos()) / 2;

        float timeToReachMidPoint = Vector2.Distance(vehicle.GetPos(), midPoint);

        // 现在我们有时间T，计算T时间后的，A,B所到的地点
        Vector2 aPos = agentA.Velocity() * timeToReachMidPoint + agentA.GetPos();
        Vector2 bPos = agentB.Velocity() * timeToReachMidPoint + agentB.GetPos();

        midPoint = (aPos + bPos) / 2;

        // 到达这里
        return Arrive(midPoint, Deceleration.FAST);
    }

    public PointPath path = null;

    /// <summary>
    /// 路径跟随
    /// </summary>
    /// <returns></returns>
    public Vector2 FollowPath()
    {
        if(path == null)
        {
            return Vector2.zero;
        }

        if(Vector2.Distance(path.GetCurrWayPoint(), vehicle.GetPos()) < ParamLoader.GetInstance().WaypointSeekDistSq)
        {
            path.SetNextWayPoint();
        }
        if (!path.IsFinished())
        {
            return Seek(path.GetCurrWayPoint());
        }
        else
        {
            return Arrive(path.GetCurrWayPoint(), Deceleration.NORMAL);
        }
    }

    /// <summary>
    /// 保持一定偏移的追逐
    /// </summary>
    /// <param name="leader">队长</param>
    /// <param name="offset">以队长的局部坐标计算的偏移位置</param>
    /// <returns></returns>
    public Vector2 OffsetPursuit(Soldier leader, Vector2 offset)
    {
        // 计算世界坐标
        Vector2 worldOffsetPos = Vector2Tool.PointToWorldSpace(offset, leader.Heading(), leader.Side(), leader.GetPos());

        Vector2 toOffset = worldOffsetPos - vehicle.GetPos();

        // 预期的时间正比于追逐者的距离
        // 反比与2个智能体的速度和
        float lookAheadTime = toOffset.magnitude / (vehicle.MaxSpeed() + leader.Speed());

        // 到达预期的位置
        return Arrive(lookAheadTime * leader.Velocity() + worldOffsetPos, Deceleration.FAST);
    }

    // 计算
    bool AccumulateForce(Vector2 force, Vector2 forceToAdd, out Vector2 res)
    {
        res = force;
        float magnitudeSoFar = force.magnitude;
        float magnitudeRemaining = vehicle.m_dMaxForce - magnitudeSoFar;
        if (magnitudeRemaining < 0) return false;

        float magnitudeToAdd = forceToAdd.magnitude;
        if(magnitudeToAdd < magnitudeRemaining)
        {
            res = force + forceToAdd;
        }
        else
        {
            res = force + (forceToAdd.normalized * magnitudeRemaining);
        }
        
        return true;
    }


    /// <summary>
    /// 判断墙并避开
    /// </summary>
    /// <returns></returns>
    public Vector2 TryWallAvoidance()
    {
        if (vehicle.BBstatus == BASE_BATTLE_STATUS.NORMAL)
        {
            // 墙
            BPolygon boundary = BattleWorld.battleMap.GetBoundary();
            List<BPolygon> boundarys = new List<BPolygon>();
            boundarys.Add(boundary);

            // 避开墙
            Vector2 addForce = WallAvoidance(boundarys);

            return addForce;
        }

        return Vector2.zero;
    }

    /// <summary>
    /// 合力
    /// </summary>
    /// <returns></returns>
    public Vector2 Calculate()
    {
        Vector2 force = Vector2.zero;
        // 不是停止状态
            force += Arrive(vehicle.GetTarget(), Deceleration.FAST);

            Debug.Log("Calculate");
            Debug.Log(force.magnitude);
            return force;
    }
}
