using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EventComponent : MonoBehaviour
{   
    // 内部类
    private class EventData
    {
        public string eventName;
        public UEventListener.EventListenerDelegate callback;

        public EventData(string eventName, UEventListener.EventListenerDelegate callback)
        {
            this.eventName = eventName;
            this.callback = callback;
        }
    }

    // 记录已注册的事件
    private List<EventData> callbacks;


    public EventComponent()
    {

    }

    private void AddCallback(string eventName, UEventListener.EventListenerDelegate callback)
    {
        if(callbacks == null)
        {
            callbacks = new List<EventData>();
        }

        callbacks.Add(new EventData(eventName, callback));
    }

    private void RemoveCallback(string eventName, UEventListener.EventListenerDelegate callback)
    {
        callbacks.Remove(
            callbacks.Where(p => p.eventName.Equals(eventName) && p.callback.Equals(callback))
            .FirstOrDefault());
    }

    /// <summary>
    /// 全局事件注册方法
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="callback"></param>
    public void RegisterEvent(string eventName, UEventListener.EventListenerDelegate callback)
    {
        ObjectEventDispatcher.dispatcher.addEventListener(eventName, callback);
        this.AddCallback(eventName, callback);
    }

    public void RemoveEvent(string eventName, UEventListener.EventListenerDelegate callback)
    {
        ObjectEventDispatcher.dispatcher.removeEventListener(eventName, callback);
        this.RemoveCallback(eventName, callback);
    }
    
    public void FireEvent(string eventName, object obj = null)
    {
        ObjectEventDispatcher.dispatcher.dispatchEvent(new UEvent(eventName, obj), this);
    }


    /**
     * 
     * 
     * 
     * 
     */

    private UEventDispatcher dispatcher;

    /// <summary>
    /// 传递事件注册方法
    /// </summary>
    public void RegisterMessage(string messageName, UEventListener.EventListenerDelegate callback)
    {
        if(dispatcher == null)
        {
            dispatcher = new UEventDispatcher();
        }

        dispatcher.addEventListener(messageName, callback);
    }

    public void RemoveMessage(string messageName, UEventListener.EventListenerDelegate callback)
    {
        if(dispatcher != null)
        {
            dispatcher.removeEventListener(messageName, callback);
        }
    }

    public void FireMessage(string messageName, EventComponent target = null, object obj = null)
    {

    }

    /// <summary>
    /// 向上传递信息
    /// </summary>
    /// <param name="messageName"></param>
    /// <param name="target"></param>
    /// <param name="obj"></param>
    public void FireUpMessage(string messageName, EventComponent target = null, object obj = null)
    {
        if(target == null)
        {
            target = this;
        }
        UEvent e = new UEvent(messageName, obj);

        EventComponent currTarget = target;
        // 判断目标 是否有
        if (currTarget.dispatcher.hasListener(messageName))
        {
            // 执行事件
            currTarget.dispatcher.dispatchEvent(e, this);

            EventComponent[] events = currTarget.GetComponentsInParent<EventComponent>();
            foreach(EventComponent parentE in events)
            {
                if (parentE.dispatcher.hasListener(messageName))
                {
                    // 执行事件
                    parentE.dispatcher.dispatchEvent(e, this);
                }
            }
            
        }


    }

    /// <summary>
    /// 向下传递信息
    /// </summary>
    /// <param name="messageName"></param>
    /// <param name="target"></param>
    /// <param name="obj"></param>
    public void FireDownMessage(string messageName, EventComponent target = null, object obj = null)
    {
    }





    public void RegisterListener(string eventName, UEventListener.EventListenerDelegate callback)
    {
        if (dispatcher == null)
        {
            dispatcher = new UEventDispatcher();
        }
        dispatcher.addEventListener(eventName, callback);
    }

    public void RemoveListener(string eventName, UEventListener.EventListenerDelegate callback)
    {
        if (dispatcher == null)
        {
            return;
        }

        dispatcher.removeEventListener(eventName, callback);
    }

    public void SendListener(string eventName, object obj = null)
    {
        if (dispatcher == null)
        {
            return;
        }
        dispatcher.dispatchEvent(new UEvent(eventName, obj), this);
    }




    protected virtual void DestroyEnd()
    {

    }

    private void OnDestroy()
    {
        if(callbacks == null)
        {
            DestroyEnd();
            return;
        }
        foreach(EventData e in callbacks)
        {
            ObjectEventDispatcher.dispatcher.removeEventListener(e.eventName, e.callback);
        }

        callbacks.Clear();
        DestroyEnd();
    }
}
