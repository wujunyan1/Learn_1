using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 矩形阵型
/// </summary>
public class Formation
{
    protected FormationType _formationType;
    public FormationType formationType
    {
        get
        {
            return _formationType;
        }
    }

    // 宽度 一行多少人
    protected int width;

    /// <summary>
    /// 列间距
    /// </summary>
    public float lineSpace;

    /// <summary>
    /// 行间距
    /// </summary>
    public float rowSpace;

    protected List<Vector2Int> offsets;

    // 谁在什么位置 offsetId[y][x] = soliderId
    protected List<List<int>> offsetIds;

    protected int maxNum;

    // 是否已经初始化
    protected bool isInitOffset;

    // 中心的index
    public int center_index;

    public int centerIndex {
        get
        {
            return center_index;
        }
    }

    public float angle;

    public Formation(int maxNum)
    {
        _formationType = FormationType.RECTANGLE;
        this.maxNum = maxNum;
        offsets = new List<Vector2Int>();
        offsetIds = new List<List<int>>();
        isInitOffset = false;
    }

    public virtual void SetWidth(int width)
    {
        this.width = width;
    }

    public virtual void SetCenterIndex(int index)
    {
        center_index = index;

        if (!isInitOffset)
        {
            ClacInitOffsets();
        }
    }

    /// <summary>
    /// 第一次计算偏移位置
    /// </summary>
    public virtual void ClacInitOffsets()
    {
        offsetIds.Clear();
        offsetIds = new List<List<int>>();

        List<int> row = new List<int>();
        for(int i = 0; i < maxNum; i++)
        {
            if(row.Count >= width)
            {
                row = new List<int>();
                this.offsetIds.Add(row);
            }

            row.Add(i);
        }
    }

    // 之后发生的偏移
    public virtual void ClacOffsets(int old_width)
    {
        int n_center = width / 2;
        int o_center = old_width / 2;

        int min_o_i = o_center - n_center + 1;
        int max_o_i = o_center + width - n_center;

        for(int i = 0; i< maxNum; i++)
        {
            
        }
    }

    public Vector3 GetVector3(int index)
    {
        return new Vector3(0, 0, 0);
    }

    public void CalcWidth(float distance)
    {

    }

    public class PosIndex
    {
        public float pos;
        public int index;
        public int order;

        public PosIndex(float pos, int index)
        {
            this.pos = pos;
            this.index = index;
            order = 0;
        }
    }

    

}
