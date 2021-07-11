using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗常量
/// </summary>
public class BattleConstant
{
    public const float deltaTime = 1.0f / 30;

    /// <summary>
    /// 战斗人物移动速度修正
    /// </summary>
    public const float moveSpeedCorrection = 1.0f / 8;

    public const float maxMoveSpeed = 250f;

    public const float collisionDetectionDistance = 30;
}
