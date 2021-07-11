using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 在3d物品上 显示按钮
/// </summary>
public class UIShow : MonoBehaviour
{
    // 偏移坐标
    public Vector3 offset;

    // 偏移坐标
    public Vector3 UIOffset;

    /// <summary>
    /// 3d物品
    /// </summary>
    public GameObject obj = null;

    public GameObject Obj
    {
        get
        {
            return obj;
        }
        set
        {
            obj = value;
            Update();
        }
    }

    public Vector3 potion = Vector3.zero;

    /// <summary>
    /// 显示的按钮
    /// </summary>
    public GameObject showUI;

    // 按钮默认大小
    private Vector3 baseScale;
    private float baseFomat;     //默认字与摄像机的距离
    private float currentFomat;  //当前相机的距离

    private bool isSet = false;

    private void Start()
    {
        //计算以下默认的距离

        baseFomat = 20f; // Vector3.Distance(Vector3.zero, Camera.main.transform.position);
        if (baseFomat == 0)
        {
            baseFomat = 0.01f;
        }

        

        if (!showUI)
        {
            showUI = gameObject;
        }

        baseScale = showUI.transform.localScale;
        currentFomat = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (baseFomat != currentFomat || !isSet)
        {
            Vector3 newPos;
            if (obj == null)
            {
                newPos = potion + offset;
            }
            else
            {
                newPos = obj.transform.position + offset;
            }
            isSet = true;
            //保存当前相机到文字UI的距离
            currentFomat = Vector3.Distance(newPos, Camera.main.transform.position);

            float myscale = baseFomat / currentFomat;  //计算出缩放比例 
            showUI.transform.position = WorldToUI(newPos) + UIOffset; //计算UI显示的位置
            showUI.transform.localScale = baseScale * myscale;           //缩放UI 
        }
    }

    /// <summary>
    /// 把3D点换算成NGUI屏幕上的2D点。
    /// </summary>
    public static Vector3 WorldToUI(Vector3 point)
    {
        Vector3 pt = Camera.main.WorldToScreenPoint(point);  //将世界坐标转换为视口坐标
        pt.z = 0;
        return pt;
    }
}
