using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleGroupView : EventComponent
{
    public RectTransform leftBattleGroup;
    public RectTransform leftSelectGroup;

    public RectTransform rightBattleGroup;
    public RectTransform rightSelectGroup;

    public RectTransform soldierPanelPrefab;


    public Slider octavesSlider;
    public Slider lacunaritySlider;
    public Slider persistenceSlider;

    public Text octavesText;
    public Text lacunarityText;
    public Text persistenceText;

    public InputField warMapSeedInput;
    public InputField frequencyInput;

    public Slider seasonSlider;
    public Text seasonText;

    public int warMapSeed = 23;

    /// <summary>
    /// 变化度
    /// </summary>
    public float frequency = 10;
    


    /// <summary>
    /// 战斗数据
    /// </summary>
    private BattleDataManager battleDataManager;

    /// <summary>
    /// 选择的士兵，需要点击2次才算选中，这个记录第一次
    /// </summary>
    private int selectCamp = -1;
    private string selectKey = "";

    /// <summary>
    /// 取消选择的兵
    /// </summary>
    private int cancelSelectCamp = -1;
    private int cancelSelectIndex = -1;

    private void Start()
    {
        if (HeroConfig.GetInstance().IsLoadSoldierConfig)
        {
            InitData();
        }
        else
        {
            StartCoroutine(LoadSoldierConfig());
        }
    }

    private IEnumerator LoadSoldierConfig()
    {
        yield return null;
        HeroConfig.GetInstance().LoadSoldierConfig();

        yield return null;
        InitData();
    }

    private void InitData()
    {
        InitBattleData();
        UpdataView();
    }

    /// <summary>
    /// 初始化战斗数据
    /// </summary>
    private void InitBattleData()
    {
        battleDataManager = new BattleDataManager(2, 0);
        warMapSeedInput.text = "10";
        frequencyInput.text = "2";
    }

    private void UpdataView()
    {
        UpdateLeftBattleGroup();
        UpdateLeftSelectGroup();

        UpdateRightBattleGroup();
        UpdateRightSelectGroup();
    }

    /// <summary>
    /// 更新左侧战斗队列
    /// </summary>
    private void UpdateLeftBattleGroup()
    {
        List<SoldierConfigData> soldierConfigDatas = battleDataManager.GetTeamData(0)
            .GetSoldierConfigs();

        // 更新前的数据暂时隐藏
        int count = leftBattleGroup.childCount;
        for (int i = 0; i < count; i++)
        {
            leftBattleGroup.GetChild(i).gameObject.SetActive(false);
        }

        int num = 0;
        foreach (var item in soldierConfigDatas)
        {
            SoldierConfigData soldierConfigData = item;

            Transform child;
            // 没有的则创建
            if (num >= count)
            {
                child = Instantiate<Transform>(soldierPanelPrefab, this.leftBattleGroup);
            }
            else
            {
                child = leftBattleGroup.GetChild(num);
            }

            int index = num;
            Button btn = child.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(delegate ()
            {
                CancelSoldier(0, index);
            });

            // 更新内容
            child.gameObject.SetActive(true);
            child.Find("name").GetComponent<Text>().text = soldierConfigData.solider_name;
            
            num++;
        }

        Vector2 sizeDelta = leftBattleGroup.sizeDelta;
        leftBattleGroup.sizeDelta = new Vector2(sizeDelta.x, Mathf.Max(num * 50, 200));
    }

    private void UpdateLeftSelectGroup()
    {
        Dictionary<string, SoldierConfigData> soldierConfigDatas = HeroConfig.GetInstance().GetSoldierConfigList();

        int count = leftSelectGroup.childCount;
        for(int i = 0; i < count; i++)
        {
            leftSelectGroup.GetChild(i).gameObject.SetActive(false);
        }

        int num = 0;
        foreach (var item in soldierConfigDatas)
        {
            SoldierConfigData soldierConfigData = item.Value;

            Transform child;
            if(num >= count)
            {
                child = Instantiate<Transform>(soldierPanelPrefab, this.leftSelectGroup);
            }
            else
            {
                child = leftSelectGroup.GetChild(num);
            }

            Button btn = child.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(delegate ()
            {
                SelectSoldier(0, soldierConfigData.key);
            });

            child.gameObject.SetActive(true);
            child.Find("name").GetComponent<Text>().text = soldierConfigData.solider_name;

            num++;
        }

        Vector2 sizeDelta = leftSelectGroup.sizeDelta;
        leftSelectGroup.sizeDelta = new Vector2(sizeDelta.x, Mathf.Max(num * 50, 300));
    }

    private void UpdateRightBattleGroup()
    {
        List<SoldierConfigData> soldierConfigDatas = battleDataManager.GetTeamData(1)
            .GetSoldierConfigs();

        // 更新前的数据暂时隐藏
        int count = rightBattleGroup.childCount;
        for (int i = 0; i < count; i++)
        {
            rightBattleGroup.GetChild(i).gameObject.SetActive(false);
        }

        int num = 0;
        foreach (var item in soldierConfigDatas)
        {
            SoldierConfigData soldierConfigData = item;

            Transform child;
            // 没有的则创建
            if (num >= count)
            {
                child = Instantiate<Transform>(soldierPanelPrefab, this.rightBattleGroup);
            }
            else
            {
                child = rightBattleGroup.GetChild(num);
            }

            int index = num;
            Button btn = child.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(delegate ()
            {
                CancelSoldier(1, index);
            });

            // 更新内容
            child.gameObject.SetActive(true);
            child.Find("name").GetComponent<Text>().text = soldierConfigData.solider_name;

            num++;
        }

        Vector2 sizeDelta = rightBattleGroup.sizeDelta;
        rightBattleGroup.sizeDelta = new Vector2(sizeDelta.x, Mathf.Max(num * 50, 200));
    }

    private void UpdateRightSelectGroup()
    {
        Dictionary<string, SoldierConfigData> soldierConfigDatas = HeroConfig.GetInstance().GetSoldierConfigList();

        int count = rightSelectGroup.childCount;
        for (int i = 0; i < count; i++)
        {
            rightSelectGroup.GetChild(i).gameObject.SetActive(false);
        }

        int num = 0;
        foreach (var item in soldierConfigDatas)
        {
            SoldierConfigData soldierConfigData = item.Value;

            Transform child;
            if (num >= count)
            {
                child = Instantiate<Transform>(soldierPanelPrefab, this.rightSelectGroup);
                            }
            else
            {
                child = rightSelectGroup.GetChild(num);
            }

            Button btn = child.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(delegate ()
            {
                SelectSoldier(1, soldierConfigData.key);
            });

            child.gameObject.SetActive(true);
            child.Find("name").GetComponent<Text>().text = soldierConfigData.solider_name;

            num++;
        }

        Vector2 sizeDelta = rightSelectGroup.sizeDelta;
        rightSelectGroup.sizeDelta = new Vector2(sizeDelta.x, Mathf.Max(num * 50, 300));
    }

    /// <summary>
    /// 选择了一个士兵
    /// </summary>
    /// <param name="comp"></param>
    /// <param name="key"></param>
    void SelectSoldier(int camp, string key)
    {
        if (selectCamp == camp && selectKey == key)
        {
            battleDataManager.AddSoldierByConfig(camp, key);
            if(camp == 0)
            {
                UpdateLeftBattleGroup();
            }
            else
            {
                UpdateRightBattleGroup();
            }

            selectKey = "";
            this.FireEvent("CloseSoldierMessageView");
        }
        else
        {
            selectCamp = camp;
            selectKey = key;

            BattleSoldierData data = battleDataManager.GetBattleSoldierData(selectKey);

            this.FireEvent("OpenSoldierMessageView", data);
        }
    }

    public void CancelSoldier(int camp, int index)
    {
        Debug.Log(camp + "  " + index);
        if (cancelSelectCamp == camp && cancelSelectIndex == index)
        {
            
            battleDataManager.RemoveSoldierByConfig(camp, index);
            if (camp == 0)
            {
                UpdateLeftBattleGroup();
            }
            else
            {
                UpdateRightBattleGroup();
            }

            cancelSelectIndex = -1;
            this.FireEvent("CloseSoldierMessageView");
        }
        else
        {
            cancelSelectCamp = camp;
            cancelSelectIndex = index;

            BattleSoldierData data = battleDataManager.GetBattleSoldierData(cancelSelectCamp, cancelSelectIndex);
            this.FireEvent("OpenSoldierMessageView", data);
        }
    }

    public void StartBattle()
    {
        if (battleDataManager.CanStartBattle())
        {
            battleDataManager.warMapSeed = int.Parse(warMapSeedInput.text);
            battleDataManager.frequency = float.Parse(frequencyInput.text);
            battleDataManager.octaves = Mathf.FloorToInt(octavesSlider.value);
            battleDataManager.lacunarity = lacunaritySlider.value;
            battleDataManager.persistence = persistenceSlider.value;

            EnumSeason season = (EnumSeason)(Mathf.FloorToInt(seasonSlider.value));
            battleDataManager.season = season;


            battleDataManager.SetBattleData();
            SceneManager.LoadScene("Scenes/WarScene");
        }
        else
        {
            this.FireEvent("SHOW_MSG", new MsgUI.MsgData("当前不能开始战斗", 3));
        }
    }

    public void CloseView()
    {
        GameObject.Destroy(this.gameObject);
    }


    public void OctavesSliderValueChange()
    {
        octavesText.text = octavesSlider.value.ToString();
    }

    public void LacunarityValueChange()
    {
        lacunarityText.text = lacunaritySlider.value.ToString();
    }

    public void PersistenceSliderValueChange()
    {
        persistenceText.text = persistenceSlider.value.ToString();
    }

    public void SeasonSliderValueChange()
    {
        EnumSeason season = (EnumSeason)(Mathf.FloorToInt(seasonSlider.value));
        seasonText.text = season.GetName();
    }
}
