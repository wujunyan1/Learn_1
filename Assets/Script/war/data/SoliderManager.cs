using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 创建队伍，队伍数据管理
/// </summary>
public class SoliderManager : MonoBehaviour
{
    public GameWorldOLD world;

    private Dictionary<string, SoldierData> soldierData;
    private Dictionary<string, SoliderConfig> soliderPrefabConfig;

    private static SoliderManager m_instance = null;
    public static SoliderManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<SoliderManager>();
            }
            return m_instance;
        }
    }

    private void Awake()
    {
        soldierData = ParamLoader.GetInstance().LoadSoliderCSV();
        soliderPrefabConfig = ParamLoader.GetInstance().LoadSoliderPrefabConfigCSV();
    }

    public SoldierData GetSoldierData(string key)
    {
        SoldierData obj;
        if (soldierData.TryGetValue(key, out obj))
        {
            return obj;
        }
        else
        {
            Debug.LogWarning("key: " + key + "  is not exit");
            return null;
        }
    }

    SoliderConfig GetSoliderConfig(string key)
    {
        SoliderConfig obj;
        if (soliderPrefabConfig.TryGetValue(key, out obj))
        {
            return obj;
        }
        else
        {
            Debug.LogWarning("key: " + key + "  is not exit");
            return null;
        }
    }

    /// <summary>
    /// 创建一个士兵
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public Soldier CreateSoldier(string key)
    {
        //SoliderConfig config = GetSoliderConfig(key);

        //// 以后可能改为代码添加组件，而不是在模板上添加
        //Soldier soldierPrefab = Resources.Load<Soldier>(config.prefabPath);

        //Soldier soldier = Instantiate<Soldier>(soldierPrefab);

        //// 设置数据
        //soldier.world = world;
        //soldier.SetSoliderData(GetSoldierData(config.soliderKey));

        //soldier.SetHeading(new Vector2(0, 1));
        //soldier.m_vSide = new Vector2(1, 0);

        return null;
    }

    /// <summary>
    /// 生成将军
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public Genera CreateGenera(string key, GeneraData generaData)
    {
        //SoliderConfig config = GetSoliderConfig(key);

        //// 以后可能改为代码添加组件，而不是在模板上添加
        //Genera soldierPrefab = Resources.Load<Genera>(config.generaPath);

        //Genera soldier = Instantiate<Genera>(soldierPrefab);

        //soldier.world = world;
        //soldier.SetHeading(new Vector2(0, 1));
        //soldier.m_vSide = new Vector2(1, 0);

        //// 设置数据
        //soldier.SetGeneraData(generaData);
        
        return null;
    }
}
