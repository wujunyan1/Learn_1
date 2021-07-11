using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampControl : BaseControl
{
    // 所有的队伍
    public List<TeamControl> controlList;

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        foreach(TeamControl control in controlList)
        {
            control.LogicUpdate();
        }
    }
}
