using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAi : EventObject
{
    protected AIControl aiControl;

    protected Dictionary<string, UEventListener.EventListenerDelegate> listener = null;

    private bool active = true;

    public BaseAi()
    {
    }

    /// <summary>
    /// 清除监听
    /// </summary>
    ~BaseAi()
    {
        if (listener == null)
        {
            return;
        }

        foreach (var item in listener)
        {
            aiControl.RemoveListener(item.Key, item.Value);
        }

        listener.Clear();
    }

    public void SetAIControl(AIControl control)
    {
        aiControl = control;

        InitializeListener();
        StartAI();
    }

    /// <summary>
    /// 初始化监听
    /// </summary>
    private void InitializeListener()
    {
        if(listener == null)
        {
            return;
        }

        foreach (var item in listener)
        {
            aiControl.RegisterListener(item.Key, item.Value);
        }
    }

    protected virtual void StartAI()
    {

    }

    public virtual void LogicAI()
    {

    }

    public void SetActive(bool isActive)
    {
        this.active = isActive;
    }

    public bool isActive()
    {
        return active;
    }
}
