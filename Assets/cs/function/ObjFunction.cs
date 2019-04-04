using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 功能
/// </summary>
public class ObjFunction : Object
{
    protected string name;
    public string Name
    {
        get
        {
            return name;
        }
    }

    public PersonControl control;

    /// <summary>
    /// 这个功能是否可用
    /// </summary>
    /// <returns></returns>
    public virtual bool IsActive()
    {
        return false;
    }

    public virtual Button GetFunctionBtn()
    {
        return null;
    }
}
