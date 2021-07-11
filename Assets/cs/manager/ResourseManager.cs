using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourseManager
{
    public const string UI_CIYT_BUILD = "uibuild";



    static Dictionary<string, Sprite> headRes = new Dictionary<string, Sprite>();
    static Dictionary<string, Sprite> uiRes = new Dictionary<string, Sprite>();

    public static bool HasHeadRes(string key)
    {
        return headRes.ContainsKey(key);
    }

    public static void AddHeadRes(string key, Sprite head)
    {
        headRes.Add(key, head);
    }

    public static Sprite GetHeadRes(string key)
    {
        Debug.Log(key);
        Sprite head;
        if(headRes.TryGetValue(key, out head))
        {
            return head;
        }

        return null;
    }

    public static string GetRandomHeadKey()
    {
        Dictionary<string, Sprite>.KeyCollection keys = headRes.Keys;

        int index = Random.Range(0, keys.Count);
        foreach (var item in keys)
        {
            if(index == 0)
            {
                return item;
            }

            index--;
        }

        return "于禁";
    }


    public static IEnumerator LoadHeadRes()
    {
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(Application.dataPath + "/ABs/hero_head.abs");

        yield return request;

        AssetBundle ab = request.assetBundle;

        if (ab == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            yield break;
        }

        string[] names = ab.GetAllAssetNames();

        foreach (var name in names)
        {
            string[] s = name.Split('/');
            string s_k = s[s.Length - 1];
            string key = s_k.Split('.')[0];

            if (!ResourseManager.HasHeadRes(key))
            {
                var assetLoadRequest = ab.LoadAssetAsync<Sprite>(name);

                yield return assetLoadRequest;
                Sprite tempObj = assetLoadRequest.asset as Sprite;
                if (null == tempObj)
                {
                    Debug.Log(" Assetbundle里没有对应" + name + "的头像");
                }
                else
                {
                    //string key = 
                    ResourseManager.AddHeadRes(key, tempObj);
                }
            }
        }
    }




    public static bool HasUIRes(string key)
    {
        return uiRes.ContainsKey(key);
    }

    public static void AddUIRes(string key, Sprite sprite)
    {
        uiRes.Add(key, sprite);
    }

    public static Sprite GetUIRes(string key)
    {
        Sprite sprite;
        if (uiRes.TryGetValue(key, out sprite))
        {
            return sprite;
        }

        return null;
    }


    public static IEnumerator LoadUIRes(string abName)
    {
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(Application.dataPath + "/ABs/"+ abName + ".abs");

        yield return request;

        AssetBundle ab = request.assetBundle;

        if (ab == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            yield break;
        }

        string[] names = ab.GetAllAssetNames();

        foreach (var name in names)
        {
            string[] s = name.Split('/');
            string s_k = s[s.Length - 1];
            string key = s_k.Split('.')[0];

            key = abName + "_" + key;

            if (!ResourseManager.HasUIRes(key))
            {
                var assetLoadRequest = ab.LoadAssetAsync<Sprite>(name);

                yield return assetLoadRequest;
                Sprite tempObj = assetLoadRequest.asset as Sprite;
                if (null == tempObj)
                {
                    Debug.Log(" Assetbundle里没有对应" + name + "的Image");
                }
                else
                {
                    //string key = 

                    Debug.Log(key);
                    ResourseManager.AddUIRes(key, tempObj);
                }
            }
        }
    }
}
