using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller; // 角色控制器
    
    public float speed = 12f; // 移动速度
    public float gravity = -9.81f * 2; // 重力
    public float jumpHeight = 3f; // 跳跃高度
    
    public Transform groundCheck; // 地面检测,用来检测是否站在地面上,用来跳跃
    public float groundDistance = 0.4f; // 角色与地面的距离
    public LayerMask groundMask; // 地面遮罩
    
    Vector3 velocity; // 跳跃速度
    
    bool isGrounded;
    bool isMoving;
    
    private Vector3 lastPosition = new Vector3(0f,0f,0f);
    
    void Start()
    {
        controller = GetComponent<CharacterController>(); 
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        // 右手坐标系中,x=左右,y=上下 z=前后 | z的正方向是对准你的方向
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * (speed * Time.deltaTime));

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // 跳跃 向上
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            
        }
        // 降落
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (lastPosition != gameObject.transform.position && isGrounded == true  )
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
        lastPosition = gameObject.transform.position;
    }
}
