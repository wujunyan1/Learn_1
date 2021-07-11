using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorRiver : MonoBehaviour
{

    public void isRiverChange(int mode)
    {
        HexMapEditorData.GetInstance().UpdateRiver(mode);
    }
}
