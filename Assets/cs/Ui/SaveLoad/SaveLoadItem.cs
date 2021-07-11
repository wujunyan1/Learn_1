using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SaveLoadItem : MonoBehaviour
{
    public SaveLoadMenu menu;

    public string MapName
    {
        get
        {
            return mapName;
        }
    }
    string mapName;

    int versions;

    DateTime time;

    string filePath;

    public string GetPath()
    {
        return filePath;
    }

    public void SetItem( string path, string mapName, int versions, DateTime time)
    {
        this.filePath = path;
        this.mapName = mapName;
        this.versions = versions;
        this.time = time;
        
        transform.GetChild(0).GetComponent<Text>().text = mapName;
        
        string s = string.Format("ver:{0}      {1}",
            GameVersions.GetOldVersionsStr(versions),
            time.ToString("g")
            );

        transform.GetChild(1).GetComponent<Text>().text = s;
    }

    public void Select()
    {
        menu.SelectItem(this, mapName);
    }

}
