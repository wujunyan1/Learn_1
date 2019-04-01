using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 酒馆 招募野英雄
/// </summary>
public class Tavern : CityBuild
{
    List<Hero> heros;

    int maxHeroNum;

    public Tavern()
    {
        level = 1;
        maxLevel = 5;

        maxHeroNum = level;

        heros = new List<Hero>();
    }

    public override void AddLevel()
    {
        base.AddLevel();
        maxHeroNum = level;
    }


}
