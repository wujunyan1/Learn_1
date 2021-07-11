using System;
using System.Reflection;

/// <summary>
/// 纯数据 英雄配置文件
/// </summary>
public class HeroConfigData
{
    public int id = 0;
    public string name = "";
    public int blood = 0;

    public int attackRange = 0;
    public int attack { get; set; }
    public int attackSpeed;
    public int moveSpeed;
    public int fangyu;
    public int mofang;
    public int faqiang;

    public int addBlood;
    public int addAttack;
    public int addAttackSpeed;
    public int addMoveSpeed;
    public int addFangyu;
    public int addMofang;
    public int addFaqiang;

    public int maxLevel;

    public int star;
    public string headRes;
    public string msg;
    public int num;

    public string modPath;

    public string PrintString()
    {
        return ObjBaseTool.PrintObj(this);
    }
}
