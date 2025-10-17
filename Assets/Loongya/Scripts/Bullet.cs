using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private string logMessage;
    // 碰撞事件
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            logMessage = "hit" + collision.gameObject.name + " !";
            Debug.Log(logMessage); // 同时输出到控制台
            //print("hit" + collision.gameObject.name + " !");
            Destroy(this.gameObject);
        }
        
        
    }
    void OnGUI()
    {
        // 设置文本样式（位置、颜色、字体大小等）
        GUI.color = Color.white;
        GUIStyle style = new GUIStyle();
        style.fontSize = 24; // 字体大小
        style.normal.textColor = Color.red; // 文本颜色

        // 在屏幕左上角显示文本（x=10, y=10 是坐标）
        GUI.Label(new Rect(10, 10, 500, 30), logMessage, style);
    }
}
