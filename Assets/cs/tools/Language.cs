using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LanguageText
{
    名字,
    人口_1,
    金币_1,
    民粮_1,
    军粮_1,
    物资_1,
    铁料_1,
    人口_ADD_MAX_3,
    金币_ADD_2,
    民粮_ADD_2,
    军粮_ADD_2,
    物资_ADD_2,
    铁料_ADD_2,
};

public static class Language
{
    static int choose = 1;
    static string[] language = { "English", "Chinese" };

    static readonly string[] EnglishText =
    {
        "name",
        "person : {0}",
        "gold : {0}",
        "food : {0}",
        "troop food : {0}",
        "materials : {0}",
        "iron : {0}",
        "person : {0} (+{1})/{2}",
        "gold : {0} (+{1})",
        "food : {0} (+{1})",
        "troop food : {0} (+{1})",
        "materials : {0} (+{1})",
        "iron : {0} (+{1})",
    };

    static readonly string[] ChineseText =
    {
        "名字",
        "人口：{0}",
        "金币：{0}",
        "民粮：{0}",
        "军粮：{0}",
        "物资：{0}",
        "铁料：{0}",
        "人口：{0} (+{1})/{2}",
        "金币：{0} (+{1})",
        "民粮：{0} (+{1})",
        "军粮：{0} (+{1})",
        "物资：{0} (+{1})",
        "铁料：{0} (+{1})",
    };

    static readonly string[][] languageText =
    {
        EnglishText,
        ChineseText
    };

    public static string Text(int textNum)
    {
        return languageText[choose][textNum];
    }

    public static string Text(this LanguageText textNum)
    {
        return Text((int)textNum);
    }

    public static string Text(this LanguageText textNum, params object[] o)
    {
        return string.Format(Text((int)textNum), o);
    }
}
