using System;

public class UEvent
{
    /// <summary>
    /// 事件类别
    /// </summary>
    public string eventType;

    /// <summary>
    /// 参数
    /// </summary>
    public object eventParams;

    /// <summary>
    /// 事件抛出者
    /// </summary>
    public object target;

    /// <summary>
    /// 事件是否停止
    /// </summary>
    public bool isStop = false;

    public UEvent(string eventType, object eventParams = null)
    {
        this.eventType = eventType;
        this.eventParams = eventParams;
        this.isStop = false;
    }

    public UEvent(string eventType, params object[] eventParams)
    {
        this.eventType = eventType;
        this.eventParams = eventParams;
        this.isStop = false;
    }
}