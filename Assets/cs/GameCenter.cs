using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class GameCenter : EventComponent, SaveLoadInterface
{
    public static GameCenter instance;

    public HexGrid grid;
    public HexMapGenerator mapGenerator;

    private Camera mainCamera;
    public Camera miniCamera;

    public Texture2D campColorTexture;
    public Color32[] campColors;

    public ObjGenerate objGenerate;

    public NewGameData gameData;

    int playerCampId = 0;

    /// <summary>
    /// 玩家阵营默认为0
    /// </summary>
    public int PlayerCampId {
        get
        {
            return playerCampId;
        }
    }

    /// <summary>
    /// 回合数
    /// </summary>
    int round;

    // 每回合经历的天数
    int roundDay;

    public Text roundText;
    public GameObject basePanel;

    // 阵营只会增加不会减少
    List<Camp> camps;

    // 部队
    List<Troop> troops;

    List<MapBuild> mapBuilds;

    View editorView;

    // 季节
    static EnumSeason season;
    public static EnumSeason Seanson
    {
        get
        {
            return season;
        }
    }

    private void Awake()
    {
        instance = this;
        camps = new List<Camp>();
        troops = new List<Troop>();

        mapBuilds = new List<MapBuild>();

        objGenerate = ObjGenerate.GetInstance();
        
    }

    private void Start()
    {
        mainCamera = Camera.main;  // FindObjectOfType<Camera>();

        // Shader.SetGlobalTexture("_campColor", campColorTexture);
        
        this.campColors = campColorTexture.GetPixels32();
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(round);
        writer.Write((byte)playerCampId);

        gameData.Save(writer);
        grid.Save(writer);

        writer.Write((byte)camps.Count);
        Debug.Log("save camp count" + (byte)camps.Count);
        foreach (Camp camp in camps)
        {
            camp.Save(writer);
        }

        writer.Write(mapBuilds.Count);
        foreach (MapBuild build in mapBuilds)
        {
            writer.Write((byte)build.BuildType);

            build.Save(writer);
        }
    }

    public IEnumerator Load(BinaryReader reader)
    {
        round = reader.ReadInt32();
        season = (EnumSeason)Mathf.FloorToInt((round % BaseConstant.RoundOfYear) / 10);

        playerCampId = reader.ReadByte();

        if (gameData == null)
        {
            gameData = new NewGameData(1,1);
        }

        IEnumerator itor = gameData.Load(reader);
        while (itor.MoveNext())
        {
            yield return null;
        }

        itor = grid.Load(reader);
        while (itor.MoveNext())
        {
            yield return null;
        }

        int count = reader.ReadByte();
        Debug.Log("load camp count" + count);

        //清除原来的数据
        foreach (Camp camp in camps)
        {
            camp.ClearData();
        }
        camps.Clear();

        // 阵营
        for (int i = 0; i < count; i++)
        {
            Camp camp = AddCamp();
            itor = camp.Load(reader);
            while (itor.MoveNext())
            {
                yield return null;
            }
        }

        count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            BuildType buildType = (BuildType)reader.ReadByte();
            MapBuild build = MapBuildFactory.CreateMapBuild(buildType);
            itor = build.Load(reader);
            while (itor.MoveNext())
            {
                yield return null;
            }
        }


        LoadGenerateMapEnd();
    }

    public void StartLoad(BinaryReader reader)
    {
        StartCoroutine(Load(reader));
    }
    
    public IEnumerator GenerateMap(NewGameData data)
    {
        gameData = data;
        
        round = 0;

        CreaterCamp();

        IEnumerator tor;
        if (data.isRandMap)
        {
            tor = mapGenerator.GenerateMap(data);
        }
        else
        {
            tor = grid.CreateMap(data);
        }
        

        while (tor.MoveNext())
        {
            yield return null;
        }

        yield return null;

        LoadGenerateMapEnd();
        yield return null;
    }

    /// <summary>
    /// 数据加载完之后
    /// </summary>
    void LoadGenerateMapEnd()
    {
        BattlePlayerControl control = BattlePlayerControl.GetInstance();

        // 数据Load完之后再刷地图
        grid.StartGenerateMap();

        HexMapCamera.ValidatePosition();

        mapGenerator.SetNewGameData(gameData);


        // 再修改摄像机位置，放置到对应阵营建造者上
        //Transform transform = GetPlayerCenterTransform();
        //ResetCamera(transform);

        // roundText.text = string.Format("{0}", round);
        //basePanel.SetActive(true);

        Vector2 mapSize = grid.GetMapSize();
        miniCamera.transform.localPosition = new Vector3(mapSize.x/2, 200, mapSize.y /2);
    }


    public void NextRound()
    {
        round++;

        season = (EnumSeason)Mathf.FloorToInt((round % BaseConstant.RoundOfYear) / BaseConstant.RoundOfEeason);

        mapGenerator.NextRound();

        grid.NextRound();

        if (_showTypeIndex != -1)
        {
            ShowMapData(_showTypeIndex);
        }

        foreach (var camp in camps)
        {
            camp.NextRound();
        }


        LaterNextRound();

        this.FireEvent("NextRoundEnd", round);
    }

    public void LaterNextRound()
    {
        grid.LaterNextRound();

        foreach (var camp in camps)
        {
            camp.LaterNextRound();
        }
    }


    // 生成初始化数据
    public void GenerateInitialData()
    {
        season = EnumSeason.spring;
        CreaterCamp();

        foreach (Camp camp in camps)
        {
            camp.GenerateInitialData();
        }
    }



    void CreaterCamp()
    {
        foreach (Camp camp in camps)
        {
            camp.ClearData();
        }
        camps.Clear();

        int campNum = gameData.campNum;
        for (int i = 0; i < campNum; i++)
        {
            AddCamp();
        }

        playerCampId = campNum - 1;
    }
    
    Camp AddCamp()
    {
        Camp camp = new Camp(camps.Count);
        camps.Add(camp);
        return camp;
    }

    public Camp GetCamp(int id)
    {
        return camps[id];
    }

    public void GenerateCreater(Creater creater)
    {
        objGenerate.GenerateCreater(creater);
    }

    public TroopControl GenerateTroopModel(Troop person)
    {
        TroopControl control = objGenerate.GenerateTroopModel(person);
        return control;
    }

    public Troop GenerateNewTroop(HexCell cell)
    {
        Troop troop = GetNewTroop();
        troop.IsNew();
        
        TroopControl troopControl = GenerateTroopModel(troop);
        troopControl.Location = cell;
        cell.Troop = troop;

        return troop;
    }

    public Troop GenerateNewTroop(Troop troop, HexCell cell)
    {
        TroopControl troopControl = GenerateTroopModel(troop);
        troopControl.Location = cell;
        cell.Troop = troop;

        return troop;
    }

    public Troop GetNewTroop()
    {
        Troop troop = new Troop();
        troops.Add(troop);

        return troop;
    }

    /// <summary>
    /// 杀死某个部队
    /// </summary>
    /// <param name="troop"></param>
    public void KillTroop(Troop troop)
    {
        troop.camp.RemoveTroop(troop);

        GameObject.Destroy(troop.control);
        troops.Remove(troop);
    }

    /// <summary>
    /// 杀死某个部队
    /// </summary>
    /// <param name="troop"></param>
    public void LaterKillTroop(Troop troop)
    {
        troop.camp.LaterRemoveTroop(troop);

        GameObject.Destroy(troop.control.gameObject);
        troops.Remove(troop);
    }



    public CityControl GenerateCityModel()
    {
        CityControl control = objGenerate.GenerateCityModel();
        return control;
    }

    public City GenerateNewCity()
    {
        CityControl cityControl = GenerateCityModel();

        City city = new City();
        city.SetModel(cityControl);

        mapBuilds.Add(city);

        return city;
    }


    public Farm GenerateNewFarm()
    {
        FarmControl farmControl = objGenerate.GenerateFarmModel();

        Farm farm = new Farm();
        farm.SetModel(farmControl);

        mapBuilds.Add(farm);

        return farm;
    }










    /// <summary>
    /// 获取玩家的中心位置
    /// </summary>
    /// <returns></returns>
    public Transform GetPlayerCenterTransform()
    {
        Camp camp = GetCamp(playerCampId);
        City city = camp.GetCity(0);
        if(city != null)
        {
            HexCell cityCell = city.GetCenter();
            return cityCell.transform;
        }

        Troop troop = camp.GetTroop(0);
        HexCell cell = troop.control.Location;
        return cell.transform;
    }

    /// <summary>
    /// 使摄像机 观看这个对象
    /// </summary>
    /// <param name="transform"></param>
    public void ResetCamera(Transform transform)
    {
        Vector3 vector = transform.localPosition;
        vector.y += 10;
        mainCamera.transform.localPosition = vector;

        mainCamera.transform.Translate(Vector3.forward * -1 * 30);
    }

    public void Update()
    {
        if(gameData != null && gameData.isCanEditor)
        {
            if (Input.GetKeyUp(KeyCode.P))
            {
                ShowOrHideHexMapEditor();
            }
        }
       
    }

    void ShowOrHideHexMapEditor()
    {
        if(editorView != null)
        {
            ViewManager.Instance.RemoveView(editorView);
            editorView = null;
        }
        else
        {
            ViewManager.Instance.OpenView("HexMapEditorView", out editorView);
        }
    }


    int _showTypeIndex = -1;
    public void ShowMapData(int showTypeIndex)
    {
        _showTypeIndex = showTypeIndex;
        for (int i = 0; i < gameData.CellCount; i++)
        {
            SetCellMapData(i, showTypeIndex);
        }

        HexGrid.instance.SetCellMapDataShader(true);
    }

    public void HideMapData()
    {
        grid.SetCellMapDataShader(false);

        _showTypeIndex = -1;

        for (int i = 0; i < gameData.CellCount; i++)
        {
            HideCellMapData(i);
        }
    }

    void SetCellMapData(int cellIndex, int mapDataIndex)
    {
        HexCell cell = grid.GetCell(cellIndex);
        ClimateData climate = grid.GetClimateData(cellIndex);

        cell.SetLabel(null);
        cell.SetLabelDir(0);

        float showData = 0;
        switch (mapDataIndex)
        {
            case 0: // 风
                showData = climate.wind.magnitude / 12;
                if (climate.wind.magnitude > 0.001f)
                {
                    float angle = Vector2Tool.Angle(Vector2.up, climate.wind, true);
                    //cell.SetLabel(string.Format("{0}", Mathf.RoundToInt(angle)));
                    float num = climate.wind.magnitude;
                    // num = climate.clouds;
                    //num = climateData.temperature * 100;
                    cell.SetLabel("个个"+ Mathf.RoundToInt(num) );
                    cell.SetLabelDir(360 - angle);
                }

                break;
            case 1: // 温度
                showData = climate.temperature;
                break;
            case 2: // 云
                showData = climate.clouds;
                break;
            case 3: // 雨
                showData = climate.moisture;
                break;
        }

        cell.SetLabel(cell.Vector.toString2());
        cell.SetMapData(showData);
    }

    void HideCellMapData(int cellIndex)
    {
        HexCell cell = grid.GetCell(cellIndex);
        cell.SetLabel(null);
        cell.SetLabelDir(0);
    }
}
