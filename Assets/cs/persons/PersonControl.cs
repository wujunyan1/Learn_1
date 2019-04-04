using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonControl : MonoBehaviour
{
    public PersonControlView controlView;
    public Text name;

    Person person;

    /// <summary>
    /// 功能
    /// </summary>
    protected List<ObjFunction> functions;

    private void Awake()
    {
        functions = new List<ObjFunction>();
    }

    public T GetPerson<T>() where T : Person
    {
        return (T)person;
    }

    public void SetPerson<T>(T person) where T : Person
    {
        this.person = person;
        UpdateName();
    }

    public virtual void ShowView()
    {
        controlView.control = this;
        controlView.GetComponent<UIShow>().Obj = gameObject;
    }

    public virtual void CloseView()
    {
        controlView.Close();
    }

    public void UpdateName()
    {
        name.text = person.personName;
    }
    
    public void AddFunction(ObjFunction func)
    {
        functions.Add(func);
    }

    public List<ObjFunction> GetActiveFunctions()
    {
        List<ObjFunction> list = new List<ObjFunction>();

        foreach (ObjFunction func in functions)
        {
            if (func.IsActive())
            {
                list.Add(func);
            }
        }

        return list;
    }
}
