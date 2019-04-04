using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class GameCenter : MonoBehaviour
{
    public static GameCenter instance;

    public HexGrid grid;

    public ObjGenerate objGenerate;

    public Transform camera;

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

        LoadGenerateMapEnd();
    }

    public void GenerateMap(NewGameData data)
    {
        round = 0;
        grid.CreateMap(data);

        int campNum = data.campNum;
        for(int i = 0; i < campNum; i++)
        {
            AddCamp();
        }

        LoadGenerateMapEnd();
    }

    /// <summary>
    /// 数据加载完之后
    /// </summary>
    void LoadGenerateMapEnd()
    {
        // 数据Load完之后再刷地图
        grid.Refresh();

        // 再修改摄像机位置，放置到对应阵营建造者上
        Transform transform = GetPlayerCenterTransform();
        ResetCamera(transform);

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

    void AddCamp()
    {
        Camp camp = new Camp(camps.Count);
        camps.Add(camp);
    }

    public Camp GetCamp(int id)
    {
        return camps[id];
    }

    public void GenerateCreater(Creater creater)
    {
        objGenerate.GenerateCreater(creater);
    }


    /// <summary>
    /// 获取玩家的中心位置
    /// </summary>
    /// <returns></returns>
    public Transform GetPlayerCenterTransform()
    {
        Camp camp = GetCamp(playerCampId);
        City city = camp.GetCity(0);
        if(city != null)
        {
            HexCell cityCell = grid.GetCell(city.GetPoint());
            return cityCell.transform;
        }

        Creater creater = camp.GetCreater(0);
        HexCell cell = grid.GetCell(creater.point);
        return cell.transform;
    }

    /// <summary>
    /// 使摄像机 观看这个对象
    /// </summary>
    /// <param name="transform"></param>
    public void ResetCamera(Transform transform)
    {
        Vector3 vector = transform.localPosition;
        vector.y += 10;
        camera.transform.localPosition = vector;

        camera.transform.Translate(Vector3.forward * -1 * 30);
    }
}
