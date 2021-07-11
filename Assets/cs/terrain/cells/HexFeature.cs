using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 格子上的添加物类型
/// </summary>
public enum HexFeatureType
{
    /// <summary>
    /// 没有
    /// </summary>
    NULL,

    /// <summary>
    /// 树
    /// </summary>
    Wood,               // 树

    /// <summary>
    /// 金矿
    /// </summary>
    Gold,

    /// <summary>
    /// 建筑
    /// </summary>
    Build,

    /// <summary>
    /// 农场
    /// </summary>
    Farm,
}
