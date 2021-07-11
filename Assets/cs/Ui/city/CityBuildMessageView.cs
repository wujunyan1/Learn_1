using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuildMessageView : View
{
    CityBuild cityBuild;


    public override void Open(UObject o)
    {
        base.Open(o);
        cityBuild = o.GetT<CityBuild>("CityBuild", null);

        UpdateView();
    }

    public override void Show()
    {
        base.Show();
    }

    public override void Close()
    {
        base.Close();
    }

    public override void UpdateView()
    {
        base.UpdateView();
        
    }
}
