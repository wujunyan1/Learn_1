using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MapSizeType
{
    Small = 30,
    Medium = 60,
    Large = 120,
}

public class NewMapMenu : MonoBehaviour
{
    //public HexGrid grid;

    public HexMapGenerator mapGenerator;
    public GameCenter gameCenter;

    // 选择大小
    public GameObject chooseSize;

    public Text mapSizeText;

    MapSizeType mapSize = MapSizeType.Small;
    int mapSeed;
    public Text defaultMapSeedText;
    public Text mapSeedText;

    public MapSizeType MapSize
    {
        get
        {
            return mapSize;
        }
        set
        {
            mapSize = value;
            mapSizeText.text = string.Format("{0}", mapSize);
        }
    }

    private void Awake()
    {
        
    }

    public void Open()
    {
        CameraMove.Locked = true;
        gameObject.SetActive(true);
        System.TimeSpan ts = System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 0, 0, 0);
        long ret = System.Convert.ToInt64(ts.TotalSeconds);
        mapSeed = (int)(ret % 100000);

        mapSeedText.text = defaultMapSeedText.text = string.Format("{0}", mapSeed);
    }

    public void Close() {
        CameraMove.Locked = false;
        gameObject.SetActive(false);
    }

    public void ButtonChooseSize()
    {
        chooseSize.SetActive(true);
    }

    public void CancelChooseSize()
    {
        chooseSize.SetActive(false);
    }

    public void ChooseSize(int size)
    {
        MapSize = (MapSizeType)size;
        CancelChooseSize();
    }

    public void MapSeedInputEnd()
    {
        Debug.Log(mapSeedText.text);

        int newSeed;
        if(int.TryParse(mapSeedText.text, out newSeed))
        {
            mapSeed = newSeed;
        }
        Debug.Log(mapSeed);
    }

    public void NewGame()
    {
        NewGameData data = new NewGameData((int)mapSize, (int)mapSize);
        data.mapSeed = mapSeed;

        //mapGenerator.GenerateMap(data);
        gameCenter.GenerateMap(data);

        Close();
    }
}
