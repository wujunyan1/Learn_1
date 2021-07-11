using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 人物控制面板
/// </summary>
public class PersonControlView : View
{
    public PersonControl control;
    public Button buttonPrefab;

    public override void Open(UObject o)
    {
        UpdateView();
        this.gameObject.SetActive(true);
        this.transform.parent.gameObject.SetActive(true);
    }

    public override void Close()
    {
        this.gameObject.SetActive(false);
        this.transform.parent.gameObject.SetActive(false);
    }

    public override void UpdateView()
    {
        // 移除原来的数据
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        List<ObjFunction> funs = control.GetActiveFunctions();

        Debug.Log(funs.Count);
        foreach (ObjFunction func in funs)
        {
            Button btn = Instantiate(buttonPrefab);
            Text text = btn.GetComponentInChildren<Text>();
            text.text = func.Name;
            btn.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);

            btn.onClick.AddListener(
                delegate
                {
                    TouchFunc(func);
                }
                );

            btn.transform.SetParent(transform, false);
        }

    }

    public void TouchFunc(ObjFunction func)
    {
        
    }
}
