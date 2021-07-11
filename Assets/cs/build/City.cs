using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public enum CityProperty
{
    maxBuildNum,
    maxPerson,
    add_persons,
    add_gold,
    add_provisions,
    add_troopProvisions,
    add_materials,
    add_iron,
}

public class RecruitTroopData
{
    public int troopKey;
    public int round;
}

// 城
public class City : MapBuild, RoundObject, SaveLoadInterface
{

    static string[] CitySuffix = { "村", "乡", "镇", "县", "市", "城" };

    static string[] CityNameList =
    {
        "郑州","洛阳","焦作","商丘","信阳","周口","鹤壁","安阳","濮阳","驻马店","南阳","开封","漯河","许昌","新乡","济源","灵宝","偃师","邓州","登封","三门峡","新郑","禹州","巩义","永城","长葛","义马","林州","项城","汝州","荥阳","平顶山","卫辉","辉","舞钢","新密","孟州","沁阳","郏",
        "合肥","亳州","芜湖","马鞍山","池州","黄山","滁州","安庆","淮南","淮北","蚌埠","宿州","宣城","六安","阜阳","铜陵","明光","天长","宁国","界首","桐城","潜山",
        "福州","厦门","泉州","漳州","南平","三明","龙岩","莆田","宁德","龙海","建瓯","武夷山","长乐","福清","晋江","南安","福安","邵武","石狮","福鼎","建阳","漳平","永安",
        "兰州","白银","武威","金昌","平凉","张掖","嘉峪关","酒泉","庆阳","定西","陇南","天水","玉门","临夏","合作","敦煌","甘南州",
        "贵阳","安顺","遵义","六盘水","兴义","都匀","凯里","毕节","清镇","铜仁","赤水","仁怀","福泉",
        "海口","三亚","万宁","文昌","儋州","琼海","东方","五指山",
        "石家庄","保定","唐山","邯郸","邢台","沧州","衡水","廊坊","承德","迁安","鹿泉","秦皇岛","南宫","任丘","叶城","辛集","涿州","定州","晋州","霸州黄骅","遵化","张家口","沙河","三河","冀州","武安","河间","深州","新乐","泊头","安国","双滦","高碑店",
        "哈尔滨","伊春","牡丹江","大庆","鸡西","鹤岗","绥化","齐齐哈尔","黑河","富锦","虎林","密山","佳木斯","双鸭山","海林","铁力","北安","五大连池","阿城","尚志","五常","安达","七台河","绥芬河","双城","海伦","宁安","讷河","穆棱","同江","肇东",
        "武汉","荆门","咸宁","襄阳","荆州","黄石","宜昌","随州","鄂州","孝感","黄冈","十堰","枣阳","老河口","恩施","仙桃","天门","钟祥","潜江","麻城","洪湖","汉川","赤壁","松滋","丹江口","武穴","广水","石首","大冶","枝江","应城","宜城","当阳","安陆","宜都","利川",
        "长沙","郴州","益阳","娄底","株洲","衡阳","湘潭","岳阳","常德","邵阳","永州","张家界","怀化","浏阳","醴陵","湘乡","耒阳","沅江","涟源","常宁","吉首","津","冷水江","临湘","汨罗","武冈","韶山","湘西州",
        "长春","吉林","通化","白城","四平","辽源","松原","白山","集安","梅河口","双辽","延吉","九台","桦甸","榆树","蛟河","磐石","大安","德惠","洮南","龙井","珲春","公主岭","图们","舒兰","和龙","临江","敦化",
        "南京","无锡","常州","扬州","徐州","苏州","连云港","盐城","淮安","宿迁","镇江","南通","泰州","兴化","东台","常熟","江阴","张家港","通州","宜兴","邳州","海门","溧阳","泰兴","如皋","昆山","启东","江都","丹阳","吴江","靖江","扬中","新沂","仪征","太仓","姜堰","高邮","金坛","句容","灌南",
        "南昌","赣州","上饶","宜春","景德镇","新余","九江","萍乡","抚州","鹰潭","吉安","丰城","樟树","德兴","瑞金","井冈山","高安","乐平","南康","贵溪","瑞昌","东乡","广丰","信州区","三清山",
        "沈阳","葫芦岛","大连","盘锦","鞍山","铁岭","本溪","丹东","抚顺","锦州","辽阳","阜新","调兵山","朝阳","海城","北票","盖州","凤城","庄河","凌源","开原","兴城","新民","大石桥","东港","北宁","瓦房店","普兰店","凌海","灯塔","营口",
        "西宁","格尔木","德令哈",
        "济南","青岛","威海","潍坊","菏泽","济宁","东营","烟台","淄博","枣庄","泰安","临沂","日照","德州","聊城","滨州","乐陵","兖州","诸城","邹城","滕州","肥城","新泰","胶州","胶南","即墨","龙口","平度","莱西",
        "太原","大同","阳泉","长治","临汾","晋中","运城","忻州","朔州","吕梁","古交","高平","永济","孝义","侯马","霍州","介休","河津","汾阳","原平","晋城","潞城",
        "西安","咸阳","榆林","宝鸡","铜川","渭南","汉中","安康","商洛","延安","韩城","兴平","华阴",
        "成都","广安","德阳","乐山","巴中","内江","宜宾","南充","都江堰","自贡","泸州","广元","达州","资阳","绵阳","眉山","遂宁","雅安","阆中","攀枝花","广汉","绵竹","万源","华蓥","江油","西昌","彭州","简阳","崇州","什邡","峨眉山","邛崃","双流",
        "昆明","玉溪","大理","曲靖","昭通","保山","丽江","临沧","楚雄","开远","个旧","景洪","安宁","宣威",
        "杭州","宁波","绍兴","温州","台州","湖州","嘉兴","金华","舟山","衢州","丽水","余姚","乐清","临海","温岭","永康","瑞安","慈溪","义乌","上虞","诸暨","海宁","桐乡","兰溪","龙泉","建德","富德","富阳","平湖","东阳","嵊州","奉化","临安","江山",
        "台北","台南","台中","高雄","桃源",
        "广州","深圳","珠海","汕头","佛山","韶关","湛江","肇庆","江门","茂名","惠州","梅州","汕尾","河源","阳江","清远","东莞","中山","潮州","揭阳","云浮",
        "南宁","贺州","玉林","桂林","柳州","梧州","北海","钦州","百色","防城港","贵港","河池","崇左","来宾","东兴","桂平","北流岑溪","合山","凭祥","宜州",
        "呼和浩特","呼伦贝尔","赤峰","扎兰屯","鄂尔多斯","乌兰察布","巴彦淖尔","二连浩特","霍林郭勒","包头","乌海","阿尔山","乌兰浩特","锡林浩特","根河","满洲里","额尔古纳","牙克石","临河","丰镇","通辽",
        "银川","固原","石嘴山","青铜峡","中卫","吴忠","灵武",
        "拉萨","那曲","山南","林芝","昌都","阿里地区日喀则",
        "乌鲁木齐","石河子","喀什","阿勒泰","阜康","库尔勒","阿克苏","阿拉尔","哈密","克拉玛依","昌吉","奎屯","米泉","和田",
        "香港","澳门",
    };

    public class HouseSite
    {
        public int index = 0;
        public float angle = 0f;
        public float x = 0f;
        public float z = 0f;
    }

    new string name;
    public string Name
    {
        get
        {
            string suffix = CitySuffix[builds[0].GetLevel()-1];

            return name + suffix;
        }
        set
        {
            name = value;
        }
    }

    // 人口
    //public int persons = 1000;
    //public int gold = 0;
    //public int provisions = 0;  // 民粮，没法操控，仅作为人口增长的判断
    //public int troopProvisions = 0; // 军粮，部队的吃食
    //public int wood = 0;    // 木 主要军事物资，驻扎，物资耗损用
    //public int stone = 0;   // 石 主要建筑资源，升级和建筑
    //public int iron = 0;    // 铁 主要军事物资，部队战斗耗损

    public GameBattleResourse battleResourse;

    
    int visionRange = 6;

    public int maxBuildNum;
    public int maxPerson;

    /// <summary>
    /// 城内建筑
    /// </summary>
    List<CityBuild> builds;

    /// <summary>
    /// 城范围
    /// </summary>
    List<HexCell> cells;
    HexCell center;

    CityControl cityModel;
    int centerAngle = 0;
    List<HouseSite> houseSites;

    public Camp camp;

    HexCell location;
    public HexCell Location
    {
        get
        {
            return location;
        }
        set
        {
            DecreaseVisibility();
            if (location)
            {
                location.Build = null;
            }
            location = value;

            if(cityModel != null)
            {
                HexGrid.instance.TroopUpdatePostion(cityModel.transform, value);
                cityModel.transform.localPosition = value.Position;
            }

            if(location != null)
            {
                location.Build = this;
                location.FeatureType = HexFeatureType.NULL;
            }

            IncreaseVisibility();
        }
    }



    public GameBattleResourse nextAddBattleResourse;

    List<CityBuff> buffs;

    List<RecruitTroopData> recruitList;

    public City()
    {
        buildType = BuildType.City;

        name = "111";
        battleResourse = new GameBattleResourse();
        battleResourse.persons = 100;

        nextAddBattleResourse = new GameBattleResourse();

        houseSites = new List<HouseSite>();

        builds = new List<CityBuild>();
        cells = new List<HexCell>();

        buffs = new List<CityBuff>();

        recruitList = new List<RecruitTroopData>();
    }

    public void Init()
    {
        name = CityNameList[Random.Range(0, CityNameList.Length)];
        cityModel.SetCityName(name);

        center = location;
        center.city = this;

        int centerRota = Random.Range(0, 360);
        centerAngle = centerRota;
        cityModel.SetCenterRota(centerAngle);

        HouseSite site = new HouseSite();
        houseSites.Add(site);
        site.index = Random.Range(0, 5);

        Vector3[] housePos = CityControl.housePos;
        Vector3 v = housePos[Random.Range(0, housePos.Length)];
        v.x += Random.Range(-2f, 2f);
        v.z += Random.Range(-2f, 2f);
        site.x = v.x;
        site.z = v.z;

        int rota = Random.Range(0, 360);
        site.angle = rota;

        cityModel.AddHouse(site);


        TownHall town = new TownHall();
        AddBuilds(town);
        town.Init( CityBuildConfigExtend.GetCityBuildConfig(CityBuildType.TownHall, 1) );

        HexGrid grid = HexGrid.instance;

        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            HexCell neighbor = center.GetNeighbor(d);
            if (neighbor == null)
            {
                continue;
            }

            int elevation = Mathf.Abs(center.ViewElevation - neighbor.ViewElevation);
            if (elevation > 1)
            {
                continue;
            }

            AddHexCell(neighbor);
        }

        CalcResourseAdd();
    }

    public override void Save(BinaryWriter writer)
    {
        // 资源数据
        writer.Write((byte)camp.GetId());
        writer.Write(name);
        writer.Write(battleResourse.persons);
        writer.Write(battleResourse.gold);
        writer.Write(battleResourse.provisions);
        writer.Write(battleResourse.troopProvisions);
        writer.Write(battleResourse.materials);
        writer.Write(battleResourse.iron);

        // 自身位置数据
        writer.Write(center.index);
        writer.Write(centerAngle);
        writer.Write(location.index);

        // 自身模型数据
        writer.Write((byte)houseSites.Count);
        foreach (var item in houseSites)
        {
            writer.Write((byte)item.index);
            writer.Write(item.angle);
            writer.Write(item.x);
            writer.Write(item.z);
        }

        // 城市建筑物数据
        writer.Write((byte)builds.Count);
        foreach (var item in builds)
        {
            writer.Write((byte)item.BuildType);
            item.Save(writer);
        }

        // 城市范围格子数据
        writer.Write((byte)cells.Count);
        foreach (var item in cells)
        {
            writer.Write(item.index);
        }

        // 招募队列数据
        writer.Write((byte)recruitList.Count);
        Debug.Log(cells.Count);
        foreach (var item in recruitList)
        {
            writer.Write(item.troopKey);
            writer.Write((byte)item.round);
        }
    }

    public override IEnumerator Load(BinaryReader reader)
    {
        houseSites.Clear();
        builds.Clear();
        cells.Clear();
        cityModel.ClearAllHouse();
        recruitList.Clear();

        HexGrid grid = HexGrid.instance;

        int campId = reader.ReadByte();
        Camp camp = GameCenter.instance.GetCamp(campId);
        

        name = reader.ReadString();
        cityModel.SetCityName(name);

        // 资源数据
        battleResourse.persons = reader.ReadInt32();
        battleResourse.gold = reader.ReadInt32();
        battleResourse.provisions = reader.ReadInt32();
        battleResourse.troopProvisions = reader.ReadInt32();
        battleResourse.materials = reader.ReadInt32();
        battleResourse.iron = reader.ReadInt32();

        // 自身位置数据 放到后面
        int index = reader.ReadInt32();
        center = grid.GetCell(index);
        //center.city = this;

        centerAngle = reader.ReadInt32();
        cityModel.SetCenterRota(centerAngle);

        index = reader.ReadInt32();
        location = grid.GetCell(index);
        location.Build = this;
        HexGrid.instance.TroopUpdatePostion(cityModel.transform, location);
        cityModel.transform.localPosition = location.Position;
        //IncreaseVisibility();


        Debug.Log("11111111111");
        camp.AddCity(this);
        center.city = this;


        // 自身模型数据
        int count = reader.ReadByte();
        for (int i = 0; i < count; i++)
        {
            HouseSite site = new HouseSite();
            site.index = reader.ReadByte();
            site.angle = reader.ReadSingle();
            site.x = reader.ReadSingle();
            site.z = reader.ReadSingle();
            
            houseSites.Add(site);
            cityModel.AddHouse(site);
        }

        // 城市建筑物数据
        count = reader.ReadByte();
        IEnumerator itor;
        for (int i = 0; i < count; i++)
        {
            CityBuildType buildType = (CityBuildType)reader.ReadByte();

            CityBuild cityBuild = CityBuild.CreateCityBuildByLoad(buildType);
            AddBuilds(cityBuild);

            itor = cityBuild.Load(reader);
            while (itor.MoveNext())
            {

            }
        }

        // 城市范围数据
        count = reader.ReadByte();
        for (int i = 0; i < count; i++)
        {
            int cellindex = reader.ReadInt32();
            AddHexCell(grid.GetCell(cellindex));
        }

        // 城市招募队列数据
        count = reader.ReadByte();
        for (int i = 0; i < count; i++)
        {
            RecruitTroopData recruitTroopData = new RecruitTroopData();
            recruitTroopData.troopKey = reader.ReadInt32();
            recruitTroopData.round = reader.ReadByte();

            recruitList.Add(recruitTroopData);
        }


        CalcResourseAdd();
        yield return null;
    }


    // 下一回合
    public void NextRound()
    {
        // 人口变化
        int person = battleResourse.persons;
        int provisions = battleResourse.provisions;

        float round_add = 1.0f / (BaseConstant.RoundOfYear * 0.5f);

        float add_p = round_add * provisions  / person;
        if(add_p > round_add)
        {
            // 有食物时，人口增长1%
            nextAddBattleResourse.persons += Mathf.FloorToInt((add_p - round_add) * 0.01f * person);
        }
        else
        {
            // 没食物时，人口锐减 10%
            nextAddBattleResourse.persons += Mathf.FloorToInt((add_p - round_add) * 0.1f * person);
        }

        nextAddBattleResourse.provisions -= nextAddBattleResourse.persons;

        // 资源增加
        battleResourse += nextAddBattleResourse;

        foreach (CityBuff buff in buffs)
        {
            buff.Execute();
        }


        foreach (CityBuild build in builds)
        {
            build.NextRound();
        }

        int count = recruitList.Count;
        for (int i = 0; i < count; i++)
        {
            RecruitTroopData data = recruitList[i];
            data.round++;
            TroopsConfigData troopsConfigData = TroopsConfigDataManager.GetConfig(data.troopKey);

            if (data.round >= troopsConfigData.recruit_round)
            {
                if(camp.GenerateNewTroop(data.troopKey, this))
                {
                    recruitList.Remove(data);
                    i--;
                    count--;
                }
            }
        }
        

        CalcResourseAdd();
    }


    public void LaterNextRound()
    {

    }


    public void ClearBuildEffect()
    {
        maxBuildNum = 0;
        maxPerson = 0;
    }

    public void StartBuildEffect()
    {
        foreach (var build in builds)
        {
            build.StartEffect();
        }
    }


    public void Clear()
    {
        GameObject.Destroy(cityModel.gameObject);
    }

    public void SetModel(CityControl control)
    {
        cityModel = control;
        cityModel.SetCity(this);

        if (Location != null)
        {
            HexGrid.instance.TroopUpdatePostion(cityModel.transform, Location);
            cityModel.transform.localPosition = Location.Position;
        }
    }
    

    public HexCell GetCenter()
    {
        return center;
    }





    public void AddHexCell(HexCell cell)
    {
        cells.Add(cell);
        cell.city = this;
    }

    public List<HexCell> GetHexCells()
    {
        return cells;
    }

    public List<CityBuild> GetBuilds()
    {
        return builds;
    }

    public void AddBuilds(CityBuild build)
    {
        build.SetCity(this);
        builds.Add(build);
    }

    public void RemoveBuilds(CityBuild build)
    {
        builds.Remove(build);
    }

    public void RemoveBuilds(int index)
    {
        builds.RemoveAt(index);
    }

    public CityBuild GetBuild(int index)
    {
        return builds[index];
    }

    public T GetBuild<T>() where T : CityBuild
    {
        T t = default;
        foreach (CityBuild build in builds)
        {
            if(build is T)
            {
                t = (T)build;
            }
        }

        return t;
    }

    public bool CanBuild(CityBuildType cityBuildType, int level)
    {
        // 数量限制
        int maxNum = 1;
        int num = 0;
        foreach (var build in builds)
        {
            if(build.BuildType == cityBuildType)
            {
                num++;
                maxNum = build.GetCityBuildConfig().buildNum;
            }
        }

        if(num >= maxNum)
        {
            return false;
        }

        // 资源足够
        return CanUpgrade(cityBuildType, level);
    }


    public bool CanUpgrade(CityBuildType cityBuildType, int level)
    {
        // 资源足够
        CityBuildConfig cityBuildConfig = CityBuildConfigExtend.GetCityBuildConfig(cityBuildType, level);

        if (cityBuildConfig.gold <= battleResourse.gold
            && cityBuildConfig.materials <= battleResourse.materials)
        {
            return true;
        }

        return false;
    }


    // 建造建筑
    public void BuildCityBuild(CityBuildConfig config)
    {
        battleResourse.gold -= config.gold;
        battleResourse.materials -= config.materials;

        CityBuild cityBuild = CityBuild.CreateCityBuildByLoad(config.BuildType);
        cityBuild.Init(config);
        AddBuilds(cityBuild);
    }

    // 建造建筑
    public void UpgradeCityBuild(CityBuild build)
    {
        CityBuildConfig config = build.GetCityBuildConfig();
        CityBuildConfig nextConfig = CityBuildConfigExtend.GetCityBuildConfig(config.BuildType, config.level + 1);
        if (nextConfig == null)
        {
            return;
        }
        
        battleResourse.gold -= nextConfig.gold;
        battleResourse.materials -= nextConfig.materials;

        build.AddLevel();
    }



    /// <summary>
    /// 清除视野
    /// </summary>
    /// <returns></returns>
    public bool DecreaseVisibility()
    {
        if (location && camp != null && camp.HasPlayerVisibility())
        {
            HexGrid.instance.DecreaseVisibility(location, visionRange);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 获得视野
    /// </summary>
    /// <returns></returns>
    public bool IncreaseVisibility()
    {
        if (location && camp != null && camp.HasPlayerVisibility())
        {
            HexGrid.instance.IncreaseVisibility(location, visionRange);
            return true;
        }
        return false;
    }




    public int GetCityBaseProperty(CityProperty property)
    {
        switch (property)
        {
            case CityProperty.maxBuildNum:
                return maxBuildNum;
            case CityProperty.maxPerson:
                return maxPerson;

            case CityProperty.add_persons:
                return nextAddBattleResourse.persons;
            case CityProperty.add_gold:
                return nextAddBattleResourse.gold;
            case CityProperty.add_provisions:
                return nextAddBattleResourse.provisions;
            case CityProperty.add_troopProvisions:
                return nextAddBattleResourse.troopProvisions;
            case CityProperty.add_materials:
                return nextAddBattleResourse.materials;
            case CityProperty.add_iron:
                return nextAddBattleResourse.iron;
            default:
                break;
        }

        return 0;
    }

    public int GetCityProperty(CityProperty property)
    {
        int baseProper = GetCityBaseProperty(property);
        int addProper = 0;
        foreach (var buff in buffs)
        {
            addProper += buff.GetCityProperty(property, baseProper);
        }

        return baseProper + addProper;
    }

    public void AddCityBuff(CityBuff buff)
    {
        buffs.Add(buff);
        buff.SetCity(this);
    }


    public void RemoveCityBuff(CityBuff buff)
    {
        buffs.Remove(buff);
    }




    // 人口
    //public int _add_persons = 0;
    //public int _add_gold = 0;
    //public int _add_provisions = 0;  // 民粮，没法操控，仅作为人口增长的判断
    //public int _add_troopProvisions = 0; // 军粮，部队的吃食
    //public int _add_wood = 0;    // 木 主要军事物资，驻扎，物资耗损用
    //public int _add_stone = 0;   // 石 主要建筑资源，升级和建筑
    //public int _add_iron = 0;    // 铁 主要军事物资，部队战斗耗损


    /// <summary>
    /// 计算资源增加量
    /// </summary>
    void CalcResourseAdd()
    {
        nextAddBattleResourse.clear();

        foreach (var cell in cells)
        {
            nextAddBattleResourse += cell.GetGameBattleResourse();
        }

        // 农业税
        int troop_provis = Mathf.FloorToInt(nextAddBattleResourse.provisions * camp.GetAgriculturalTax());
        nextAddBattleResourse.provisions -= troop_provis;
        nextAddBattleResourse.troopProvisions += troop_provis;

        // 人头税
        nextAddBattleResourse.gold += Mathf.FloorToInt(battleResourse.persons * camp.GetPersonTax());
        

    }




    /// <summary>
    /// 这个兵种能否招募
    /// </summary>
    /// <param name="troopsIndex"></param>
    /// <returns></returns>
    public bool CanRecruit(int troopsIndex)
    {
        UObject o = UObjectPool.Get();
        o.Set("city", this);
        o.Set("troopIndex", troopsIndex);
        o.Set("result", false);

        this.SendListener("CanRecruitTroop", o);

        bool result = (bool)o.Get("result");

        return result;
    }

    public List<int> CanRecruitTroopList()
    {
        List<TroopsConfigData> troopsConfigDatas = TroopsConfigDataManager.GetConfigList();
        List<int> canRecruitTroopList = new List<int>();

        foreach (var data in troopsConfigDatas)
        {
            if (CanRecruit(data.key)) 
            {
                canRecruitTroopList.Add(data.key);
            }
        }

        return canRecruitTroopList;
    }

    public List<RecruitTroopData> GetRecruitList()
    {
        return recruitList;
    }

    /// <summary>
    /// 是否有足够的资源招募
    /// </summary>
    /// <param name="troopKey"></param>
    /// <returns></returns>
    public bool HasResRecruit(int troopKey)
    {
        TroopsConfigData configData = TroopsConfigDataManager.GetConfig(troopKey);
        
        return battleResourse.gold >= configData.recruit_gold;
    }

    public void AddRecruitList(int troopKey)
    {
        TroopsConfigData configData = TroopsConfigDataManager.GetConfig(troopKey);
        battleResourse.gold -= configData.recruit_gold;

        RecruitTroopData recruitTroopData = new RecruitTroopData();
        recruitTroopData.troopKey = troopKey;
        recruitTroopData.round = 0;

        recruitList.Add(recruitTroopData);
    }

    public void RemoveRecruitList(int index)
    {
        RecruitTroopData recruitTroopData = recruitList[index];
        TroopsConfigData configData = TroopsConfigDataManager.GetConfig(recruitTroopData.troopKey);

        if (recruitTroopData.round == 0)
        {
            battleResourse.gold += configData.recruit_gold;
        }
        else
        {
            battleResourse.gold += Mathf.FloorToInt(configData.recruit_gold * 0.5f);
        }

        recruitList.RemoveAt(index);
    }
}
