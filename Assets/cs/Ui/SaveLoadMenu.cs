using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class SaveLoadMenu : MonoBehaviour
{
    public GameCenter gameCenter;

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

    public void Open(bool saveMode)
    {
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
        gameObject.SetActive(true);
        CameraMove.Locked = true;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        CameraMove.Locked = false;
    }

    string GetSelectedPath()
    {
        string mapName = nameInput.text;
        if (mapName.Length == 0)
        {
            return null;
        }
        return Path.Combine(Application.persistentDataPath, mapName + saveFile);
    }

    public void SelectItem(string name)
    {
        nameInput.text = name;
    }

    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="path">保存的路径</param>
    public void Save(string path)
    {
        using (
            BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create))
            )
        {
            // 版本
            writer.Write(header);
            gameCenter.Save(writer);
        }
    }

    /// <summary>
    /// 加载
    /// </summary>
    /// <param name="path">加载的路径</param>
    public void Load(string path)
    {
        Debug.Log(path);
        if (!File.Exists(path))
        {
            Debug.LogError("File does not exist " + path);
            return;
        }
        using (
            BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open))
        )
        {
            int fileHeader = reader.ReadInt32();
            Debug.Log(fileHeader);
            if (fileHeader == header)
            {
                gameCenter.Load(reader);
            }
        }
    }

    /// <summary>
    /// 点击了保存或是加载
    /// </summary>
    public void Action()
    {
        string path = GetSelectedPath();
        if (path == null)
        {
            Debug.Log("path is null");
            return;
        }
        if (saveMode)
        {
            Save(path);
        }
        else
        {
            Load(path);
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

        // 取出所有文件
        string[] paths = Directory.GetFiles(Application.persistentDataPath, "*"+saveFile);

        Debug.Log(paths.Length);
        Array.Sort(paths);

        // 添加列表
        for (int i = 0; i < paths.Length; i++)
        {
            SaveLoadItem item = Instantiate(itemPrefab);
            item.menu = this;
            item.MapName = Path.GetFileNameWithoutExtension(paths[i]);
            item.transform.SetParent(listContent, false);
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
        FillList();
    }
}
