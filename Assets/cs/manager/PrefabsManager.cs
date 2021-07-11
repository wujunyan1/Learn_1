using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabsManager : EventObject
{
    private static PrefabsManager instance;
    private static object locker = new object();

    public static PrefabsManager GetInstance()
    {
        if (instance == null)
        {
            lock (locker)
            {
                if (instance == null)
                {
                    instance = new PrefabsManager();
                }
            }
        }
        return instance;
    }


    public delegate void LoadEndFunc(GameObject prefab);
    private Dictionary<string, GameObject> prefabs;
    private Dictionary<string, string> prefabNames;


    private PrefabsManager()
    {
        prefabs = new Dictionary<string, GameObject>();
        prefabNames = new Dictionary<string, string>();
    }

    public IEnumerator LoadBasePrefabs()
    {
        string[] prefabPaths = {
            "Button", "prefabs/Base/Button",
            "MsgPrefab", "prefabs/Base/MsgPrefab",
            "SoldierMessagePanel", "prefabs/Base/SoldierMessagePanel",
            "TroopHammer", "prefabs/BigBattle/prop/hammer",
            "House_1", "prefabs/BigBattle/Build/rpgpp_lt_building_01",
            "House_2", "prefabs/BigBattle/Build/rpgpp_lt_building_02",
            "House_3", "prefabs/BigBattle/Build/rpgpp_lt_building_03",
            "House_4", "prefabs/BigBattle/Build/rpgpp_lt_building_04",
            "House_5", "prefabs/BigBattle/Build/rpgpp_lt_building_05",
            "City", "prefabs/BigBattle/Build/City",
            "City_Terrain_path_1", "prefabs/BigBattle/Build/rpgpp_lt_terrain_path_01a.prefab",
            "City_Terrain_path_2", "prefabs/BigBattle/Build/rpgpp_lt_terrain_path_01b.prefab",
            "Farm", "prefabs/BigBattle/Build/Farm",
        };

        IEnumerator itor;
        for (int i = 0; i < prefabPaths.Length;)
        {
            string name = prefabPaths[i];
            string path = prefabPaths[i + 1];

            if (!prefabs.ContainsKey(path))
            {
                itor = LoadPrefabAsyncByCoroutine(name, path);

                while (itor.MoveNext())
                {
                    yield return null;
                }
            }

            i = i + 2;
        }
    }

    public void AddPrefab(string name, string path, GameObject obj)
    {
        prefabs.Add(path, obj);

        if(name != null && !prefabNames.ContainsKey(name))
        {
            prefabNames.Add(name, path);
        }
    }
    

    private GameObject LoadPrefab(string prefabPath)
    {
        GameObject obj = Resources.Load<GameObject>(prefabPath);
        //AddPrefab(prefabPath, obj);
        return obj;
    }

    public void LoadPrefabAsync(string name, string prefabPath, LoadEndFunc endfunc)
    {
        Camera camera = Camera.current;

        ViewManager.Instance.StartCoroutine(LoadPrefabAsyncByCoroutine(name, prefabPath));

        GameObject obj;
        if(prefabs.TryGetValue(prefabPath, out obj))
        {
            endfunc(obj);
        }
    }

    public IEnumerator LoadPrefabAsyncByCoroutine(string prefabName, string prefabPath)
    {
        yield return null;

        ResourceRequest ObjReq = Resources.LoadAsync<GameObject>(prefabPath);

        while (ObjReq.progress < 1)
        {
            yield return null;
        }

        GameObject gameObject = (GameObject)ObjReq.asset;
        AddPrefab(prefabName, prefabPath, gameObject);
        yield return null;
    }

    public void GetPrefab(out GameObject prefab, string name, string prefabPath = null)
    {
        if(prefabPath == null)
        {
            if (!prefabNames.TryGetValue(name, out prefabPath))
            {
                prefab = null;
                return;
            }
        }

        if (prefabs.TryGetValue(prefabPath, out prefab))
        {
            return;
        }
        prefab = LoadPrefab(prefabPath);
        
        AddPrefab(name, prefabPath, prefab);
        return;
    }

    public void GetPrefabAsync(string prefabPath, LoadEndFunc endfunc)
    {

    }

    public bool HasePrefab(string name, string prefabPath = null)
    {
        if (prefabPath == null)
        {
            if (!prefabNames.TryGetValue(name, out prefabPath))
            {
                return false;
            }
        }

        if (prefabs.ContainsKey(prefabPath))
        {
            return true;
        }
        return false;
    }


    public void GetGameObj(out GameObject obj, string name)
    {
        GameObject prefab;
        GetPrefab(out prefab, name);
        if(null == prefab)
        {
            obj = null;
            return;
        }

        obj = GameObject.Instantiate(prefab);
    }
}
