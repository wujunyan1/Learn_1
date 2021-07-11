using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoliderConfig
{
    public string key;
    public string prefabPath;
    public string soliderKey;
    public string generaPath;

    public void SetData(string dataKey, string data)
    {
        switch (dataKey)
        {
            case "key":
                this.key = data;
                break;
            case "prefabPath":
                this.prefabPath = data;
                break;
            case "soliderKey":
                this.soliderKey = data;
                break;
            case "generaPath":
                this.generaPath = data;
                break;
        }
    }
}
