using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityView : MonoBehaviour
{
    public HexMapEditor showUIManager;

    City currCity;

    public Text cityName;

    /// <summary>
    /// 列表
    /// </summary>
    public RectTransform listContent;

    // 添加新建筑
    public Button addButtonPrefab;

    public Button cityBuildPrefab;

    // 当前显示的视图
    CityBuildView currShowView;

    public AddBuildList addBuildListView;

    public CityBuildView[] views;

    public void Open(City city)
    {
        currCity = city;

        UpdateView();
        gameObject.SetActive(true);
        OnAddBuildBtn();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void UpdateView()
    {
        cityName.text = currCity.Name;

        UpdateBuildList();

    }

    public void UpdateBuildList()
    {
        // 移除原来的数据
        for (int i = 0; i < listContent.childCount; i++)
        {
            Destroy(listContent.GetChild(i).gameObject);
        }

        List<CityBuild> builds = currCity.GetBuilds();
        foreach (CityBuild build in builds)
        {
            Button buildItem = Instantiate(cityBuildPrefab);
            buildItem.transform.SetParent(listContent, false);
            buildItem.onClick.AddListener(
                delegate
                {
                    OpenCityBuildView(build.BuildType);
                }
                );
            buildItem.GetComponentInChildren<Text>().text = build.Name;
        }

        Button item = Instantiate(addButtonPrefab);
        item.transform.SetParent(listContent, false);
        item.onClick.AddListener(OnAddBuildBtn);
    }

    void OnAddBuildBtn()
    {
        ChangeView(addBuildListView);
    }

    void OpenCityBuildView(CityBuildType type)
    {
        CityBuildView view = views[(int)type];
        ChangeView(view);
    }

    void ChangeView(CityBuildView newView)
    {
        if (currShowView)
        {
            currShowView.Close();
        }

        currShowView = newView;
        currShowView.SetCurrCity(currCity);
        currShowView.Open();
    }
}
