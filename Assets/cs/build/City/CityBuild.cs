using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// 城市建筑
public class CityBuild
{
    // 等级
    protected int level;

    protected int maxLevel;

    public void Save(BinaryWriter writer)
    {
       
    }

    public void Load(BinaryReader reader)
    {
    }

    public virtual void AddLevel()
    {
        level++;
    }
}
