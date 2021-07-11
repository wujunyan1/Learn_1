using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MsgUI : MonoBehaviour
{
    Text text;

    public class MsgData
    {
        public string msg;
        public int sec;
        
        public MsgData(string _msg = "", int _sec = 2)
        {
            msg = _msg;
            sec = _sec;
        }

        public MsgData Copy()
        {
            return new MsgData(msg, sec);
        }
    }

    MsgData _data;

    private void Awake()
    {
        _data = new MsgData();
    }

    private void Start()
    {
        text = GetComponent<Text>();
        text.text = _data.msg;

        GameObject.Destroy(this.gameObject, _data.sec);
    }

    public void SetMsg(MsgData data)
    {
        _data = data.Copy();
        if (text)
        {
            text.text = _data.msg;
        }
    }
    
}
