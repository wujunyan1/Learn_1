using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReadlyEditorMapView : View
{
    public InputField mapSeedText;

    public InputField widthText;
    public InputField heightText;

    private void Start()
    {
        mapSeedText.text = "111";
        widthText.text = "10";
        heightText.text = "10";
    }

    public void StartBattle()
    {
        NewGameData data = new NewGameData(int.Parse(widthText.text), int.Parse(heightText.text));

        data.mapSeed = int.Parse(mapSeedText.text);
        data.isCanEditor = true;
        data.isRandMap = false;
        data.campNum = 3;
        data.equator = int.Parse(heightText.text) / 2;

        GameLoadData gameLoadData = GameLoadData.GetInstance();
        gameLoadData.Clear();
        gameLoadData.newGameData = data;

        SceneManager.LoadScene("Scenes/BattleScene");
    }
}
