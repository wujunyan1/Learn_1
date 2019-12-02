using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour
{
    public float speed;

    private void Update()
    {
        this.transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("111111111111");
    }

    // 触发器的三个事件

    // 进入触发范围会调用一次
    void OnTriggerEnter(Collider other)
    {

        Debug.Log("onTriggerEnter");
        Rigidbody body = GetComponent<Rigidbody>();

    }

    // 当持续在触发范围内发生时调用

    void OnTriggerStay(Collider other)
    {

        Debug.Log("onTriggerStay");

    }

    // 离开触发范围会调用一次
    void OnTriggerExit(Collider other)
    {

        Debug.Log("onTriggerExit");

    }
}
