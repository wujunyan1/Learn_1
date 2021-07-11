using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapBuildFactory : MonoBehaviour
{
    public static void Save(MapBuild build, BinaryWriter writer)
    {
        if(build != null)
        {
            writer.Write(true);
            //build.Save(writer);
        }
        else
        {
            writer.Write(false);
        }
    }

    public static MapBuild Load(BinaryReader reader)
    {
        MapBuild build = null;

        if (reader.ReadBoolean())
        {
            BuildType type = (BuildType)reader.ReadByte();

            switch (type)
            {
                case BuildType.City:
                    break;
            }
        }

        return build;
    }

    public static MapBuild CreateMapBuild(BuildType type)
    {
        MapBuild build = null;
        switch (type)
        {
            case BuildType.City:
                build = GameCenter.instance.GenerateNewCity();
                break;
            case BuildType.Farm:
                build = GameCenter.instance.GenerateNewFarm();
                break;
        }

        //build.SetPoint(cell.coordinates.X, cell.coordinates.Z);

        return build;
    }

    public static void CreateMapBuild(HexCell cell, BuildType type)
    {
        MapBuild build = null;
        switch (type)
        {
            case BuildType.City:
                break;
        }

        //build.SetPoint(cell.coordinates.X, cell.coordinates.Z);

        cell.Build = build;
    }

    static City CreateCity(HexCell cell)
    {
        City city = new City();
        //city.BuildNewCity(cell.coordinates);
        return city;
    }
}
