using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroConfigPool : MonoBehaviour
{
    public static HeroRes[] heroRes;

    private void Start()
    {
        
    }

    public static HeroRes GetRandomHeroRes()
    {
        int index = Random.Range(0, heroRes.Length);

        return heroRes[index];
    }
}
