using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorHeight : MonoBehaviour
{
    public Slider slider;
    public Toggle toggle;

    private void Start()
    {
        SelectUpdateHeight();
    }

    public void SelectUpdateHeight()
    {

        HexMapEditorData.GetInstance().SetUpdateHeight(toggle.isOn);
    }

    public void HeightValueChange()
    {
        int value = (int)slider.value;
        HexMapEditorData.GetInstance().UpdateHeight(value);
    }
}
