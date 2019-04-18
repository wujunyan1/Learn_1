using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 人物的模型处理
/// </summary>
public class PersonControl : MonoBehaviour
{
    public PersonInspectorView controlView;
    public Text name;
    
    /// <summary>
    /// 模型
    /// </summary>
    public GameObject model;


    Person person;
    CollionClick collionClick;

    HexCell location;
    public HexCell Location
    {
        get
        {
            return location;
        }
        set
        {
            location = value;

            string s = string.Format("{0} {1} {2}", location.index, location.coordinates.X, location.coordinates.Z);
            curr = transform.localPosition = value.Position;
            //person.Point = value.coordinates;
            value.Person = this;
            start = value;
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

    HexCell end;

    Vector3 curr;
    public HexCell End
    {
        get
        {
            return end;
        }
        set
        {
            end = value;
            path = null;
            pathIndex = 0;

            if (end)
            {
                path = new Vector3[4];
                HexEdgeType edgeType = start.GetEdgeType(end);

                Vector3 startVector3 = start.transform.localPosition;
                Vector3 endVector3 = end.transform.localPosition;

                path[0] = startVector3;

                HexDirection direction = start.Direction(end);
                Vector3 s1 = startVector3 + HexMetrics.GetSolidCorner(direction);
                Vector3 s2 = endVector3 + HexMetrics.GetSolidCorner(direction.Opposite());

                path[1] = s1;
                path[2] = s2;
                path[3] = endVector3;
            }
        }
    }
    HexCell start;

    Vector3[] path;
    int pathIndex = 0;

    private void Awake()
    {
        path = null;
        pathIndex = 0;
    }

    private void Start()
    {
        collionClick = model.GetComponent<CollionClick>();
        collionClick.SetClickAction(TouchModel);
    }

    public T GetPerson<T>() where T : Person
    {
        return (T)person;
    }

    public Person GetPerson()
    {
        return person;
    }

    public void SetPerson<T>(T person) where T : Person
    {
        this.person = person;
        this.person.Control = this;
        Location = HexGrid.instance.GetCell(this.person.Point);
        UpdateName();
    }

    public virtual void ShowView()
    {
        controlView.control = this;
        controlView.Open();
    }

    public virtual void CloseView()
    {
        controlView.Close();
    }

    public void UpdateName()
    {
        name.text = person.personName;
    }

    public List<ObjFunction> GetActiveFunctions()
    {
        return person.GetActiveFunctions();
    }

    float MoveTime = -1f;


    float speed = 10f;

    private void Update()
    {
        if (start != null && path != null)
        {
            Vector3 targetVector = path[pathIndex];

            Debug.Log(targetVector);
            if(MoveTime == -1f)
            {
                MoveTime = Time.time;
            }

            float distance = Vector3.Distance(curr, targetVector);

            transform.position = Vector3.Lerp(curr, targetVector, (Time.time - MoveTime) * speed / distance);
            
            if(transform.position == targetVector)
            {
                MoveNextPath();
            }
        }
    }

    void MoveNextPath()
    {
        pathIndex++;

        curr = transform.position;
        MoveTime = -1f;
        if (pathIndex > 3)
        {
            pathIndex = 0;
            start.DisableHighlight();
            HexCoordinates coordinates = End.coordinates;
            Location = End;
            End = null;
            //person.Point = coordinates;
            person.MoveNextCell();

            start.EnableHighlight(Color.blue);
        }
    }
    

    public void TouchModel()
    {
        EventSystem.current.SetSelectedGameObject(this.gameObject);
        ShowView();
    }
    
    public void showDebug()
    {
        Debug.Log(person.personName);
    }

    /// <summary>
    ///  更新当前位置
    /// </summary>
    public void ValidateLocation()
    {
        transform.localPosition = location.Position;
    }

    /// <summary>
    /// 死亡动画
    /// </summary>
    public void Die()
    {
    }

    public void Clear()
    {
        location.Person = null;
        Destroy(gameObject);
    }
}
