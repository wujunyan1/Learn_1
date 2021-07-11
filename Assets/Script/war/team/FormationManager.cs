using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum FormationType
{
    /// <summary>
    /// 矩形
    /// </summary>
    RECTANGLE,
}

/// <summary>
/// 阵型管理
/// </summary>
public class FormationManager
{
    public static Formation GetDafultFormation(int num)
    {
        return new Formation(num);
    }
}
