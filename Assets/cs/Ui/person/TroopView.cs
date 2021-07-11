using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TroopView : View
{
    Troop troop;

    public TroopMessageView troopMessageView;

    public CircleTroopHead circleTroopHead;

    public GameObject operationPanel;
    public Button currOperationBtn;

    Button[] btns;

    BuildCityFunction buildCityFunction;
    BuildbarrackFunction buildbarrackFunction;
    PlunderFunction plunderFunction;
    NormalFunction normalFunction;

    private void Start()
    {
        currOperationBtn.onClick.AddListener(ChooseOperation);

        btns = operationPanel.GetComponentsInChildren<Button>();
        btns[0].onClick.AddListener(Normal);
        btns[1].onClick.AddListener(BuildBarracks);
        btns[2].onClick.AddListener(BuildCity);
        btns[3].onClick.AddListener(Plunder);

        Debug.Log("start");

        UpdateView();
    }

    public override void Close()
    {
        base.Close();
    }

    public override void Open(UObject o)
    {
        base.Open(o);

        Troop troop = o.GetT<Troop>("Troop", null);

        this.troop = troop;
        troopMessageView.SetTroop(troop);

        string head = troop.data.Head;
        circleTroopHead.SetHead(head);

        buildCityFunction = troop.GetObjFunction<BuildCityFunction>();
        buildbarrackFunction = troop.GetObjFunction<BuildbarrackFunction>();
        plunderFunction = troop.GetObjFunction<PlunderFunction>();
        normalFunction = troop.GetObjFunction<NormalFunction>();
    }

    public override void UpdateView()
    {
        base.UpdateView();

        btns[0].gameObject.SetActive(normalFunction != null);
        btns[1].gameObject.SetActive(buildbarrackFunction != null);
        btns[2].gameObject.SetActive(buildCityFunction != null);
        btns[3].gameObject.SetActive(plunderFunction != null);


        ObjFunction currFunc = troop.GetCurrObjFunctions();


        Debug.Log(currFunc.Name);
        Debug.Log(currFunc.Type);

        currOperationBtn.GetComponent<Image>().sprite =
            btns[(int)currFunc.Type].GetComponent<Image>().sprite;
    }

    void ChooseOperation()
    {
        if (operationPanel.activeSelf)
        {
            operationPanel.SetActive(false);
        }
        else
        {
            operationPanel.SetActive(true);
        }
    }

    void CloseChooseOperation()
    {
        operationPanel.SetActive(false);
    }

    /// <summary>
    /// 建城
    /// </summary>
    void BuildCity()
    {
        if (buildCityFunction.IsActive(troop))
        {
            troop.SetCurrObjFunction<BuildCityFunction>();
        }

        CloseChooseOperation();
        UpdateView();
    }

    /// <summary>
    /// 正常
    /// </summary>
    void Normal()
    {
        if (normalFunction.IsActive(troop))
        {
            troop.SetCurrObjFunction<NormalFunction>();
        }

        CloseChooseOperation();
        UpdateView();
    }

    /// <summary>
    /// 建设营寨
    /// </summary>
    void BuildBarracks()
    {
        if (buildbarrackFunction.IsActive(troop))
        {
            troop.SetCurrObjFunction<BuildbarrackFunction>();
        }

        CloseChooseOperation();
        UpdateView();
    }

    /// <summary>
    /// 劫掠
    /// </summary>
    void Plunder()
    {
        if (plunderFunction.IsActive(troop))
        {
            troop.SetCurrObjFunction<PlunderFunction>();
        }

        CloseChooseOperation();
        UpdateView();
    }
}
