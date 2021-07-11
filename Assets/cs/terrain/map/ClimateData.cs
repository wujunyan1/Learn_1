using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ClimateData : SaveLoadInterface
{
    /// <summary>
    /// 云
    /// </summary>
    public float clouds = 0;

    /// <summary>
    /// 湿度，地表水，可能是冰，地下水，河流等
    /// </summary>
    public float moisture = 0f;

    /// <summary>
    /// 温度
    /// </summary>
    public float temperature = 0f;

    /// <summary>
    /// 风
    /// </summary>
    public Vector2 wind = Vector2.zero;

    /// <summary>
    /// 雨
    /// </summary>
    public float rain = 0f;

    /// <summary>
    /// 向各个方向吹的风的力
    /// 临时数据
    /// </summary>
    public float[] neiWindForce = { 0f, 0f, 0f, 0f, 0f, 0f };

    //////////////////////////////////////////////////////////////
    public float water = 0f;
    public float ice = 0f;


    public void Save(BinaryWriter writer)
    {
        writer.Write(clouds);
        writer.Write(moisture);
        writer.Write(temperature);

        writer.Write(wind.x);
        writer.Write(wind.y);

        writer.Write(rain);

        writer.Write(water);
        writer.Write(ice);
    }

    public IEnumerator Load(BinaryReader reader)
    {
        clouds = reader.ReadSingle();
        moisture = reader.ReadSingle();
        temperature = reader.ReadSingle();

        wind = Vector2.zero;
        wind.x = reader.ReadSingle();
        wind.y = reader.ReadSingle();

        rain = reader.ReadSingle();
        water = reader.ReadSingle();
        ice = reader.ReadSingle();

        yield return null;
    }

}
