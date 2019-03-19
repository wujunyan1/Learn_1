using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateTools : MonoBehaviour
{
    public static long getCurrDate()
    {
        System.TimeSpan ts = System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 0, 0, 0);
        return System.Convert.ToInt64(ts.TotalSeconds);
    }
}
