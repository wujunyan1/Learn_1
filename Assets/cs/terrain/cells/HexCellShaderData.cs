using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCellShaderData : MonoBehaviour
{
    Texture2D cellTexture;
    Color32[] cellTextureData;

    public Texture2D cellCampTexture;
    public Color32[] cellCampTextureData;

    bool campChanged = false;

    const float transitionSpeed = 255f;
    List<HexCell> transitioningCells = new List<HexCell>();
    // 地图可见性（有，无）变化的的过渡
    public bool ImmediateMode { get; set; }

    // 是否需要重置可见性（亮，暗）
    bool needsVisibilityReset;



    public HexGrid Grid { get; set; }


    public void Initialize(int x, int z)
    {
        if (cellTexture)
        {
            cellTexture.Resize(x, z);
        }
        else
        {

            cellTexture = new Texture2D(
                x, z, TextureFormat.RGBA32, false, true
            );
            cellTexture.filterMode = FilterMode.Point;
            //cellTexture.wrapMode = TextureWrapMode.Clamp;
            cellTexture.wrapModeU = TextureWrapMode.Repeat;
            cellTexture.wrapModeV = TextureWrapMode.Clamp;
            Shader.SetGlobalTexture("_HexCellData", cellTexture);

        }
        Shader.SetGlobalVector(
            "_HexCellData_TexelSize",
            new Vector4(1f / x, 1f / z, x, z)
        );

        if (cellTextureData == null || cellTextureData.Length != x * z)
        {
            cellTextureData = new Color32[x * z];

            for (int i = 0; i < cellTextureData.Length; i++)
            {
                cellTextureData[i] = new Color32(0, 0, 0, 0);
            }
        }
        else
        {
            for (int i = 0; i < cellTextureData.Length; i++)
            {
                cellTextureData[i] = new Color32(0, 0, 0, 0);
            }
        }

        transitioningCells.Clear();


        if (cellCampTexture)
        {
            cellCampTexture.Resize(x, z);
        }
        else
        {
            cellCampTexture = new Texture2D(
                x, z, TextureFormat.RGBA32, false, true
            );
            cellCampTexture.filterMode = FilterMode.Point;
            cellCampTexture.wrapModeU = TextureWrapMode.Repeat;
            cellCampTexture.wrapModeV = TextureWrapMode.Clamp;
            Shader.SetGlobalTexture("_campColor", cellCampTexture);
        }

        if (cellCampTextureData == null || cellCampTextureData.Length != x * z)
        {
            cellCampTextureData = new Color32[x * z];

            for (int i = 0; i < cellCampTextureData.Length; i++)
            {
                cellCampTextureData[i] = new Color32(0, 0, 0, 0);
            }
        }
        else
        {
            for (int i = 0; i < cellCampTextureData.Length; i++)
            {
                cellCampTextureData[i] = new Color32(0, 0, 0, 0);
            }
        }


        enabled = true;
    }

    public void RefreshTerrain(HexCell cell)
    {
        cellTextureData[cell.index].a = (byte)cell.TerrainType.LandType();
        enabled = true;
    }

    void LateUpdate()
    {
        if (needsVisibilityReset)
        {
            needsVisibilityReset = false;
            Grid.ResetVisibility();
        }

        // 可见性变化速率
        int delta = (int)(Time.deltaTime * transitionSpeed);
        if (delta == 0)
        {
            delta = 1;
        }
        for (int i = 0; i < transitioningCells.Count; i++)
        {
            // 这个不需要迭代时，去掉
            if (!UpdateCellData(transitioningCells[i], delta))
            {
                // 将需要去掉的设置为最后的数据，然后去掉最后的，完成删除
                transitioningCells[i--] =
                    transitioningCells[transitioningCells.Count - 1];
                transitioningCells.RemoveAt(transitioningCells.Count - 1);
            }
        }

        cellTexture.SetPixels32(cellTextureData);
        cellTexture.Apply();


        if (campChanged)
        {
            campChanged = false;
            cellCampTexture.SetPixels32(cellCampTextureData);
            cellCampTexture.Apply();
        }


        enabled = transitioningCells.Count > 0;
    }

    public void RefreshVisibility(HexCell cell)
    {
        int index = cell.index;
        if (ImmediateMode)
        {
            cellTextureData[index].r = cell.IsVisible ? (byte)255 : (byte)0;
            cellTextureData[index].g = cell.IsExplored ? (byte)255 : (byte)0;
        }
        else if(cellTextureData[index].b != 255)
        {
            cellTextureData[index].b = 255;
            transitioningCells.Add(cell);
        }
        enabled = true;
    }


    bool UpdateCellData(HexCell cell, int delta)
    {
        int index = cell.index;
        Color32 data = cellTextureData[index];
        bool stillUpdating = false;
        // 已经被侦测了，但可见性没变为255 则正在变化中
        if (cell.IsExplored && data.g < 255)
        {
            stillUpdating = true;
            int t = data.g + delta;
            data.g = t >= 255 ? (byte)255 : (byte)t;
        }

        if (cell.IsVisible)
        {
            if (data.r < 255)
            {
                stillUpdating = true;
                int t = data.r + delta;
                data.r = t >= 255 ? (byte)255 : (byte)t;
            }
        }
        // 正在变暗
        else if (data.r > 0)
        {
            stillUpdating = true;
            int t = data.r - delta;
            data.r = t < 0 ? (byte)0 : (byte)t;
        }

        if (!stillUpdating)
        {
            data.b = 0;
        }
        cellTextureData[index] = data;
        return stillUpdating;
    }


    public void ViewElevationChanged()
    {
        needsVisibilityReset = true;
        enabled = true;
    }

    public void SetMapData(HexCell cell, float data)
    {
        cellTextureData[cell.index].b =
            data < 0f ? (byte)0 : (data < 1f ? (byte)(data * 254f) : (byte)254);
        enabled = true;
    }

    public void UpdateCellCampData(HexCell cell, int camp)
    {
        int index = cell.index;
        cellCampTextureData[index] = GameCenter.instance.campColors[camp];

        campChanged = true;
    }

}
