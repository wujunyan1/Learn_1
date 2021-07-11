using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// save 数据
/// </summary>
public class TroopsData : SaveLoadInterface
{
    private class TroopsSaveData
    {
        public int index;
        public string name;
        public int campId;

        public int troop_num;

        /// <summary>
        /// 血量  待修改
        /// </summary>
        public int blood;

        /// <summary>
        /// 攻击 命中概率为  攻击力/敌方防御力 * 0.5
        /// </summary>
        public int ATK;

        public int power;

        /// <summary>
        /// 护甲
        /// </summary>
        public int armor;

        /// <summary>
        /// 防御 
        /// </summary>
        public int dodge;

        /// <summary>
        /// 格挡 100
        /// </summary>
        public int parry;

        /// <summary>
        /// 冲锋
        /// </summary>
        public int charge;

        /// <summary>
        /// 士气
        /// </summary>
        public int morale;
        

        /// <summary>
        /// 射程
        /// </summary>
        public int l_ATKRange;

        /// <summary>
        /// 远程威力
        /// </summary>
        public int l_power;

        /// <summary>
        /// 精准
        /// </summary>
        public float accurate;

        /// <summary>
        /// 弹药
        /// </summary>
        public int ammo;

        /// <summary>
        /// 速度
        /// </summary>
        public int speed;

        /// <summary>
        /// 侦查范围
        /// </summary>
        public int visionRange;

        /// <summary>
        /// 头像
        /// </summary>
        public string head;

        /// <summary>
        /// 携带食物
        /// </summary>
        public int carryFood;

        public int configIndex;

        public void LoadByConfig(TroopsConfigData config)
        {
            index = 0;
            name = config.solider_name;
            campId = 0;

            troop_num = config.troop_num;

            blood = config.blood;
            ATK = config.ATK;
            armor = config.armor;
            dodge = config.dodge;
            parry = config.parry;
            power = config.power;
            charge = config.charge;
            morale = config.morale;

            l_ATKRange = config.l_ATKRange;
            l_power = config.l_power;
            accurate = config.accurate;
            ammo = config.ammo;

            speed = config.speed;

            carryFood = 0;

            visionRange = config.visionRange;

            head = ResourseManager.GetRandomHeadKey();

            configIndex = config.key;
        }
    }

    public TroopsConfigData config;

    // 保存的数据
    private TroopsSaveData saveData;

    public string Name
    {
        get
        {
            return saveData.name;
        }
    }

    public TroopsType TroopsType
    {
        get
        {
            return (TroopsType)config.solider_type;
        }
    }

    public int TroopNum
    {
        get
        {
            return saveData.troop_num;
        }
        set
        {
            saveData.troop_num = value;
        }
    }

    public string Head
    {
        get
        {
            return saveData.head;
        }
    }

    Dictionary<string, object> propertyMap = new Dictionary<string, object>();
    

    // 这里的数据是临时数据，经过各种buff加成后的

    /// <summary>
    /// 血量
    /// </summary>
    public int blood;

    /// <summary>
    /// 攻击 命中概率为  攻击力/敌方防御力 * 0.5
    /// </summary>
    public int ATK;

    public int power;

    /// <summary>
    /// 护甲
    /// </summary>
    public int armor;

    /// <summary>
    /// 防御 
    /// </summary>
    public int dodge;

    /// <summary>
    /// 格挡 100
    /// </summary>
    public int parry;

    /// <summary>
    /// 冲锋
    /// </summary>
    public int charge;

    /// <summary>
    /// 士气
    /// </summary>
    public int morale;

    /// <summary>
    /// 射程
    /// </summary>
    public int l_ATKRange;

    /// <summary>
    /// 远程威力
    /// </summary>
    public int l_power;

    /// <summary>
    /// 精准
    /// </summary>
    public float accurate;
    

    /// <summary>
    /// 弹药
    /// </summary>
    public int ammo;
    
    /// <summary>
    /// 速度
    /// </summary>
    public int speed; 

    /// <summary>
    /// 侦查范围
    /// </summary>
    public int visionRange;

    Camp _camp;
    public Camp camp
    {
        get
        {
            return _camp;
        }
        set
        {
            _camp = value;
            if(camp != null)
            {
                saveData.campId = _camp.GetId();
            }
        }
    }



    public TroopsData()
    {
        saveData = new TroopsSaveData();

    }

    public T GetData<T>(string name, T defaultData)
    {
        object o;
        if (propertyMap.TryGetValue(name, out o))
        {
            return (T)o;
        }

        o = _GetData(name);
        if(o != null)
        {
            propertyMap.Add(name, o);
            return (T)o;
        }

        return defaultData;
    }

    public void SetData(string name, object data)
    {
        if (propertyMap.ContainsKey(name))
        {
            propertyMap.Remove(name);
        }

        propertyMap.Add(name, data);

        _SetData(name, data);
    }

    object _GetData(string name)
    {
        switch (name)
        {
            case "index":
                return saveData.index;
            case "name":
                return saveData.name;
            case "campId":
                return saveData.campId;
            case "troop_num":
                return saveData.troop_num;
            case "blood":
                return saveData.blood;
            case "ATK":
                return saveData.ATK;
            case "armor":
                return saveData.armor;
            case "dodge":
                return saveData.dodge;
            case "parry":
                return saveData.parry;
            case "power":
                return saveData.power;
            case "charge":
                return saveData.charge;
            case "morale":
                return saveData.morale;
            case "l_ATKRange":
                return saveData.l_ATKRange;
            case "l_power":
                return saveData.l_power;
            case "accurate":
                return saveData.accurate;
            case "ammo":
                return saveData.ammo;
            case "speed":
                return saveData.speed;
            case "visionRange":
                return saveData.visionRange;
            case "head":
                return saveData.head;
            case "configIndex":
                return saveData.configIndex;
            case "carryFood":
                return saveData.carryFood;
            default:
                break;
        }


        return null;
    }

    void _SetData(string name, object data)
    {
        switch (name)
        {
            case "index":
                saveData.index = (int)data;
                break;
            case "name":
                saveData.name = (string)data;
                break;
            case "campId":
                saveData.campId = (int)data;
                break;
            case "troop_num":
                saveData.troop_num = (int)data;
                break;
            case "blood":
                saveData.blood = (int)data;
                break;
            case "ATK":
                saveData.ATK = (int)data;
                break;
            case "armor":
                saveData.armor = (int)data;
                break;
            case "dodge":
                saveData.dodge = (int)data;
                break;
            case "parry":
                saveData.parry = (int)data;
                break;
            case "power":
                saveData.power = (int)data;
                break;
            case "charge":
                saveData.charge = (int)data;
                break;
            case "morale":
                saveData.morale = (int)data;
                break;
            case "l_ATKRange":
                saveData.l_ATKRange = (int)data;
                break;
            case "l_power":
                saveData.l_power = (int)data;
                break;
            case "accurate":
                saveData.accurate = (int)data;
                break;
            case "ammo":
                saveData.ammo = (int)data;
                break;
            case "speed":
                saveData.speed = (int)data;
                break;
            case "visionRange":
                saveData.visionRange = (int)data;
                break;
            case "head":
                saveData.head = (string)data;
                break;
            case "configIndex":
                saveData.configIndex = (int)data;
                break;
            case "carryFood":
                saveData.carryFood = (int)data;
                break;
            default:
                break;
        }
        
    }


    public void Save(BinaryWriter writer)
    {
        writer.Write(saveData.index);
        writer.Write(saveData.name);
        writer.Write((byte)saveData.campId);
        writer.Write((byte)saveData.troop_num);

        writer.Write(saveData.blood);
        writer.Write(saveData.ATK);
        writer.Write(saveData.armor);
        writer.Write(saveData.dodge);
        writer.Write((byte)saveData.parry);
        writer.Write(saveData.power);
        writer.Write(saveData.charge);
        writer.Write(saveData.morale);

        writer.Write((byte)saveData.l_ATKRange);
        writer.Write(saveData.l_power);
        writer.Write((byte)saveData.accurate);
        writer.Write((byte)saveData.ammo);

        writer.Write((byte)saveData.speed);
        writer.Write((byte)saveData.visionRange);

        writer.Write(saveData.head);
        writer.Write((byte)saveData.configIndex);
    }

    public IEnumerator Load(BinaryReader reader)
    {
        saveData.index = reader.ReadInt32();
        saveData.name = reader.ReadString();
        saveData.campId = reader.ReadByte();

        saveData.troop_num = reader.ReadByte();

        saveData.blood = reader.ReadInt32();
        saveData.ATK = reader.ReadInt32();
        saveData.armor = reader.ReadInt32();
        saveData.dodge = reader.ReadInt32();
        saveData.parry = reader.ReadByte();
        saveData.power = reader.ReadInt32();
        saveData.charge = reader.ReadInt32();
        saveData.morale = reader.ReadInt32();

        saveData.l_ATKRange = reader.ReadByte();
        saveData.l_power = reader.ReadInt32();
        saveData.speed = reader.ReadByte();
        saveData.accurate = reader.ReadByte();
        saveData.ammo = reader.ReadByte();

        saveData.visionRange = reader.ReadByte();

        saveData.head = reader.ReadString();
        saveData.configIndex = reader.ReadByte();

        
        LoadSaveData();
        LoadConfig();
        yield return null;
    }

    public void SetConfig(TroopsConfigData configData)
    {
        config = configData;
    }

    private void LoadSaveData()
    {
        propertyMap.Clear();

        blood = saveData.blood;       // 血
        ATK = saveData.ATK;         // 攻击
        armor = saveData.armor;       // 护甲
        dodge = saveData.dodge;       // 防御
        parry = saveData.parry;       // 格挡
        power = saveData.power;       // 伤害
        charge = saveData.charge;      // 体力
        morale = saveData.morale;      // 士气
        accurate = saveData.accurate;
        ammo = saveData.ammo;

        l_power = saveData.l_power;     // 远程威力


        speed = saveData.speed;       // 速度
        visionRange = saveData.visionRange; // 侦查范围
    }

    private void LoadConfig()
    {
        SetConfig(TroopsConfigDataManager.GetConfig(saveData.configIndex));
    }

    public void IsNewObj()
    {
        saveData.LoadByConfig(config);
        LoadSaveData();
    }

    public void ReSetRound()
    {
        speed = config.speed;
    }

    public int EatFood()
    {
        return config.food;
    }
}
