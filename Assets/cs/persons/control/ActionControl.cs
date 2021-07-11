using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionControl : EventComponent
{
    ////////////////////////////////////////////////////////////////

    private const int add_action_num = 4;
    private Action[] actions;
    private int un_action_index;


    public ActionControl()
    {
        actions = new Action[4];
        un_action_index = 0;

        for (int i = 0; i < actions.Length; i++)
        {
            actions[i] = null;
        }
    }

    private int _addAction(Action action)
    {
        int index = un_action_index;
        action.SetTarget(this);
        actions[index] = action;

        un_action_index++;

        for (int i = un_action_index; i < actions.Length; un_action_index++)
        {
            if(actions[un_action_index] == null)
            {
                break;
            }
        }

        if(un_action_index >= actions.Length)
        {
            Action[] n_as = new Action[actions.Length + add_action_num];
            actions.CopyTo(n_as, 0);

            actions = n_as;
        }

        return index;
    }

    private void _remove_action(int index)
    {
        actions[index] = null;
    }

    public int RunAction(Action action)
    {
        int index = _addAction(action);

        _start_run_action(index);

        return index;
    }

    public void StopAction(int index)
    {
        _remove_action(index);
    }

    public void StopAllActions()
    {
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i] = null;
        }

        StopAllCoroutines();
    }

    private void _start_run_action(int index)
    {
        StartCoroutine(_run_action(index));
    }

    private IEnumerator _run_action(int index)
    {
        while (_can_run_action(index))
        {
            Action action = actions[index];
            action.Execute(this, Time.deltaTime);
            yield return null;
        }

    }

    private bool _can_run_action(int index)
    {
        Action action = actions[index];
        if(action == null)
        {
            return false;
        }

        if (action.IsEnd())
        {
            _remove_action(index);
            return false;
        }

        return true;
    }


    protected override void DestroyEnd()
    {
        StopAllActions();
    }
    ////////////////////////////////////////////////////////////////

}
