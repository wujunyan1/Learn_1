using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoldierMessageView : EventComponent
{
    //名字
    public Text t_name;

    // 兵种类型
    public Text t_soldier_type;

    public Progress blood_progress;

    public RectTransform obj_msg;

    // 护甲
    public Text t_armor;

    // 士气
    public Text t_morale;

    // 移速
    public Text t_move_speed;

    // 近战攻击
    public Text t_ATK;

    // 近战防御
    public Text t_dodge;

    // 武器威力
    public Text t_power;

    // 冲锋加成
    public Text t_charge;

    // 弹药容量
    public Text t_ammo;

    // 部队射程
    public Text t_l_ATKRange
;
    // 远程威力
    public Text t_l_power;



    // 护甲
    public CompareProgress p_armor;

    // 士气
    public CompareProgress p_morale;

    // 移速
    public CompareProgress p_move_speed;

    // 近战攻击
    public CompareProgress p_ATK;

    // 近战防御
    public CompareProgress p_dodge;

    // 武器威力
    public CompareProgress p_power;

    // 冲锋加成
    public CompareProgress p_charge;

    // 弹药容量
    public CompareProgress p_ammo;

    // 部队射程
    public CompareProgress p_l_ATKRange
;
    // 远程威力
    public CompareProgress p_l_power;


    private BattleSoldierData battleSoldierData;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetBattleSoldierData(BattleSoldierData data)
    {
        battleSoldierData = data;
        this.UpdateView();
    }
    
    void UpdateView()
    {
        SoldierConfigData configData = battleSoldierData.GetConfig();

        //名字
        this.t_name.text = battleSoldierData.solider_name;

        // 兵种类型
        //Debug.Log(battleSoldierData.solider_type);
        //Debug.Log(configData.solider_type);
        //Debug.Log(configData.ToString());

        this.t_soldier_type.text = SoldierConfigData.SOLDIER_TYPE_NAME(battleSoldierData.solider_type);

        int curr_blood = battleSoldierData.blood;
        int max_blood = configData.blood;
        blood_progress.SetProgress( 1.0f * curr_blood / max_blood);
        blood_progress.SetCustomText(curr_blood.ToString());

        // 护甲
        int curr_armor = battleSoldierData.armor;
        int base_armor = configData.armor;
        int max_armor = 150;
        t_armor.text = curr_armor.ToString();
        p_armor.SetBaseProgress(1.0f * base_armor / max_armor);
        p_armor.SetProgress(1.0f * curr_armor / max_armor);

        // 士气
        int curr_morale = battleSoldierData.morale;
        int base_morale = configData.morale;
        int max_morale = 150;
        t_morale.text = curr_morale.ToString();
        p_morale.SetBaseProgress(1.0f * base_morale / max_morale);
        p_morale.SetProgress(1.0f * curr_morale / max_morale);

        // 移速
        int curr_move_speed = battleSoldierData.move_speed;
        int base_move_speed = configData.m_dMaxSpeed;
        int max_move_speed = 250;
        t_move_speed.text = curr_move_speed.ToString();
        p_move_speed.SetBaseProgress(1.0f * base_move_speed / max_move_speed);
        p_move_speed.SetProgress(1.0f * curr_move_speed / max_move_speed);


        // 近战攻击
        int curr_ATK = battleSoldierData.ATK;
        int base_ATK = configData.ATK;
        int max_ATK = 250;
        t_ATK.text = curr_ATK.ToString();
        p_ATK.SetBaseProgress(1.0f * base_ATK / max_ATK);
        p_ATK.SetProgress(1.0f * curr_ATK / max_ATK);

        // 近战防御
        int curr_dodge = battleSoldierData.dodge;
        int base_dodge = configData.dodge;
        int max_dodge = 250;
        t_dodge.text = curr_dodge.ToString();
        p_dodge.SetBaseProgress(1.0f * base_dodge / max_dodge);
        p_dodge.SetProgress(1.0f * curr_dodge / max_dodge);

        // 武器威力
        int curr_power = battleSoldierData.power;
        int base_power = configData.power;
        int max_power = 500;
        t_power.text = curr_power.ToString();
        p_power.SetBaseProgress(1.0f * base_power / max_power);
        p_power.SetProgress(1.0f * curr_power / max_power);

        // 冲锋加成
        int curr_charge = battleSoldierData.charge;
        int base_charge = configData.charge;
        int max_charge = 200;
        t_charge.text = curr_charge.ToString();
        p_charge.SetBaseProgress(1.0f * base_charge / max_charge);
        p_charge.SetProgress(1.0f * curr_charge / max_charge);

        // 以下为远程部队才有的属性
        if(configData.ammo == 0)
        {
            Vector2 sizeDelta = obj_msg.sizeDelta;
            obj_msg.sizeDelta = new Vector2(sizeDelta.x, 140);

            // 弹药容量
            t_ammo.gameObject.SetActive(false);
            p_ammo.gameObject.SetActive(false);

            // 部队射程
            t_l_ATKRange.gameObject.SetActive(false);
            p_l_ATKRange.gameObject.SetActive(false);

            // 远程威力
            t_l_power.gameObject.SetActive(false);
            p_l_power.gameObject.SetActive(false);
        }
        else
        {
            Vector2 sizeDelta = obj_msg.sizeDelta;
            obj_msg.sizeDelta = new Vector2(sizeDelta.x, 200);

            // 弹药容量
            t_ammo.gameObject.SetActive(true);
            p_ammo.gameObject.SetActive(true);

            // 部队射程
            t_l_ATKRange.gameObject.SetActive(true);
            p_l_ATKRange.gameObject.SetActive(true);

            // 远程威力
            t_l_power.gameObject.SetActive(true);
            p_l_power.gameObject.SetActive(true);

            // 弹药容量
            int curr_ammo = battleSoldierData.ammo;
            int base_ammo = configData.ammo;
            int max_ammo = 100;
            t_ammo.text = curr_ammo.ToString();
            p_ammo.SetBaseProgress(1.0f * base_ammo / max_ammo);
            p_ammo.SetProgress(1.0f * curr_ammo / max_ammo);

            // 部队射程
            int curr_l_ATKRange = battleSoldierData.l_ATKRange;
            int base_l_ATKRange = configData.l_ATKRange;
            int max_l_ATKRange = 1000;
            t_l_ATKRange.text = curr_l_ATKRange.ToString();
            p_l_ATKRange.SetBaseProgress(1.0f * base_l_ATKRange / max_l_ATKRange);
            p_l_ATKRange.SetProgress(1.0f * curr_l_ATKRange / max_l_ATKRange);

            // 远程威力
            int curr_l_power = battleSoldierData.l_power;
            int base_l_power = configData.l_power;
            int max_l_power = 200;
            t_l_power.text = curr_l_power.ToString();
            p_l_power.SetBaseProgress(1.0f * base_l_power / max_l_power);
            p_l_power.SetProgress(1.0f * curr_l_power / max_l_power);
        }
    }

}
