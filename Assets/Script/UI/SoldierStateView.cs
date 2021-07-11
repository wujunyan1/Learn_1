using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoldierStateView : EventComponent
{
    public RectTransform blood;
    public RectTransform morale;
    public RectTransform ammo;

    private BattleSoldierData _data;
    public BattleSoldierData data
    {
        get
        {
            return _data;
        }
        set
        {
            _data = value;
            SoldierConfigData configData = _data.GetConfig();

            int base_ammo = configData.ammo;
            if (base_ammo == 0)
            {
                ammo.gameObject.SetActive(false);
            }
            else
            {
                ammo.gameObject.SetActive(true);
            }
        }
    }

    public float baseScale = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        SoldierConfigData configData = data.GetConfig();
        int id = configData.id;
        this.RegisterEvent(string.Format("SOLDIER_BLOOD_UPDATE_{0}", id), UpdateBloodView);
        this.RegisterEvent(string.Format("SOLDIER_MORALE_UPDATE_{0}", id), UpdateMoraleView);

        int base_ammo = configData.ammo;
        if (base_ammo != 0)
        {
            this.RegisterEvent(string.Format("SOLDIER_AMMO_UPDATE_{0}", id), UpdateAmmoView);
        }
    }
    
    private void UpdateBloodView(UEvent e)
    {
        SoldierConfigData configData = data.GetConfig();

        int curr_blood = data.blood;
        int max_blood = configData.blood;
        blood.localScale = new Vector3(curr_blood * baseScale / max_blood, baseScale, baseScale);
    }

    private void UpdateMoraleView(UEvent e)
    {
        SoldierConfigData configData = data.GetConfig();

        int curr_morale = data.morale;
        int base_morale = configData.morale;
        morale.localScale = new Vector3(curr_morale * baseScale / base_morale, baseScale, baseScale);
    }

    public void UpdateAmmoView(UEvent e)
    {
        SoldierConfigData configData = data.GetConfig();

        // 弹药容量
        int curr_ammo = data.ammo;
        int base_ammo = configData.ammo;
        if(base_ammo != 0)
        {
            ammo.localScale = new Vector3(curr_ammo * baseScale / base_ammo, baseScale, baseScale);
        }
    }
}
