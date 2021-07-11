using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleTroopHead : MonoBehaviour
{
    public Image headImg;

    public void SetHeadPath(string headPath)
    {

    }

    public void SetHead(string head)
    {
        headImg.sprite = ResourseManager.GetHeadRes(head);
    }
}
