using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorWall : MonoBehaviour
{
    // Start is called before the first frame update
    Toggle[] toggles;
    bool isInit = false;

    void Start()
    {
        //toggles = new Toggle[HexMetrics.HexDirectionNum];
        toggles = GetComponentsInChildren<Toggle>();
        isInit = true;
    }

    public void isWallChange(int mode)
    {
        if (!isInit)
        {
            return;
        }
        bool ison = toggles[mode].isOn;

        HexMapEditorData.GetInstance().SetWallDir(mode, ison);
    }
}
