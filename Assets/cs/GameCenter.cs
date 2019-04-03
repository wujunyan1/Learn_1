using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class GameCenter : MonoBehaviour
{
    public static GameCenter instance;

    public HexGrid grid;

    /// <summary>
    /// 回合数
    /// </summary>
    int round;

    // 每回合经历的天数
    int roundDay;

    public Text roundText;
    public GameObject basePanel;

    List<Camp> camps;

    private void Awake()
    {
        instance = this;
        camps = new List<Camp>();
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(round);
        grid.Save(writer);
    }

    public void Load(BinaryReader reader)
    {
        round = reader.ReadInt32();
        grid.Load(reader);

        roundText.text = string.Format("{0}", round);
        basePanel.SetActive(true);
    }

    public void GenerateMap(NewGameData data)
    {
        round = 0;
        grid.CreateMap(data);
        roundText.text = string.Format("{0}", round);
        basePanel.SetActive(true);
    }

    public void NextRound()
    {
        round++;
        roundText.text = string.Format("{0}", round);

        foreach (Camp camp in camps)
        {
            camp.NextRound();
        }
    }
}
