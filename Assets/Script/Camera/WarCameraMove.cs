using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarCameraMove : MonoBehaviour
{
    public float moveSpeed = 50;
    public float scaleSpeed = 120f;

    public float xRotate = 60f;
    float baseZ;
    float baseY;

    float rotate = 1.41421f;
    private static float dl = 0.707105f;

    // 为了判断地图边界
    public BattleMap map;

    float zoom = 1f;

    public float moveSpeedMinZoom, moveSpeedMaxZoom;

    public float rotateSpeed = 1f;

    public static WarCameraMove instance;
    private static bool _lock;
    
    public static bool Locked
    {
        set
        {
            instance.enabled = !value;
        }

    }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(xRotate, 0, 0);
        //transform = GetComponent<Transform>();
        instance.enabled = !_lock;

        float pi_xRotate = xRotate * Mathf.PI / 180f;
        baseY = Mathf.Sin(pi_xRotate);
        baseZ = Mathf.Cos(pi_xRotate);
    }

    // Update is called once per frame
    void LateUpdate()
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
            transform.Translate(new Vector3(0, baseY, baseZ) * Time.deltaTime * moveSpeed);
            //transform.position = transform.position + (new Vector3(0, 0, 1) * Time.deltaTime * moveSpeed);
            //transform.Translate(new Vector3(0, 0, 1) * Time.deltaTime * moveSpeed);
        }

        // 后
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0, -baseY, -baseZ) * Time.deltaTime * moveSpeed);
            //transform.position = transform.position + (new Vector3(0, 0, -1) * Time.deltaTime * moveSpeed);

            //transform.Translate(new Vector3(0, 0, -1) * Time.deltaTime * moveSpeed);
        }

        // 缩小
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && transform.position.y < 80)
        {
            transform.Translate(Vector3.back * Time.deltaTime * scaleSpeed);
        }

        // 放大
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && transform.position.y > 4)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * scaleSpeed);
        }

        // 右旋
        if (Input.GetKey(KeyCode.E))
        {
            // Debug.Log(Input.mousePosition);
            float rotateNum = rotateSpeed * Time.deltaTime;
            transform.Rotate(0, rotateNum, 0, Space.World);

            //float angle = rotateNum * Mathf.PI / 180f;
            //float x = Mathf.Sin(angle) * transform.localPosition.y;
            //float y = (1 - Mathf.Cos(angle)) * transform.localPosition.y;

            //transform.Translate(new Vector3(-x, baseY * y, dl * y));
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

        System.TimeSpan ts = System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 0, 0, 0);
        long ret = System.Convert.ToInt64(ts.TotalSeconds);
    }
}
