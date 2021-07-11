using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestControl : MonoBehaviour
{
    public SolderTest solderTest;

    protected int targetMask;

    // Start is called before the first frame update
    void Start()
    {
        targetMask = LayerMask.GetMask("BattleMap");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            Debug.Log("GetMouseButtonUp");
            // 显示属性的
            RaycastHit hit;
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(inputRay, out hit, 200f, targetMask))
            {
                Debug.Log("GetMouseButtonUp111");

                solderTest.MoveTo(hit.point);
            }
        }
    }
}
