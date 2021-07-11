using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class BtnListMenuView : MonoBehaviour
{
    public void AddButton(string name, UnityAction action , int index = -1)
    {
        GameObject obj;
        //PrefabsManager.GetInstance().GetGameObj(out obj, "Button");
        PrefabsManager.GetInstance().GetPrefab(out obj, "Button");
        obj = GameObject.Instantiate(obj);


        Button btn = obj.GetComponent<Button>();
        btn.onClick.AddListener(action);

        //obj.transform.sizeDelta = new Vector2(sizeDelta.x, Mathf.Max(num * 50, 300));

        GameObject child = obj.transform.GetChild(0).gameObject;
        Text text = child.GetComponent<Text>();
        text.text = name;

        obj.transform.SetParent(transform);
    }

}
