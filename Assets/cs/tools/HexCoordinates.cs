using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public struct HexCoordinates
{
    [SerializeField]
    private int x, z;

    public int X
    {
        get
        {
            return x;
        }
    }

    public int Z
    {
        get
        {
            return z;
        }
    }

    public HexCoordinates(int x, int z)
    {
        if (HexMetrics.Wrapping)
        {
            int oX = x + z / 2;
            if (oX < 0)
            {
                x += HexMetrics.wrapSize;
            }
            else if (oX >= HexMetrics.wrapSize)
            {
                x -= HexMetrics.wrapSize;
            }
        }

        this.x = x;
        this.z = z;
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(x);
        writer.Write(z);
    }

    public static HexCoordinates Load(BinaryReader reader)
    {
        HexCoordinates c;
        c.x = reader.ReadInt32();
        c.z = reader.ReadInt32();
        return c;
    }

    public static HexCoordinates FromOffsetCoordinates(int x, int z)
    {
        return new HexCoordinates(x - z / 2, z);
    }

    public static HexCoordinates FromPosition(Vector3 position)
    {
        float x = position.x / HexMetrics.innerDiameter;
        float y = -x;

        float offset = position.z / (HexMetrics.outerRadius * 3f);
        x -= offset;
        y -= offset;

        int iX = Mathf.RoundToInt(x);
        int iY = Mathf.RoundToInt(y);
        int iZ = Mathf.RoundToInt(-x - y);

        if (iX + iY + iZ != 0)
        {
            float dX = Mathf.Abs(x - iX);
            float dY = Mathf.Abs(y - iY);
            float dZ = Mathf.Abs(-x - y - iZ);
            if (dX > dY && dX > dZ)
            {
                iX = -iY - iZ;
            }
            else if (dZ > dY)
            {
                iZ = -iX - iY;
            }
        }

        return new HexCoordinates(iX, iZ);
    }

    // 六边形坐标系转世界坐标
    public static Vector2 HexToGameCoordinateV2(int x, int z)
    {
        Vector2 position = new Vector2(0f, 0f);
        position.x = (x + z * 0.5f) * (HexMetrics.innerRadius * 2f);
        // position.y = 0f;
        position.y = z * (HexMetrics.outerRadius * 1.5f);
        return Vector2.zero;
    }

    // 六边形坐标系转世界坐标
    public static Vector3 HexToGameCoordinateV3(int x, int z)
    {
        return HexToGameCoordinateV3(x, 0, z);
    }

    // 六边形坐标系转世界坐标
    public static Vector3 HexToGameCoordinateV3(int x, int y, int z)
    {
        Vector3 position = new Vector3(0f, 0f, 0f);
        position.x = (x + z * 0.5f) * (HexMetrics.innerRadius * 2f);
        position.y = HexMetrics.cellHeight * y;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        return position;
    }

    // 世界坐标系转六边形坐标
    public static HexVector GameToHexCoordinate(Vector3 position)
    {
        float x = position.x / (HexMetrics.innerRadius * 2f);
        float y = -x;

        float offset = position.z / (HexMetrics.outerRadius * 3f);
        x -= offset;
        y -= offset;
        
        int iX = Mathf.RoundToInt(x);
        int iY = Mathf.RoundToInt(y);
        int iZ = Mathf.RoundToInt(-x - y);
        
        if (iX + iY + iZ != 0)
        {
            Debug.Log("xxxxxxxxxxxxxxxxxx");
            float dX = Mathf.Abs(x - iX);
            float dY = Mathf.Abs(y - iY);
            float dZ = Mathf.Abs(-x - y - iZ);
            if (dX > dY && dX > dZ)
            {
                iX = -iY - iZ;
            }
            else if (dZ > dY)
            {
                iZ = -iX - iY;
            }
        }

        return new HexVector(iX, iZ);
    }
    /*
    public override string ToString()
    {
        return "(" + X.ToString() + ", " + Z.ToString() + ")";
    }

    public string ToStringOnSeparateLines()
    {
        return X.ToString() + "\n" + Z.ToString();
    }
    */

    // 增加Y轴坐标
    public int Y
    {
        get
        {
            return -X - Z;
        }
    }

    public int DistanceTo(HexCoordinates other)
    {
        int xy =
            (x < other.x ? other.x - x : x - other.x) +
            (Y < other.Y ? other.Y - Y : Y - other.Y);

        if (HexMetrics.Wrapping)
        {
            other.x += HexMetrics.wrapSize;
            int xyWrapped =
                (x < other.x ? other.x - x : x - other.x) +
                (Y < other.Y ? other.Y - Y : Y - other.Y);
            if (xyWrapped < xy)
            {
                xy = xyWrapped;
            }
            else
            {
                other.x -= 2 * HexMetrics.wrapSize;
                xyWrapped =
                    (x < other.x ? other.x - x : x - other.x) +
                    (Y < other.Y ? other.Y - Y : Y - other.Y);
                if (xyWrapped < xy)
                {
                    xy = xyWrapped;
                }
            }
        }
        return (xy + (z < other.z ? other.z - z : z - other.z)) / 2;
    }

    public override string ToString()
    {
        return "(" +
            X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
    }

    public string ToStringOnSeparateLines()
    {
        return X.ToString() + "\n" + Y.ToString() + "\n" + Z.ToString();
    }

    public Vector3 GetPosition()
    {
        HexCell cell = HexGrid.instance.GetCell(this);
        return cell.Position;
    }

    public override bool Equals(object obj)
    {
        HexCoordinates o = (HexCoordinates)obj;
        if(o.X == this.X && o.Z == this.Z)
        {
            return true;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
