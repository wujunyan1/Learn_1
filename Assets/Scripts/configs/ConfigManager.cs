using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;

public class ConfigManager
{
    static ConfigManager instance;
    //定义一个保证线程同步的标识
    private static readonly object locker = new object();

    private ConfigManager()
    {

    }

    public static ConfigManager GetInstance()
    {
        if (instance == null)
        {
            lock (locker)
            {
                if (instance == null)
                {
                    instance = new ConfigManager();
                }
            }
        }
        return instance;
    }


    List<UpLevelConfigData> upLevelConfigDatas;

    public void LoadUpLevelConfig()
    {
        upLevelConfigDatas = new List<UpLevelConfigData>();
        DataTable data = NPOIOprateExcel.ExcelUtility.ExcelToDataTable("config/upLevel.xlsx");

        int lineNum = data.Rows.Count;

        for (int i = 0; i < lineNum; i++)
        {
            UpLevelConfigData levelConfig = new UpLevelConfigData();
            for (int j = 0; j < 7; j++)
            {
                ObjBaseTool.SetProperty(levelConfig, data.Columns[j].ToString(), data.Rows[i][j].ToString());
            }

            upLevelConfigDatas.Add(levelConfig);
        }
    }

    public UpLevelConfigData GetLevelConfig(int level)
    {
        return upLevelConfigDatas[level - 1];
    }
}
