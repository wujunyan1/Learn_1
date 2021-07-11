using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorPerson : MonoBehaviour
{
    public Toggle toggle;

    public void Change()
    {
        HexMapEditorData.GetInstance().SetAddPerson(toggle.isOn);
    }
}
