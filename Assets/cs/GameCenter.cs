using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class GameCenter : MonoBehaviour
{
    public static GameCenter instance;

    public HexGrid grid;

    int playerCampId = 0;

    /// <summary>
    /// 玩家阵营默认为0
    /// </summary>
    public int PlayerCampId {
        get
        {
            return playerCampId;
        }
    }

    /// <summary>
    /// 回合数
    /// </summary>
    int round;

    // 每回合经历的天数
    int roundDay;

    public Text roundText;
    public GameObject basePanel;

    // 阵营只会增加不会减少
    List<Camp> camps;

    private void Awake()
    {
        instance = this;
        camps = new List<Camp>();
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(round);
        writer.Write((byte)playerCampId);

        grid.Save(writer);

        foreach (Camp camp in camps)
        {
            camp.Save(writer);
        }
    }

    public void Load(BinaryReader reader)
    {
        round = reader.ReadInt32();
        playerCampId = reader.ReadByte();

        grid.Load(reader);

        foreach (Camp camp in camps)
        {
            camp.Load(reader);
        }

        // 数据Load完之后再刷地图
        grid.Refresh();

        roundText.text = string.Format("{0}", round);
        basePanel.SetActive(true);
    }

    public void GenerateMap(NewGameData data)
    {
        round = 0;
        grid.CreateMap(data);
        roundText.text = string.Format("{0}", round);

        int campNum = data.campNum;
        for(int i = 0; i < campNum; i++)
        {
            AddCamp();
        }

        // 数据Load完之后再刷地图
        grid.Refresh();

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

    void AddCamp()
    {
        Camp camp = new Camp(camps.Count);
        camps.Add(camp);
    }

    public Camp GetCamp(int id)
    {
        return camps[id];
    }
}
