using UnityEngine;

public class FreeCameraToggle : MonoBehaviour
{
    [Header("基础设置")]
    public Camera targetCamera; // 要控制的摄像机（如角色的跟随摄像机）
    public KeyCode toggleKey = KeyCode.F8; // 切换快捷键（模拟F8）
    public float moveSpeed = 10f; // 自由移动速度
    public float rotateSpeed = 2f; // 自由旋转速度

    [Header("状态记录")]
    private bool isFreeMode = false; // 是否处于自由模式
    private Transform originalParent; // 记录摄像机原来的父对象（用于恢复绑定）
    private Vector3 originalLocalPos; // 记录摄像机原来的本地位置
    private Quaternion originalLocalRot; // 记录摄像机原来的本地旋转
    private float xRotation = 0f; // 自由模式下的X轴旋转角度


    void Start()
    {
        // 初始化：记录摄像机的初始状态（父对象、位置、旋转）
        if (targetCamera == null)
            targetCamera = Camera.main; // 默认使用主摄像机

        originalParent = targetCamera.transform.parent;
        originalLocalPos = targetCamera.transform.localPosition;
        originalLocalRot = targetCamera.transform.localRotation;
    }


    void Update()
    {
        // 按下快捷键切换模式
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleFreeMode();
        }

        // 如果处于自由模式，处理摄像机移动和旋转
        if (isFreeMode)
        {
            HandleFreeCameraMovement();
        }
    }


    // 切换自由模式/绑定模式
    void ToggleFreeMode()
    {
        isFreeMode =!isFreeMode;

        if (isFreeMode)
        {
            // 进入自由模式：解除摄像机与角色的绑定
            targetCamera.transform.parent = null; // 取消父对象，让摄像机独立
        }
        else
        {
            // 退出自由模式：恢复摄像机与角色的绑定
            targetCamera.transform.parent = originalParent;
            targetCamera.transform.localPosition = originalLocalPos;
            targetCamera.transform.localRotation = originalLocalRot;
        }
    }


    // 自由模式下的摄像机控制（类似编辑器视角）
    void HandleFreeCameraMovement()
    {
        // 鼠标旋转控制
        float mouseX = Input.GetAxis("Mouse X") * rotateSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotateSpeed;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // 限制上下旋转角度（避免过度翻转）

        // 应用旋转
        targetCamera.transform.localRotation = Quaternion.Euler(xRotation, targetCamera.transform.localEulerAngles.y + mouseX, 0f);

        // 键盘移动控制（WASD或方向键）
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float upDown = 0;

        // 可以用Q/E控制上下移动（可选）
        if (Input.GetKey(KeyCode.Q)) upDown = -1;
        if (Input.GetKey(KeyCode.E)) upDown = 1;

        // 计算移动方向（基于摄像机当前朝向）
        Vector3 moveDir = targetCamera.transform.right * horizontal + 
                         targetCamera.transform.forward * vertical + 
                         targetCamera.transform.up * upDown;

        // 应用移动
        targetCamera.transform.position += moveDir * (moveSpeed * Time.deltaTime);
    }
}