using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class HexMapEditor : MonoBehaviour
{
    public Color[] colors;

    public HexGrid hexGrid;

    private Color activeColor;

    public NewMapMenu newMapMenu;

    int activeElevation;

    void Awake()
    {
        SelectColor(0);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            //EditCell(hexGrid.GetCell(hit.point));
        }
    }

    void EditCell(HexCell cell)
    {
        //cell.color = activeColor;
        cell.Elevation = activeElevation;
        //hexGrid.Refresh();
    }


    public void SelectColor(int index)
    {
        activeColor = colors[index];
    }

    public void ChangeColor(bool index)
    {
        activeColor = colors[0];
    }

    public void SetElevation(float elevation)
    {
        activeElevation = (int)elevation;
    }


    public void Save()
    {
        string saveName = "test.map";
        Debug.Log(Application.persistentDataPath);
        string path = Path.Combine(Application.persistentDataPath, saveName);
        using (
            BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create))
            )
        {
            writer.Write(saveName);
            // 版本
            writer.Write(1);
            hexGrid.Save(writer);
        }
    }

    public void Load()
    {
        string saveName = "test.map";
        string path = Path.Combine(Application.persistentDataPath, saveName);
        using (
            BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open))
        )
        {
            //Debug.Log(reader.ReadInt32());
            if(reader.ReadString() == saveName)
            {
                int header = reader.ReadInt32();
                hexGrid.Load(reader);
            }
        }
    }

    public void New()
    {
        gameObject.SetActive(false);
        newMapMenu.Open();
    }
}
