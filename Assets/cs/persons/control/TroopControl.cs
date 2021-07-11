using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TroopControl : ActionControl, RoundObject
{
    private GameObject modelObj;

    private Person person;
    private Troop troop;

    HexCell location;
    public HexCell Location
    {
        get
        {
            return location;
        }
        set
        {
            DecreaseVisibility();
            if (location)
            {
                location.Person = null;
                location.Troop = null;
            }
            location = value;

            if(troop != null)
            {
                troop.CellIndex = location.index;
            }

            //string s = string.Format("{0} {1} {2}", location.index, location.coordinates.X, location.coordinates.Z);
            //transform.localPosition = value.Position;

            HexGrid.instance.TroopUpdatePostion(transform, value);
            transform.localPosition = value.Position;

            IncreaseVisibility();
        }
    }

    HexDirection direction;
    public HexDirection Direction
    {
        get
        {
            return direction;
        }
        set
        {
            direction = value;
            transform.localRotation = Quaternion.Euler(0f, (int)value * 60 + 30, 0f);
        }
    }

    // 是否被发现
    bool isFind = false;

    public GameObject propGameObj = null;


    public Transform bloodImg;
    public Transform moraleImg;
    public Text nameText;
    public Text funcStatusRoundText;



    private void Start()
    {
        // CreatePropGameObj();
    }

    void CreatePropGameObj()
    {
        if(propGameObj != null)
        {
            GameObject.Destroy(propGameObj);
        }
        propGameObj = new GameObject();
        propGameObj.name = "prop";
        propGameObj.transform.SetParent(this.transform);
        propGameObj.transform.localPosition = Vector3.zero;
    }

    public void SetModel(GameObject model)
    {
        model.transform.parent = this.transform;
        model.transform.localPosition = Vector3.zero;
        model.name = "model";
        modelObj = model;
    }

    public void SetPerson(Person _person)
    {
        person = _person;

        Debug.Log(person);
    }

    public void SetTroop(Troop troop)
    {
        this.troop = troop;
    }

    // 更新坐标
    public void ValidateLocation()
    {
        //transform.localPosition = location.Position;

        HexGrid.instance.TroopUpdatePostion(transform, location);
    }

    List<HexCell> currPath;
    int moveActionIndex;
    FuncAction.Func moveEndFunc;
    public void MovePath(List<HexCell> path, FuncAction.Func func = null)
    {
        currPath = path;
        this.moveEndFunc = func;
        MoveNext();
    }

    void MoveNext()
    {
        if(currPath.Count == 0)
        {
            MoveEnd();
            return;
        }

        HexCell nextCell = currPath[currPath.Count - 1];
        int speed = Location.GetDistanceCost(nextCell);
        if(troop.data.speed >= speed)
        {
            HexDirection dir = location.Direction(nextCell);
            int n = dir - direction;

            RotateByAction r1 = new RotateByAction(new Vector3(0, n * 60, 0), 0.2f * Mathf.Abs(n));
            FuncAction f1 = new FuncAction(() => {
                Direction = dir;
            });

            Vector3 postion = nextCell.Position;

            //循环的情况可能会从尾闪到头的现象
            int currColumnIndex = location.ColumnIndex;
            int nextColumnIndex = nextCell.ColumnIndex;
            
            // 跨了超过一块（5格），这只能是在循环边缘才能发生
            if(nextColumnIndex <= currColumnIndex - 2)
            {
                // 向后移动
                postion.x += HexGrid.instance.GridWidth;
            }
            else if (nextColumnIndex >= currColumnIndex + 2)
            {
                // 向前移动
                postion.x -= HexGrid.instance.GridWidth;
            }

            MoveToAction a = new MoveToAction(nextCell.Position, 0.5f);

            FuncAction f = new FuncAction(() => {
                HexGrid.instance.ClearShowPath(Location);
                troop.data.speed -= speed;
                Location = nextCell;
                nextCell.Troop = troop;
                currPath.RemoveAt(currPath.Count - 1);

                MoveNext();
            });

            QueueAction action = new QueueAction(r1, f1, a, f);
            moveActionIndex = RunAction(action);
        }
        else
        {
            MoveEnd();
        }
    }

    void MoveEnd()
    {
        HexGrid.instance.ClearShowPath(currPath);
        if (moveEndFunc != null)
        {
            moveEndFunc.Invoke();
            moveEndFunc = null;
        }
    }

    public void NextRound()
    {
        
    }

    public void LaterNextRound()
    {

    }


    /// <summary>
    /// 清除视野
    /// </summary>
    /// <returns></returns>
    public bool DecreaseVisibility()
    {
        if (location && troop.camp != null && troop.camp.HasPlayerVisibility())
        {
            HexGrid.instance.DecreaseVisibility(location, troop.data.visionRange);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 获得视野
    /// </summary>
    /// <returns></returns>
    public bool IncreaseVisibility()
    {
        if (location && troop.camp != null && troop.camp.HasPlayerVisibility())
        {
            HexGrid.instance.IncreaseVisibility(location, troop.data.visionRange);
            return true;
        }
        return false;
    }


    public void UpdateCurrFuncStatus()
    {
        CreatePropGameObj();

        ObjFunction objFunction = troop.GetCurrObjFunctions();
        objFunction.UpdateStatusView(troop, this);
    }
}
