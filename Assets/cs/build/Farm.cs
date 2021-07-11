using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Farm : MapBuild, SaveLoadInterface, RoundObject
{
    
    FarmControl model;

    /// <summary>
    /// 开始工作的回合
    /// </summary>
    int startWorkRound;

    HexCell location;
    public HexCell Location
    {
        get
        {
            return location;
        }
        set
        {
            if (location)
            {
                location.Build = null;
            }
            location = value;
            
            if (location != null)
            {
                location.Build = this;
                location.FeatureType = HexFeatureType.NULL;

                if (model != null)
                {
                    HexGrid.instance.TroopUpdatePostion(model.transform, value);
                    model.transform.localPosition = value.Position;
                }

                CalcFertileLevel();
            }
        }
    }

    /// <summary>
    /// 肥沃度，越肥沃收货越丰富
    /// </summary>
    float fertileLevel = 1f;

    /// <summary>
    /// 产量
    /// </summary>
    int yield;

    public Farm()
    {
        buildType = BuildType.Farm;
        yield = 0;
    }


    public override void Save(BinaryWriter writer)
    {
        writer.Write(startWorkRound);
        writer.Write(yield);

        writer.Write(location.index);
    }

    public override IEnumerator Load(BinaryReader reader)
    {
        startWorkRound = reader.ReadInt32();
        yield = reader.ReadInt32();

        int index = reader.ReadInt32();
        Location = HexGrid.instance.GetCell(index);

        RefreshModel();

        yield return null;
    }

    public void NextRound()
    {
        if(GameCenter.Seanson != EnumSeason.winter)
        {
            yield += Mathf.FloorToInt(fertileLevel * BaseConstant.FarmYield);
        }
    }

    public void LaterNextRound()
    {
        
    }

    // 计算肥沃度
    void CalcFertileLevel()
    {
        int level = BaseConstant.GetFertileLevel(location);
        fertileLevel = level * 0.5f;
    }

    /// <summary>
    /// 收获
    /// </summary>
    /// <returns></returns>
    public int Reap()
    {
        int reap = yield;
        yield = 0;

        return reap;
    }






    void RefreshWheat()
    {

    }

    public void SetModel(FarmControl control)
    {
        this.model = control;
    }

    public void RefreshModel()
    {
        model.RefreshGround(location);
    }
}
