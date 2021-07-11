using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoadData
{
    private static GameLoadData instance;
    private static object locker = new object();

    public NewGameData newGameData = null;
    public string loadPath = null;

    public static GameLoadData GetInstance()
    {
        if (instance == null)
        {
            lock (locker)
            {
                if (instance == null)
                {
                    instance = new GameLoadData();
                }
            }
        }
        return instance;
    }

    private GameLoadData()
    {

    }

    public void Clear()
    {
        newGameData = null;
        loadPath = null;
    }


    public void StartLoadGame()
    {
        Scene scene = SceneManager.GetActiveScene();
        //mapGenerator.GenerateMap(data);

        Debug.Log(scene.name);
        if (scene.name == "BattleScene")
        {

            LoadBattleRes view;
            if (ViewManager.Instance.OpenView("LoadBattleRes", out view))
            {
                
            }

            //GameCenter.instance.GenerateMap(newGameData);
        }
        else
        {
            SceneManager.LoadScene("BattleScene");
        }
    }
}
