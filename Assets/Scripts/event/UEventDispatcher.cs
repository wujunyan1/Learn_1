using System;
using System.Collections.Generic;

public class UEventDispatcher
{
    protected Dictionary<string, UEventListener> eventListenerDict;

    public UEventDispatcher()
    {
        this.eventListenerDict = new Dictionary<string, UEventListener>();
    }

    /// <summary>
    /// 侦听事件
    /// </summary>
    /// <param name="eventType">事件类别</param>
    /// <param name="callback">回调函数</param>
    public void addEventListener(string eventType, UEventListener.EventListenerDelegate callback)
    {
        if (!this.eventListenerDict.ContainsKey(eventType))
        {
            this.eventListenerDict.Add(eventType, new UEventListener());
        }
        this.eventListenerDict[eventType].OnEvent += callback;
    }

    /// <summary>
    /// 移除事件
    /// </summary>
    /// <param name="eventType">事件类别</param>
    /// <param name="callback">回调函数</param>
    public void removeEventListener(string eventType, UEventListener.EventListenerDelegate callback)
    {
        if (this.eventListenerDict.ContainsKey(eventType))
        {
            this.eventListenerDict[eventType].OnEvent -= callback;
        }
    }

    /// <summary>
    /// 发送事件
    /// </summary>
    /// <param name="evt">Evt.</param>
    /// <param name="gameObject">Game object.</param>
    public void dispatchEvent(UEvent evt, object gameObject)
    {
        UEventListener listener;
        if(eventListenerDict.TryGetValue(evt.eventType, out listener))
        {
            evt.target = gameObject;
            listener.Excute(evt);
        }
    }

    /// <summary>
    /// 是否存在事件
    /// </summary>
    /// <returns><c>true</c>, if listener was hased, <c>false</c> otherwise.</returns>
    /// <param name="eventType">Event type.</param>
    public bool hasListener(string eventType)
    {
        return this.eventListenerDict.ContainsKey(eventType);
    }
}