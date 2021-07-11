using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierManager : MonoBehaviour
{
    public static Transform soldiersObj;

    public static SoldierControl CreaterSoldierControl(int camp, SoldierConfigData data, Vector3 vector3, Quaternion rotation)
    {
        BattleResManager resManager = BattleResManager.GetInstance();

        GameObject obj = resManager.soldierPrefabs;

        string key = data.model;

        GameObject model = resManager.GetSoldierModel(key);
        if (model != null)
        {
            Debug.Log(vector3);
            vector3 = BattleWorld.battleMap.GetPos(vector3);
            Debug.Log(vector3);

            // 基础model  
            GameObject soldierObj = Instantiate(obj, vector3, rotation, soldiersObj);

            // 每个兵种 自己的模型
            Transform addedMol = soldierObj.transform.Find("mod");
            Instantiate(model, addedMol);

            soldierObj.name = string.Format("{0}", data.solider_name);

            SteeringBehaviors steering = soldierObj.AddComponent<SteeringBehaviors>();
            SoldierControl control = soldierObj.AddComponent<SoldierControl>();

            steering.vehicle = control;
            control.m_pSteering = steering;

            // 创建数据
            SoldierConfigData addData = data.Clone();
            BattleSoldierData battleData = new BattleSoldierData();
            battleData.SetConfigData(addData);
            battleData.camp = camp;
            control.SetData(battleData); // data = battleData;

            // 添加碰撞模型
            CollisionObj collision = soldierObj.AddComponent<CollisionObj>();
            collision.SetBattleData(battleData);
            collision.m_pSteering = steering;

            control.collisionObj = collision;


            // 添加行为AI
            AIControl aIControl = control.aiControl;
            AttackAI attackAi = aIControl.AddAI<AttackAI>();
            if(battleData.AttackType == BattleSoldierData.ATTACK_TYPE.SHOOT)
            {
                // 关闭近战攻击模式
                ShootAI shootAI = aIControl.AddAI<ShootAI>();
                attackAi.SetActive(false);
            }

            BattleWorld.battleCenter.GetGameControl().AddSoldier(camp, control, collision);

            return control;
        }

        return null;
    }

    public static void CreaterSoldierTeamControl(BattleTeamData team)
    {
        BattleMap battleMap = BattleWorld.battleMap;
        Rect area = battleMap.GetStartArea(team.GetCamp());
        Vector2 center = area.center;

        Quaternion rotation = Quaternion.Euler(0, 0, 0);
        if (team.GetCamp() != BattleWorld.playerCamp)
        {
            rotation.y = 180f;
        }
        
        List<SoldierConfigData> configs = team.GetSoldierConfigs();
        int num = configs.Count;
        int lineNum = 20;
        int line = num % lineNum > 0 ? num / lineNum + 1 : num / lineNum;
        int cow = num > lineNum ? lineNum : num;

        float min_z = center.y - 1.5f * (line - 1);
        float min_x = center.x - 1.5f * (cow - 1);

        float x = min_x;
        float z = min_z;

        float add_x = 3f;
        float add_z = 3f;

        int index = 0;
        foreach (SoldierConfigData data in configs)
        {
            string key = data.model;

            Vector3 point = new Vector3(x, 0, z);

            SoldierControl control = SoldierManager.CreaterSoldierControl(team.GetCamp(), data, point, rotation);

            index++;
            x += add_x;
            if(index > lineNum)
            {
                index = 0;
                x = min_x;
                z += add_z;
            }
        }
    }
}
