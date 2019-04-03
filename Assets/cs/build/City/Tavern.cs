using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        name = "tavern";
        level = 0;
        maxLevel = 5;
        upgradeRound = 0;
        type = CityBuildType.Tavern;

        maxHeroNum = level;

        heros = new List<Hero>();
    }

    public override void Save(BinaryWriter writer)
    {
        base.Save(writer);

        int number = heros.Count;
        writer.Write((byte)number);
        foreach(Hero hero in heros)
        {
            hero.Save(writer);
        }
    }

    public override void Load(BinaryReader reader)
    {
        base.Load(reader);

        int number = reader.ReadByte();
        for(int i = 0; i < number; i++)
        {
            Hero hero = HeroFactory.CreateHero();
            hero.Load(reader);
            AddHero(hero);
        }
    }

    public override void AddLevel()
    {
        base.AddLevel();
        maxHeroNum = level;
    }

    public override void NextRound()
    {
        base.NextRound();

        RandomHero();
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
            if (hero.Id == heroId)
            {
                return hero;
            }
        }

        return null;
    }

    public List<Hero> GetHeros()
    {
        return heros;
    }

    void RandomHero()
    {
        for(int i = 0; i < maxHeroNum; )
        {
            // 这里有人
            if(i < heros.Count)
            {
                // 有80%概率继续留下
                if(Random.Range(0, 100) > 80)
                {
                    heros.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            // 没人
            else
            {
                // 有30%概率来个人
                if (Random.Range(0, 100) > 70)
                {
                    Debug.Log("AddHero");
                    AddHero(HeroFactory.CreateHero());
                }
                i++;
            }
        }
    }
}
