using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UPhysics : MonoBehaviour
{
    public float timerScale;

    System.Diagnostics.Stopwatch sw_update;
    System.Diagnostics.Stopwatch sw_late_update;
    System.Diagnostics.Stopwatch sw_fixed_update;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = timerScale;
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
    }

    private void FixedUpdate()
    {
        
    }

    private void OnDestroy()
    {
    }
}
