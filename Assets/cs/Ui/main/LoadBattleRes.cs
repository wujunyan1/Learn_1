using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadBattleRes : View
{

    public Progress progress;
    private int progressNum = 0;

    int currProgressNum = 0;

    // 点击加载
    BinaryReader reader = null;

    private void AddProgress(int progressNum)
    {
        currProgressNum += progressNum;
        progress.SetProgress(currProgressNum * 1.0f / 100);
    }

    private void SubAddProgress(float progressNum)
    {
        float subNum = currProgressNum + progressNum;
        progress.SetProgress(subNum / 100);
    }

    public void LoadSaveFile()
    {
        
    }

    private void Start()
    {
        HeroConfig data = HeroConfig.GetInstance();
        StartCoroutine(StartProgress());
    }

    private IEnumerator StartProgress()
    {
        HexMapCamera.Locked = true;
        yield return null;

        progress.SetCustomText("加载配置");
        Coroutine loadResourse = StartCoroutine(LoadConfigs().WrapEnumerator());
        yield return loadResourse;
        AddProgress(10);

        progress.SetCustomText("加载资源");
        loadResourse = StartCoroutine(LoadResources().WrapEnumerator());
        yield return loadResourse;
        AddProgress(10);

        progress.SetCustomText("加载世界");
        loadResourse = StartCoroutine(LoadGameCenter().WrapEnumerator());
        yield return loadResourse;
        AddProgress(10);

        progress.SetCustomText("加载人物模型");
        loadResourse = StartCoroutine(LoadPrefabs().WrapEnumerator());
        yield return loadResourse;
        AddProgress(10);

        progress.SetCustomText("加载基础模型");
        loadResourse = StartCoroutine(LoadBasePrefabs().WrapEnumerator());
        yield return loadResourse;
        AddProgress(10);


        // 是加载存档，还是创建新的
        GameLoadData gameLoadData = GameLoadData.GetInstance();
        if (gameLoadData.loadPath != null)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(gameLoadData.loadPath, FileMode.Open)))
            {
                progress.SetCustomText("加载数据");
                loadResourse = StartCoroutine(LoadData(reader, 20).WrapEnumerator());
                yield return loadResourse;
                AddProgress(20);
            }
        }
        else
        {
            progress.SetCustomText("生成地图");
            loadResourse = StartCoroutine(GenerateMap().WrapEnumerator());
            yield return loadResourse;
            AddProgress(10);

            progress.SetCustomText("生成数据");
            loadResourse = StartCoroutine(GenerateData().WrapEnumerator());
            yield return loadResourse;
            AddProgress(10);
        }

        progress.SetCustomText("加载UI");
        loadResourse = StartCoroutine(LoadUI().WrapEnumerator());
        yield return loadResourse;
        AddProgress(10);

        HexMapCamera.Locked = false;

        Close();
        //GameObject.Destroy(this.gameObject);
    }

    IEnumerator LoadConfigs()
    {
        yield return null;
        if (TroopsConfigDataManager.GetConfigList() == null)
        {
            TroopsConfigDataManager.LoadConfig();
        }
        yield return null;
        
        if (CityBuildConfigExtend.GetAllCityBuildConfigs() == null)
        {
                CityBuildConfigExtend.LoadConfig();
        }

        yield return null;
    }

    IEnumerator LoadResources()
    {
        yield return null;

        IEnumerator itor = ResourseManager.LoadHeadRes();
        while (itor.MoveNext())
        {
            yield return null;
        }

        itor = ResourseManager.LoadUIRes(ResourseManager.UI_CIYT_BUILD);
        while (itor.MoveNext())
        {
            yield return null;
        }

        yield return null;
    }

    IEnumerator LoadGameCenter()
    {
        yield return null;


        GameObject obj = GameObject.Find("GameCenter");
        if(obj != null)
        {
            yield break;
        }

        ResourceRequest mapReq = Resources.LoadAsync<GameObject>("prefabs/BigBattle/GameCenter");

        while (mapReq.progress < 1)
        {
            yield return null;
        }

        obj = (GameObject)mapReq.asset;
        obj = Instantiate<GameObject>(obj);
        obj.name = "GameCenter";

        yield return null;
    }

    IEnumerator LoadPrefabs()
    {
        yield return null;

        PrefabsManager prefabsManager = PrefabsManager.GetInstance();
        List<TroopsConfigData> troopsConfigs = TroopsConfigDataManager.GetConfigList();

        bool hasLoad = true;
        foreach (var item in troopsConfigs)
        {
            string name = item.modelName;
            if (!prefabsManager.HasePrefab(name))
            {
                hasLoad = false;
            }
        }

        if (hasLoad)
        {
            yield break;
        }

        yield return null;
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(Application.dataPath + "/ABs/troops_prefabs.abs");

        yield return request;

        AssetBundle ab = request.assetBundle;

        if (ab == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            yield break;
        }
        
        
        
        foreach (var item in troopsConfigs)
        {
            string name = item.modelName;

            // 没有则加进去
            if (!prefabsManager.HasePrefab(name))
            {
                string modPath = item.model;
                var assetLoadRequest = ab.LoadAssetAsync<GameObject>(modPath + ".prefab");

                yield return assetLoadRequest;
                GameObject tempObj = assetLoadRequest.asset as GameObject;
                if (null == tempObj)
                {
                    Debug.Log(gameObject.name + " Assetbundle里没有对应" + modPath + "的预制体");
                }
                else
                {
                    prefabsManager.AddPrefab(name, modPath, tempObj);
                }
            }
        }

        if (!prefabsManager.HasePrefab("Troop"))
        {
            IEnumerator itor = prefabsManager.LoadPrefabAsyncByCoroutine("Troop", "prefabs/BigBattle/Troop");

            while (itor.MoveNext())
            {
                yield return null;
            }
        }
        yield return null;
    }

    IEnumerator LoadBasePrefabs()
    {
        PrefabsManager prefabsManager = PrefabsManager.GetInstance();

        IEnumerator itor = prefabsManager.LoadBasePrefabs();

        while (itor.MoveNext())
        {
            yield return null;
        }

        yield return null;
    }


    /// <summary>
    /// 加载数据
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadData(BinaryReader reader, int proLoad)
    {
        yield return null;

        float pro = 0.0f;

        int fileHeader = reader.ReadInt32();
        GameVersions.currLoadVersions = fileHeader;


        IEnumerator itor = GameCenter.instance.Load(reader);

        float t = Time.time;
        Debug.Log(t);

        while (itor.MoveNext())
        {
            float _t = Time.time;
            if( _t - 1 > t)
            {
                yield return null;
                t = Time.time;
            }
            pro += 0.001f * proLoad;
            SubAddProgress(pro);
        }
    }


    IEnumerator GenerateMap()
    {
        yield return null;

        GameLoadData gameLoadData = GameLoadData.GetInstance();

        if (gameLoadData.newGameData != null)
        {
            IEnumerator tor = GameCenter.instance.GenerateMap(gameLoadData.newGameData);

            while (tor.MoveNext())
            {
                yield return null;
            }
        }
    }
    
    IEnumerator GenerateData()
    {
        yield return null;

        GameCenter.instance.GenerateInitialData();

    }

    IEnumerator LoadUI()
    {
        yield return null;

        if(GameObject.Find("UI") == null)
        {
            ResourceRequest UIReq = Resources.LoadAsync<BigBattleUI>("prefabs/UI/BigBattle/BigBattleUI");

            while (UIReq.progress < 1)
            {
                yield return null;
            }

            BigBattleUI obj = (BigBattleUI)UIReq.asset;
            BigBattleUI mapObj = Instantiate(obj, this.transform.parent);
            mapObj.name = "UI";
        }

        yield return null;
    }
}
