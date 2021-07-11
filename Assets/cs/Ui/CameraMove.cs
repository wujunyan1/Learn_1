using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float moveSpeed = 50;
    public float scaleSpeed = 120f;

    float rotate = 1.41421f;
    private static float dl = 0.707f;

    // 为了判断地图边界
    public HexGrid grid;

    float zoom = 1f;

    public float moveSpeedMinZoom, moveSpeedMaxZoom;

    public float rotateSpeed = 1f;

    public static CameraMove instance;
    private static bool _lock;

    public static bool Locked
    {
        set
        {
            if (instance)
            {
                instance.enabled = !value;
            }
            _lock = value;
        }

    }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        instance.enabled = !_lock;
    }

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
            transform.Translate(new Vector3(0, dl, dl) * Time.deltaTime * moveSpeed);
        }

        // 后
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0, -dl, -dl) * Time.deltaTime * moveSpeed);
        }

        // 缩小
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            transform.Translate(Vector3.back * Time.deltaTime * scaleSpeed);
        }

        // 放大
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * scaleSpeed);
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

        System.TimeSpan ts = System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 0, 0, 0);
        long ret = System.Convert.ToInt64(ts.TotalSeconds);
    }

    // 移动
    void AdjustPosition(float xDelta, float zDelta)
    {
        Vector3 direction = transform.localRotation * new Vector3(xDelta, 0f, zDelta).normalized;
        float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
        float distance = Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) * damping * Time.deltaTime;

        Vector3 position = transform.localPosition;
        position += direction * distance;
        transform.localPosition = ClampPosition(position);
    }

    // 判断边界
    Vector3 ClampPosition(Vector3 position)
    {
        float xMax = (grid.cellCountX - 0.5f) * (2f * HexMetrics.innerRadius);
        position.x = Mathf.Clamp(position.x, 0f, xMax);

        float zMax = (grid.cellCountX - 1f) * (1.5f * HexMetrics.outerRadius);
        position.z = Mathf.Clamp(position.z, 0f, zMax);

        return position;
    }
}
