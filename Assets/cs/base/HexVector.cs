using UnityEngine;

[System.Serializable]
public class HexVector
{
    // 宽
    private int x;
    // 长
    private int z;
    // 高
    private int y;

    public int X{
        get
        {
            return x;
        }
    }

    public int Y{
        get
        {
            return y;
        }
        set
        {
            y = value;
        }
    }

    public int Z{
        get
        {
            return z;
        }
    }

    public HexVector () : this(0 , 0, 0) {
	}

    public HexVector (int x, int z) : this(x , 0, z) {
	}

    public HexVector (int x, int y, int z) {
		this.x = x;
		this.z = z;
        this.y = y;
	}

    public int GetIndex(){
        // GameConfigData data = GameConfigData.GetInstance();
        NewGameData data = GameCenter.instance.gameData;

        return x + this.z * data.width + this.z / 2;
    }

    public static HexVector FromOffsetCoordinates (int x, int z) {
		return FromOffsetCoordinates(x, 0, z);
	}

    public static HexVector FromOffsetCoordinates (int x, int y, int z) {
		return new HexVector(x - z / 2 , y, z);
	}

    public Vector3 GetGamePoint(){
        return HexCoordinates.HexToGameCoordinateV3(this.X, this.Y, this.Z);
    }

    public string toString(){
        return x + "," + y + "," + z;
    }

    public string toString2()
    {
        return x + "\n" + z + "\n" + GetIndex();
    }
}
