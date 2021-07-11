using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResManager
{
    private static ResManager instance;
    private ResManager()
    {
        headImgDic = new Dictionary<int, Texture2D>();
        heroPrefabs = new Dictionary<int, GameObject>();
    }

    public static ResManager GetInstance()
    {
        if (instance == null)
        {
            instance = new ResManager();
        }
        return instance;
    }



    private Dictionary<int, Texture2D> headImgDic;
    private Dictionary<int, GameObject> heroPrefabs;

    public void AddHeadImg(int id, Texture2D img)
    {
        headImgDic.Add(id, img);
    }

    public Texture2D GetHeadImg(int id)
    {
        Texture2D img;
        if(headImgDic.TryGetValue(id, out img)){
            return img;
        }
        return null;
    }

    public void AddModPrefab(int id, GameObject obj)
    {
        heroPrefabs.Add(id, obj);
    }

    public GameObject GetHeroMod(int id)
    {
        GameObject obj;
        if (heroPrefabs.TryGetValue(id, out obj))
        {
            return GameObject.Instantiate<GameObject>(obj);
        }
        return null;
    }
}
