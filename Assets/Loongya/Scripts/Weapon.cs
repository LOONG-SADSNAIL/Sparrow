using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab; // 子弹预制体
    public Transform bulletSpawn; // 子弹生成时的位移
    public float bulletVelocity = 30; // 子弹的初始速度
    public float bulletPrefabLifeTime = 30f; // 子弹生命周期-销毁时间 默认3秒
    
    //public Rigidbody bulletRigidbodyPrefabRb; // 子弹的刚体
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            FireWeapon();
        }
    }

    private void FireWeapon()
    {
        // 实例化子弹
        // 在指定的位置,生成指定预制体,且设置为初始旋转状态的四元数(无任何旋转,保留初始状态)
        //Rigidbody bulletRb = Instantiate(bulletRigidbodyPrefabRb,bulletSpawn.position,Quaternion.identity); 
        GameObject bullet = Instantiate(bulletPrefab,bulletSpawn.position,Quaternion.identity); 
        // 射出子弹
        // 获取子弹的物理特性,为子弹施加一个力,让其从某个方向射出去 Impulse: 冲量式
        //将方向向量 “归一化”（确保向量长度为 1）。这一步的作用是只保留方向，忽略长度影响，避免因 bulletSpawn 的缩放等因素导致方向向量长度异常，保证力的方向准确
        //bulletRb.AddForce(bulletSpawn.forward.normalized * bulletVelocity, ForceMode.Impulse);
        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward.normalized * bulletVelocity, ForceMode.Impulse);
        // 使用协程销毁子弹
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
