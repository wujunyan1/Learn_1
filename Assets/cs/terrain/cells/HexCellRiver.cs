using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RiverDirection
{
    Null,
    Incoming,
    Outgoing
}

public enum RiverType
{
    ONE_1,  // 1,0,0,0,0,0   只有一个流入
    ONE_2, // 2,0,0,0,0,0   只有一个流出
    TWO_1,  // 1,2,0,0,0,0   一个流入一个流出，相邻
    TWO_2,  // 1,0,2,0,0,0      一个流入一个流出 间隔一
    TWO_3,  // 1,0,0,2,0,0      一个流入一个流出 间隔二
    TWO_4,  // 1,1,0,0,0,0      2个流入，相邻
    TWO_5,  // 1,0,1,0,0,0      2个流入 间隔一
    TWO_6,  // 1,0,0,1,0,0      2个流入 间隔二
    TWO_7,  // 2,2,0,0,0,0      2个流出，相邻
    TWO_8,  // 2,0,2,0,0,0      2个流出 间隔一
    TWO_9,  // 2,0,0,2,0,0      2个流出 间隔二
    THREE_1,// 1,1,1,0,0,0      3个流入
    THREE_2,// 1,1,0,1,0,0      3个流入，有一个不相邻
    THREE_3,// 1,0,1,0,1,0      3个流入，相互不相邻
    THREE_4,// 2,2,2,0,0,0      3个流出
    THREE_5,// 2,2,0,2,0,0      3个流出，有一个不相邻
    THREE_6,// 2,0,2,0,2,0      3个流出，相互不相邻

    THREE_7,// 1,2,2,0,0,0      2个流出，流入在旁边
    THREE_8,// 2,1,2,0,0,0      2个流出，流入在中间
    THREE_9,// 1,2,0,2,0,0      2个流出，有一个不相邻的流出
    THREE_10,// 2,1,0,2,0,0      2个流出，有一个不相邻的流出, 流入在中间
    THREE_11,// 1,0,2,0,2,0      2个流出，相互不相邻
}

public class HexCellRiver : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
