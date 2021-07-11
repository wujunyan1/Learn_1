using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : BaseControl
{
    int camp;
    public int Camp
    {
        get
        {
            return camp;
        }
    }

    GameControl controlCenter;

    List<SoldierControl> soldiers;
    List<SoldierControl> deadSoldiers;

    List<SoldierControl> selectSoldiers;

    public PlayerControl(int camp, GameControl controlCenter)
    {
        this.camp = camp;
        this.controlCenter = controlCenter;

        soldiers = new List<SoldierControl>();

        deadSoldiers = new List<SoldierControl>();

        selectSoldiers = new List<SoldierControl>();
    }

    public void AddSoldier(SoldierControl control)
    {
        soldiers.Add(control);
    }

    public void SelectAreaSoldiers(IsoscelesTrapezoid isosceles)
    {
        ClearSelectAreaSoldiers();

        List<SoldierControl> controls = soldiers;
        foreach (SoldierControl control in controls)
        {
            Vector3 vector = control.transform.position;
            if (isosceles.IsInIsoscelesTrapezoid(new Vector2(vector.x, vector.z)))
            {
                AddSelectSoldiers(control);
            }
        }
    }

    public void AddSelectSoldiers(SoldierControl control)
    {
        selectSoldiers.Add(control);
        control.ShowSelect();
    }

    public void ClearSelectAreaSoldiers()
    {
        foreach(var data in selectSoldiers)
        {
            data.HideSelect();
        }

        selectSoldiers.Clear();
    }

    // 集体移动 找到中心人物，中心人物移动 到指定点，其他人物则相对移动
    public void MoveTo(Vector3 vector)
    {
        Vector2 v = Vector3Tool.ToVector2(vector);

        int count = selectSoldiers.Count;
        Vector3 center = Vector3.zero;

        // 找到所有对象的最中心的点
        foreach (var data in selectSoldiers)
        {
            center += data.transform.localPosition;
        }

        center = center / count;
        Vector2 offestPos = v - Vector3Tool.ToVector2(center);


        foreach (var data in selectSoldiers)
        {
            Vector2 movePos = Vector3Tool.ToVector2(data.transform.localPosition - center) + v;

            Debug.Log(v);
            Debug.Log(movePos);

            data.ClearOrderList();
            data.MoveTo(movePos);
        }

        /***
        // 找到的最中心的对象
        float minLength = float.MaxValue;
        SoldierControl center = selectSoldiers[0];
        Vector3 centerPos;

        // 赋予初始值 ls 为 第i个与其他 的距离
        Vector3[] ls = new Vector3[count];
        for(int i = 0; i < count; i++)
        {
            ls[i] = Vector3.zero;
        }


        // 算每个对象到其他对象的向量总和， 值越小则越中心
        for (int i = 0; i < count; i++)
        {
            SoldierControl one = selectSoldiers[i];

            // 这样 j = i  能减少接近一半的计算量 需要一个ls的临时变量
            for (int j = i; j < count; j++)
            {
                SoldierControl two = selectSoldiers[j];

                // one 到 two
                Vector3 add_l = one.transform.localPosition - two.transform.localPosition;
                // one 对 two 为正
                ls[i] += add_l;

                // two 到 one 为负
                Vector3 two_l = ls[j];
                ls[j] -= add_l;
                
            }

            float length = ls[i].magnitude;
            if (length < minLength)
            {
                minLength = length;
                center = one;
            }
        }
        centerPos = center.transform.localPosition;
        foreach (var data in selectSoldiers)
        {
            Vector2 movePos = v + Vector3Tool.ToVector2(data.transform.localPosition - centerPos);

            Debug.Log(v);
            Debug.Log(movePos);

            data.MoveTo(movePos);
        }
        */


    }

    // 攻击
    public void Attack(SoldierControl enemy)
    {
        foreach (var data in selectSoldiers)
        {
            data.ClearOrderList();

            Debug.Log("------1111---------");
            data.AttackTo(enemy.GetId());
        }
    }
}
