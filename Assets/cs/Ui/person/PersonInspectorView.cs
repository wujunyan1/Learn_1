using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonInspectorView : View
{
    // 每行的按钮数量
    int actionRowNum = 3;

    public PersonControl control;
    public Button buttonPrefab;

    public Transform actionView;
    public Transform inspector;

    public Text nameLabel;
    public Text camp;
    public Text age;

    public Image header;

    ObjFunction Choosefunc;

    private void Awake()
    {
        actionView.gameObject.SetActive(true);
        inspector.gameObject.SetActive(true);

        nameLabel.gameObject.SetActive(true);
        camp.gameObject.SetActive(true);
        age.gameObject.SetActive(true);

        header.gameObject.SetActive(true);
    }

    public override void Open()
    {
        this.gameObject.SetActive(true);
        this.transform.parent.gameObject.SetActive(true);

        HexMapEditor.instance.SetDefaultTouchAction(CloseAction);

        UpdateView();
    }

    public override void Close()
    {
        if (Choosefunc)
        {
            Choosefunc.CloseFuncView();
            Choosefunc = null;
        }

        HidePersonPath();

        HexMapEditor.instance.SetDefaultTouchAction(null);

        this.gameObject.SetActive(false);
        this.transform.parent.gameObject.SetActive(false);
    }

    public override void UpdateView()
    {
        Person person = control.GetPerson();
        nameLabel.text = person.personName;
        camp.text = person.camp.CampName;

        header.sprite = person.res.header;


        UpdateActionBtns();
        ShowPersonPath();
    }



    // 更新功能区
    void UpdateActionBtns()
    {
        // 移除原来的数据
        for (int i = 0; i < actionView.childCount; i++)
        {
            actionView.GetChild(i).gameObject.SetActive(false);
        }

        List<ObjFunction> funs = control.GetActiveFunctions();

        Debug.Log(funs.Count);
        for (int i = 0; i < funs.Count; i++)
        {
            ObjFunction func = funs[i];

            Button btn;
            if (i >= actionView.childCount)
            {
                btn = CreateButton(func);
            }
            else
            {
                Transform child = actionView.GetChild(i);
                btn = child.GetComponent<Button>();
            }

            UpdateButton(btn, func);
            btn.gameObject.SetActive(true);
        }
    }

    // 创建按钮
    Button CreateButton(ObjFunction func)
    {
        Button btn = Instantiate(buttonPrefab);
        btn.transform.SetParent(actionView, false);

        return btn;
    }

    void UpdateButton(Button btn, ObjFunction func)
    {
        Text text = btn.GetComponentInChildren<Text>();
        text.text = func.Name;

        btn.onClick.RemoveAllListeners();

        btn.onClick.AddListener(
            delegate
            {
                TouchFunc(func);
            }
            );
    }

    public void TouchFunc(ObjFunction func)
    {
        Choosefunc = func;
        func.OnStartBtn();
    }


    // 显示路径
    void ShowPersonPath()
    {
        Person person = control.GetPerson();
        if (person.Path != null && person.Path.Count > 0)
        {
            HexGrid.instance.ShowPath(person.CurrCell, person.Path, person.speed);
        }
    }

    // 清除路径
    void HidePersonPath()
    {
        Person person = control.GetPerson();
        if (person.Path != null && person.Path.Count > 0)
        {
            HexGrid.instance.ClearShowPath(person.Path);
        }
    }

    public void CloseAction(HexCell cell)
    {
        Close();
    }
}
