using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollisionType
{
    CYLINDER,   // 圆柱
    CUBE,       // 立方体
    POINT,      // 点     
}

public class CollisionMessage
{
    int one_id;
    int other_id;
    Vector3 pos;

    public CollisionMessage(CollisionObj one, CollisionObj other, Vector3 pos)
    {
        this.one_id = one.GetId();
        this.other_id = other.GetId();
        this.pos = pos;
    }

    public bool IsCollision(int id, int _id)
    {
        if((one_id == id && other_id == _id)
            || (one_id == _id && other_id == id))
        {
            return true;
        }

        return false;
    }
}

/// <summary>
/// 碰撞检查， 这一套检查都是简易检查，不精细，粗略
/// 目的仅是快速
/// </summary>
public class CollisionManager
{
    private int sleep = 0;

    private static CollisionDetection[][] detections;

    private Dictionary<int, List<CollisionMessage>> collisionMessage;

    public CollisionManager()
    {
        detections = new CollisionDetection[3][];

        collisionMessage = new Dictionary<int, List<CollisionMessage>>();

        // 圆
        detections[0] = new CollisionDetection[3]{
            new CollisionDetectionCylinderTCylinder(),
            new CollisionDetectionCubeTCylinder(),
            new CollisionDetectionCylinderTPoint()
        };

        // 立方体
        detections[1] = new CollisionDetection[3]{
            new CollisionDetectionCubeTCylinder(),
            new CollisionDetectionCubeTCube(),
            new CollisionDetectionCubeTPoint()
        };

        // 点
        detections[2] = new CollisionDetection[3]{
            new CollisionDetectionCylinderTPoint(),
            new CollisionDetectionCubeTPoint(),
            new CollisionDetectionPointTPoint()
        };
        
    }

    public bool GetCollisionList(int id, out List<CollisionMessage> list)
    {
        if (collisionMessage.TryGetValue(id, out list))
        {
            return true;
        }

        return false;
    }

    // 简单检查
    public static bool IsBriefnessCollision(CollisionPH one, CollisionPH other, out float distance)
    {
        float dis = Vector3Tool.ToVector2(one.pos - other.pos).magnitude;
        distance = dis;
        if (dis > other.briefnessBrradius + one.briefnessBrradius)
        {
            distance = dis;
            return false;
        }

        return true;
    }

    /// <summary>
    /// 复杂的碰撞检查检查,已检查过简单检测
    /// </summary>
    /// <param name="one">一个碰撞体</param>
    /// <param name="other">另一个碰撞体</param>
    /// <returns>是否碰撞</returns>
    public static bool IsCollision(CollisionPH one, CollisionPH other, out Vector3 pos)
    {
        CollisionType one_type = one.collisionType;
        CollisionType other_type = other.collisionType;

        CollisionDetection detection = detections[(int)one_type][(int)other_type];
        detection.PrintName();

        return detection.Detection(one, other, out pos);
    }
    
    
    /// <summary>
    /// 更新每个物体的碰撞检查范围
    /// </summary>
    /// <param name="collisionObjs"></param>
    private void UpdateAllObjCollision(List<CollisionObj> collisionObjs)
    {
        foreach (CollisionObj obj in collisionObjs)
        {
            obj.ClearCollisions();
            obj.UpdateInvestigationBradius();
        }

        int count = collisionObjs.Count;
        for (int i = 0; i < count ; i++)
        {
            CollisionObj one = collisionObjs[i];

            for (int j = i + 1; j < count; j++)
            {
                // 在 范围内的 添加
                CollisionObj other = collisionObjs[j];
                float dis = (one.transform.localPosition - other.transform.localPosition).magnitude;
                if(dis <= one.investigationBradius)
                {
                    one.collisions.Add(other);
                }

                if (dis <= other.investigationBradius)
                {
                    other.collisions.Add(one);
                }
            }

        }
    }

    /// <summary>
    /// 碰撞检查
    /// </summary>
    public void CollisionDetection()
    {

        collisionMessage.Clear();

        List<CollisionObj> collisionObjs = BattleWorld.battleCenter.gameControl.GetAllLifeCollisions();
        sleep--;
        if(sleep < 0)
        {
            sleep = 30;
            UpdateAllObjCollision(collisionObjs);
        }

        foreach (CollisionObj obj in collisionObjs)
        {
            obj.UpdateCoordinate2();
        }

        List<int> leaveObj = new List<int>();

        foreach (CollisionObj obj in collisionObjs)
        {
            List<CollisionObj> others = obj.collisions;
            leaveObj.Clear();

            int count = others.Count;
            for (int i = 0; i < count; i++) // foreach (CollisionObj other in others)
            {
                CollisionObj other = others[i];
                float distance;

                CollisionPH one_ph = obj.GetCollisionPH();
                CollisionPH other_ph = other.GetCollisionPH();

                if (IsBriefnessCollision(one_ph, other_ph, out distance))
                {
                    Vector3 pos;
                    if(IsCollision(one_ph, other_ph, out pos))
                    {
                        List<CollisionMessage> message;
                        CollisionMessage news = new CollisionMessage(obj, other, pos);

                        if (collisionMessage.TryGetValue(obj.GetId(), out message))
                        {
                            message.Add(news);
                        }
                        else
                        {
                            message = new List<CollisionMessage>();
                            message.Add(news);

                            collisionMessage.Add(obj.GetId(), message);
                        }

                        if (collisionMessage.TryGetValue(other.GetId(), out message))
                        {
                            message.Add(news);
                        }
                        else
                        {
                            message = new List<CollisionMessage>();
                            message.Add(news);

                            collisionMessage.Add(other.GetId(), message);
                        }

                        obj.Collision(other, pos);
                    }
                }
                else
                {
                    // 待清除 远离的对象
                    if(distance > obj.investigationBradius)
                    {
                        leaveObj.Add(i);
                    }
                }
            }

            // 清除离开的对象
            for (int i = 0; i < leaveObj.Count; i++)
            {
                int index = leaveObj[i];

                // 因为每减少一个，后面的坐标都会前移一位
                others.RemoveAt(index - i);
            }
        }
    }
}
