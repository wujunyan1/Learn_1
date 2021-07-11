using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleUI : EventComponent
{
    public RectTransform frameImage;

    // 操作
    protected int targetMask;
    protected bool mouseDown = false;
    protected Vector3 downPosition;
    protected Vector3 upPosition;

    // UI
    public RectTransform baseUI;
    protected SoldierMessageView openSoldierMessageView;

    public RectTransform soldierStatePanel;
    public RectTransform soldierStatePrefab;

    // Start is called before the first frame update
    void Start()
    {
        targetMask = LayerMask.GetMask("BattleMap");

        this.RegisterEvent("SHOW_SOLDIER_MESSAGE", ShowSoldierMessage);
    }

    public void Update()
    {
        // 左键点下 开始选择框
        if (Input.GetMouseButtonDown(0) && !mouseDown && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(inputRay, out hit, 200f, targetMask))
            {
                mouseDown = true;
                Vector3 position = hit.point;
                downPosition = Input.mousePosition;

                frameImage.gameObject.SetActive(true);
            }
        }

        // 选择框结束 显示抬起时的的对象信息
        if (Input.GetMouseButtonUp(0))
        {
            mouseDown = false;

            frameImage.gameObject.SetActive(false);

            // 显示属性的
            RaycastHit hit;
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(inputRay, out hit, 200f, targetMask))
            {
                Vector2 upPostions = new Vector2(hit.point.x, hit.point.z);
                BattleWorld.battleCenter.gameControl.SeletShowSoldier(upPostions);
            }
        }

        // 随着选择框拖动，框选对象
        if (mouseDown)
        {
            Vector3 currPos = Input.mousePosition;
            UpdateSelectFrame(currPos, downPosition);
        }

        // 右键 选择的对象进行移动/攻击
        if (Input.GetMouseButtonUp(1))
        {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(inputRay, out hit, 200f, targetMask))
            {
                Vector3 position = hit.point;

                GameControl gameControl = BattleWorld.battleCenter.gameControl;
                SoldierControl enemy = gameControl.GetSelectEnemy(Vector3Tool.ToVector2(position));

                if(enemy != null)
                {
                    Debug.Log("---------------");
                    gameControl.playerControl.Attack(enemy);
                }
                else
                {
                    gameControl.playerControl.MoveTo(position);
                }
            }
        }

        // 按了左alt键， 显示所有对象的 血条，士气等简略信息
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            ShowSoldierStatesView();
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            HideSoldierStatesView();
        }
    }

    // 更新选择框和选择的部队
    private void UpdateSelectFrame(Vector2 currPos, Vector2 downPosition)
    {
        float minX = currPos.x;
        float maxX = downPosition.x;
        if (currPos.x > downPosition.x)
        {
            minX = downPosition.x;
            maxX = currPos.x;
        }

        float minY = currPos.y;
        float maxY = downPosition.y;
        if (currPos.y > downPosition.y)
        {
            minY = downPosition.y;
            maxY = currPos.y;
        }

        Vector2 leftUp = Vector2.zero, leftDown = Vector2.zero,
            rightUp = Vector2.zero, rightDown = Vector2.zero;

        Ray inputRay = Camera.main.ScreenPointToRay(new Vector3(minX, minY));
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit, 200f, targetMask))
        {
            leftDown = new Vector2(hit.point.x, hit.point.z);
        }

        inputRay = Camera.main.ScreenPointToRay(new Vector3(minX, maxY));
        if (Physics.Raycast(inputRay, out hit, 200f, targetMask))
        {
            leftUp = new Vector2(hit.point.x, hit.point.z);
        }

        inputRay = Camera.main.ScreenPointToRay(new Vector3(maxX, minY));
        if (Physics.Raycast(inputRay, out hit, 200f, targetMask))
        {
            rightDown = new Vector2(hit.point.x, hit.point.z);
        }

        inputRay = Camera.main.ScreenPointToRay(new Vector3(maxX, maxY));
        if (Physics.Raycast(inputRay, out hit, 200f, targetMask))
        {
            rightUp = new Vector2(hit.point.x, hit.point.z);
        }

        IsoscelesTrapezoid istr = new IsoscelesTrapezoid(leftUp, leftDown, rightUp, rightDown);

        BattleWorld.battleCenter.gameControl.playerControl.SelectAreaSoldiers(istr);
        

        DrawFrame(minX, maxX, minY, maxY);
    }

    // 画选择框
    private void DrawFrame(float minX, float maxX, float minY, float maxY)
    {
        frameImage.sizeDelta = new Vector2(maxX - minX, maxY - minY);
        frameImage.position = new Vector3((maxX + minX) / 2, (maxY + minY) / 2, 0);
    }

    // 显示所有对象的 血条，士气等简略信息
    private void ShowSoldierStatesView()
    {
        soldierStatePanel.gameObject.SetActive(true);
    }

    // 隐藏所有对象的 血条，士气等简略信息
    private void HideSoldierStatesView()
    {
        soldierStatePanel.gameObject.SetActive(false);
    }

    public void AddSoldierStateView()
    { 
        GameControl control = BattleWorld.battleCenter.gameControl;
        List<SoldierControl> soldiers = control.GetAllLifeSoldiers();

        foreach (SoldierControl soldier in soldiers)
        {
            RectTransform obj = Instantiate(soldierStatePrefab, soldierStatePanel);

            UIShow uiShow = obj.GetComponent<UIShow>();
            uiShow.obj = soldier.gameObject;
            uiShow.offset = new Vector3(0, soldier.data.GetConfig().ph_y * 1.3f, 0.7f);

            obj.GetComponent<SoldierStateView>().data = soldier.data;
        }

    }

    public void StopBtn()
    {
        if(BattleWorld.battleCenter.GetCurrBattleSpeed() != BattleCenter.BattleSpeed.STOP)
            BattleWorld.battleCenter.UpdateSpeed(BattleCenter.BattleSpeed.STOP);
    }

    public void NormalBtn()
    {
        if (BattleWorld.battleCenter.GetCurrBattleSpeed() != BattleCenter.BattleSpeed.NORMAL)
            BattleWorld.battleCenter.UpdateSpeed(BattleCenter.BattleSpeed.NORMAL);
    }

    public void FastBtn()
    {
        if (BattleWorld.battleCenter.GetCurrBattleSpeed() != BattleCenter.BattleSpeed.FAST)
            BattleWorld.battleCenter.UpdateSpeed(BattleCenter.BattleSpeed.FAST);
    }

    public void FastestBtn()
    {
        if (BattleWorld.battleCenter.GetCurrBattleSpeed() != BattleCenter.BattleSpeed.FASTEST)
            BattleWorld.battleCenter.UpdateSpeed(BattleCenter.BattleSpeed.FASTEST);
    }

    // 显示人物信息
    public void ShowSoldierMessage(UEvent e)
    {
        SoldierControl control = (SoldierControl)e.eventParams;
        BattleSoldierData data = control.data;

        OpenSoldierMessageView(data);
    }

    // 显示队伍信息
    public void ShowTeamMessage()
    {

    }

    // 显示人物头像和技能
    public void ShowSoldierHeaderAndSkill()
    {

    }


    public void OpenSoldierMessageView(BattleSoldierData data)
    {
        SoldierMessageView openUI;
        if (openSoldierMessageView != null)
        {
            openUI = openSoldierMessageView;
        }
        else
        {
            SoldierMessageView prefab = Resources.Load<SoldierMessageView>("prefabs/Base/SoldierMessagePanel");
            openUI = GameObject.Instantiate<SoldierMessageView>(prefab, this.transform);
        }
        openUI.SetBattleSoldierData(data);
        openSoldierMessageView = openUI;

        // 设置位置
        RectTransform transform = openUI.GetComponent<RectTransform>();

        // 锚点 对父节点
        transform.anchorMin = new Vector2(0, 0.5f);
        transform.anchorMax = new Vector2(0, 0.5f);
        Rect rect = transform.rect;

        // 内锚点
        transform.pivot = new Vector2(0, 0.5f);

        // 偏移 距离左边0
        transform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, transform.rect.size.x);
    }

    public void CloseSoldierMessageView(UEvent org)
    {
        if (openSoldierMessageView)
        {
            GameObject.Destroy(openSoldierMessageView.gameObject);
            openSoldierMessageView = null;
        }
    }
}
