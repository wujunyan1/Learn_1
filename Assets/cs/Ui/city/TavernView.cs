using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TavernView : CityBuildView
{
    /// <summary>
    /// 列表
    /// </summary>
    public RectTransform listContent;

    Tavern tavern;

    public HeroRecruit itemPrefab;

    public override void Open()
    {

        UpdateView();
        gameObject.SetActive(true);
    }

    public override void Close()
    {
        gameObject.SetActive(false);
    }

    public override void UpdateView()
    {
        // 移除原来的数据
        for (int i = 0; i < listContent.childCount; i++)
        {
            Destroy(listContent.GetChild(i).gameObject);
        }

        List<Hero> heros = tavern.GetHeros();
        for (int index = 0; index < heros.Count; index++)
        {
            //Hero hero = heros[index];
            HeroRecruit item = Instantiate(itemPrefab);
            
            item.transform.SetParent(listContent, false);

            Button btn = item.GetComponentInChildren<Button>();
            btn.onClick.AddListener(
                delegate
                {
                    OnRecruit(index);
                }
                );
        }
    }

    public override void SetCurrCity(City city)
    {
        base.SetCurrCity(city);
        tavern = currCity.GetBuild<Tavern>();
    }

    public void OnRecruit(int index) {
        Debug.Log(string.Format("OnRecruit {0}", index));

        List<Hero> heros = tavern.GetHeros();
        Hero hero = heros[index];
        Barracks barracks = currCity.GetBuild<Barracks>();
        barracks.AddHero(hero);

        heros.RemoveAt(index);

        UpdateView();
    }
}
