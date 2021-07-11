using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum BuildType
{
    City,
    Farm,
}

public class MapBuild : EventObject, SaveLoadInterface
{
    protected BuildType buildType;

    public BuildType BuildType
    {
        get
        {
            return buildType;
        }
    }

    public virtual IEnumerator Load(BinaryReader reader)
    {
        yield return null;
    }

    public virtual void Save(BinaryWriter writer)
    {
    }
}
