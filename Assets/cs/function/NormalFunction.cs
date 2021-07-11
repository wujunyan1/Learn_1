using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 普通状态
/// </summary>
public class NormalFunction : ObjFunction
{
    public NormalFunction()
    {
        _type = FunctionType.Normal;
        name = "normal";
    }

    public override bool IsActive(Troop troop)
    {
        return true;
    }

    public override bool Execute(Troop troop)
    {
        return base.Execute(troop);
    }

    public override bool IsExecute(Troop troop)
    {
        return base.IsExecute(troop);
    }
}
