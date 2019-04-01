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
    ButtonShow createButtonShow;

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

        createButtonShow = createButton.GetComponent<ButtonShow>();
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

        Debug.Log(string.Format("-------------"));

        // 没有碰到不需要的layer 并且碰到地图
        if ( !Physics.Raycast(inputRay, out hit, 200f, noMask) 
            && Physics.Raycast(inputRay, out hit, 200f, targetMask))  // && !EventSystem.current.IsPointerOverGameObject()
        {
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
        Debug.Log(string.Format("------------- {0}", cell.index));

        createButtonShow.Obj = selectCell.gameObject;
        createButton.SetActive(true);
        Vector3 v = cell.Position;
        v.y += 1;
    }


    void CloseHexCellMessage()
    {
        CloseCreateButton();

        isShow = false;
        selectCell = null;
        closeFrame = true;
    }

    // 关闭创建建筑面板
    void CloseCreateButton()
    {
        isShow = false;
        createButton.SetActive(false);
        selectCell = null;

        closeFrame = true;
    }
    
    public void OnBuildButton()
    {
        Debug.Log(string.Format("xxxxxxxxx"));
        selectCell.chunk.AddBuild(selectCell, BuildType.City);

        CloseCreateButton();
    }

}
