using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public Transform parent;
    // public意味着会被暴露到编辑器,可以在编辑器里面修改这个值
    public float mouseSensitivity = 500f; // 鼠标灵敏度
    float xRotation = 0f; // x轴旋转 pitch:俯仰角变动
    float yRotation = 0f; // y轴旋转 yaw: 偏航角变动 很显然:x是角色的横向坐标,y是角色的纵向坐标

    public float topClamp = -75f;
    public float bottomClamp = 75f;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 第一人称游戏中不需要光标,所以这里要锁定光标
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);
        yRotation += mouseX;
        //yRotation = Mathf.Clamp(yRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation,0f, 0f);
        parent.localRotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
