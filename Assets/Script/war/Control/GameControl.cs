using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : BaseControl
{
    List<SoldierControl> soldiers;
    List<SoldierControl> deadSoldiers;

    List<CollisionObj> collisionObjs;

    Dictionary<int, SoldierControl> controls;

    // 这个仅记录 用于AI使用
    List<SoldierControl>[] campSoldiers;

    public PlayerControl playerControl
    {
        get;set;
    }

    CollisionManager collisionManager;
    public CollisionManager CollisionManager
    {
        get
        {
            return collisionManager;
        }
    }

    public GameControl()
    {
        soldiers = new List<SoldierControl>();
        collisionObjs = new List<CollisionObj>();
        controls = new Dictionary<int, SoldierControl>();

        deadSoldiers = new List<SoldierControl>();

        playerControl = new PlayerControl(BattleWorld.playerCamp, this);

        collisionManager = new CollisionManager();
    }

    public void StopBattle()
    {
        foreach (SoldierControl con in soldiers)
        {
            con.Stop();
        }
    }

    public void SetCampNum(int num)
    {
        campSoldiers = new List<SoldierControl>[num];
        for(int i = 0; i < num; i ++)
        {
            campSoldiers[i] = new List<SoldierControl>();
        }
    }

    public List<SoldierControl> GetAllLifeSoldiers()
    {
        return soldiers;
    }

    public List<CollisionObj> GetAllLifeCollisions()
    {
        return collisionObjs;
    }

    public List<SoldierControl> GetCampLifeSoldiers(int campNum)
    {
        List<SoldierControl> list = new List<SoldierControl>();

        List<SoldierControl> n = campSoldiers[campNum];

        foreach(SoldierControl con in n)
        {
            if (con.isLife())
            {
                list.Add(con);
            }
        }

        return list;
    }

    public void AddSoldier(int camp, SoldierControl control, CollisionObj collision)
    {
        soldiers.Add(control);

        List<SoldierControl> soldierControls = campSoldiers[camp];
        soldierControls.Add(control);

        if(camp == BattleWorld.playerCamp)
        {
            playerControl.AddSoldier(control);
        }

        collisionObjs.Add(collision);

        controls.Add(control.GetId(), control);
    }

    // 判断 包围半径
    public void TagObstaclesWithInViewRange(MovingEntity entity, float length)
    {
        Vector2 m_pos = entity.GetPos();

        List<BaseGameEntity> objstacles = BattleWorld.battleMap.getObstacles();

        foreach (BaseGameEntity e in objstacles)
        {
            // 距离 小于 包围盒距离 + 半径
            if (Vector2.Distance(e.GetPos(), m_pos) < length + e.GetRadius())
            {
                e.SetTag(true);
            }
        }
    }

    public void ResetTag()
    {
        List<BaseGameEntity> objstacles = BattleWorld.battleMap.getObstacles();

        foreach (BaseGameEntity e in objstacles)
        {
            e.SetTag(false);
        }
    }

    public override void LogicUpdate()
    {
        // 先判断是否死亡
        foreach(SoldierControl control in soldiers)
        {
            control.CheckLife();
        }

        // 所有buff执行

        // 移动执行，碰撞检测
        // AI逻辑运算

        // 进行移动
        foreach(SoldierControl control in soldiers)
        {
            control.MoveUpdate();
        }

        // 碰撞检查
        collisionManager.CollisionDetection();

        // 逻辑判断
        foreach (SoldierControl control in soldiers)
        {
            control.LogicUpdate();
        }
    }

    public void SeletShowSoldier(Vector2 pos)
    {
        SoldierControl showControl = null;
        List<SoldierControl> controls = soldiers;
        foreach (SoldierControl control in controls)
        {
            Vector3 vector = control.transform.position;

            if ((pos - Vector3Tool.ToVector2(vector)).magnitude < control.bradius)
            {
                // 如果没有被选中 则加入
                if (!control.IsSelect() && control.data.camp == playerControl.Camp)
                {
                    playerControl.AddSelectSoldiers(control);
                }

                showControl = control;
                //this.FireEvent("SHOW_SOLDIER_MESSAGE", control);
                break;
            }
        }

        if (showControl != null)
        {
            this.FireEvent("SHOW_SOLDIER_MESSAGE", showControl);
        }
    }

    // 返回这个坐标点的敌人
    public SoldierControl GetSelectEnemy(Vector2 pos)
    {
        SoldierControl select = null;

        foreach (SoldierControl control in soldiers)
        {
            if(control.data.camp != BattleWorld.playerCamp)
            {
                Vector3 vector = control.transform.position;

                if ((pos - Vector3Tool.ToVector2(vector)).magnitude < control.bradius)
                {
                    select = control;
                    break;
                }
            }
        }

        return select;
    }

    public SoldierControl GetSoldierControlById(int id)
    {
        SoldierControl control = null;
        if (controls.TryGetValue(id, out control))
        {
            return control;
        }

        return null;
    }
}
