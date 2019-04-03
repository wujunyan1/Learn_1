using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

// 添加建筑列表
public class AddBuildList : CityBuildView
{
    public TavernView tavernView;

    /// <summary>
    /// 列表
    /// </summary>
    public RectTransform listContent;

    public Button addButtonPrefab;

    private void Awake()
    {
    }

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

        foreach (CityBuildType type in Enum.GetValues(typeof(CityBuildType)))
        {
            Button item = Instantiate(addButtonPrefab);
            item.transform.SetParent(listContent, false);
            item.onClick.AddListener(delegate {
                OnBuildBtn(type);
            });
            Text buildName = item.transform.Find("Text").GetComponent<Text>();
            buildName.text = string.Format("{0}", type);
        }

    }

    void OnBuildBtn(CityBuildType type)
    {
        CityBuildFactory.CreateCityBuild(CurrCity, type);
        cityView.UpdateBuildList();
    }
}
