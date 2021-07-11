using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;

public class CityBuildConfig
{
    public int id; // id 
    public int type; // 建筑类型
    public string name; // 建筑名称
    public int level; //  等级
    public int buildNum;    // 一个城市限制建造个数
    public int gold; //升级所需 金币
    public int materials; //升级所需 物资
    public int buildRound; // 升级的回合
    public string image; // 图标样式
    public int upId; // 升级为

    public string param1;   // 参数，不同的类型需要
    public string param2;
    public string param3;


    public CityBuildType BuildType
    {
        get
        {
            return (CityBuildType)type;
        }
    }
}

public static class CityBuildConfigExtend
{
    private static List<List<CityBuildConfig>> cityBuildConfigs = null;

    // 获取对应的该类型列表
    private static List<CityBuildConfig> _GetOrCreateCityBuildConfigs(int buildType)
    {
        List<CityBuildConfig> configs;

        // 获取对应的该类型列表
        // 超出则创建
        if (buildType >= cityBuildConfigs.Count)
        {
            int j = cityBuildConfigs.Count;
            for (; j < buildType; j++)
            {
                cityBuildConfigs.Add(new List<CityBuildConfig>());
            }

            configs = new List<CityBuildConfig>();
            cityBuildConfigs.Add(configs);
        }
        else
        {
            configs = cityBuildConfigs[buildType];
        }

        return configs;
    }

    public static void LoadConfig()
    {
        cityBuildConfigs = new List<List<CityBuildConfig>>();

        DataTable data = NPOIOprateExcel.ExcelUtility.ExcelToDataTable("config/cityBuild.xlsx");

        int lineNum = data.Rows.Count;
        int colNum = data.Columns.Count;

        for (int i = 0; i < lineNum; i++)
        {
            CityBuildConfig _config = new CityBuildConfig();
            for (int j = 0; j < colNum; j++)
            {
                ObjBaseTool.SetProperty(_config, data.Columns[j].ToString(), data.Rows[i][j].ToString());
            }

            int buildType = _config.type;
            List<CityBuildConfig> configs = _GetOrCreateCityBuildConfigs(buildType);

            int level = _config.level;
            configs.Add(_config);
        }
        

    }

    public static CityBuildConfig GetCityBuildConfig(CityBuildType buildType, int level)
    {
        List<CityBuildConfig> configs = cityBuildConfigs[(int)buildType];

        foreach (var item in configs)
        {
            if(item.level == level)
            {
                return item;
            }
        }

        return configs[0];
    }

    public static List<CityBuildConfig> GetCityBuildConfigs(CityBuildType buildType)
    {
        return cityBuildConfigs[(int)buildType];
    }

    public static List<List<CityBuildConfig>> GetAllCityBuildConfigs()
    {
        return cityBuildConfigs;
    }
}
