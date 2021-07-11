using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗地图
/// </summary>
public class BattleMap : MonoBehaviour
{
    // 六边形战斗地图半径
    public const float P = 0.866025404f;
    public const float mapRadius = 30;
    public const float mapInnerRadius = mapRadius * 0.866025404f;

    public const float mapOutnerRadius = 2 * mapInnerRadius;


    /// <summary>
    /// 地图上的圆形障碍物
    /// </summary>
    List<BaseGameEntity> obstacles;

    BPolygon bPolygon;


    public terrain_g terrain_g;

    public BattleMap()
    {
        obstacles = new List<BaseGameEntity>();

        bPolygon = new BPolygon();

        Vector2 one = new Vector2(-mapRadius / 2, mapInnerRadius);
        Vector2 two = new Vector2(mapRadius / 2, mapInnerRadius);
        Vector2 three = new Vector2(mapRadius, 0);
        Vector2 four = new Vector2(mapRadius / 2, -mapInnerRadius);
        Vector2 five = new Vector2(-mapRadius / 2, -mapInnerRadius);
        Vector2 six = new Vector2(-mapRadius, 0);

        //bPolygon.lines.Add(new Line(one, two));
        //bPolygon.lines.Add(new Line(two, three));
        //bPolygon.lines.Add(new Line(three, four));
        //bPolygon.lines.Add(new Line(four, five));
        //bPolygon.lines.Add(new Line(five, six));
        //bPolygon.lines.Add(new Line(six, one));
    }

    // 判断是否在战斗区域内

    public bool isInBattleArea(Vector3 point)
    {
        float x = Mathf.Abs(point.x);
        float z = Mathf.Abs(point.z);

        if(x > mapRadius || z > mapInnerRadius)
        {
            return false;
        }

        // (mapRadius - x) / mapRadius = (mapOutnerRadius - z) / mapOutnerRadius
        float _z = mapOutnerRadius - ((mapRadius - x) * 2 * 0.866025404f);

        if(z > _z)
        {
            return false;
        }

        return true;
    }

    // 不同阵容起始位置区域
    // 玩家所处阵容为下方
    // 后续需要修改 添加多阵营对抗
    public Rect GetStartArea(int camp)
    {
        float max_z = -P * mapRadius * 0.8f;
        float min_z = -mapInnerRadius;

        float max_x = mapRadius * 0.4f;
        float min_x = -mapRadius * 0.4f;

        float width = mapRadius * 0.8f;
        float hight = mapRadius * P * 0.2f;

        if (camp == BattleWorld.playerCamp)
        {
            min_z = -mapInnerRadius;
        }
        else
        {
            min_z = mapInnerRadius - hight;
        }
        return new Rect(min_x, min_z, width, hight);
    }

    public BPolygon GetBoundary()
    {
        return bPolygon;
    }

    public List<BaseGameEntity> getObstacles()
    {
        return obstacles;
    }

    public void GenerateMap(int seed, float frequency, int octaves
        , float lacunarity, float persistence)
    {
        terrain_g.GenerateMap(seed, frequency, octaves, lacunarity, persistence);
    }

    public IEnumerator GenerateTree()
    {
        return terrain_g.GenerateTree();
    }

    public Vector3 GetPos(Vector3 v)
    {
        return terrain_g.GetMapPos(v);
    }

}
