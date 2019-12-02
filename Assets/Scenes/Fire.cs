using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public move obj;

    public Transform panel;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            move i = Instantiate(obj);

            //i.transform.SetParent(panel);
            i.transform.localPosition = transform.localPosition;

            i.transform.rotation = transform.rotation;

        }
    }
}
