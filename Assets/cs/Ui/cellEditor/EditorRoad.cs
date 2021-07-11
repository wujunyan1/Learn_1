using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorRoad : MonoBehaviour
{
    public void isRoadChange(int mode)
    {
        HexMapEditorData.GetInstance().SetRoadMode(mode);
    }
}
