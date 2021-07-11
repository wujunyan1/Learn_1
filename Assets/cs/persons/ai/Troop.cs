using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Troop : EventObject, RoundObject, SaveLoadInterface
{
    public Camp camp {
        get
        {
            return data.camp;
        }
        set
        {
            data.camp = value;
        }
    }
    public TroopsData data;
    public TroopControl control;

    int _cellIndex;
    public int CellIndex
    {
        get
        {
            return _cellIndex;
        }
        set
        {
            _cellIndex = value;
        }
    }

    /// <summary>
    /// 功能列表（状态型，劫掠，建城等）
    /// </summary>
    List<ObjFunction> funcList;

    /// <summary>
    /// 当前执行的状态
    /// </summary>
    int currObjFunctionIndex = 0;

    /// <summary>
    /// 执行的回合数
    /// </summary>
    int funcRound = 0;
    public int FuncRound
    {
        get
        {
            return funcRound;
        }
    }


    /// <summary>
    /// 军粮
    /// </summary>
    public int provisions;

    /// <summary>
    /// 物资
    /// </summary>
    public int materials;

    public Troop()
    {
        data = new TroopsData();
        data.SetConfig(TroopsConfigDataManager.GetConfig(0));

        funcList = data.TroopsType.GetFuncList();
        Debug.Log(funcList.Count);
    }

    public void IsNew()
    {
        data.IsNewObj();
    }
    
    public void Save(BinaryWriter writer)
    {
        // 位置
        //int hexCellIndex = control.Location.index;
        writer.Write(CellIndex);

        // 阵营
        //writer.Write(camp.GetId());

        data.Save(writer);

        writer.Write((byte)currObjFunctionIndex);
        writer.Write(funcRound);
    }

    public IEnumerator Load(BinaryReader reader)
    {
        CellIndex = reader.ReadInt32();
        
        IEnumerator itor = data.Load(reader);
        while (itor.MoveNext())
        {
            yield return null;
        }

        currObjFunctionIndex = reader.ReadByte();
        funcRound = reader.ReadInt32();

        yield return null;

        funcList = data.TroopsType.GetFuncList();
    }



    public void NextRound()
    {
        funcRound++;

        // 状态执行
        ObjFunction objFunction = GetCurrObjFunctions();
        objFunction.Execute(this);


        _EatFood();


        // 每回合重置
        data.ReSetRound();
    }

    public void LaterNextRound()
    {

    }


    public void MoveToHexCell(HexCell cell, FuncAction.Func func = null)
    {
        List<HexCell> path = HexGrid.instance.FindPath(control.Location, cell, data.speed);
        control.MovePath(path, func);
    }
    


    public List<ObjFunction> GetObjFunctions()
    {
        return funcList;
    }

    public ObjFunction GetCurrObjFunctions()
    {
        return funcList[currObjFunctionIndex];
    }

    public T GetObjFunction<T>() where T : ObjFunction
    {
        foreach (var func in funcList)
        {
            if (func is T)
            {
                return (T)func;
            }
        }

        return null;
    }

    public void SetCurrObjFunction<T>() where T : ObjFunction
    {
        // 没变
        if(funcList[currObjFunctionIndex] is T)
        {
            return;
        }


        for (int i = 0; i < funcList.Count; i++)
        {
            ObjFunction func = funcList[i];
            if (func is T)
            {
                SetCurrObjFunctionStatus(i);
                return;
            }
        }
    }

    void SetCurrObjFunctionStatus(int index)
    {
        currObjFunctionIndex = index;
        funcRound = 0;

        control.UpdateCurrFuncStatus();
    }



    


    


    void _EatFood()
    {
        int food = data.EatFood();
        provisions -= (food * data.TroopNum );
    }

    
}
