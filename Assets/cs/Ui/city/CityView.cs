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

    public void Open(City city)
    {
        currCity = city;

        UpdateView();
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void UpdateView()
    {
        cityName.text = currCity.Name;

        // 移除原来的数据
        for (int i = 0; i < listContent.childCount; i++)
        {
            Destroy(listContent.GetChild(i).gameObject);
        }

        List<CityBuild> builds = currCity.GetBuilds();
        foreach(CityBuild build in builds)
        {

        }

        Button item = Instantiate(addButtonPrefab);
        item.transform.SetParent(listContent, false);
        item.onClick.AddListener(OnAddBuildBtn);

    }

    void OnAddBuildBtn()
    {

    }
}
