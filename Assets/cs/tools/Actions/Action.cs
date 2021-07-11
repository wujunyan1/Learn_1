using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Action
{
    void SetTarget(ActionControl obj);

    void Execute(ActionControl obj, float _time);

    bool IsEnd();
}

public class MoveToAction : Action
{
    private Vector3 moveToTarget;
    private float time;
    private bool isOver;
    

    public MoveToAction(Vector3 _target, float _time)
    {
        time = _time;
        moveToTarget = _target;
        isOver = false;
    }

    public void Execute(ActionControl obj, float _time)
    {
        if(time < _time)
        {
            obj.transform.localPosition = moveToTarget;
            isOver = true;
            return;
        }

        Vector3 dir = moveToTarget - obj.transform.localPosition;
        float dis = dir.magnitude;

        Vector3 move = dir.normalized * dis * _time / time;
        time -= _time;

        obj.transform.localPosition += move;
    }

    public bool IsEnd()
    {
        return isOver;
    }

    public void SetTarget(ActionControl obj)
    {
        
    }
}

public class MoveByAction : Action
{
    private Vector3 moveByTarget;
    // 单位时间移动距离
    private Vector3 _time_move;
    private float time;
    private bool isOver;

    public MoveByAction(Vector3 _target, float _time)
    {
        time = _time;
        moveByTarget = _target;
        isOver = false;

        if(_time == 0)
        {
            time = -1;
        }
        _time_move = moveByTarget.normalized / time;
    }

    public void Execute(ActionControl obj, float _time)
    {
        if (time < _time)
        {
            obj.transform.localPosition += moveByTarget;
            isOver = true;
            return;
        }
        
        Vector3 move = _time_move * _time;
        time -= _time;
        moveByTarget -= move;

        obj.transform.localPosition += move;
    }

    public bool IsEnd()
    {
        return isOver;
    }

    public void SetTarget(ActionControl obj)
    {
        
    }
}

public class QueueAction : Action
{
    ActionControl control = null;
    List<Action> actions;
    int index;

    public QueueAction()
    {
        actions = new List<Action>();
        index = 0;
    }

    public QueueAction(params Action[] actionStack) : this()
    {
        for (int i = 0; i < actionStack.Length; i++)
        {
            AddAction(actionStack[i]);
        }
    }

    public void AddAction(Action action)
    {
        actions.Add(action);
        if(control != null)
        {
            action.SetTarget(control);
        }
    }
    

    public void Execute(ActionControl obj, float _time)
    {
        Action action = actions[index];

        action.Execute(obj, _time);

        if (action.IsEnd())
        {
            index++;
        }
    }

    public bool IsEnd()
    {
        return index >= actions.Count;
    }

    public void SetTarget(ActionControl obj)
    {
        control = obj;
        SetActionsTarget();
    }

    private void SetActionsTarget()
    {
        foreach (var item in actions)
        {
            item.SetTarget(control);
        }
    }
}

public class FuncAction : Action
{
    public delegate void Func();

    Func func;
    bool isOver = false;

    public FuncAction(Func func)
    {
        this.func = func;
        isOver = false;
    }

    public void Execute(ActionControl obj, float _time)
    {
        func();
        isOver = true;
    }

    public bool IsEnd()
    {
        return isOver;
    }

    public void SetTarget(ActionControl obj)
    {
        
    }
}

public class RotateToAction : Action
{
    private Vector3 rotate;
    private float time;
    private bool isOver;

    public RotateToAction(Vector3 rotate, float time)
    {
        this.rotate = rotate;
        this.time = time;
        isOver = false;
    }

    public void Execute(ActionControl obj, float _time)
    {
        if (time < _time)
        {
            obj.transform.rotation = Quaternion.Euler(rotate);
            isOver = true;
            return;
        }

        Quaternion rotation = obj.transform.rotation;
        Vector3 angle = rotation.eulerAngles;

        Vector3 difference = rotate - angle;
        Vector3 move = difference * _time / time;
        time -= _time;

        rotation.eulerAngles += move;
        obj.transform.rotation = rotation;
    }

    public bool IsEnd()
    {
        return isOver;
    }

    public void SetTarget(ActionControl obj)
    {
        
    }
}

public class RotateByAction : Action
{
    private Vector3 rotate;
    // 单位时间移动距离
    private Vector3 _time_rotate;

    private float time;
    private bool isOver;

    public RotateByAction(Vector3 rotate, float _time)
    {
        this.rotate = rotate;
        this.time = _time;
        isOver = false;

        if (_time == 0)
        {
            time = -1;
        }
        _time_rotate = rotate.normalized / time;
    }

    public void Execute(ActionControl obj, float _time)
    {
        Quaternion rotation = obj.transform.rotation;
        if (time < _time)
        {
            rotation.eulerAngles += rotate;
            isOver = true;
            return;
        }

        Vector3 move = _time_rotate * _time;
        time -= _time;
        rotate -= move;

        rotation.eulerAngles += move;
    }

    public bool IsEnd()
    {
        return isOver;
    }

    public void SetTarget(ActionControl obj)
    {
        
    }
}

public class DelayAction : Action
{
    private float time;
    private bool isOver;

    public DelayAction(float _time)
    {
        time = _time;
    }

    public void Execute(ActionControl obj, float _time)
    {
        time -= _time;
        if(time <= 0)
        {
            isOver = true;
        }
    }

    public bool IsEnd()
    {
        return isOver;
    }

    public void SetTarget(ActionControl obj)
    {
        
    }
}