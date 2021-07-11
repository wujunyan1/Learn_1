using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarCamera2 : MonoBehaviour
{
    // 前后左右
    Transform swivel;

    // 放大缩小
    Transform stick;

    float zoom = 1f;

    public float stickMinZoom, stickMaxZoom;

    public float swivelMinZoom, swivelMaxZoom;

    //public float moveSpeed;
    public float moveSpeedMinZoom, moveSpeedMaxZoom;

    public float rotationSpeed;

    // 选择的角度
    float rotationAngle;
    

    public static WarCamera2 instance;

    private static int _lock = 0;

    public static bool Locked
    {
        set
        {
            _lock = value ? _lock + 1 : _lock - 1;
            if (instance)
            {
                instance.enabled = _lock <= 0;
            }

            Debug.Log(_lock);
        }

    }

    void Awake()
    {
        instance = this;
        swivel = transform.GetChild(0);
        stick = swivel.GetChild(0);
    }

    private void Start()
    {
        Debug.Log(_lock);
        instance.enabled = _lock <= 0;
    }

    void OnEnable()
    {
        instance = this;
        ValidatePosition();
    }

    public void Update()
    {
        // 放大缩小
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
        if (zoomDelta != 0f)
        {
            AdjustZoom(zoomDelta);
        }

        // 旋转
        float rotationDelta = Input.GetAxis("Rotation");
        if (rotationDelta != 0f)
        {
            AdjustRotation(rotationDelta);
        }

        // 移动
        float xDelta = Input.GetAxis("Horizontal");
        float zDelta = Input.GetAxis("Vertical");
        if (xDelta != 0f || zDelta != 0f)
        {
            AdjustPosition(xDelta, zDelta);
        }
    }

    // 放大缩小
    void AdjustZoom(float delta)
    {
        zoom = Mathf.Clamp01(zoom + delta);

        float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
        stick.localPosition = new Vector3(0f, 0f, distance);

        float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
        swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
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
        //float xMax = (grid.cellCountX - 0.5f) * HexMetrics.innerDiameter;
        //position.x = Mathf.Clamp(position.x, 0f, xMax);

        //float zMax = (grid.cellCountZ - 1f) * (1.5f * HexMetrics.outerRadius);
        //position.z = Mathf.Clamp(position.z, 0f, zMax);

        return position;
    }

    void AdjustRotation(float delta)
    {
        rotationAngle += delta * rotationSpeed * Time.deltaTime;
        if (rotationAngle < 0f)
        {
            rotationAngle += 360f;
        }
        else if (rotationAngle >= 360f)
        {
            rotationAngle -= 360f;
        }
        transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
    }

    public static void ValidatePosition()
    {
        instance.AdjustPosition(0f, 0f);
    }

    Vector3 WrapPosition(Vector3 position)
    {
        //float width = grid.cellCountX * HexMetrics.innerDiameter;
        //while (position.x < 0f)
        //{
        //    position.x += width;
        //}
        //while (position.x > width)
        //{
        //    position.x -= width;
        //}

        //float zMax = (grid.cellCountZ - 1) * (1.5f * HexMetrics.outerRadius);
        //position.z = Mathf.Clamp(position.z, 0f, zMax);

        //grid.CenterMap(position.x);
        return position;
    }
}
