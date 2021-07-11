using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UObject : Object
{
    Dictionary<string, object> maps;

    public UObject()
    {
        maps = new Dictionary<string, object>();
    }

    public void Set(string key, object value)
    {
        if(value == null)
        {
            maps.Remove(key);
        }
        else
        {
            if (maps.ContainsKey(key))
            {
                maps.Remove(key);
            }
            maps.Add(key, value);
        }
    }

    public object Get(string key)
    {
        object outObj;
        if(maps.TryGetValue(key, out outObj))
        {
            return outObj;
        }

        return null;
    }

    public T GetT<T>(string key, T t)
    {
        object outObj;
        if (maps.TryGetValue(key, out outObj))
        {
            return (T)outObj;
        }

        return t;
    }

    public void Clear()
    {
        maps.Clear();
    }
}


public static class UObjectPool
{
    private static List<UObject> os = new List<UObject>();

    public static UObject Get()
    {
        if(os.Count > 0)
        {
            return os[os.Count - 1];
        }

        return new UObject();
    }

    public static void Add(UObject o)
    {
        o.Clear();
        os.Add(o);
    }

}