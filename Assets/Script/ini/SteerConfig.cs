using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SteerConfig : ScriptableObject
{
    public float minDetectionBoxLength = 5;
    public float m_WaypointSeekDistSq = 1.0f;
}
