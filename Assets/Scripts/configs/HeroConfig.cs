using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;

public class HeroConfig
{
    static HeroConfig instance;
    //定义一个保证线程同步的标识
    private static readonly object locker = new object();

    private HeroConfig()
    {

    }

    public static HeroConfig GetInstance()
    {
        if (instance == null)
        {
            lock (locker)
            {
                if (instance == null)
                {
                    instance = new HeroConfig();
                }
            }
        }
        return instance;
    }

    Dictionary<string, SoldierConfigData> soldierData;
    bool isLoadSoldierConfig = false;
    public bool IsLoadSoldierConfig
    {
        get
        {
            return isLoadSoldierConfig;
        }
    }

    public void LoadSoldierConfig()
    {
        soldierData = new Dictionary<string, SoldierConfigData>();
        DataTable data = NPOIOprateExcel.ExcelUtility.ExcelToDataTable("config/soliderConfig.xlsx");

        int lineNum = data.Rows.Count;

        for(int i = 0; i < lineNum; i++)
        {
            SoldierConfigData heroData = new SoldierConfigData(false);
            for (int j = 0; j < 29; j++)
            {
                //Debug.Log(data.Columns[j].ToString() + "  " +data.Rows[i][j].ToString());
                ObjBaseTool.SetProperty(heroData, data.Columns[j].ToString(), data.Rows[i][j].ToString());
            }

            //Debug.Log(heroData.KEY);
            //Debug.Log(heroData);
            soldierData.Add(heroData.KEY, heroData);
        }
        isLoadSoldierConfig = true;

    }
    
    /*
    public void LoadHeroHeadConfig()
    {
        DataTable data = NPOIOprateExcel.ExcelUtility.ExcelToDataTable("config/herosHead.xlsx");

        int lineNum = data.Rows.Count;
        int colNum = data.Columns.Count;

        for (int i = 0; i < lineNum; i++)
        {
            int id = 0;
            for (int j = 0; j < colNum; j++)
            {
                if(data.Columns[j].ToString() == "id")
                {
                    id = int.Parse(data.Rows[i][j].ToString());
                    break;
                }
            }

            HeroConfigData heroData;
            if(herosData.TryGetValue(id, out heroData))
            {
                for (int j = 0; j < colNum; j++)
                {
                    ObjBaseTool.SetProperty(heroData, data.Columns[j].ToString(), data.Rows[i][j].ToString());
                }
            }
        }


        //foreach (KeyValuePair<int, HeroConfigData> kvp in herosData)
        //{
        //    kvp.Value.PrintString();
        //}
    }
    */

    public Dictionary<string, SoldierConfigData> GetSoldierConfigList()
    {
        return soldierData;
    }

    public SoldierConfigData GetSoldierConfig(string id)
    {
        SoldierConfigData data;
        if (soldierData.TryGetValue(id, out data))
        {
            return data;
        }

        return null;
    }
}
