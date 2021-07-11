using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class SaveLoadMenu : LockCameraView
{
    GameCenter gameCenter;

    /// <summary>
    ///  保存还是加载
    ///  true 是保存
    ///  false 是加载
    /// </summary>
    bool saveMode;

    public Text menuLabel, actionButtonLabel;

    public InputField nameInput;

    /// <summary>
    ///  版本
    /// </summary>
    int header = 1;

    /// <summary>
    /// 列表
    /// </summary>
    public RectTransform listContent;

    /// <summary>
    /// 每个地图选项
    /// </summary>
    public SaveLoadItem itemPrefab;

    string saveFile = ".save";

    const char verSplit = '_';

    List<SaveLoadItem> items;
    SaveLoadItem selectItem;

    private void Awake()
    {
        items = new List<SaveLoadItem>();
    }

    public override void Open(UObject o)
    {
        base.Open(o);

        gameCenter = GameCenter.instance;

        bool saveMode = o.GetT<bool>("saveMode", false);

        this.saveMode = saveMode;
        if (saveMode)
        {
            menuLabel.text = "Save Map";
            actionButtonLabel.text = "Save";
        }
        else
        {
            menuLabel.text = "Load Map";
            actionButtonLabel.text = "Load";
        }

        FillList();
    }

    public override void Close()
    {
        base.Close();
    }

    string GetSelectedPath()
    {
        string mapName = nameInput.text;
        if (mapName.Length == 0)
        {
            return null;
        }

        if (selectItem != null)
        {
            if (selectItem.MapName == mapName)
            {
                return selectItem.GetPath();
            }
        }

        // 判断是否有文件名相同的
        for (int i = 0; i < items.Count; i++)
        {
            if(items[i].MapName == mapName)
            {
                return items[i].GetPath();
            }
        }

        // 没有选择已有的文件

        mapName += verSplit + GameVersions.Versions;

        return Path.Combine(Application.persistentDataPath, mapName + saveFile);
    }

    public void SelectItem( SaveLoadItem item, string name)
    {
        nameInput.text = name;
        selectItem = item;
    }

    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="path">保存的路径</param>
    public void Save()
    {
        string mapName = nameInput.text;

        // 判断是否是同一个文件名，但版本不同
        if (selectItem != null)
        {
            if (selectItem.MapName == mapName)
            {
                // 删除老的版本
                File.Delete(selectItem.GetPath());
            }
        }

        mapName += "_" + GameVersions.Versions;
        string path = Path.Combine(Application.persistentDataPath, mapName + saveFile);

        using (
            BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create))
            )
        {
            // 版本
            writer.Write(GameVersions.Versions);
            gameCenter.Save(writer);
        }
    }

    /// <summary>
    /// 加载
    /// </summary>
    /// <param name="path">加载的路径</param>
    public void Load()
    {
        string path = GetSelectedPath();

        Debug.Log(path);
        if (!File.Exists(path))
        {
            Debug.LogError("File does not exist " + path);
            return;
        }

        string fileName = Path.GetFileNameWithoutExtension(path);
        string[] strs = fileName.Split(verSplit);
        int fileVer = int.Parse(strs[1]);

        // 版本低于现在的，可以加载
        if (fileVer <= GameVersions.SaveHeader)
        {
            GameLoadData gameLoadData = GameLoadData.GetInstance();
            gameLoadData.Clear();
            gameLoadData.loadPath = path;

            gameLoadData.StartLoadGame();
        }
        else
        {
            Debug.Log("版本不对");
        }
    }

    /// <summary>
    /// 点击了保存或是加载
    /// </summary>
    public void Action()
    {
        string mapName = nameInput.text;
        if (mapName.Length == 0)
        {
            Debug.Log("path is null");
            return;
        }
        if (saveMode)
        {
            Save();
        }
        else
        {
            Load();
        }
        Close();
    }

    void FillList()
    {
        // 移除原来的数据
        for (int i = 0; i < listContent.childCount; i++)
        {
            Destroy(listContent.GetChild(i).gameObject);
        }
        items.Clear();

        // 取出所有文件
        string[] paths = Directory.GetFiles(Application.persistentDataPath, "*"+saveFile);


        Array.Sort(paths,
            delegate(string p1, string p2)
            {
                DateTime t1 = Directory.GetLastWriteTime(p1);
                DateTime t2 = Directory.GetLastWriteTime(p2);
                

                return t2.CompareTo(t1);
            }
         );

        // 添加列表
        for (int i = 0; i < paths.Length; i++)
        {
            string path = paths[i];
            SaveLoadItem item = Instantiate(itemPrefab);
            item.menu = this;

            DateTime time = Directory.GetLastWriteTime(path);
            string fileName = Path.GetFileNameWithoutExtension(path);

            string[] strs = fileName.Split(verSplit);
            item.SetItem(path, strs[0], int.Parse(strs[1]), time);
            
            item.transform.SetParent(listContent, false);

            items.Add(item);
        }

    }

    public void Delete()
    {
        string path = GetSelectedPath();
        if (path == null)
        {
            return;
        }
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        nameInput.text = "";
        selectItem = null;
        FillList();
    }
}
