using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 阵容
/// </summary>
public class Camp : RoundObject
{
    // id
    int id;

    // 名称
    string name;

    // 颜色
    Color color;

    // 拥有的城市
    List<City> citys;

    // 金钱
    int money;

    public override void NextRound()
    {
        foreach(City city in citys)
        {
            city.NextRound();
        }
    }
}
