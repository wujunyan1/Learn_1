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

        Creater creater = GetPerson<Creater>();

        controlView.control = this;
        //controlView.Open();
    }
}
