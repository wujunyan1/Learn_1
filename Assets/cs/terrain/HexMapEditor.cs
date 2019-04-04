using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;



public class HexMapEditor : MonoBehaviour
{
    public HexGrid hexGrid;

    private Color activeColor;

    public NewMapMenu newMapMenu;

    HexCell selectCell;

    public GameObject createButton;
    UIShow createButtonShow;

    // 城市界面
    public CityView cityView;

    int targetMask;
    int noMask;

    bool isShow;
    bool closeFrame;

    void Awake()
    {
        isShow = false;
        closeFrame = false;
    }

    private void Start()
    {
        targetMask = LayerMask.GetMask("Map");
        noMask = LayerMask.GetMask("UI");

        createButtonShow = createButton.GetComponent<UIShow>();
        //Transform child = transform.Find("CreateBuild");
        //createButton = child.gameObject;
    }

    void Update()
    {
        if (closeFrame)
        {
            closeFrame = false;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.Log(string.Format("------------- {0} {1}", Physics.Raycast(inputRay, out hit, 200f, noMask), Physics.Raycast(inputRay, out hit, 200f, targetMask)));

        // 没有碰到不需要的layer 并且碰到地图
        if ( !Physics.Raycast(inputRay, out hit, 200f, noMask) 
            && Physics.Raycast(inputRay, out hit, 200f, targetMask))  // && !EventSystem.current.IsPointerOverGameObject()
        {
            Debug.Log(isShow);
            if (isShow)
            {
                CloseHexCellMessage();
            }
            else
            {
                ShowHexCellMessage(hexGrid.GetCell(hit.point));
            }
        }
    }

    // 展示 格子信息
    void ShowHexCellMessage(HexCell cell)
    {
        isShow = true;
        selectCell = cell;

        Debug.Log("1111111111111");

        PersonControl person = cell.person;
        if(person != null)
        {
            Debug.Log("22222222222222");
            person.ShowView();
            return;
        }

        // 存在建筑则显示建筑信息
        MapBuild build = cell.Build;
        if(build != null)
        {
            switch (build.BuildType)
            {
                case BuildType.City:
                    cityView.Open((City)build);
                    break;
            }
        }
        else
        {
            ShowCreateButton(cell);
        }
    }

    // 显示创建建筑面板
    void ShowCreateButton(HexCell cell)
    {
        if (cell.CanBuild(BuildType.City))
        {
            Debug.Log(string.Format("------------- {0}", cell.index));

            createButtonShow.Obj = selectCell.gameObject;
            createButton.SetActive(true);
            Vector3 v = cell.Position;
            v.y += 1;
        }
    }


    void CloseHexCellMessage()
    {
        CloseCreateButton();

        PersonControl person = selectCell.person;
        if (person != null)
        {
            Debug.Log("44444444");
            person.CloseView();
        }

        isShow = false;
        selectCell = null;
        closeFrame = true;
    }

    // 关闭创建建筑面板
    void CloseCreateButton()
    {
        createButton.SetActive(false);
    }
    
    public void OnBuildButton()
    {
        Debug.Log(string.Format("xxxxxxxxx"));
        selectCell.chunk.AddBuild(selectCell, BuildType.City);

        CloseCreateButton();
    }

}
