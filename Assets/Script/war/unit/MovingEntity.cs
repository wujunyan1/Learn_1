using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEntity : BaseGameEntity
{
    /// <summary>
    /// 速率
    /// </summary>
    public Vector2 m_vVelocity;

    public float velocity = 0f;

    /// <summary>
    /// 头面朝方向
    /// </summary>
    private Vector2 m_vHeading;

    /// <summary>
    /// 垂直于朝向方向向量
    /// </summary>
    public Vector2 m_vSide;

    // 质量
    public float m_dMass;

    /// <summary>
    /// 速度
    /// </summary>
    public float m_dMaxSpeed;

    /// <summary>
    /// 最大力
    /// </summary>
    public float m_dMaxForce;

    /// <summary>
    /// 转向速度
    /// </summary>
    public float m_dMaxTurnRate;

    public float CalcAngle(Vector3 offset)
    {
        Vector3 targetDir = offset; // offset.normalized;

        /***
         * result  angle
         * 0        90
         * 1        0
         * -1       180
         * 
         * 大于0 则是正面  -90 ` 90
         * 
         **/
        float result = Vector3.Dot(Vector3.forward, targetDir);

        // 通过反余弦函数获取 向量 a、b 夹角（默认为 弧度）
        float radians = Mathf.Acos(result);

        // 将弧度转换为 角度
        float angle = radians * Mathf.Rad2Deg;

        // 右手法则
        // y 小于0 则在左边   180 -- 360
        // 叉积
        // 小于0 则 后面的是前面的逆时针方向
        // 大于0 则 后面的是前面的顺时针方向
        Vector3 c = Vector3.Cross(Vector3.forward, targetDir);

        if (c.y < 0)
        {
            angle = 360 - angle;
        }
        
        return angle;
    }

    public float MaxSpeed()
    {
        return m_dMaxSpeed;
    }

    public Vector2 Velocity()
    {
        return m_vVelocity;
    }

    public Vector2 Heading()
    {
        return m_vHeading;
    }

    public void Rotation(float angle)
    {
        this.transform.Rotate(new Vector3(0, angle, 0));

        //this.transform.localRotation = Quaternion.Euler(0, angle, 0);
        m_vHeading = Vector3Tool.ToVector2(this.transform.TransformDirection(Vector3.forward)).normalized;
        m_vSide = Vector2Tool.Perp(m_vHeading);
    }

    public void SetHeading(Vector2 dir)
    {
        m_vHeading = dir;

        float angle = CalcAngle(Vector2Tool.ToVector3(m_vHeading));

        this.transform.localRotation = Quaternion.Euler(0, angle, 0);
        ////this.transform.Rotate(new Vector3(0, angle, 0), Space.World);
    }

    public Vector2 Side()
    {
        return m_vSide;
    }

    public float Speed()
    {
        return Velocity().magnitude;
    }

    public Coordinate2 GetCoordinate2()
    {
        Vector2 start = Vector3Tool.ToVector2(transform.localPosition);
        return new Coordinate2(start, start + m_vHeading);
    }
}
