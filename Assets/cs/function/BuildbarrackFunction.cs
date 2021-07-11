using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 建造简易营寨
/// </summary>
public class BuildbarrackFunction : ObjFunction
{
    public BuildbarrackFunction()
    {
        _type = FunctionType.BuildBarrack;
        name = "buildBarrack";
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
