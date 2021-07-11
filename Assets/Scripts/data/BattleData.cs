using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleData
{
    public int selfIndex { get; }

    private Camp[] camps;
    private int campNum;

    private static BattleData instance;
    private BattleData(int _selfIndex, Camp[] camps)
    {
        selfIndex = _selfIndex;
        this.camps = camps;
        campNum = camps.Length;
    }

    public static BattleData GetInstance()
    {
        return instance;
    }

    public static void InitBattleData(int _selfIndex, Camp[] camps)
    {
        instance = new BattleData(_selfIndex, camps);
    }

    public Camp GetCamp(int index)
    {
        return camps[index];
    }

    public Camp GetSelfCamp()
    {
        return camps[selfIndex];
    }

    public int GetCampNum()
    {
        return campNum;
    }
}
