using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddObj
{
    public static AddObj GetObj(string id, List<string> args)
    {
        switch (id)
        {
            case "gold":
                return new GoldObj(args);
            default:
                break;
        }

        return null;
    }

    public AddObj(List<string> args)
    {

    }
}

public class GoldObj : AddObj
{
    public GoldObj(List<string> args) : base(args)
    {
        int num = int.Parse(args[0]);
        int campIndex = int.Parse(args[1]);
        int cityIndex = int.Parse(args[2]);

        Camp camp = GameCenter.instance.GetCamp(campIndex);
        City city = camp.GetCity(cityIndex);

        city.battleResourse.gold += num;
    }
}