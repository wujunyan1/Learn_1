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
    string name;

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
        foreach (City city in citys)
        {
            city.Save(writer);
        }
    }

    public void Load(BinaryReader reader)
    {
        foreach (City city in citys)
        {
            city.Load(reader);
        }
    }

    public Creater CreatCreater(Point point)
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
                return CreatCreater(new Point(x, y));
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
