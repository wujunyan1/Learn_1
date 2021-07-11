using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EventObject
{
    // 内部类
    protected class EventData
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

    // 仅记录在这个对象上的事件
    private UEventDispatcher dispatcher = null;

    public EventObject()
    {

    }

    ~EventObject()
    {
        if (callbacks == null)
        {
            return;
        }
        foreach (EventData e in callbacks)
        {
            ObjectEventDispatcher.dispatcher.removeEventListener(e.eventName, e.callback);
        }

        callbacks.Clear();
    }

    private void AddCallback(string eventName, UEventListener.EventListenerDelegate callback)
    {
        if (callbacks == null)
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



    public void RegisterListener(string eventName, UEventListener.EventListenerDelegate callback)
    {
        if(dispatcher == null)
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
}
