using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityControl : EventComponent
{
    City city;

    List<GameObject> houses = ListPool<GameObject>.Get();

    public Transform cityCenter;
    public OnMouseEvent mouseEvent;
    public Text nameText;

    static string[] houseIndexs =
    {
        "House_1", "House_2", "House_3", "House_4", "House_5"
    };

    public static Vector3[] housePos =
    {
        new Vector3(10, 0, 10),
        new Vector3(10, 0, 0),
        new Vector3(10, 0, -10),
        new Vector3(0, 0, 10),
        new Vector3(0, 0, -10),
        new Vector3(-10, 0, 10),
        new Vector3(-10, 0, 0),
        new Vector3(-10, 0, -10),
    };

    private void Awake()
    {
    }

    private void Start()
    {
        mouseEvent.RegisterListener("OnPointerClick", delegate (UEvent e)
        {
            this.FireEvent("SelectCity", city);
        });
    }

    public void SetCity(City city)
    {
        this.city = city;
    }

    public void SetCityName(string name)
    {
        nameText.text = name;
    }

    protected override void DestroyEnd()
    {
        ListPool<GameObject>.Add(houses);
    }

    public void SetCenterRota(int angle)
    {
        cityCenter.localRotation = Quaternion.Euler(0, angle, 0);
    }
    

    public void ClearAllHouse()
    {
        if(houses != null)
        {
            foreach (var item in houses)
            {
                GameObject.Destroy(item);
            }

            houses.Clear();
        }
    }


    public void AddRandomHouse()
    {
        int index = Random.Range(0, 5);

    }

    public void AddHouse(int index, Vector3 pos, Vector3 rata)
    {
        GameObject house;
        PrefabsManager.GetInstance().GetGameObj( out house, houseIndexs[index]);

        house.transform.SetParent(transform);
        house.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        house.transform.localPosition = pos;
        house.transform.localRotation = Quaternion.Euler(rata);

        houses.Add(house);
    }

    public void AddHouse(City.HouseSite site)
    {
        AddHouse(site.index, 
            new Vector3(site.x, 0, site.z), new Vector3(0, site.angle, 0));
    }
    
}
