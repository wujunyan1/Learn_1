using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorColor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameConfigData configData = GameConfigData.GetInstance();
        HexTerrainType[] colors = configData.GetColors();

        GameObject toggerPrefab = this.transform.Find("Toggle0").gameObject;

        Toggle baseToggle = toggerPrefab.GetComponent<Toggle>();
        baseToggle.onValueChanged.AddListener((bool isOn) => { OnToggleClick(baseToggle, 0, isOn); });
        Image baseBackground = baseToggle.GetComponentInChildren<Image>();
        Text bt = baseBackground.GetComponentInChildren<Text>();
        bt.text = colors[0].Name();
        //baseBackground.color = colors[0];

        for (int i = 0; i < colors.Length; i++)
        {
            Transform transform = this.transform.Find("Toggle" + i);
            if (transform == null)
            {
                GameObject toggerObj = GameObject.Instantiate(toggerPrefab, this.transform);
                toggerObj.name = "Toggle" + i;
                Toggle toggle = toggerObj.GetComponent<Toggle>();

                int index = i;
                Image background = toggle.GetComponentInChildren<Image>();
                Text t = background.GetComponentInChildren<Text>();
                t.text = colors[index].Name();
                //background.color = colors[index];

                toggle.onValueChanged.AddListener((bool isOn) => { OnToggleClick(toggle, index, isOn); });
                
            }
        }
    }

    // 某个按钮被点击了
    void OnToggleClick(Toggle toggle, int index, bool isOn)
    {
        if (isOn)
        {
            GameConfigData configData = GameConfigData.GetInstance();
            HexTerrainType[] colors = configData.GetColors();

            HexTerrainType color = colors[index];

            HexMapEditorData.GetInstance().UpdateColor(color);
        }
    }


}
