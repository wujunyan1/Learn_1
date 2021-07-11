using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorRange : MonoBehaviour
{
    public Slider slider;

    public void RangeValueChange()
    {
        int value = (int)slider.value;
        HexMapEditorData.GetInstance().UpdateBrushSize(value);
    }
}
