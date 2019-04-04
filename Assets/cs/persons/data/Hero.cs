using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// 英雄
/// </summary>
public class Hero : Person
{
    int id;
    public int Id
    {
        get
        {
            return id;
        }
    }

    public string name;

    // 资源ID，对应 头像和全身像 Id, 甚至是模型
    public string resId;

    public Hero(int id)
    {
        this.id = id;
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(id);
        writer.Write(name);
        writer.Write(resId);
    }

    public void Load(BinaryReader reader)
    {
        //type = (CityBuildType)reader.ReadByte();
        id = reader.ReadInt32();
        name = reader.ReadString();
        resId = reader.ReadString();
    }
}
