using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 功能
/// </summary>
public class ObjFunction
{
    private static List<ObjFunction> objFunctions = new List<ObjFunction>() {
        new BuildbarrackFunction(),
        new NormalFunction(),
        new PlunderFunction(),
        new BuildCityFunction(),
    };

    public static T GetFunction<T>() where T : ObjFunction, new()
    {
        foreach (var func in objFunctions)
        {
            if (func is T)
            {
                return (T)func;
            }
        }

        T new_func = new T();
        objFunctions.Add(new_func);

        return new_func;
    }


    public enum FunctionType
    {
        Normal,
        BuildBarrack,
        BuildCity,
        Plunder,
    }

    protected FunctionType _type;
    public FunctionType Type
    {
        get
        {
            return _type;
        }
    }

    protected string name;
    public string Name
    {
        get
        {
            return name;
        }
    }

    /// <summary>
    /// 这个功能是否可用
    /// </summary>
    /// <returns></returns>
    public virtual bool IsActive(Troop troop)
    {
        return false;
    }

    /// <summary>
    /// 执行，每回合执行一遍 nextRound时执行
    /// </summary>
    /// <param name="troop"></param>
    /// <returns></returns>
    public virtual bool Execute(Troop troop)
    {
        return false;
    }

    /// <summary>
    /// 是否正在执行
    /// </summary>
    /// <param name="troop"></param>
    /// <returns></returns>
    public virtual bool IsExecute(Troop troop)
    {
        ObjFunction func = troop.GetCurrObjFunctions();
        return func.Type == _type;
    }
    

    public virtual void UpdateStatusView(Troop troop, TroopControl troopControl)
    {

    }
}
