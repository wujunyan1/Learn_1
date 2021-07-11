using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecruitTroopCard : EventComponent
{
    public GameObject CDGameObj;

    public Text nameText;
    public Text goldText;
    public Text CDText;

    public RawImage rawImage;

    public GameObject showModelPerfabs;
    GameObject showModel = null;
    RenderTexture texture;

    int index;
    int y;

    public void SetIndex(int index)
    {
        this.index = index;
    }

    public void SetRecruitTroopData(RecruitTroopData recruitTroopData)
    {
        y = 0;
        CDGameObj.SetActive(true);

        TroopsConfigData configData = TroopsConfigDataManager.GetConfig(recruitTroopData.troopKey);

        UpdateView(configData);

        int round = configData.recruit_round - recruitTroopData.round;

        if(round < 0)
        {
            round = 0;
        }
        CDText.text = round.ToString();
    }

    public void SetRecruitTroopCardKey(int key)
    {
        y = 100;

        CDGameObj.SetActive(false);

        TroopsConfigData configData = TroopsConfigDataManager.GetConfig(key);
        UpdateView(configData);
    }

    void UpdateView(TroopsConfigData configData)
    {
        nameText.text = configData.solider_name;

        goldText.text = string.Format("{0} ({1})", configData.recruit_gold, 
            configData.maintain_gold * configData.troop_num);

        _ShowModel(configData);
    }

    void _ShowModel(TroopsConfigData configData)
    {
        if(showModel == null)
        {
            texture = RenderTexture.GetTemporary(56, 70);
            showModel = Instantiate<GameObject>(showModelPerfabs);
        }
        showModel.transform.localPosition = new Vector3(index * 100, y, 0);
        Transform model = showModel.transform.Find("model");
        if(model.childCount > 0)
        {
            GameObject.Destroy(model.GetChild(0).gameObject);
        }

        PrefabsManager prefabsManager = PrefabsManager.GetInstance();
        GameObject modelObj;
        prefabsManager.GetGameObj(out modelObj, configData.modelName);
        

        modelObj.transform.SetParent(model);
        modelObj.transform.localPosition = Vector3.zero;
        modelObj.transform.localRotation = Quaternion.Euler(0, 0, 0);

        SetLayerAndAllChildren(model.transform, LayerMask.NameToLayer("Model"));


        Camera camera = showModel.GetComponentInChildren<Camera>();
        camera.targetTexture = texture;

        rawImage.texture = texture;
    }

    void SetLayerAndAllChildren(Transform obj, int layer)
    {
        obj.gameObject.layer = layer;
        if (obj.childCount > 0)
        {
            for (int i = 0; i < obj.childCount; i++)
            {
                SetLayerAndAllChildren(obj.GetChild(i), layer);
            }
        }
    }

    protected override void DestroyEnd()
    {
        Debug.Log("DestroyEnd");

        if(showModel != null)
        {
            RenderTexture.ReleaseTemporary(texture);
            texture = null;

            GameObject.Destroy(showModel);
            showModel = null;
        }
    }

}
