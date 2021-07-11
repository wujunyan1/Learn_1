using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 加载战斗场景资源，进度条界面
/// </summary>
public class LoadBattleSceneRes : EventComponent
{
    public Progress progress;
    private int progressNum = 0;

    private void Start()
    {
        HeroConfig data = HeroConfig.GetInstance();

        progress.SetDirection(Progress.Direction.LeftToRight);

        /*
        // 加载英雄的 配置文件
        Coroutine loadHeroCoroutine = StartCoroutine(LoadHeroConfig(80));

        // 加载模型
        StartCoroutine(LoadAbs(10, loadHeroCoroutine));

        // 创建卡池
        StartCoroutine(CreateCardPool(10, loadHeroCoroutine));
        */

        StartCoroutine(LoadRes());
    }

    private IEnumerator LoadRes()
    {
        yield return null;

        HeroConfig data = HeroConfig.GetInstance();
        yield return null;

        //加载英雄的 配置文件
        //data.LoadHeroConfig();
        yield return null;

        progressNum += 10;
        Debug.Log(string.Format("LoadHeroConfig addNum {0}", 10));

        //data.LoadHeroHeadConfig();
        yield return null;

        progressNum += 10;
        Debug.Log(string.Format("LoadHeroHeadConfig addNum {0}", 10));
        yield return null;

        ConfigManager manager = ConfigManager.GetInstance();
        manager.LoadUpLevelConfig();
        progressNum += 20;
        Debug.Log(string.Format("LoadUpLevelConfig addNum {0}", 20));
        yield return null;

        // 加载模型
        yield return StartCoroutine(LoadAbs(20));

        // 加载人物头像
        yield return StartCoroutine(LoadHeroHeadAbs(20));

        // 创建卡池
        yield return StartCoroutine(CreateCardPool(20));
    }

    private IEnumerator LoadHeroConfig(int addNum)
    {
        HeroConfig data = HeroConfig.GetInstance();
        yield return null;

        //data.LoadHeroConfig();
        yield return null;

        progressNum += addNum / 2;
        Debug.Log(string.Format("LoadHeroConfig addNum {0}", addNum/2));

        //data.LoadHeroHeadConfig();
        yield return null;

        progressNum += addNum / 2;
        Debug.Log(string.Format("LoadHeroHeadConfig addNum {0}", addNum / 2));
    }

    private IEnumerator LoadAbs(int addNum)
    {
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(Application.dataPath + "/ABs/heros_prefabs.abs");

        yield return request;

        AssetBundle ab = request.assetBundle;

        if (ab == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            yield break;
        }

        //Texture2D[] t = ab.LoadAllAssets<Texture2D>();
        //foreach(Texture2D _t in t)
        //{
        //    Debug.Log(string.Format("Texture2D name {0}", _t.name));
        //}

        

        ResManager resManager = ResManager.GetInstance();
        Dictionary<int, HeroConfigData> heros = new Dictionary<int, HeroConfigData>(); // HeroConfig.GetInstance().GetHerosConfig();

        foreach (KeyValuePair<int, HeroConfigData> kvp in heros)
        {
            HeroConfigData data = kvp.Value;

            string modPath = data.modPath;

            var assetLoadRequest = ab.LoadAssetAsync<GameObject>(modPath);

            yield return assetLoadRequest;
            GameObject tempObj = assetLoadRequest.asset as GameObject;
            if (null == tempObj)
            {
                Debug.Log(gameObject.name + " Assetbundle里没有对应" + modPath + "的预制体");
            }
            else
            {
                resManager.AddModPrefab(data.id, tempObj);
            }
        }

        ab.Unload(false);

        progressNum += addNum;
    }


    private IEnumerator LoadHeroHeadAbs(int addNum)
    {
        //Debug.Log(Application.dataPath);
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(Application.dataPath + "/ABs/hero_head.abs");

        yield return request;

        AssetBundle ab = request.assetBundle;

        if (ab == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            yield break;
        }

        //Texture2D[] t = ab.LoadAllAssets<Texture2D>();
        //foreach(Texture2D _t in t)
        //{
        //    Debug.Log(string.Format("Texture2D name {0}", _t.name));
        //}

        ResManager resManager = ResManager.GetInstance();
        Dictionary<int, HeroConfigData> heros = new Dictionary<int, HeroConfigData>(); // HeroConfig.GetInstance().GetHerosConfig();

        foreach (KeyValuePair<int, HeroConfigData> kvp in heros)
        {
            HeroConfigData data = kvp.Value;

            string resPath = data.headRes;

            var assetLoadRequest = ab.LoadAssetAsync<Texture2D>(resPath);

            yield return assetLoadRequest;
            Texture2D tempTex = assetLoadRequest.asset as Texture2D;
            if (null == tempTex)
            {
                Debug.Log(gameObject.name + "Assetbundle里没有对应" + resPath + "的贴图");
            }
            else
            {
                resManager.AddHeadImg(data.id, tempTex);
            }
        }

        ab.Unload(false);

        progressNum += addNum;
    }

    private IEnumerator CreateCardPool(int addNum)
    {
        yield return null;

        

        yield return null;
        progressNum += addNum;

        Debug.Log( string.Format("CreateCardPool addNum {0}", addNum));
    }

    private void Update()
    {
        progress.SetProgress(progressNum / 100.0f);

        if(progressNum >= 100)
        {
            this.FireEvent("StartGame");
            this.gameObject.SetActive(false);
        }
    }
}
