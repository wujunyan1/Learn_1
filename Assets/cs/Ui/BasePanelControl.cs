using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasePanelControl : MonoBehaviour
{
    public Material terrainMaterial;
    public Toggle toggle;

    void Awake()
    {
        terrainMaterial.DisableKeyword("GRID_ON");
    }

    private void Start()
    {
        GridOnValueChanged();
    }

    public void GridOnValueChanged()
    {
        ShowGrid(toggle.isOn);
    }

    public void ShowGrid(bool visible)
    {
        if (visible)
        {
            terrainMaterial.EnableKeyword("GRID_ON");
        }
        else
        {
            terrainMaterial.DisableKeyword("GRID_ON");
        }
    }
}
