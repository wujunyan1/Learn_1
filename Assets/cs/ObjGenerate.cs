using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjGenerate : EventObject
{
    public City cityPrefab;

    public PersonInspectorView personUIShow;

    public FunctionGenerate functionGenerate;

    bool isf = true;

    private static ObjGenerate instance;

    public static ObjGenerate GetInstance()
    {
        if (instance == null)
        {
            instance = new ObjGenerate();
        }
        return instance;
    }

    private ObjGenerate()
    {
        
    }

    public TroopControl GenerateTroopModel(Troop person)
    {
        TroopsData data = person.data;
        string modelPath = data.config.model;
        string name = data.config.modelName;

        PrefabsManager prefabsManager = PrefabsManager.GetInstance();
        GameObject modelObj;
        prefabsManager.GetGameObj(out modelObj, name);


        GameObject troopObj;
        prefabsManager.GetGameObj(out troopObj, "Troop");
        

        TroopControl control = troopObj.GetComponent<TroopControl>();
        control.SetModel(modelObj);
        control.SetTroop(person);

        person.control = control;

        return control;
    }



    public void GenerateCreater(Creater creater)
    {
        GameObject createrPrefab;


        PrefabsManager.GetInstance().GetPrefab(out createrPrefab, "Creater");

        GameObject obj = Object.Instantiate(createrPrefab);
        CreaterControl item = obj.GetComponent<CreaterControl>();
        item.SetPerson(creater);
        item.controlView = personUIShow;
        //item.AddFunction(new MoveFunction());

        HexGrid grid = HexGrid.instance;
        //HexCell cell = grid.GetCell(creater.Point);

        //item.transform.localPosition = cell.transform.localPosition;
        item.Direction = (HexDirection)Random.Range(0, HexMetrics.HexDirectionNum);

        if (isf)
        {
            //new Vector3(0, 10, 0); //
            //item.transform.localPosition = new Vector3(0, 10, 0);
            isf = false;
        }
        item.transform.SetParent(grid.objPlane, false);

        //cell.Person = item;
    }


    public CityControl GenerateCityModel()
    {
        string name = "City";

        PrefabsManager prefabsManager = PrefabsManager.GetInstance();
        GameObject modelObj;
        prefabsManager.GetGameObj(out modelObj, name);

        CityControl city = modelObj.GetComponent<CityControl>();

        return city;
    }


    public City CreateCity()
    {
        //City item = Object.Instantiate(cityPrefab);

        //HexGrid grid = HexGrid.instance;
        //item.transform.SetParent(grid.objPlane, false);
        //return item;
        return null;
    }

    public City CreateCity(HexCoordinates coordinates)
    {
        //City item = Object.Instantiate(cityPrefab);

        //HexGrid grid = HexGrid.instance;
        //item.transform.SetParent(grid.objPlane, false);

        //item.BuildNewCity(coordinates);

        return null;
    }




    public FarmControl GenerateFarmModel()
    {
        string name = "Farm";

        PrefabsManager prefabsManager = PrefabsManager.GetInstance();
        GameObject modelObj;
        prefabsManager.GetGameObj(out modelObj, name);

        FarmControl farm = modelObj.GetComponent<FarmControl>();

        return farm;
    }

}
