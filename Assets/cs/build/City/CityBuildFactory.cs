using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuildFactory : MonoBehaviour
{
    public static CityBuild CreateCityBuild(City city, CityBuildType type)
    {
        CityBuild build = null;
        switch (type)
        {
            case CityBuildType.Barracks:
                build = CreateBarracks(city);
                break;
            case CityBuildType.Tavern:
                build = CreateTavern(city);
                break;
        }

        return build;
    }

    static Tavern CreateTavern(City city)
    {
        Tavern tavern = new Tavern();
        city.AddBuilds(tavern);

        return tavern;
    }

    static Barracks CreateBarracks(City city)
    {
        Barracks barracks = new Barracks();
        city.AddBuilds(barracks);

        return barracks;
    }
}
