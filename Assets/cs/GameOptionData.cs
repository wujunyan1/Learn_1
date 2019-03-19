using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOptionData : MonoBehaviour
{
    private static GameOptionData instance;

    // 湿度  1是100%
    public float humidity;



    private GameOptionData()
    {
        humidity = 2f;
    }

    public static GameOptionData getInstance()
    {
        if(!instance)
        {
            instance = new GameOptionData();
        }

        return instance;
    }
}
