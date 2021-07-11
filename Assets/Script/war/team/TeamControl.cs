using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 队伍管理
/// </summary>
public class TeamControl : BaseControl
{
    public GeneraData generaData;

    // 士兵数量
    public int soldierNum;

    // 士兵经验值
    public int exp;

    // 兵种key
    public string soldierKey;

    // 最大成员数量
    public int maxMemberNum;

    public int lineSpace;
    public int rowSpace;

    // 队伍
    public Soldier[] teamMembers;

    // 一个队伍由一个将军和多名士兵组成， 将军位于中间
    private Soldier genera;

    // 领导者 一般为 将军，将军死后才会是其他小兵
    private Soldier leader;

    // 列数量
    public int rowNum;

    // 中心，旗帜所在，如果将军活着则是将军，不然则是排序中间的士兵
    int _centerIndex;
    int CenterIndex
    {
        get
        {
            return _centerIndex;
        }
        set
        {
            _centerIndex = value;

            leader = teamMembers[_centerIndex];
            for (int i = 0; i < soldierNum; i++)
            {
                Soldier soldier = teamMembers[i];

                if(i == _centerIndex)
                {
                    // 领导自己本身没有跟随目标
                    soldier.SetLeader(null);
                }
                else
                {
                    soldier.SetLeader(leader);
                }
            }
            
        }
    }

    // 阵容
    public Formation formation;

    public Transform world;

    private void Awake()
    {
        teamMembers = new Soldier[maxMemberNum];

        formation = FormationManager.GetDafultFormation(maxMemberNum);

        formation.lineSpace = lineSpace;
        formation.rowSpace = rowSpace;
        
        formation.SetWidth(rowNum);
    }

    private void Start()
    {
        InitTeamMembers();
        
    }

    // 更新
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        foreach(Soldier soldier in teamMembers)
        {
            if (soldier != null)
            {
                soldier.LogicUpdate();
            }
        }
    }


    private Soldier CreateControl(int index)
    {
        Soldier member;

        // 判断中心位置
        if (0 == index)
        {
            member = SoliderManager.Instance.CreateGenera(soldierKey, generaData);
            genera = member;
        }
        else
        {
            member = SoliderManager.Instance.CreateSoldier(soldierKey); // Instantiate<Soldier>(prefabs);
        }

        return member;
    }

    /// <summary>
    /// 初始化初始成员
    /// </summary>
    private void InitTeamMembers()
    {
        for (int i = 0; i < soldierNum; i++)
        {
            Soldier member = CreateControl(i);
            teamMembers[i] = member;

            member.control = this;
            //member.transform.SetParent(world);
            //member.transform.localPosition = formation.GetVector3(i);
            member.SetIndex(i);
        }
    }

    /// <summary>
    /// 追逐某个敌人
    /// </summary>
    /// <param name="enemy"></param>
    public void Follow(TeamControl enemy)
    {

    }

    public void MoveTo(Vector3 target)
    {
        leader.targetPos = Vector3Tool.ToVector2(target);
    }

    public void MoveTo(Vector3 left, Vector3 right)
    {
        SetTeamFormationWidth(left, right);

        // 第一个位置与中心的偏移
        Vector3 offset = formation.GetVector3(0);
        Vector3 centerVector = left - offset;

        MoveTo(centerVector);
        /*
        for (int i = 0; i < maxMemberNum; i++)
        {
            Soldier control = teamMembers[i];
            if (control)
            {
                //control.transform.localPosition = formation.GetVector3(i) + centerVector;

                Vector3 t = formation.GetVector3(i) + centerVector;
                control.MoveTo(t);
            }
        }
        */
    }

    public void SetPosition(Vector3 point)
    {
        for (int i = 0; i < maxMemberNum; i++)
        {
            Soldier control = teamMembers[i];
            if (control != null)
            {
                //control.transform.localPosition = formation.GetVector3(i) + point;
            }
        }
    }

    /// <summary>
    /// 需要修改
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    // 设置默认阵容宽度
    public void SetTeamFormationWidth(Vector3 left, Vector3 right)
    {
        float distance = Vector3.Distance(left, right);
        formation.CalcWidth(distance);

        Vector3 targetDir = right - left;
        float angle = Vector3.Angle(Vector3.forward, targetDir); // 返回当前坐标与目标坐标的角度

        if (targetDir.x < 0)
        {
            angle = 360 - angle;
        }

        angle = angle - 90;
        if (angle < 0)
        {
            angle += 360;
        }

        Debug.Log(string.Format("{0}", angle));
        formation.angle = angle * Mathf.PI / 180;

        int newCenterIndex = formation.centerIndex;

        Soldier c = teamMembers[newCenterIndex];
        teamMembers[newCenterIndex] = teamMembers[CenterIndex];
        teamMembers[CenterIndex] = c;

        CenterIndex = newCenterIndex;
    }

    // 设置阵容位置和宽度
    public void SetTeamFormationWidthAndPosition(Vector3 left, Vector3 right)
    {
        SetTeamFormationWidth(left, right);

        float angle = formation.angle * 180 / Mathf.PI;

        // 第一个位置与中心的偏移
        Vector3 offset = formation.GetVector3(0);
        Vector3 centerVector = left - offset;

        for (int i = 0; i < maxMemberNum; i++)
        {
            Soldier control = teamMembers[i];
            if (control != null)
            {
                //control.transform.localPosition = formation.GetVector3(i) + centerVector;
                //control.transform.rotation = Quaternion.Euler(0, angle, 0);
            }
        }
    }
}
