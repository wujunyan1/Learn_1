using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleResManager
{
    private static BattleResManager instance;
    //定义一个保证线程同步的标识
    private static readonly object locker = new object();

    private Dictionary<string, GameObject> soldierModels;

    public GameObject soldierPrefabs;

    private Dictionary<string, GameObject> objModels;

    private BattleResManager()
    {
        soldierModels = new Dictionary<string, GameObject>();
        objModels = new Dictionary<string, GameObject>();
    }

    public static BattleResManager GetInstance()
    {
        if (instance == null)
        {
            lock (locker)
            {
                if (instance == null)
                {
                    instance = new BattleResManager();
                }
            }
        }
        return instance;
    }


    public GameObject GetSoldierModel(string key)
    {
        GameObject model;
        if(soldierModels.TryGetValue(key, out model)){
            return model;
        }

        return null;
    }

    public void AddSoldierModel(string key, GameObject model)
    {
        soldierModels.Add(key, model);
    }

    public GameObject GetObjModel(string key)
    {
        GameObject model;
        if (objModels.TryGetValue(key, out model))
        {
            return model;
        }

        return null;
    }

    public void AddObjModel(string key, GameObject model)
    {
        objModels.Add(key, model);
    }

}
