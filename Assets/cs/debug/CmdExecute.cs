using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CmdExecute
{
    public static void Execute(string cmd)
    {
        string[] strs = cmd.Split(' ');
        string cmdHead = strs[0];

        List<string> cmds = new List<string>(strs);
        cmds.RemoveAt(0);

        switch (cmdHead)
        {
            case "FireEvent":
                FireEvent(cmds);
                break;
            case "Add":
                AddResourse(cmds);
                break;
            default:
                break;
        }
    }

    static void FireEvent(List<string> args)
    {

    }

    static void AddResourse(List<string> args)
    {
        string objId = args[0];
        args.RemoveAt(0);

        AddObj.GetObj(objId, args);
    }

    

}
