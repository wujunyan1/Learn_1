using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroFactory : MonoBehaviour
{
    public static int nextId = 0;

    static int GetNextId()
    {
        return nextId++;
    }

    public static Hero CreateHero()
    {
        Hero newHero = new Hero(GetNextId());
        newHero.resId = HeroConfigPool.GetRandomHeroRes().id;
        newHero.name = "蔡文姬";
        return newHero;
    }
}
