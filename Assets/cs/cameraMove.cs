using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public int speed = 50;
    float scale_speed = 120f;

    float rotate = 1.41421f;

    private Transform transform;

    // 为了判断地图边界
    public HexGrid grid;

    float zoom = 1f;

    public float moveSpeedMinZoom, moveSpeedMaxZoom;

    float rotateSpeed = 1f;

    public static CameraMove instance;
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
        transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(new Vector3(0, 1 / rotate, 1 / rotate) * speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0, -1 / rotate, -1 / rotate) * speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(new Vector3(-1, 0, 0) * speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector3(1, 0, 0) * speed * Time.deltaTime);
        }

        // 缩放
        if (Input.mouseScrollDelta.y > 0)
        {
            transform.Translate(Vector3.forward * scale_speed * Time.deltaTime);
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            transform.Translate(Vector3.forward * -1 * scale_speed * Time.deltaTime);
        }

        // 转向
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(new Vector3(0, 0 - rotateSpeed, 0), Space.World);
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(new Vector3(0, rotateSpeed, 0), Space.World);
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
