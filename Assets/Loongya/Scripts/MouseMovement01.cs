using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement01 : MonoBehaviour
{
    public Transform parent;
    // 旋转轴（控制X轴左右，Y轴上下）
    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;

    // 灵敏度（X/Y可分别设置，建议X稍高，Y稍低避免上下晃动）
    [Range(1f, 10f)] public float sensitivityX = 2f;
    [Range(1f, 10f)] public float sensitivityY = 2f;

    // Y轴旋转限制（避免仰头/低头超过180度）
    public float minimumY = -60f;
    public float maximumY = 60f;

    // 平滑因子（值越小越平滑，0.1-0.3之间较合适）
    [Range(0.01f, 0.5f)] public float smoothTime = 0.1f;

    private float rotationY = 0f;
    private Vector2 currentVelocity; // 平滑插值用的速度变量

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        // 获取鼠标输入（用Time.deltaTime补偿帧率，避免帧率影响移动幅度）
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime * 100f;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime * 100f;

        // 平滑处理：用Vector2.SmoothDamp过滤抖动，让移动更连贯
        Vector2 smoothInput = Vector2.SmoothDamp(
            new Vector2(mouseX, mouseY),  // 当前输入
            Vector2.zero,                 // 目标（每次更新后归0，避免累积）
            ref currentVelocity,          // 速度引用（内部计算平滑用）
            smoothTime                    // 平滑时间
        );

        if (axes == RotationAxes.MouseXAndY)
        {
            // X轴旋转（左右）：直接累加，无限制
            transform.parent.parent.Rotate(0, smoothInput.x, 0);

            // Y轴旋转（上下）：限制角度范围
            rotationY -= smoothInput.y;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            // 应用Y轴旋转（保持X和Z轴不偏移）
            transform.localEulerAngles = new Vector3(rotationY, transform.localEulerAngles.y, 0);
        }
        else if (axes == RotationAxes.MouseX)
        {
            // 仅左右旋转
            transform.parent.parent.Rotate(0, smoothInput.x, 0);
        }
        else
        {
            // 仅上下旋转（限制角度）
            rotationY -= smoothInput.y;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
            transform.localEulerAngles = new Vector3(rotationY, transform.localEulerAngles.y, 0);
        }
    }
}
