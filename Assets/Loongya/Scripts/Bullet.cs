using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private string logMessage;
    // 碰撞事件
    private void OnCollisionEnter(Collision ObjectWeHit)
    {
        if (ObjectWeHit.gameObject.CompareTag("Target"))
        {
            logMessage = "hit" + ObjectWeHit.gameObject.name + " !";
            Debug.Log(logMessage); // 同时输出到控制台
            CreateBulletImpactEffect(ObjectWeHit);
            Destroy(this.gameObject);
        }
        if (ObjectWeHit.gameObject.CompareTag("Wall"))
        {
            logMessage = "hit a wall!";
            Debug.Log(logMessage); // 同时输出到控制台
            CreateBulletImpactEffect(ObjectWeHit);
            Destroy(this.gameObject);
        }
    }

    void CreateBulletImpactEffect(Collision ObjectWehit)
    {
        // 获取第一个接触点
        ContactPoint contact = ObjectWehit.contacts[0];
        // 实例化贴花弹孔
        // 弹孔实例化,我们没法直接获取孔洞预制体的引用,因为子弹本身就是代码里创建的
        // 所以,另辟蹊径,我们在场景里面创建一个全局引用物体,然后从里面获取?
        GameObject hole = Instantiate(
            GlobalReferences.Instance.bulletImpactEffectPrefab,
            contact.point,
            Quaternion.LookRotation(contact.normal)
        );
        // 把弹孔附着在碰撞的物体上
        hole.transform.SetParent(ObjectWehit.gameObject.transform);
    }
}
