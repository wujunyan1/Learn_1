using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// 军营
public class Barracks : CityBuild
{
    // 该城市内的己方英雄
    List<Hero> heros;

    public Barracks()
    {
        name = "兵营";
        heros = new List<Hero>();
        type = CityBuildType.Barracks;
    }

    public override void Save(BinaryWriter writer)
    {
        base.Save(writer);

        int number = heros.Count;
        writer.Write((byte)number);
        foreach (Hero hero in heros)
        {
            hero.Save(writer);
        }
    }

    public override void Load(BinaryReader reader)
    {
        base.Load(reader);

        int number = reader.ReadByte();
        for (int i = 0; i < number; i++)
        {
            Hero hero = HeroFactory.CreateHero();
            hero.Load(reader);
            AddHero(hero);
        }
    }

    public void AddHero(Hero hero)
    {
        heros.Add(hero);
    }

    public void RemoveHero(Hero hero)
    {
        heros.Remove(hero);
    }

    public Hero GetHero(int heroId)
    {
        foreach (Hero hero in heros)
        {
            if(hero.Id == heroId)
            {
                return hero;
            }
        }

        return null;
    }
}
