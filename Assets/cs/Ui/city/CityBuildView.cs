using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuildView : View
{
    protected City currCity;

    public City CurrCity
    {
        get
        {
            return currCity;
        }
    }

    public virtual void SetCurrCity(City city)
    {
        currCity = city;
    }

    public CityView cityView;
}
