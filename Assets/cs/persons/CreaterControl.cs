using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreaterControl : PersonControl
{
    public CreaterControl()
    {
        
    }

    public override void ShowView()
    {
        base.ShowView();

        Debug.Log("ShowView");

        Creater creater = GetPerson<Creater>();


        controlView.control = this;
        controlView.Open();
    }
}
