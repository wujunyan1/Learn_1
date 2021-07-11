using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateCityBuild : EventComponent
{
    OnMouseEvent mouseEvent;
    CityBuildConfig config;
    CityBuild cityBuild;
    CityBuildMessageView view = null;

    public Transform CDTransform;
    public Transform GoldTransform;
    public Transform MaterialsTransform;

    bool isIn = false;

    private void Start()
    {
        mouseEvent = GetComponent<OnMouseEvent>();

        mouseEvent.RegisterListener("OnPointerEnter", ShowTip);
        mouseEvent.RegisterListener("OnPointerExit", CloseTip);
    }

    public void SetConfig(CityBuildConfig config)
    {
        this.config = config;

        cityBuild = CityBuild.CreateCityBuildByLoad((CityBuildType)config.type);
        cityBuild.SetConfig(config);
    }

    public void UpdateView()
    {
        string image = config.image;
        Sprite sprite = ResourseManager.GetUIRes(ResourseManager.UI_CIYT_BUILD + "_" + image);
        
        transform.GetChild(0).GetComponent<Image>().sprite = sprite;
        transform.GetChild(1).GetComponent<Text>().text = BaseConstant.LevelText[config.level];
    }


    public void SetCanCreate(bool canCreate)
    {
        if (canCreate)
        {
            transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        else
        {
            transform.GetChild(0).GetComponent<Image>().color = new Color32(63, 63, 63, 255);
        }
    }

    void ShowTip(UEvent evt)
    {
        isIn = true;

        _ShowBuildMessage();

        CDTransform.gameObject.SetActive(true);
        CDTransform.GetComponentInChildren<Text>().text = config.buildRound.ToString();

        GoldTransform.gameObject.SetActive(true);
        GoldTransform.GetComponentInChildren<Text>().text = config.gold.ToString();

        MaterialsTransform.gameObject.SetActive(true);
        MaterialsTransform.GetComponentInChildren<Text>().text = config.materials.ToString();

        Invoke("_ShowTip", 0.5f);
    }

    void CloseTip(UEvent evt)
    {
        isIn = false;

        CDTransform.gameObject.SetActive(false);
        GoldTransform.gameObject.SetActive(false);
        MaterialsTransform.gameObject.SetActive(false);

        if(view != null)
        {
            ViewManager.Instance.RemoveView(view);
            view = null;
        }
    }

    void _ShowTip()
    {
        if (!isIn)
        {
            return;
        }
    }

    void _ShowBuildMessage()
    {
        UObject o = UObjectPool.Get();
        o.Set("CityBuild", cityBuild);

        ViewManager.Instance.OpenOnlyView<CityBuildMessageView>("CityBuildMessageView", out view, o);
    }
}
