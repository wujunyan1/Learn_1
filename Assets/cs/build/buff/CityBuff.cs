using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuff : EventObject
{
    protected City city;

    public void SetCity(City city)
    {
        this.city = city;
    }

    public virtual void Execute()
    {

    }

    public virtual int GetCityProperty(CityProperty property, int baseProper)
    {
        return 0;
    }
}

public class CityBuff_AddMaxBuildNum : CityBuff
{
    int maxBuildNum;

    public void SetMaxBuildNum(int num)
    {
        maxBuildNum = num;
    }

    public override void Execute()
    {

    }

    public override int GetCityProperty(CityProperty property, int baseProper)
    {
        if(property == CityProperty.maxBuildNum)
        {
            return maxBuildNum;
        }
        return 0;
    }
}


public class CityBuff_AddMaxPersonNum : CityBuff
{
    int maxPersonNum;

    public void SetMaxPersonNum(int num)
    {
        maxPersonNum = num;
    }

    public override void Execute()
    {

    }

    public override int GetCityProperty(CityProperty property, int baseProper)
    {
        if (property == CityProperty.maxPerson)
        {
            return maxPersonNum;
        }
        return 0;
    }
}

