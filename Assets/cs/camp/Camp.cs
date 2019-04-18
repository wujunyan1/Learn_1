using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// 阵容
/// </summary>
public class Camp : RoundObject
{
    // id
    int id;

    // 名称
    string campName;
    public string CampName
    {
        get
        {
            return campName;
        }
    }

    // 颜色
    Color color;

    // 拥有的城市
    List<City> citys;

    // 建造者
    List<Creater> creaters;

    // 金钱
    int money;

    public Camp(int id)
    {
        this.id = id;

        citys = new List<City>();
        creaters = new List<Creater>();

        AddCreater(RandomCreatCreater());
    }

    public override void NextRound()
    {
        foreach(City city in citys)
        {
            city.NextRound();
        }
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write((byte)citys.Count);
        foreach (City city in citys)
        {
            city.Save(writer);
        }

        writer.Write((byte)creaters.Count);
        foreach (Creater creater in creaters)
        {
            creater.Save(writer);
        }
    }

    public void Load(BinaryReader reader)
    {
        ObjGenerate objGenerate = ObjGenerate.instance;
        
        int count = reader.ReadByte();
        for (int i = 0; i < count; i++)
        {
            City city = objGenerate.CreateCity();
            citys.Add(city);
            city.Load(reader);
        }

        int createrCount = reader.ReadByte();
        for (int i = 0; i < count; i++)
        {
            Creater creater = new Creater();
            creaters.Add(creater);
            creater.Load(reader);
        }
    }

    public void ClearData()
    {
        foreach (City city in citys)
        {
            city.ClearData();
            Destroy(city);
        }

        citys = new List<City>();

        foreach (Creater creater in creaters)
        {
            creater.ClearData();
        }
        creaters = new List<Creater>();
    }

    public Creater CreatCreater(HexCoordinates point)
    {
        Creater creater = new Creater(point);
        return creater;
    }

    public Creater RandomCreatCreater()
    {
        HexGrid grid = GameCenter.instance.grid;
        int maxX = grid.cellCountX;
        int maxY = grid.cellCountZ;

        while (true)
        {
            int x = Random.Range(0, maxX);
            int y = Random.Range(0, maxY);

            HexCell cell = grid.GetCell(x, y);
            if(cell.TerrainType != HexTerrainType.Water && cell.TerrainType != HexTerrainType.Ridge)
            {
                return CreatCreater(new HexCoordinates(x, y));
            }
        }
    }

    public void AddCreater(Creater creater)
    {
        creater.camp = this;
        creaters.Add(creater);
        GameCenter.instance.GenerateCreater(creater);
    }

    public City GetCity(int index)
    {
        if(index >= citys.Count)
        {
            return null;
        }
        return citys[index];
    }

    public Creater GetCreater(int index)
    {
        return creaters[index];
    }
}
