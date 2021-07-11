using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 劫掠
/// </summary>
public class PlunderFunction : ObjFunction
{
    public PlunderFunction()
    {
        _type = FunctionType.Plunder;
        name = "plunder";
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
