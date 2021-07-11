using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitControl : MonoBehaviour
{
    //public UnitAttribute attribute;
    public UnitAnimation animation;
    //public TeamAttribute teamAttribute;
    public Soldier soldier;

    public Rigidbody rigidbody;

    public const float PRECISION = 1f;
    public const float NEGATIVE = 0.1f;


    public TeamControl control;

    // 当前的速度
    //Vector3 speed;

    Vector3 changeSpeed;

    // 目标位置
    Vector3 target;

    // 目标角度
    float targetAngle;

    // 前进方向
    Vector3 dir;

    float _speed;
    // 移动速度
    float speed {
        get
        {
            return _speed;  
        }

        set
        {
            _speed = value;
            animation.UpdateMoveSpeed(_speed);
        }
    }

    bool _moveEnd;
    bool moveEnd
    {
        get
        {
            return _moveEnd;
        }
        set
        {
            _moveEnd = value;
            animation.isMove = !_moveEnd;
        }
    }

    bool _rotateEnd;
    bool rotateEnd
    {
        get
        {
            return _rotateEnd;
        }
        set
        {
            _rotateEnd = value;
            animation.isRotate = !_rotateEnd;
        }
    }

    // 是否加速
    bool isSpeedUp;

    public TextMesh text;

    private void Awake()
    {
        dir = Vector3.forward;
        changeSpeed = Vector3.zero;
        target = this.transform.localPosition;
        
    }

    private void Start()
    {
        //teamAttribute = control.teamAttribute;
        //animation.attribute = teamAttribute;

        speed = 0f;
        moveEnd = true;
        rotateEnd = true;
    }

    private void FixedUpdate()
    {
        if (target.Equals(this.transform.localPosition))
        {
            return;
        }

        UpdateRotation();

        UpdateMove();
    }

    // 预算下一个速度
    float PreClacSpeed()
    {
        // 角度没转完的情况下
        float distance = Vector3.Distance(target, transform.localPosition);
        if (distance < soldier.GetRadius() && !rotateEnd)
        {
            isSpeedUp = false;
        }
        else
        {
            isSpeedUp = true;
        }

        float acceleratedSpeed = soldier.Speed(); // teamAttribute.acceleratedSpeed;
        if (!isSpeedUp)
        {
            acceleratedSpeed = -acceleratedSpeed;
        }

        float nextSpeed = speed + acceleratedSpeed;
        if (nextSpeed > soldier.m_dMaxSpeed) // teamAttribute.maxMoveSpeed)
        {
            nextSpeed = soldier.m_dMaxSpeed; // teamAttribute.maxMoveSpeed;
        }
        if (nextSpeed < 0)
        {
            nextSpeed = 0;
        }

        return nextSpeed;
    }

    // 更新速度
    void UpdateSpeed()
    {
        speed = PreClacSpeed();
    }

    // 预算下一个位置
    float PreClacPosition( bool isUpdate)
    {
        if (moveEnd)
        {
            return -1;
        }

        float nextSpeed = PreClacSpeed();
        if (isUpdate)
        {
            speed = nextSpeed;
        }

        float moveDistance = nextSpeed * Time.fixedDeltaTime;

        Debug.Log(string.Format(" cccccccccccccc {0} {1}", moveDistance, Time.fixedDeltaTime));

        // 与目标的距离小于点则
        float targetDistance = Vector3.Distance(target, this.transform.localPosition);
        if (targetDistance < moveDistance)
        {
            moveDistance = targetDistance;

            if (isUpdate)
            {
                moveEnd = true;
            }
        }

        return moveDistance;
        //this.transform.Translate(Vector3.forward * moveDistance);
        //float angle = this.transform.rotation.eulerAngles.y;


        //nextPosition = this.transform.localPosition + this.transform.rotation.eulerAngles * moveDistance;
       
        //return nextMoveEnd;
    }

    // 移动
    void UpdateMove()
    {
        if (moveEnd)
        {
            return;
        }
        
        float moveDistance = PreClacPosition(true);

        this.transform.Translate(Vector3.forward * moveDistance);

        if (moveEnd)
        {
            speed = 0;
            target = this.transform.localPosition;
        }
    }

    // 预算下一个位置
    public Vector3 PreClacMove()
    {
        if (moveEnd)
        {
            return this.transform.localPosition;
        }

        float moveDistance = PreClacPosition(false);

        Vector3 currPosition = this.transform.localPosition;
        if (moveDistance < 0f)
        {
            return currPosition;
        }
        this.transform.Translate(Vector3.forward * moveDistance);

        Vector3 nextPosition = this.transform.localPosition;

        this.transform.localPosition = currPosition;

        return nextPosition;
    }

    float ClacTargetAngle()
    {
        Vector3 offset = this.target - this.transform.localPosition;
        offset.y = 0;

        Vector3 targetDir = Vector3Tool.Normalized(offset); // offset.normalized;

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

        //Debug.Log(string.Format("result {0}  angle  {1} ", result, angle));

        // 右手法则
        // y 小于0 则在左边   180 -- 360
        Vector3 c = Vector3.Cross(Vector3.forward, targetDir);

        if (c.y < 0)
        {
            angle = 360 - angle;
        }

        //targetAngle = angle;

        return angle;
    }

    private float PreClacRoutateEnd(float angle)
    {
        Vector3 offset = this.target - this.transform.localPosition;
        offset.y = 0;

        Vector3 targetDir = Vector3Tool.Normalized(offset); // offset.normalized;
        //Vector3 speedDir = Vector3Tool.Normalized(speed);  //speed.normalized;

        float curr_angle = transform.rotation.eulerAngles.y;

        float offsetAngle = Mathf.Abs(angle - curr_angle);
        if (offsetAngle < 0.1f)
        {
            //transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
            //rotateEnd = true;
            return angle - curr_angle;
        }

        // 转向速度
        float rotateSpeed = soldier.m_dMaxTurnRate; // teamAttribute.swerve;

        if (offsetAngle < rotateSpeed)
        {
            rotateSpeed = offsetAngle;
        }

        float a = angle;
        if (a - curr_angle < 0)
        {
            a += 360;
        }

        if (a - curr_angle > 180)
        {
            rotateSpeed = -rotateSpeed;
        }

        return rotateSpeed;
    }

    public float PreNextRotation()
    {
        float curr_angle = transform.rotation.eulerAngles.y;
        if (rotateEnd)
        {
            return curr_angle;
        }

        float angle = ClacTargetAngle();

        float rotateSpeed = PreClacRoutateEnd(angle);

        return curr_angle + rotateSpeed;
    }

    // 更新角度
    private void UpdateRotation()
    {
        if (rotateEnd)
        {
            return;
        }

        targetAngle = ClacTargetAngle();

        float rotateSpeed = PreClacRoutateEnd(targetAngle);

        //Vector3 offset = this.target - this.transform.localPosition;
        //offset.y = 0;

        //Vector3 targetDir = Vector3Tool.Normalized(offset); // offset.normalized;
        ////Vector3 speedDir = Vector3Tool.Normalized(speed);  //speed.normalized;

        //float curr_angle = transform.rotation.eulerAngles.y;

        //float offsetAngle = Mathf.Abs(targetAngle - curr_angle);
        //if (offsetAngle < 0.1f)
        //{
        //    transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        //    rotateEnd = true;
        //    return;
        //}

        //// 转向速度
        //float rotateSpeed = teamAttribute.swerve;

        //if (offsetAngle < rotateSpeed)
        //{
        //    rotateSpeed = offsetAngle;
        //}

        //float a = targetAngle;
        //if (a - curr_angle < 0)
        //{
        //    a += 360;
        //}

        //if(a - curr_angle > 180)
        //{
        //    rotateSpeed = -rotateSpeed;
        //}

        //Debug.Log(string.Format("targetAngle {0}  curr_angle  {1}  rotateSpeed {2} ", targetAngle, curr_angle, rotateSpeed));

        transform.Rotate(new Vector3(0, rotateSpeed, 0), Space.World);

        float curr_angle = transform.rotation.eulerAngles.y;
        float offsetAngle = Mathf.Abs(targetAngle - curr_angle);
        if (offsetAngle < 0.01f)
        {
            rotateEnd = true;
            return;
        }
    }



    // 向这个点移动
    public void MoveTo(Vector3 target)
    {
        target.y = 0;

        //target.x = 20;
        //target.z = 20;

        this.target = target;
        moveEnd = false;
        rotateEnd = false;
    }

    /*
    Vector3 GetChangeSpeed()
    {
        Vector3 offset = this.target - this.transform.localPosition;
        offset.y = 0;
        if(offset.magnitude < 0.1f)
        {
            speed = Vector3.zero;
            return Vector3.zero;
        }

        Vector3 dir = Vector3Tool.Normalized(offset); // offset.normalized;
        Vector3 speedDir = Vector3Tool.Normalized(speed);  //speed.normalized;

        Debug.Log(string.Format("dir == {0} {1} ", offset, dir));
        Debug.Log(string.Format("speedDir == {0} ", speedDir));

        float result = Vector3.Dot(dir, speedDir);
        Debug.Log(string.Format("result == {0} ",result));

        Vector3 d = dir * Mathf.Abs(result) * speed.magnitude;

        if(dir == -speedDir)
        {
            Debug.Log(string.Format("相反方向 == {0} ", result));
        }

        //if (Mathf.Abs(speed.x) < PRECISION && Mathf.Abs(speed.z) < PRECISION)
        //{
        //    d = dir;
        //}

        Vector3 forceDir = d - speed;
        Debug.Log(forceDir);
        Debug.Log( string.Format("{0} {1} {2} {3}", forceDir.x, forceDir.x * 100, Mathf.Abs(forceDir.x * 100), Mathf.Abs(forceDir.x * 100) < PRECISION));
        Debug.Log( string.Format("{0} {1} {2} {3}", forceDir.z, forceDir.z * 100, Mathf.Abs(forceDir.z * 100), Mathf.Abs(forceDir.z * 100) < PRECISION));
        if (Vector3Tool.Magnitude(forceDir) < NEGATIVE)
        {
            Debug.Log(22222222222222);
            forceDir = dir;
        }

        Debug.Log(forceDir);

        forceDir = Vector3Tool.Normalized(forceDir); // forceDir.normalized;
        Debug.Log(forceDir);

        // 获取力的大小
        float force = attribute.force;
        Vector3 addForce = force * forceDir;

        // F = ma  a = F/m
        // F 力 m 质量 a 加速度
        changeSpeed = addForce / attribute.mass;

        // result == 0 表示反方向 并且当前速度小与减速度
        if(Mathf.Abs(result) < NEGATIVE && speed.magnitude < changeSpeed.magnitude && speed.magnitude > NEGATIVE)
        {
            Debug.Log(11111111111);
            Debug.Log(changeSpeed);
            Debug.Log(speed);
            Debug.Log(dir);
            Debug.Log(d);
            Debug.Log(forceDir);


            changeSpeed = speed * -1;

            return changeSpeed;
        }

        Debug.Log(string.Format("加速度 == {0} ", changeSpeed));
        return changeSpeed * Time.deltaTime;
    }
    */

    // 摩擦力造成的减速
    Vector3 GetFrictionSpeed()
    {
        // 获取力的大小
        float friction = soldier.m_dMaxForce; // teamAttribute.friction;
        Vector3 addfriction = friction * dir;

        // F = ma  a = F/m
        // F 力 m 质量 a 加速度
        return addfriction / soldier.m_dMass; // teamAttribute.mass;
    }

    public void SetIndex(int index)
    {
        text.text = string.Format("{0}", index);
    }
}
