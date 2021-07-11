using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockCameraView : View
{
    public override void Open(UObject o)
    {
        base.Open(o);

        HexMapCamera.Locked = true;
    }

    public override void Close()
    {
        HexMapCamera.Locked = false;

        base.Close();
    }
}
