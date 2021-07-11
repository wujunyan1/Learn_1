using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadRes : EventComponent
{
    public Progress progress;

    public Transform battleObj;
    public Transform soldiersObj;

    public RectTransform baseCanvas;

    int currProgressNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartProgress());
    }

    private void AddProgress(int progressNum)
    {
        currProgressNum += progressNum;
        progress.SetProgress(currProgressNum * 1.0f / 100);
    }

    public IEnumerator StartProgress()
    {
        BattleDataManager dataManager = BattleDataManager.GetBattleData();
        BattleWorld.playerCamp = dataManager.GetPlayerCamp();

        progress.SetCustomText("加载地图");
        Coroutine loadResourse = StartCoroutine(LoadMap());
        yield return loadResourse;
        AddProgress(10);

        progress.SetCustomText("加载模型");
        loadResourse = StartCoroutine(LoadModel());
        yield return loadResourse;
        AddProgress(10);

        progress.SetCustomText("创建管理");
        loadResourse = StartCoroutine(CreateControl());
        yield return loadResourse;
        AddProgress(10);

        progress.SetCustomText("创建部队");
        loadResourse = StartCoroutine(CreateSoldiers());
        yield return loadResourse;
        AddProgress(10);

        progress.SetCustomText("创建UI");
        loadResourse = StartCoroutine(CreateUI());
        yield return loadResourse;
        AddProgress(10);

        GameObject.Destroy(this.gameObject);
    }

    /// <summary>
    /// 加载地图
    /// </summary>
    /// <returns></returns>
    public IEnumerator LoadMap()
    {
        yield return null;

        ResourceRequest mapReq = Resources.LoadAsync<GameObject>("prefabs/Battle/Map/BasePlane");
        
        while(mapReq.progress < 1)
        {
            yield return null;
        }

        GameObject obj = (GameObject)mapReq.asset;
        GameObject mapObj = Instantiate(obj, battleObj);
        mapObj.name = "Map";

        BattleWorld.battleMap = mapObj.GetComponent<BattleMap>();

        yield return null;
        BattleDataManager dataManager = BattleDataManager.GetBattleData();

        // 生成 地图
        BattleWorld.battleMap.GenerateMap(
            dataManager.warMapSeed, dataManager.frequency, dataManager.octaves,
            dataManager.lacunarity, dataManager.persistence
            );

        yield return null;
        EnumSeason season = dataManager.season;
        string resPath = season.GetBattleTreeResource();

        //  加载树资源
        ResourceRequest treeReq = Resources.LoadAsync<GameObject>(resPath);
        while (treeReq.progress < 1)
        {
            yield return null;
        }
        GameObject treeObj = (GameObject)treeReq.asset;
        yield return null;
        BattleResManager resManager = BattleResManager.GetInstance();
        resManager.AddObjModel("tree", treeObj);
        yield return null;

        IEnumerator tor = BattleWorld.battleMap.GenerateTree();

        while (tor.MoveNext())
        {
            yield return null;
        }
    }


    /// <summary>
    /// 加载部队模型
    /// </summary>
    /// <returns></returns>
    public IEnumerator LoadModel()
    {
        yield return null;
        BattleResManager resManager = BattleResManager.GetInstance();

        // 加载基础model 每个英雄在 加到这个节点下的model上
        ResourceRequest soldierPrefabRes = Resources.LoadAsync<GameObject>("prefabs/Battle/Hero");
        while (soldierPrefabRes.progress < 1)
        {
            yield return null;
        }

        GameObject soldierPrefab = (GameObject)soldierPrefabRes.asset;
        resManager.soldierPrefabs = soldierPrefab;

        yield return null;

        BattleDataManager dataManager = BattleDataManager.GetBattleData();

        BattleTeamData[] teams = dataManager.GetTeamDatas();

        foreach(BattleTeamData team in teams)
        {
            List<SoldierConfigData> configs = team.GetSoldierConfigs();
            foreach(SoldierConfigData data in configs)
            {
                string key = data.model;
                if(resManager.GetSoldierModel(key) == null)
                {
                    // 加载每一个model
                    ResourceRequest mapReq = Resources.LoadAsync<GameObject>(key);

                    while (mapReq.progress < 1)
                    {
                        yield return null;
                    }

                    GameObject obj = (GameObject)mapReq.asset;
                    resManager.AddSoldierModel(key, obj);
                }
            }
        }

    }

    /// <summary>
    /// 创建管理
    /// </summary>
    /// <returns></returns>
    public IEnumerator CreateControl()
    {
        yield return null;
        BattleCenter battleCenter = battleObj.gameObject.AddComponent<BattleCenter>();
        BattleDataManager dataManager = BattleDataManager.GetBattleData();
        battleCenter.SetCampNum(dataManager.GetTeamNum());
        

        BattleWorld.battleCenter = battleCenter;

        SoldierManager battleManager = battleObj.gameObject.AddComponent<SoldierManager>();
        SoldierManager.soldiersObj = soldiersObj;

        yield return null;
    }

    /// <summary>
    /// 创建部队
    /// </summary>
    /// <returns></returns>
    public IEnumerator CreateSoldiers()
    {
        yield return null;
        BattleDataManager dataManager = BattleDataManager.GetBattleData();

        BattleTeamData[] teams = dataManager.GetTeamDatas();

        BattleResManager resManager = BattleResManager.GetInstance();

        foreach (BattleTeamData team in teams)
        {
            SoldierManager.CreaterSoldierTeamControl(team);

            //List<SoldierConfigData> configs = team.GetSoldierConfigs();
            //foreach (SoldierConfigData data in configs)
            //{
            //    string key = data.model;

            //    SoldierControl control = SoldierManager.CreaterSoldierControl(team.GetCamp(), data, Vector3.zero);
                
            //}
        }
    }

    IEnumerator CreateUI()
    {
        yield return null;

        ResourceRequest UIReq = Resources.LoadAsync<BattleUI>("prefabs/Battle/UI/BattleUI");

        while (UIReq.progress < 1)
        {
            yield return null;
        }

        BattleUI obj = (BattleUI)UIReq.asset;
        BattleUI mapObj = Instantiate(obj, baseCanvas);
        mapObj.name = "UI";

        yield return null;
        mapObj.AddSoldierStateView();
    }
}
