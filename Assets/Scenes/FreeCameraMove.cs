using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCameraMove : MonoBehaviour
{
    public float moveSpeed = 50;
    public float scaleSpeed = 120f;

    float rotate = 1.41421f;
    private static float dl = 0.707f;

    float zoom = 1f;

    public float moveSpeedMinZoom, moveSpeedMaxZoom;

    public float rotateSpeed = 1f;

    // Update is called once per frame
    void Update()
    {
        // 左
        if (Input.GetKey(KeyCode.A))
        {
            // Debug.Log(Input.mousePosition);
            transform.Translate(Vector3.left * Time.deltaTime * moveSpeed);
        }

        // 右
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
        }

        // 前
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
        }

        // 后
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * Time.deltaTime * moveSpeed);
        }

        // 上
        if (Input.GetKey(KeyCode.Space))
        {
            transform.Translate(Vector3.up * Time.deltaTime * moveSpeed);
        }

        // 下
        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.Translate(Vector3.down * Time.deltaTime * moveSpeed);
        }



        // 缩小
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            transform.Rotate(3 * Vector3.right * Time.deltaTime * rotateSpeed);
        }

        // 放大
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            transform.Rotate(3 * Vector3.left * Time.deltaTime * rotateSpeed);
        }


        // 右旋
        if (Input.GetKey(KeyCode.E))
        {
            // Debug.Log(Input.mousePosition);
            float rotateNum = rotateSpeed * Time.deltaTime;
            transform.Rotate(0, rotateNum, 0, Space.World);

            float angle = rotateNum * Mathf.PI / 180f;
            float x = Mathf.Sin(angle) * transform.localPosition.y;
            float y = (1 - Mathf.Cos(angle)) * transform.localPosition.y;

            transform.Translate(new Vector3(-x, dl * y, dl * y));
        }

        // 左旋
        if (Input.GetKey(KeyCode.Q))
        {
            float rotateNum = rotateSpeed * Time.deltaTime;
            transform.Rotate(0, -rotateNum, 0, Space.World);

            float angle = rotateNum * Mathf.PI / 180f;
            float x = Mathf.Sin(angle) * transform.localPosition.y;
            float y = (1 - Mathf.Cos(angle)) * transform.localPosition.y;

            transform.Translate(new Vector3(x, dl * y, dl * y));
        }

        //// 右旋
        //if (Input.GetKey(KeyCode.E))
        //{
        //    transform.Rotate(Vector3.down * Time.deltaTime * rotateSpeed);
        //}

        //// 左旋
        //if (Input.GetKey(KeyCode.Q))
        //{
        //    transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed);
        //}
    }
}
