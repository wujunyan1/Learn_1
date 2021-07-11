using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : EventComponent
{
    protected bool isCanClose = true;
    public string Name{get;set;}

    public View()
    {
    }

    public bool CanClose()
    {
        return isCanClose;
    }
    
    /// <summary>
    /// 重新显示
    /// </summary>
    public virtual void Show()
    {

    }

    public virtual void Open(UObject o)
    {
        
    }

    public virtual void Close()
    {
        ViewManager.Instance.RemoveView(this);
    }

    public virtual void UpdateView()
    {
    }
}
