using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// 城市
public class City : MapBuild
{
    string name;
    public string Name
    {
        get
        {
            return name;
        }
    }

    // 人口
    int persons;

    /// <summary>
    /// 城市内建筑
    /// </summary>
    List<CityBuild> builds;

    /// <summary>
    /// 城市范围
    /// </summary>
    List<HexCell> cells;

    public City()
    {
        name = "111";
        buildType = BuildType.City;
        builds = new List<CityBuild>();
        cells = new List<HexCell>();
    }

    public City(HexCell cell, string name)
    {
        this.name = name;
        buildType = BuildType.City;
        builds = new List<CityBuild>();
        cells = new List<HexCell>();

        AddHexCell(cell);
        for (HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++)
        {
            HexCell neighbor = cell.GetNeighbor(dir);
            if (neighbor)
            {
                AddHexCell(neighbor);
            }
        }

        CityBuildFactory.CreateCityBuild(this, CityBuildType.Barracks);
    }

    public override void SaveData(BinaryWriter writer)
    {
        writer.Write(name);
        writer.Write(persons);

        writer.Write((byte)builds.Count);
        foreach (CityBuild build in builds)
        {
            build.Save(writer);
        }

        writer.Write((byte)cells.Count);
        foreach (HexCell cell in cells)
        {
            writer.Write(cell.index);
        }
    }

    public override void LoadData(BinaryReader reader)
    {
        name = reader.ReadString();
        persons = reader.ReadInt32();

        int buildNum = reader.ReadByte();
        for (int i = 0; i < buildNum; i++)
        {
            CityBuild build = CityBuildFactory.CreateCityBuild(this, (CityBuildType)reader.ReadByte());
            build.Load(reader);
        }

        int cellNum = reader.ReadByte();
        for (int i = 0; i < cellNum; i++)
        {
            int cellIndex = reader.ReadInt32();
            HexCell cell = HexGrid.instance.GetCell(cellIndex);
            cells.Add(cell);
            cell.city = this;
        }

        HexCell currCell = HexGrid.instance.GetCell(point);
        currCell.Build = this;
    }

    // 下一回合
    public override void NextRound()
    {
        // 人口变化

        foreach (CityBuild build in builds)
        {
            build.NextRound();
        }
    }

    public void AddHexCell(HexCell cell)
    {
        cells.Add(cell);
    }

    public List<CityBuild> GetBuilds()
    {
        return builds;
    }

    public void AddBuilds(CityBuild build)
    {
        builds.Add(build);
    }

    public void RemoveBuilds(int index)
    {
        builds.RemoveAt(index);
    }

    public T GetBuild<T>() where T : CityBuild
    {
        T t = default;
        foreach (CityBuild build in builds)
        {
            if(build.GetType() == typeof(T))
            {
                t = (T)build;
            }
        }

        return t;
    }
}
