using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 阵营关系，/ 协议
public class CampRelation
{
    public enum RelationType
    {
        alliance,   //联盟
        neutrality, //中立
        rivalry,    //敌对
    }
    
    // 关系类型
    RelationType relationType;

    // 这种关系的列表
    Camp[] camps;
}
