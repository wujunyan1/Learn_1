/*
*Hi, I'm Lin Dong,
*this is about random generation terrain based on fractal noise
*if you want to get more detail please enter my blog http://blog.csdn.net/wolf96
*my email: wolf_crixus@sina.cn 
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//[ExecuteInEditMode]
public class terrain_g : MonoBehaviour
{
    /// <summary>
    /// 变化度
    /// </summary>
    float frequency = 10;

    [Range(1, 8)]
    int octaves = 8;//1普通，8烟雾状

    [Range(1f, 4f)]
    float lacunarity = 1f;

    [Range(0f, 1f)]
    float persistence = 1f;

    int randomSeed = 42;

    //材质和高度图
    public Material diffuseMap;
    public Texture2D heightMap;
    //顶点、uv、索引信息
    private Vector3[] vertives;
    private Vector2[] uvs;
    private int[] triangles;

    //生成信息
    private Vector2 size;//长宽
    private float minHeight = -10;
    private float maxHeight = 10;
    private Vector2Int segment;
    private float unitH;

    //面片mesh
    private GameObject terrain;

    List<BattleTree> trees;

    public terrain_g()
    {
        trees = new List<BattleTree>();
    }

    // Use this for initialization
    void Start()
    {
        //GenerateMap(1,1,1,1,1);
        Debug.Log("1111111111111");
    }

    public void GenerateMap(int seed, float frequency, int octaves
        , float lacunarity, float persistence)
    {
        this.frequency = frequency;
        this.octaves = octaves;//1普通，8烟雾状
        this.lacunarity = lacunarity; //1~4
        this.persistence = persistence; //0~1
        this.randomSeed = seed;

        //默认生成一个地形，如果不喜欢，注销掉然后用参数生成
        //SetTerrain();
        Debug.Log("222222222222");
        SetTerrain();
    }

    /// <summary>
    /// 生成默认地形
    /// </summary>
    public void SetTerrain()
    {
        SetTerrain(100, 100, 200, 200, 0, 10);
    }



    public void SetTerrain(float width, float height, int segmentX, int segmentY, int min, int max)
    {
        Init(width, height, segmentX, segmentY, min, max);
        GetVertives();
        DrawMesh();

        terrain.AddComponent<MeshCollider>();
    }



    private void Init(float width, float height, int segmentX, int segmentY, int min, int max)
    {
        size = new Vector2(width, height);
        maxHeight = max;
        minHeight = min;
        unitH = maxHeight - minHeight;
        segment = new Vector2Int(segmentX, segmentY);
        if (terrain != null)
        {
            Destroy(terrain);
        }
        terrain = new GameObject();
        terrain.name = "plane";
        terrain.transform.SetParent(this.transform);
        terrain.transform.localPosition = new Vector3(-width / 2, 0, -height / 2);
        terrain.layer = LayerMask.NameToLayer("BattleMap");//  LayerMask.GetMask(");
    }

    /// <summary>
    /// 绘制网格
    /// </summary>
    private void DrawMesh()
    {
        Mesh mesh = new Mesh();
        terrain.AddComponent<MeshFilter>().mesh = mesh;
        //GetComponent<MeshFilter>().mesh = new Mesh();

        MeshRenderer renderer = terrain.AddComponent<MeshRenderer>();
        if (diffuseMap == null)
        {
            Debug.LogWarning("No material,Create diffuse!!");
            diffuseMap = new Material(Shader.Find("Diffuse"));
        }
        if (heightMap == null)
        {
            Debug.LogWarning("No heightMap!!!");
        }
        renderer.material = diffuseMap;
        
        //给mesh 赋值
        mesh.Clear();
        mesh.vertices = vertives;//,pos);
        mesh.uv = uvs;
        mesh.triangles = triangles;
        //重置法线
        mesh.RecalculateNormals();
        //重置范围
        mesh.RecalculateBounds();
    }



    /* private Vector3[] GetVertives()
     {
         int sum = Mathf.FloorToInt((segment.x + 1) * (segment.y + 1));
         float w = size.x / segment.x;
         float h = size.y / segment.y;

         int index = 0;
         GetUV();
         GetTriangles();
         vertives = new Vector3[sum];
         for (int i = 0; i < segment.y + 1; i++)
         {
             for (int j = 0; j < segment.x + 1; j++)
             {
                 float tempHeight = 0;
                 if (heightMap != null)
                 {
                     tempHeight = GetHeight(heightMap, uvs[index]);
                 }
                 vertives[index] = new Vector3(j * w, tempHeight, i * h);
                 index++;
             }
         }
         return vertives;
     }
     */


    /* private Vector3[] GetVertives()//texture
      {
          int sum = Mathf.FloorToInt((segment.x + 1) * (segment.y + 1));
          float w = size.x / segment.x;
          float h = size.y / segment.y;

          int index = 0;
          GetUV();
          GetTriangles();
          vertives = new Vector3[sum];
          Vector3 point00 = new Vector3(-0.5f, -0.5f);
          Vector3 point10 = new Vector3(0.5f, -0.5f);
          Vector3 point01 = new Vector3(-0.5f, 0.5f);
          Vector3 point11 = new Vector3(0.5f, 0.5f);



          Random.seed = 42;


          for (int i = 0; i < segment.y + 1; i++)
          {
              for (int j = 0; j < segment.x + 1; j++)
              {
                  float tempHeight = 0;
       //           if ((i+j)%5 == 0)
                  {

                      tempHeight =Mathf.Abs (heightMap.GetPixel(i , j ).r -0.5f)*2* 10;//

                  }
                  vertives[index] = new Vector3(j * w, tempHeight, i * h);
                  index++;
              }
          }
          return vertives;
      }
      */
    /* private Vector3[] GetVertives()//perlin
     {



         int sum = Mathf.FloorToInt((segment.x + 1) * (segment.y + 1));
         float w = size.x / segment.x;
         float h = size.y / segment.y;

         int index = 0;
         GetUV();
         GetTriangles();
         vertives = new Vector3[sum];


         int resolution = 256;
         Vector3 point00 = new Vector3(-0.5f, -0.5f);
         Vector3 point10 = new Vector3(0.5f, -0.5f);
         Vector3 point01 = new Vector3(-0.5f, 0.5f);
         Vector3 point11 = new Vector3(0.5f, 0.5f);


         float stepSize = 1f / resolution;



         Random.seed = 42;


         for (int i = 0; i < segment.y + 1; i++)
         {
             Vector3 point0 = Vector3.Lerp(point00, point01, (i + 0.5f) * stepSize);
             Vector3 point1 = Vector3.Lerp(point10, point11, (i + 0.5f) * stepSize);
             for (int j = 0; j < segment.x + 1; j++)
             {
                 float tempHeight = 0;
                 //           if ((i+j)%5 == 0)
                 {
                     Vector3 point = Vector3.Lerp(point0, point1, (j + 0.5f) * stepSize);

                     tempHeight = NoiseMethod.Perlin2D(point, 22) * 10;//

                 }
                 vertives[index] = new Vector3(j * w, tempHeight, i * h);
                 index++;
             }
         }
         return vertives;
     }*/
    private Vector3[] GetVertives()//Fractal Noise
    {

        float frequency = this.frequency;

        //[Range(1, 8)]
        int octaves = this.octaves;//1普通，8烟雾状

        //	[Range(1f, 4f)]
        float lacunarity = this.lacunarity;

        //	[Range(0f, 1f)]
        float persistence = this.persistence;



        int sum = Mathf.FloorToInt((segment.x + 1) * (segment.y + 1));
        float w = size.x / segment.x;
        float h = size.y / segment.y;

        int index = 0;
        GetUV();
        GetTriangles();
        vertives = new Vector3[sum];


        int resolution = 256;
        Vector3 point00 = new Vector3(-0.5f, -0.5f);
        Vector3 point10 = new Vector3(0.5f, -0.5f);
        Vector3 point01 = new Vector3(-0.5f, 0.5f);
        Vector3 point11 = new Vector3(0.5f, 0.5f);


        float stepSize = 1f / resolution;



        //Random.seed = 42;
        Random.InitState(this.randomSeed);
        NoiseMethod.ResetSeed(this.randomSeed);

        for (int i = 0; i < segment.y + 1; i++)
        {
            Vector3 point0 = Vector3.Lerp(point00, point01, (i + 0.5f) * stepSize);
            Vector3 point1 = Vector3.Lerp(point10, point11, (i + 0.5f) * stepSize);
            for (int j = 0; j < segment.x + 1; j++)
            {
                float tempHeight = 0;
                //           if ((i+j)%5 == 0)
                {
                    Vector3 point = Vector3.Lerp(point0, point1, (j + 0.5f) * stepSize);
                    float sample = NoiseMethod.Sum(1, point, frequency, octaves, lacunarity, persistence);
                    tempHeight = sample * unitH + minHeight;//
                }
                vertives[index] = new Vector3(j * w, tempHeight, i * h);
                index++;
            }
        }
        return vertives;
    }


    private Vector2[] GetUV()
    {
        int sum = Mathf.FloorToInt((segment.x + 1) * (segment.y + 1));
        uvs = new Vector2[sum];
        float u = 1.0F / segment.x;
        float v = 1.0F / segment.y;
        uint index = 0;
        for (int i = 0; i < segment.y + 1; i++)
        {
            for (int j = 0; j < segment.x + 1; j++)
            {
                uvs[index] = new Vector2(j * u, i * v);
                index++;
            }
        }
        return uvs;
    }



    private int[] GetTriangles()
    {
        int sum = Mathf.FloorToInt(segment.x * segment.y * 6);
        triangles = new int[sum];
        uint index = 0;
        for (int i = 0; i < segment.y; i++)
        {
            for (int j = 0; j < segment.x; j++)
            {
                int role = Mathf.FloorToInt(segment.x) + 1;
                int self = j + (i * role);
                int next = j + ((i + 1) * role);
                triangles[index] = self;
                triangles[index + 1] = next + 1;
                triangles[index + 2] = self + 1;
                triangles[index + 3] = self;
                triangles[index + 4] = next;
                triangles[index + 5] = next + 1;
                index += 6;
            }
        }
        return triangles;
    }

    private float GetHeight(Texture2D texture, Vector2 uv)
    {
        if (texture != null)
        {
            //提取灰度。如果强制读取某个通道，可以忽略
            Color c = GetColor(texture, uv);
            float gray = c.grayscale;//或者可以自己指定灰度提取算法，比如：gray = 0.3F * c.r + 0.59F * c.g + 0.11F * c.b;
            float h = unitH * gray;
            return h;
        }
        else
        {
            return 0;
        }
    }


    private Color GetColor(Texture2D texture, Vector2 uv)
    {

        Color color = texture.GetPixel(Mathf.FloorToInt(texture.width * uv.x), Mathf.FloorToInt(texture.height * uv.y));
        return color;
    }


    public void SetPos(Vector3 pos)
    {
        if (terrain)
        {
            terrain.transform.position = pos;
        }
        else
        {
            SetTerrain();
            terrain.transform.position = pos;
        }
    }

    public Vector3 GetMapPos(Vector3 v)
    {
        float d_w = size.x / segment.x;
        float d_h = size.y / segment.y;

        float x = (v.x + size.x / 2) / d_w;
        float z = (v.z + size.y / 2) / d_h;

        int minX = Mathf.FloorToInt(x);
        int maxX = Mathf.CeilToInt(x);
        int minZ = Mathf.FloorToInt(z);
        int maxZ = Mathf.CeilToInt(z);

        minX = minX > segment.x ? segment.x : minX;
        minZ = minZ > segment.y ? segment.y : minZ;

        maxX = maxX > segment.x ? segment.x : maxX;
        maxZ = maxZ > segment.y ? segment.y : maxZ;

        int index = minX + minZ * (segment.x + 1);
        Vector3 pos1 = vertives[index];

        index = minX + maxZ * (segment.x + 1);
        Vector3 pos2 = vertives[index];

        index = maxX + minZ * (segment.x + 1);
        Vector3 pos3 = vertives[index];

        index = maxX + maxZ * (segment.x + 1);
        Vector3 pos4 = vertives[index];
       
        Vector3 min_x = Vector3.Lerp(pos1, pos2, z % 1);
        Vector3 max_x = Vector3.Lerp(pos3, pos4, z % 1);

        Vector3 rv = Vector3.Lerp(min_x, max_x, x % 1);

        rv.x = rv.x - (size.x / 2);
        rv.z = rv.z - (size.y / 2);
        return rv;
    }

    private bool NearHasTree(Vector3 pos)
    {
        float dis = 2.5f;

        foreach (var item in trees)
        {
            if( Vector3Tool.ToVector2(item.transform.localPosition - pos).magnitude < dis)
            {
                return true;
            }
        }

        return false;
    }

    // 生成树
    public IEnumerator GenerateTree()
    {
        float treeNum = 0.02f;
        float treeDensity = 0.2f;
        float treeCreater = 0.3f;

        float w = size.x / segment.x;
        float h = size.y / segment.y;

        int resolution = segment.x > segment.y ? segment.x + 1 : segment.y + 1;
        float stepSize = 1f / resolution;
        Vector3 point00 = new Vector3(-0.5f, -0.5f);
        Vector3 point10 = new Vector3(0.5f, -0.5f);
        Vector3 point01 = new Vector3(-0.5f, 0.5f);
        Vector3 point11 = new Vector3(0.5f, 0.5f);

        Random.InitState(this.randomSeed + 1000);

        yield return null;
        NoiseMethod.ResetSeed(this.randomSeed + 1000);
        int index = 0;
        yield return null;

        for (int i = 0; i < segment.y + 1; i++)
        {
            Vector3 point0 = Vector3.Lerp(point00, point01, i * stepSize);
            Vector3 point1 = Vector3.Lerp(point10, point11, i * stepSize);
            for (int j = 0; j < segment.x + 1; j++)
            {
                Vector3 point = new Vector3(j * treeNum, i * treeNum, 0); //  Vector3.Lerp(point0, point1, j * stepSize);
                float sample = NoiseMethod.Sum(1, point, 1, 8, lacunarity, persistence);


                if(sample > treeDensity && Random.value < treeCreater)
                {
                    //point.x -= size.x / 2;
                    //point.z -= size.y / 2;
                    Vector3 pos = new Vector3(j * w - (size.x / 2), sample, i * h - (size.y / 2));
                    Vector3 pot = GetMapPos(pos);
                    
                    if (!NearHasTree(pot))
                    {
                        BattleTree tree = CreateTree(pot);
                        trees.Add(tree);
                        yield return null;
                    }
                }
                index++;
            }
        }
    }

    private BattleTree CreateTree(Vector3 pos)
    {
        BattleResManager resManager = BattleResManager.GetInstance();
        GameObject treeObjPrefab = resManager.GetObjModel("tree");

        GameObject treeObj = GameObject.Instantiate<GameObject>(treeObjPrefab, pos, Quaternion.Euler(0, 0, 0), this.transform);

        return treeObj.AddComponent<BattleTree>();
    }
}

