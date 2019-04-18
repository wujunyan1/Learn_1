using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroConfigPool : MonoBehaviour
{
    public static HeroRes[] heroRes;

    private void Awake()
    {
        heroRes = new HeroRes[transform.childCount];

        for(int i = 0; i < transform.childCount; i++)
        {
            HeroRes res = transform.GetChild(i).GetComponent<HeroRes>();
            heroRes[i] = res;
            res.index = i;
        }
    }

    public static HeroRes GetRandomHeroRes()
    {
        int index = Random.Range(0, heroRes.Length);

        return heroRes[index];
    }

    public static HeroRes GetHeroRes(int index)
    {
        return heroRes[index];
    }

    public static HeroRes GetHeroRes(string id)
    {
        for (int i = 0; i < heroRes.Length; i++)
        {
            HeroRes res = heroRes[i];
            if(res.id == id)
            {
                return res;
            }
        }

        return null;
    }
}
