using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// 阵容
/// </summary>
public class Camp : RoundObject, SaveLoadInterface
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
    Color32 color;

    List<CampRelation> relations;

    // 拥有的城市
    List<City> citys;
    

    List<Troop> troops;


    List<Troop> delTroops;

    // 金钱
    int money;

    /// <summary>
    /// 税率 每个人给多少钱
    /// </summary>
    float personTax = 0.01f;

    /// <summary>
    /// 农业税
    /// </summary>
    float agriculturalTax = 0.01f;

    /// <summary>
    /// 商业税
    /// </summary>
    float businessTax = 0.01f;

    public Camp(int id)
    {
        this.id = id;

        citys = new List<City>();
        troops = ListPool<Troop>.Get();
        relations = new List<CampRelation>();

        delTroops = ListPool<Troop>.Get();
    }

    public void NextRound()
    {
        // 移到HexCell中 这里只是一个回合时间内的资源变化，无任何AI
        //foreach(City city in citys)
        //{
        //    city.NextRound();
        //}

        foreach (Troop troop in troops)
        {
            troop.NextRound();
        }
    }

    public void LaterNextRound()
    {
        foreach (var troop in delTroops)
        {
            RemoveTroop(troop);
        }
    }

    public void Save(BinaryWriter writer)
    {

        //writer.Write((byte)creaters.Count);
        //foreach (Creater creater in creaters)
        //{
        //    creater.Save(writer);
        //}

        //writer.Write((byte)citys.Count);
        //foreach (City city in citys)
        //{
        //    city.Save(writer);
        //}

        writer.Write(troops.Count);
        foreach (var troop in troops)
        {
            troop.Save(writer);
        }
    }

    public IEnumerator Load(BinaryReader reader)
    {
        GameCenter center = GameCenter.instance;
        HexGrid grid = HexGrid.instance;

        IEnumerator itor;

        //int count = reader.ReadByte();
        //for (int i = 0; i < count; i++)
        //{
        //    City city = CreateCity();

        //    itor = city.Load(reader);
        //    while (itor.MoveNext())
        //    {
        //        yield return null;
        //    }
        //}


        int count = reader.ReadInt32();

        for (int i = 0; i < count; i++)
        {
            // 加载数据
            Troop troop = center.GetNewTroop();
            itor = troop.Load(reader);
            while (itor.MoveNext())
            {
                yield return null;
            }


            // 生成模型
            TroopControl troopControl = center.GenerateTroopModel(troop);

            Debug.Log(troop.CellIndex);
            // 设置位置
            HexCell cell = grid.GetCell(troop.CellIndex);
            troopControl.Location = cell;
            cell.Troop = troop;

            AddTroop(troop);
        }
        

        yield return null;
    }

    public void ClearData()
    {
        foreach (City city in citys)
        {
            city.Clear();
        }
        citys.Clear();

        foreach (Troop troop in troops)
        {
            GameObject.Destroy(troop.control);
        }
        troops.Clear();
    }
    
    public void GenerateInitialData()
    {
        HexGrid grid = HexGrid.instance;

        HexCell cell;

        int whileNum = 0;
        while (true)
        {
            whileNum++;

            cell = grid.GetRandomLand();
            if (cell.Troop == null)
            {
                whileNum++;

                // 初始地点附近没有人物
                List<HexCell> cells = grid.GetNearCell(cell, 3);
                bool nearNull = true;
                foreach (var c in cells)
                {
                    if (c != null && c.Troop != null)
                    {
                        nearNull = false;
                        break;
                    }
                }
                
                if (nearNull || whileNum > 10000)
                {
                    break;
                }
            }
        }

        Troop troop = GameCenter.instance.GenerateNewTroop(cell);
        
        troop.data.SetConfig(TroopsConfigDataManager.GetConfigBySoliderName("开荒队"));

        AddTroop(troop);
    }
    
    public bool GenerateNewTroop(int troopKey, City city)
    {
        HexCell cell = city.GetCenter();
        List<HexCell> cells = city.GetHexCells();
        int i = 0;
        while(cell != null && cell.Troop != null)
        {
            cell = i >= cells.Count ? null : cells[i++];
        }

        if(cell == null)
        {
            return false;
        }

        GameCenter gameCenter = GameCenter.instance;
        Troop troop = gameCenter.GetNewTroop();
        troop.data.SetConfig(TroopsConfigDataManager.GetConfig(troopKey));

        troop = gameCenter.GenerateNewTroop(troop, cell);
        AddTroop(troop);
        return true;
    }

    public void AddTroop(Troop person)
    {
        if(person.camp != null)
        {
            person.camp.RemoveTroop(person);
        }

        person.camp = this;
        troops.Add(person);
        person.control.IncreaseVisibility();
    }

    
    public void RemoveTroop(Troop person)
    {
        person.control.DecreaseVisibility();
        troops.Remove(person);
        person.camp = null;
    }

    public void LaterRemoveTroop(Troop person)
    {
        delTroops.Add(person);
    }

    public Troop GetTroop(int i)
    {
        return troops[i];
    }


    public void AddCity(City city)
    {
        if (city.camp != null)
        {
            city.camp.RemoveCity(city);
        }

        city.camp = this;
        citys.Add(city);
        city.IncreaseVisibility();
    }


    public void RemoveCity(City city)
    {
        city.DecreaseVisibility();
        citys.Remove(city);
        city.camp = null;
    }


    public City GetCity(int index)
    {
        if(index >= citys.Count)
        {
            return null;
        }
        return citys[index];
    }
    
    public City CreateCity()
    {
        City city = GameCenter.instance.GenerateNewCity();
        city.camp = this;
        citys.Add(city);

        return city;
    }

    public City CreateNewCity(int cellIndex, string name)
    {
        HexCell cell = HexGrid.instance.GetCell(cellIndex);
        City city = GameCenter.instance.GenerateNewCity();
        city.camp = this;
        citys.Add(city);
        city.Location = cell;

        cell.FeatureType = HexFeatureType.NULL;
        cell.RefreshOnlySelf();

        city.Init();

        return city;
    }
    
    public int GetId()
    {
        return id;
    }

    // 重置队伍的观察 范围
    public void ResetTroopObserveVisibility()
    {
        if (HasPlayerVisibility())
        {
            HexGrid grid = HexGrid.instance;
            for (int i = 0; i < troops.Count; i++)
            {
                Troop unit = troops[i];
                grid.IncreaseVisibility(unit.control.Location, unit.data.visionRange);
            }
        }
    }



    /// <summary>
    /// 是否有可见度， 当前仅玩家阵营才有
    /// </summary>
    /// <returns></returns>
    public bool HasPlayerVisibility()
    {
        return IsPlayerCamp();
    }

    public bool IsPlayerCamp()
    {
        return id == GameCenter.instance.PlayerCampId;
    }



    /// <summary>
    /// 税率 每个人给多少钱
    /// </summary>
    public float GetPersonTax()
    {
        return personTax;
    }

    /// <summary>
    /// 农业税
    /// </summary>
    public float GetAgriculturalTax()
    {
        return agriculturalTax;
    }

    /// <summary>
    /// 商业税
    /// </summary>
    public float GetBusinessTax()
    {
        return businessTax;
    }




}
