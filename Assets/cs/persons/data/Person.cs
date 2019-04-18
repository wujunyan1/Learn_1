using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// 人物的逻辑处理
/// </summary>
public class Person : RoundObject
{
    // 坐标
    HexCoordinates point;
    public HexCoordinates Point {
        get
        {
            return point;
        }
        set
        {
            point = value;
            start = HexGrid.instance.GetCell(point);

            if (Control)
            {
                Control.Location = start;
            }

            if (end != null)
            {
                if (point.Equals(end.coordinates))
                {
                    MoveNextCell();
                }
            }
        }
    }

    public Camp camp;

    public string personName;

    // 速度
    public int speed;

    int defaultSpeed;
    public int DefaultSpeed
    {
        get
        {
            return defaultSpeed;
        }
        set
        {
            defaultSpeed = value;
        }
    }

    public HeroRes res;

    HexDirection dir;
    HexDirection Direction
    {
        get
        {
            return dir;
        }
        set
        {
            dir = value;
            if (Control)
            {
                Control.Direction = dir;
            }
        }
    }

    // 模型
    public PersonControl Control;

    /// <summary>
    /// 功能
    /// </summary>
    protected List<ObjFunction> functions;

    public Person()
    {
        functions = new List<ObjFunction>();
    }

    public virtual void Save(BinaryWriter writer)
    {
        point.Save(writer);

        writer.Write(personName);
        writer.Write((byte)speed);

        writer.Write((byte)res.index);
    }

    public virtual void Load(BinaryReader reader)
    {
        point = HexCoordinates.Load(reader);
        personName = reader.ReadString();
        speed = reader.ReadByte();

        res = HeroConfigPool.GetHeroRes(reader.ReadByte());
    }

    public virtual void ClearData()
    {
        ClearModel();
    }


    public void ClearModel()
    {
        if (Control)
        {
            Control.Clear();
            Control = null;
        }
    }


    public void AddFunction(ObjFunction func)
    {
        func.control = this;
        functions.Add(func);
    }

    public List<ObjFunction> GetActiveFunctions()
    {
        List<ObjFunction> list = new List<ObjFunction>();

        foreach (ObjFunction func in functions)
        {
            if (func.IsActive())
            {
                list.Add(func);
            }
        }

        return list;
    }

    public override void NextRound()
    {
        base.NextRound();

        // 速度回复
        speed = DefaultSpeed;
    }



    /// <summary>
    /// 移动路径
    /// </summary>
    public List<HexCell> Path { get; set; }
    HexCell end;
    HexCell start;

    HexCell End {
        get
        {
            return end;
        }
        set
        {
            end = value;
            Direction = start.Direction(end);
        }
    }

    public HexCell CurrCell
    {
        get
        {
            return start;
        }
    }

    void Move(HexCell end)
    {
        // 数据直接到达下一个点
        point = end.coordinates;
        start = HexGrid.instance.GetCell(point);

        // 模型移动
        if (Control)
        {
            Control.End = start;
        }
    }

    public void MoveNextCell()
    {
        if (Path != null && Path.Count > 0)
        {
            int index = Path.Count - 1;
            End = Path[index];
            Path.RemoveAt(index);

            speed -= start.GetDistanceCost(End);

            Move(End);
        }
        else
        {
            // 模型停止移动
            end = null;
            if (Control)
            {
                Control.End = end;
            }
        }
    }

    List<HexCell> GetMovePath(HexCell moveToCell)
    {
        Debug.Log(string.Format("想要移动到 {2} {0} {1}", moveToCell.coordinates.X, moveToCell.coordinates.Z, moveToCell.index));

        //HexGrid.instance.FindDistancesTo(searchFromCell);

        HexCell previousCell = HexGrid.instance.GetCell(point);
        moveToCell.EnableHighlight(Color.blue);

        return HexGrid.instance.FindPath(previousCell, moveToCell, speed);
    }

    // 想要移动到cell
    public void MoveTo(HexCell moveToCell)
    {
        Path = GetMovePath(moveToCell);
        MoveNextCell();
    }

    public void ShowMovePath(HexCell moveToCell)
    {
        GetMovePath(moveToCell);
    }


    public void Die()
    {
        ClearData();
    }
}
