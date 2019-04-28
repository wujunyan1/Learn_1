using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum BuildType
{
    City,
}

public class MapBuild : MonoBehaviour
{
    protected BuildType buildType;

    public BuildType BuildType
    {
        get
        {
            return buildType;
        }
    }

    // 坐标
    protected HexCoordinates point;

    public Camp camp;

    public void Save(BinaryWriter writer)
    {
        writer.Write((byte)buildType);
        writer.Write(point.X);
        writer.Write(point.Z);
        SaveData(writer);
    }

    public void Load(BinaryReader reader)
    {
        int x = reader.ReadInt32();
        int z = reader.ReadInt32();
        SetPoint(x, z);

        LoadData(reader);
    }

    public virtual void ClearData()
    {

    }

    public virtual void SaveData(BinaryWriter writer)
    {
        return;
    }

    public virtual void LoadData(BinaryReader reader)
    {
        return;
    }

    public void SetPoint(int x, int z)
    {
        point = new HexCoordinates(x, z);
        transform.localPosition = point.GetPosition();
    }

    public void SetPoint(HexCoordinates coordinates)
    {
        SetPoint(coordinates.X, coordinates.Z);
    }

    public HexCoordinates GetPoint()
    {
        return point;
    }

    public virtual void NextRound()
    {

    }
}
