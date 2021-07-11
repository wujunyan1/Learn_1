using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class ParamLoader
{
    static ParamLoader instance;

    public static ParamLoader GetInstance()
    {
        if (instance == null)
        {
            instance = new ParamLoader();
        }

        return instance;
    }

    private ParamLoader()
    {
        minDetectionBoxLength = 5;
        m_WaypointSeekDistSq = 1.0f;
    }


    float minDetectionBoxLength;
    public float MinDetectionBoxLength {
        get
        {
            return minDetectionBoxLength;
}
    }

    float m_WaypointSeekDistSq;
    public float WaypointSeekDistSq
    {
        get
        {
            return m_WaypointSeekDistSq;
        }
    }
    
    public Dictionary<string, SoliderConfig> LoadSoliderPrefabConfigCSV()
    {
        Dictionary<string, SoliderConfig> dir = new Dictionary<string, SoliderConfig>();
        List<string[]> data = this.loadFile("Assets/config", "soliderPrefab.csv");

        string[] keys = data[0];
        for (int i = 3; i < data.Count; i++)
        {
            string[] sa = data[i];
            SoliderConfig sd = new SoliderConfig();

            for (int j = 0; j < sa.Length; j++)
            {
                string s = sa[j];
                sd.SetData(keys[j], s);
            }

            dir.Add(sd.key, sd);
        }

        return dir;
    }

    public Dictionary<string, SoldierData> LoadSoliderCSV()
    {
        Dictionary<string, SoldierData> dir = new Dictionary<string, SoldierData>();
        List<string[]> data = this.loadFile("Assets/config", "soliderConfig.csv");

        string[] keys = data[0];
        for (int i = 2; i < data.Count; i++)
        {
            string[] sa = data[i];
            SoldierData sd = new SoldierData();

            for (int j = 0; j < sa.Length; j++)
            {
                string s = sa[j];
                sd.SetData(keys[j], s);
            }

            dir.Add(sd.key, sd);
        }

        return dir;
    }

    public List<string[]> loadFile(string path, string fileName)
    {
        List<string[]> m_ArratData = new List<string[]>();
        m_ArratData.Clear();
        StreamReader sr = null;
        try
        {
            sr = File.OpenText(path + "/" + fileName);
            Debug.Log("file is finded!");
        }
        catch
        {
            Debug.Log("file dont not exist");
            Debug.Log(path + "/" + fileName);
        }
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            m_ArratData.Add(line.Split(','));
        }
        sr.Close();
        sr.Dispose();

        return m_ArratData;
    }

}
