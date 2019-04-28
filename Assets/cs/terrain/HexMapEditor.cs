using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;



public class HexMapEditor : MonoBehaviour
{
    public static HexMapEditor instance;

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

    bool isTouchDown;

    //public delegate void Action<HexCell>(HexCell arg);
    //Action<HexCell> action;

    HexCell ChooesCell { get; set; }

    // 点击事件
    public delegate void ClickAction(HexCell arg);
    ClickAction clickAction;

    // 移动事件
    public delegate void MoveAction(HexCell old, HexCell curr);
    MoveAction moveAction;

    MoveAction touchMoveAction;

    // 点下事件
    public delegate void TouchAction(HexCell curr);

    TouchAction defaultTouchAction;

    HexCell currCell;
    HexCell downCell;

    void Awake()
    {
        isShow = false;
        closeFrame = false;

        instance = this;
    }

    private void Start()
    {
        targetMask = LayerMask.GetMask("Map");
        noMask = LayerMask.GetMask("UI");

        createButtonShow = createButton.GetComponent<UIShow>();
        //Transform child = transform.Find("CreateBuild");
        //createButton = child.gameObject;
    }

    //public void SetTouchAction(Action<HexCell> action)
    //{
    //    this.action = action;
    //}

    public void SetClickAction(ClickAction action)
    {
        this.clickAction = action;
    }

    public void SetMoveAction(MoveAction action)
    {
        this.moveAction = action;
    }

    public void SetTouchMoveAction(MoveAction action)
    {
        this.touchMoveAction = action;
    }

    public void SetDefaultTouchAction(TouchAction action)
    {
        this.defaultTouchAction = action;
    }

    bool isDown;
    bool isEnter = false;

    void Update()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        //if (!Physics.Raycast(inputRay, out hit, 200f, noMask))
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            HexCell cell = GetCellUnderCursor();

            if(cell == null)
            {
                return;
            }

            if (currCell == null || currCell.index != cell.index)
            {
                MouseMove(currCell, cell);
            }

            currCell = cell;

            if (!isEnter)
            {
                isEnter = true;
                MouseEnter();
            }

            //if(Input.touchCount > 0)
            if (Input.GetMouseButtonDown(0))
            {
                if (!isDown)
                {
                    isDown = true;
                    MouseDown();
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDown = false;
                MouseUp();
            }
        }

    }

    HexCell GetCellUnderCursor()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(inputRay, out hit, 200f, targetMask))
        {
            return hexGrid.GetCell(hit.point);
        }
        return null;
    }

    void HandleInput()
    {

        HexCell cell = GetCellUnderCursor();
        // 没有碰到不需要的layer 并且碰到地图
        if (cell)  // && !EventSystem.current.IsPointerOverGameObject()
        {
            
        }
    }
    

    //当鼠标进入在网格上时，
    void MouseEnter()
    {
        Debug.Log("OnMouseEnter");
    }

    // ...当鼠标悬浮在物体上
    void MouseMove(HexCell oldCell, HexCell currCell)
    {
        if (moveAction != null)
        {
            moveAction.Invoke(oldCell, currCell);
        }

        if (isTouchDown && touchMoveAction != null)
        {
            touchMoveAction.Invoke(oldCell, currCell);
        }
    }

    void MouseDrag()
    {

    }

    // ...当鼠标移开时
    void MouseExit()
    {
        isTouchDown = false;
    }
    // ...当鼠标点击
    void MouseDown()
    {
        isTouchDown = true;
        downCell = currCell;

        // 没有其他点击事件，则调用默认的
        if(touchMoveAction == null && clickAction == null && defaultTouchAction != null)
        {
            defaultTouchAction.Invoke(currCell);
        }
    }
    // ...当鼠标抬起
    void MouseUp()
    {
        if(downCell == null || currCell == null)
        {
            return;
        }
        Debug.Log(string.Format(" MouseUp {0} {1} ", downCell.index, currCell.index));

        // 点击了物品
        if (downCell.index == currCell.index)
        {
            Click();
        }

        downCell = null;
        isTouchDown = false;
    }

    void Click()
    {
        if (clickAction != null)
        {
            clickAction.Invoke(GetCellUnderCursor());
        }
    }











    // 展示 格子信息
    void ShowHexCellMessage(HexCell cell)
    {
        isShow = true;
        selectCell = cell;

        Debug.Log("1111111111111");

        PersonControl person = cell.Person;
        //if(person != null)
        //{
        //    Debug.Log("22222222222222");
        //    person.ShowView();
        //    return;
        //}

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

        PersonControl person = selectCell.Person;
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
