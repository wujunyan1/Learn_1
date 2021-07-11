using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGameEntity : EventComponent
{
    private static int _id = 0;
    public int id;
    public float bradius;

    bool m_bTag;

    public BaseGameEntity()
    {
        id = _id++;
    }

    public bool IsTag()
    {
        return m_bTag;
    }

    public void SetTag(bool tag)
    {
        m_bTag = tag;
    }

    public Vector2 GetPos()
    {
        return Vector3Tool.ToVector2(this.transform.localPosition);
    }

    public float GetRadius()
    {
        return bradius;
    }

    public Transform FindChild(string str)
    {
        string[] strs = str.Split('/');

        Transform transform = this.transform;
        foreach(string s in strs)
        {
            transform = transform.Find(s);
            if(transform == null)
            {
                return null;
            }
        }

        return transform;
    }

    public virtual void MoveUpdate()
    {

    }

    public virtual void LogicUpdate()
    {

    }
}
