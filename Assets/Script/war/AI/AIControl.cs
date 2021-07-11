using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControl : EventObject
{
    private SoldierControl control;

    private List<BaseAi> aiList;

    public AIControl(SoldierControl control)
    {
        this.control = control;

        aiList = new List<BaseAi>();

        //AddAI<AttackAI>();
    }

    public T AddAI<T>() where T : BaseAi, new()
    {
        T t = new T();
        t.SetAIControl(this);

        aiList.Add(t);
        return t;
    }

    public void AddEvent(string name, object o)
    {
        Debug.Log("------3333333333333---------");
        this.SendListener(name, o);
    }

    public SoldierControl GetGameObj()
    {
        return control;
    }

    public void LogicAI()
    {
        foreach (var item in aiList)
        {
            item.LogicAI();
        }
    }
}
