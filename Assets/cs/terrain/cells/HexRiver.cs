using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class OneRiver
{
    public HexCell cell {
        get;
    }
    public HexDirection dir;

    public OneRiver(HexCell cell)
    {
        this.cell = cell;
    }
}

public class HexRiver : SaveLoadInterface
{
    LinkedList<OneRiver> oneRivers;
    string name;

    public HexRiver()
    {
        name = "河";
        oneRivers = new LinkedList<OneRiver>();
    }

    public HexRiver(HexCell origin)
    {
        name = "河";
        oneRivers = new LinkedList<OneRiver>();

        OneRiver one = new OneRiver(origin);

        oneRivers.AddFirst(new LinkedListNode<OneRiver>(one));
    }

    public void AddOneRiver(HexCell next)
    {
        OneRiver one = new OneRiver(next);

        LinkedListNode<OneRiver> node = new LinkedListNode<OneRiver>(one);

        oneRivers.AddLast(node);

        OneRiver befor = node.Previous.Value;

        HexDirection dir = befor.cell.Direction(next);
        befor.dir = dir;
    }

    public void AddOneRiver(HexCell next, HexDirection dir)
    {
        OneRiver one = new OneRiver(next);

        LinkedListNode<OneRiver> node = new LinkedListNode<OneRiver>(one);

        oneRivers.AddLast(node);

        OneRiver befor = node.Previous.Value;
        befor.dir = dir;
    }

    public bool isOrigin(HexCell cell)
    {
        OneRiver origin = oneRivers.First.Value;
        if (cell.index == origin.cell.index)
        {
            return true;
        }

        return false;
    }

    public bool isInRiver(HexCell cell)
    {
        foreach (var item in oneRivers)
        {
            if(item.cell.index == cell.index)
            {
                return true;
            }
        }

        return false;
    }

    public void AddHexRiverr(HexRiver lastRiver)
    {
        OneRiver lastOneRiver = oneRivers.Last.Value;
        LinkedListNode<OneRiver> nextOneRiver = lastRiver.oneRivers.First;

        // 设置原来的最后一个的方向
        lastOneRiver.dir = lastOneRiver.cell.Direction(nextOneRiver.Value.cell);

        oneRivers.AddLast(nextOneRiver);

        // 其他重新添加就好
        while (nextOneRiver.Next != null)
        {
            nextOneRiver = nextOneRiver.Next;
            oneRivers.AddLast(nextOneRiver);
        }
       
    }


    public void Save(BinaryWriter writer)
    {
        writer.Write(name);

        writer.Write(oneRivers.Count);
        foreach (var item in oneRivers)
        {
            writer.Write(item.cell.index);
            writer.Write((byte)item.dir);
        }
    }

    public IEnumerator Load(BinaryReader reader)
    {
        name = reader.ReadString();

        oneRivers.Clear();

        HexGrid grid = HexGrid.instance;

        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            int cellIndex = reader.ReadInt32();
            HexDirection dir = (HexDirection)reader.ReadByte();

            OneRiver oneRiver = new OneRiver(grid.GetCell(cellIndex));
            oneRiver.dir = dir;

            oneRivers.AddLast(oneRiver);
        }

        yield return null;
    }
}


