using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityView : View
{
    public Sprite[] buidSprites;
    public RectTransform buildPrefab;
    

    public Text cityNameText;
    public RectTransform cityBuildListPanel;

    // 城市信息
    public Text cityNameText2;
    public Text personText;
    public Text goldText;
    public Text provisionsText;
    public Text troopProvisionsText;
    public Text woodText;
    public Text ironText;

    // 某建筑升级
    public RectTransform buildUpgradePanel;
    public Image upgradImage;
    public Text upgradNameText;
    public Text upgradGoldText;
    public Text upgradWoodText;
    public Text upgradRoundText;

    public RectTransform addBuild;

    //  建造建筑列表
    public ScrollRect scrollRect;
    public CreateCityBuild scrollRectBuildPrefab;


    // 建筑和招募切换按钮
    public Button buildViewBtn;
    public Button recruitViewBtn;

    int viewIndex = 0;


    // 招募队列
    public RectTransform recruitList;
    public RectTransform recruitBtn;
    public RecruitTroopCard recruitTroopCardPrefab;

    //  招募列表
    public RectTransform canRecruitList;

    // 建造农田
    public Button buildFarmBtn;


    City city;

    private void Start()
    {
        addBuild.GetComponent<Button>().onClick.AddListener(AddNewBuildBtn);
        recruitBtn.GetComponent<Button>().onClick.AddListener(ShowCanRecruitList);

        buildUpgradePanel.GetComponent<Button>().onClick.AddListener(OnBtnUpgradeBuild);

        buildViewBtn.onClick.AddListener(delegate() {
            viewIndex = 0;
            UpdateView();
        });

        recruitViewBtn.onClick.AddListener(delegate () {
            viewIndex = 1;
            UpdateView();
        });

        buildFarmBtn.onClick.AddListener(OnBtnBuildFarm);

        RegisterEvent("NextRoundEnd", NextRoundEndUpdateView);

        RegisterEvent("LeftMouseButton", LeftMouseButton);
        RegisterEvent("RightMouseButton", RightMouseButton);
        RegisterEvent("LeftMouseButtonBlank", LeftMouseButtonBlank);
    }

    public override void Open(UObject o)
    {
        base.Open(o);
        city = o.GetT<City>("City", null);

        // 默认为建筑
        viewIndex = 0;

        UpdateView();
    }

    public override void Show()
    {
        base.Show();
    }

    public override void Close()
    {
        base.Close();
    }

    void NextRoundEndUpdateView(UEvent e)
    {
        if (this.gameObject.activeSelf)
        {
            UpdateView();
        }
    }

    public override void UpdateView()
    {
        base.UpdateView();

        cityNameText.text = city.Name;


        cityBuildListPanel.gameObject.SetActive(false);
        recruitList.gameObject.SetActive(false);
        canRecruitList.gameObject.SetActive(false);

        if (viewIndex == 0)
        {
            cityBuildListPanel.gameObject.SetActive(true);
            _UpdateBuildList();
        }
        else if(viewIndex == 1)
        {
            recruitList.gameObject.SetActive(true);
            _UpdateRecruitList();
        }

        _UpdateCityMessage();
    }

    // 更新建筑列表
    void _UpdateBuildList()
    {
        List<CityBuild> list = city.GetBuilds();

        int buildCount = list.Count;
        int count = cityBuildListPanel.childCount;

        for (int i = 0; i < count; i++)
        {
            Transform child = cityBuildListPanel.GetChild(i);
            child.gameObject.SetActive(false);
        }
        

        for (int i = 0; i < buildCount; i++)
        {
            CityBuild cityBuild = list[i];

            Transform child;
            if (i < count - 1)
            {
                child = cityBuildListPanel.GetChild(i);
                child.gameObject.SetActive(true);
            }
            else
            {
                child = GameObject.Instantiate<Transform>(buildPrefab, cityBuildListPanel);

                Button btn = child.GetComponent<Button>();
                int index = i;
                btn.onClick.AddListener(delegate () {
                    OnClickBuildBtn(index);
                });
            }

            Image image = child.GetChild(0).GetComponent<Image>();
            Text level = child.GetChild(1).GetComponent<Text>();
            Image cdImage = child.GetChild(2).GetComponent<Image>();
            Text wait = child.GetChild(3).GetComponent<Text>();

            image.sprite = buidSprites[(int)cityBuild.BuildType];
            level.text = BaseConstant.LevelText[cityBuild.GetLevel()];

            // 正在建造中，则灰掉
            bool isBuilding = cityBuild.IsBuilding();
            if (isBuilding)
            {
                cdImage.gameObject.SetActive(true);
                wait.gameObject.SetActive(true);
                wait.text = cityBuild.GetBuildedNeedRoundNum().ToString();

                image.color = new Color32(63, 63, 63, 255);
            }
            else
            {
                cdImage.gameObject.SetActive(false);
                wait.gameObject.SetActive(false);

                image.color = new Color32(255, 255, 255, 255);
            }

        }

        int maxBuildNum = city.GetCityProperty(CityProperty.maxBuildNum);

        if (maxBuildNum > buildCount)
        {
            addBuild.gameObject.SetActive(true);
        }
        addBuild.SetAsLastSibling();
    }

    // 更新城市信息
    void _UpdateCityMessage()
    {
        cityNameText2.text = city.Name;
        personText.text = LanguageText.人口_ADD_MAX_3.Text(
            city.battleResourse.persons, 
            city.GetCityProperty(CityProperty.add_persons), 
            city.GetCityProperty(CityProperty.maxPerson));

        goldText.text = LanguageText.金币_ADD_2.Text(
            city.battleResourse.gold, 
            city.GetCityProperty(CityProperty.add_gold));

        provisionsText.text = LanguageText.民粮_ADD_2.Text(
            city.battleResourse.provisions,
            city.GetCityProperty(CityProperty.add_provisions));

        troopProvisionsText.text = LanguageText.军粮_ADD_2.Text(
            city.battleResourse.troopProvisions,
            city.GetCityProperty(CityProperty.add_troopProvisions));

        woodText.text = LanguageText.物资_ADD_2.Text(
            city.battleResourse.materials,
            city.GetCityProperty(CityProperty.add_materials));

        ironText.text = LanguageText.铁料_ADD_2.Text(
            city.battleResourse.iron,
            city.GetCityProperty(CityProperty.add_iron));
    }

    void _UpdateAddCityBuildPanel()
    {
    }

    // 更新招募列表
    void _UpdateRecruitList()
    {
        List<RecruitTroopData> list = city.GetRecruitList();

        ScrollRect scrollRect = recruitList.GetComponent<ScrollRect>();
        RectTransform recruitContent = scrollRect.content;

        int recruitCount = list.Count;
        int count = recruitContent.childCount;

        for (int i = 0; i < count; i++)
        {
            Transform child = recruitContent.GetChild(i);
            child.gameObject.SetActive(false);
        }


        for (int i = 0; i < recruitCount; i++)
        {
            RecruitTroopData recruit = list[i];

            Transform child;
            RecruitTroopCard recruitTroopCard;
            if (i < count - 1)
            {
                child = recruitContent.GetChild(i);
                child.gameObject.SetActive(true);
                recruitTroopCard = child.GetComponent<RecruitTroopCard>();
            }
            else
            {
                recruitTroopCard = GameObject.Instantiate<RecruitTroopCard>(recruitTroopCardPrefab, recruitContent);
                child = recruitTroopCard.transform;

                int index = i;
                recruitTroopCard.SetIndex(index);
                recruitTroopCard.GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    OnBtnRecruitCard(index);
                });
            }
            recruitTroopCard.SetRecruitTroopData(recruit);
        }

        recruitBtn.gameObject.SetActive(true);
        recruitBtn.SetAsLastSibling();

        recruitContent.sizeDelta = new Vector2(60 * (recruitCount + 1), 0);
    }

    List<int> troopsConfigDatas;
    void _UpdateCanRecruitList()
    {
        troopsConfigDatas = city.CanRecruitTroopList();


        ScrollRect scrollRect = canRecruitList.GetComponent<ScrollRect>();
        RectTransform recruitContent = scrollRect.content;

        int recruitCount = troopsConfigDatas.Count;
        int count = recruitContent.childCount;

        for (int i = 0; i < count; i++)
        {
            Transform child = recruitContent.GetChild(i);
            child.gameObject.SetActive(false);
        }


        for (int i = 0; i < recruitCount; i++)
        {
            int troopKey = troopsConfigDatas[i];

            Transform child;
            RecruitTroopCard recruitTroopCard;
            if (i < count - 1)
            {
                child = recruitContent.GetChild(i);
                child.gameObject.SetActive(true);
                recruitTroopCard = child.GetComponent<RecruitTroopCard>();
            }
            else
            {
                recruitTroopCard = GameObject.Instantiate<RecruitTroopCard>(recruitTroopCardPrefab, recruitContent);
                child = recruitTroopCard.transform;

                int index = i;
                recruitTroopCard.SetIndex(index);
                recruitTroopCard.GetComponent<Button>().onClick.AddListener(delegate()
                {
                    OnBtnCanRecruitCard(index);
                });
            }
            recruitTroopCard.SetRecruitTroopCardKey(troopKey);
        }
        
        recruitContent.sizeDelta = new Vector2(60 * recruitCount, 0);
    }

    void ShowCanRecruitList()
    {
        if (canRecruitList.gameObject.activeSelf)
        {
            canRecruitList.gameObject.SetActive(false);
            return;
        }

        canRecruitList.gameObject.SetActive(true);
        _UpdateCanRecruitList();
    }

    // 点击了正在招募，取消招募
    void OnBtnRecruitCard(int index)
    {
        city.RemoveRecruitList(index);
    }

    // 选择招募
    void OnBtnCanRecruitCard(int index)
    {
        int key = troopsConfigDatas[index];
        
        if ( city.CanRecruit(key) && city.HasResRecruit(key))
        {
            city.AddRecruitList(key);

            canRecruitList.gameObject.SetActive(false);
            UpdateView();
        }
    }


    void OnClickBuildBtn(int index)
    {
        CityBuild build = city.GetBuild(index);
        Transform childBtn = cityBuildListPanel.GetChild(index);
        Vector3 pos = childBtn.localPosition;

        List<CityBuildConfig> cityBuildConfigs = CityBuildConfigExtend.GetCityBuildConfigs(build.BuildType);

        
        foreach (var item in cityBuildConfigs)
        {
            if(item.level == build.GetLevel() + 1)
            {
                ShowUpgradePane(build, item, pos);
                return;
            }
        }
    }

    CityBuild currbuild = null;
    void ShowUpgradePane(CityBuild build, CityBuildConfig nextConfig, Vector3 pos)
    {
        if(currbuild != null && currbuild == build)
        {
            buildUpgradePanel.gameObject.SetActive(false);
            currbuild = null;
            return;
        }
        currbuild = build;

        CityBuildConfig config = build.GetCityBuildConfig();

        buildUpgradePanel.gameObject.SetActive(true);

        buildUpgradePanel.localPosition = pos + new Vector3(0, 30, 0);

        upgradImage.sprite = buidSprites[(int)build.BuildType];
        upgradNameText.text = nextConfig.name;
        upgradGoldText.text = nextConfig.gold.ToString();
        upgradWoodText.text = nextConfig.materials.ToString();
        upgradRoundText.text = nextConfig.buildRound.ToString();
    }

    void OnBtnUpgradeBuild()
    {
        if(currbuild == null)
        {
            return;
        }

        CityBuildConfig config = currbuild.GetCityBuildConfig();

        CityBuildConfig nextConfig = CityBuildConfigExtend.GetCityBuildConfig(config.BuildType, config.level + 1);
        if(nextConfig == null)
        {
            return;
        }

        if (city.CanUpgrade(nextConfig.BuildType, nextConfig.level))
        {
            city.UpgradeCityBuild(currbuild);
            buildUpgradePanel.gameObject.SetActive(false);
            currbuild = null;
            UpdateView();
        }
    }

    void AddNewBuildBtn()
    {
        Debug.Log("AddNewBuildBtn ");

        if (scrollRect.gameObject.activeSelf)
        {
            CloseAddNewBuildPanel();
        }
        else
        {
            ShowAddNewBuildPanel();
        }

    }

    bool isLoadAddNewBuildPanel = false;

    RectTransform LoadAddNewBuildModel(CityBuildConfig config)
    {
        CreateCityBuild createCityBuild = GameObject.Instantiate<CreateCityBuild>(scrollRectBuildPrefab);
        createCityBuild.SetConfig(config);
        createCityBuild.UpdateView();

        Button btn = createCityBuild.GetComponent<Button>();
        btn.onClick.AddListener(delegate() {
            OnBtnCreateCityBuild(config);
        });

        return createCityBuild.GetComponent<RectTransform>();
    }

    void LoadAddNewBuildPanel()
    {
        float maxHeight = 370;
        float maxWidth = 350;
        List<List<CityBuildConfig>> allConfigs = CityBuildConfigExtend.GetAllCityBuildConfigs();

        float pos_x = -15;
        float pos_y = -35;
        for (int x = 0; x < allConfigs.Count; x++)
        {
            List<CityBuildConfig> configs = allConfigs[x];
            pos_x += 70;

            pos_y = -35;
            
            if (pos_x > maxWidth - 35)
            {
                maxWidth = pos_x + 35;
            }

            for (int y = 0; y < configs.Count; y++)
            {
                CityBuildConfig config = configs[y];
                if (config.level != 0)
                {
                    pos_y = (85 * config.level) - 15;
                    
                    if (pos_y > maxHeight - 25)
                    {
                        maxHeight = pos_y + 25;
                    }

                    RectTransform rectTransform = LoadAddNewBuildModel(config);
                    rectTransform.SetParent(scrollRect.content);

                    rectTransform.localPosition = new Vector3(pos_x, pos_y, 0);
                }
            }
        }

        scrollRect.content.sizeDelta = new Vector2(maxWidth, maxHeight);
    }

    void ShowAddNewBuildPanel()
    {
        scrollRect.gameObject.SetActive(true);

        if (!isLoadAddNewBuildPanel)
        {
            LoadAddNewBuildPanel();
            isLoadAddNewBuildPanel = true;
        }

        UpdateAddNewBuildPanel();
    }

    void UpdateAddNewBuildPanel()
    {
        List<List<CityBuildConfig>> allConfigs = CityBuildConfigExtend.GetAllCityBuildConfigs();

        int index = 0;

        for (int x = 0; x < allConfigs.Count; x++)
        {
            List<CityBuildConfig> configs = allConfigs[x];
            for (int y = 0; y < configs.Count; y++)
            {
                CityBuildConfig config = configs[y];
                if (config.level != 0)
                {
                    Transform child = scrollRect.content.GetChild(index);

                    CreateCityBuild createCityBuild = child.GetComponent<CreateCityBuild>();


                    createCityBuild.SetCanCreate(city.CanBuild((CityBuildType)config.type, config.level));

                    index++;
                }
            }
        }
    }

    void CloseAddNewBuildPanel()
    {
        scrollRect.gameObject.SetActive(false);
    }

    void OnBtnCreateCityBuild(CityBuildConfig config)
    {
        if(city.CanBuild((CityBuildType)config.type, config.level))
        {
            city.BuildCityBuild(config);

            CloseAddNewBuildPanel();
            UpdateView();
        }
    }


    delegate void VoidEventHexCell(HexCell c);
    VoidEventHexCell func;

    public void LeftMouseButton(UEvent e)
    {
        HexCell cell = (HexCell)e.eventParams;

        if (showCells != null && func != null)
        {
            foreach (HexCell c in showCells)
            {
                if(c == cell)
                {
                    func.Invoke(c);
                }
            }
        }
    }

    public void RightMouseButton(UEvent e)
    {
    }

    public void LeftMouseButtonBlank(UEvent e)
    {
    }



    void OnBtnBuildFarm()
    {
        if(showCells != null)
        {
            CloseCanBuildMapBuildCells();
        }
        else
        {
            ShowCanBuildMapBuildCells();
        }
    }

    List<HexCell> showCells = null;
    private void ShowCanBuildMapBuildCells()
    {
        showCells = ListPool<HexCell>.Get();
        List<HexCell> cells = city.GetHexCells();
        foreach (HexCell cell in cells)
        {
            Debug.Log("cells");
            if (cell.CanBuild(BuildType.Farm))
            {
                Debug.Log("xxxxxxxx");
                showCells.Add(cell);
            }
        }

        HexGrid.instance.ShowPath(showCells);

        func = BuildFarm;
    }

    void CloseCanBuildMapBuildCells()
    {
        if(showCells != null)
        {
            HexGrid.instance.ClearShowPath(showCells);

            ListPool<HexCell>.Add(showCells);
            showCells = null;
            func = null;
        }
    }

    void BuildFarm(HexCell cell)
    {
        Farm farm = (Farm)MapBuildFactory.CreateMapBuild(BuildType.Farm); // new Farm();
        farm.Location = cell;

        farm.RefreshModel();
    }






    protected override void DestroyEnd()
    {
        base.DestroyEnd();

        CloseCanBuildMapBuildCells();
    }
}
