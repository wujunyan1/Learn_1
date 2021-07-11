using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleResourse
{
    public int persons = 0;
    public int gold = 0;
    public int provisions = 0;  // 民粮，没法操控，仅作为人口增长的判断
    public int troopProvisions = 0; // 军粮，部队的吃食 ,军粮由民粮的税收提供
    public int materials = 0;    // 物资 主要物资，驻扎，物资耗损用 建筑资源
    public int iron = 0;    // 铁 主要军事物资，部队升级


    public void clear()
    {
        persons = 0;
        gold = 0;
        provisions = 0;  // 民粮，没法操控，仅作为人口增长的判断
        troopProvisions = 0; // 军粮，部队的吃食 ,军粮由民粮的税收提供
        materials = 0;    // 木 主要军事物资，驻扎，物资耗损用
        iron = 0;    // 铁 主要军事物资，部队战斗耗损
    }


    public static GameBattleResourse operator +(GameBattleResourse leftValue, GameBattleResourse rightValue)     //重载 +号，让Vector4类能够直接加上Vector4类修饰的变量
    {
        GameBattleResourse resourse = new GameBattleResourse();
        resourse.persons = leftValue.persons + rightValue.persons;
        resourse.gold = leftValue.gold + rightValue.gold;
        resourse.provisions = leftValue.provisions + rightValue.provisions;
        resourse.troopProvisions = leftValue.troopProvisions + rightValue.troopProvisions;
        resourse.materials = leftValue.materials + rightValue.materials;
        resourse.iron = leftValue.iron + rightValue.iron;

        return resourse;
    }

    public static GameBattleResourse operator -(GameBattleResourse leftValue, GameBattleResourse rightValue)     //重载 +号，让Vector4类能够直接加上Vector4类修饰的变量
    {
        GameBattleResourse resourse = new GameBattleResourse();
        resourse.persons = leftValue.persons - rightValue.persons;
        resourse.gold = leftValue.gold - rightValue.gold;
        resourse.provisions = leftValue.provisions - rightValue.provisions;
        resourse.troopProvisions = leftValue.troopProvisions - rightValue.troopProvisions;
        resourse.materials = leftValue.materials - rightValue.materials;
        resourse.iron = leftValue.iron - rightValue.iron;

        return resourse;
    }
}

public static class GameBattleResourseExtend
{
    static int eatFood = 1;
    static int cavalryEatFood = 6;



    /// <summary>
    /// 后续肯定修改，现在先暂时
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public static GameBattleResourse GetGameBattleResourse(this HexCell cell)
    {
        GameBattleResourse resourse = new GameBattleResourse();

        // 如果是水，则没有
        if (cell.IsUnderwater)
        {
            resourse.provisions = 100;
            return resourse;
        }

        // 所有陆地都能提供少量的 食物，石头，木头和极少的铁
        resourse.provisions = 20;
        resourse.materials = 10;
        resourse.iron = 1;

        switch (cell.FeatureType)
        {
            case HexFeatureType.NULL:
                break;
            case HexFeatureType.Wood:
                resourse.materials += 20 * cell.FeatureLevel;
                resourse.provisions += 20 * cell.FeatureLevel;
                break;
            case HexFeatureType.Gold:
                resourse.gold += 100 * cell.FeatureLevel;
                break;
            case HexFeatureType.Build:
                break;
            default:
                break;
        }

        return resourse;
    }
}
