using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Camera PlayerCamera; // 玩家摄像机 用于发射射线,确认射击目标

    public bool isShooting; // 是否正在射击
    public bool readyToShoot; // 准备好射击
    
    bool allowReset = true; // 是否允许射击 控制射击间隔时间
    public float shootingDelay = 0.2f; // 射击间隔
    // burst
    public int bulletsPerBurst = 3; // 连射预加载子弹数量
    public int burstBulletsLeft; // 当前周期内剩余连射次数
    // spread
    public float spreadIntensity; // 允许射击的误差
    
    // bullet
    public GameObject bulletPrefab; // 子弹预制体
    public Transform bulletSpawn; // 子弹生成时的变换,用于确定位置
    public float bulletVelocity = 30; // 子弹的初始速度
    public float bulletPrefabLifeTime = 3f; // 子弹生命周期-销毁时间 默认3秒
    
    // 射击模式
    public enum ShootingMode
    {
        Single, // 单发
        Burst,  // 连发
        Auto    // 自动
    }

    public ShootingMode currentShootingMode; // 射击模式

    // 当前激活状态
    private void Awake()
    {
        readyToShoot = true; // 准备好射击
        //burstBulletsLeft = bulletsPerBurst; // 当前
    }
    
    //public Rigidbody bulletRigidbodyPrefabRb; // 子弹的刚体
    void Update()
    {
        if (currentShootingMode == ShootingMode.Auto)
        {
            isShooting = Input.GetKey(KeyCode.Mouse0); // GetKey:持续按住
        } 
        else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst) // 射击模式是单发或连发
        {
            isShooting = Input.GetKeyDown(KeyCode.Mouse0); // 点按
        }

        if (readyToShoot && isShooting)
        {
            burstBulletsLeft = bulletsPerBurst;
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            FireWeapon();
        }
    }

    private void FireWeapon()
    {
        readyToShoot = false;
        // 创建射击方向向量,并通过专用方法,计算实际弹道方向和散布角
        Vector3 shootingDirection = CalculateDirectionSpread().normalized;
        // 实例化子弹,确定子弹方向
        // 在指定的位置,生成指定预制体,且设置为初始旋转状态的四元数(无任何旋转,保留初始状态)
        //Rigidbody bulletRb = Instantiate(bulletRigidbodyPrefabRb,bulletSpawn.position,Quaternion.identity); 
        GameObject bullet = Instantiate(bulletPrefab,bulletSpawn.position,Quaternion.identity);
        // 确定子弹方向
        bullet.transform.forward = shootingDirection;
        
        // 射出子弹
        // 获取子弹的物理特性,为子弹施加一个力,让其从某个方向射出去 Impulse: 冲量式
        //将方向向量 “归一化”（确保向量长度为 1）。这一步的作用是只保留方向，忽略长度影响，避免因 bulletSpawn 的缩放等因素导致方向向量长度异常，保证力的方向准确
        //bulletRb.AddForce(bulletSpawn.forward.normalized * bulletVelocity, ForceMode.Impulse);
        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
        // 使用协程销毁子弹
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));
        // 检测射击间隔问题
        if (allowReset)
        {
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            Invoke("ResetShot", shootingDelay);
            allowReset = false; // 定时任务,先在主线程把allowReset设置为false,那么在定时任务前,就不会再次执行该定时任务
        }
        // 设置连发模式
        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1) // 首次射击已经完成,所以要检测>1
        {
            burstBulletsLeft--;
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            Invoke("FireWeapon",shootingDelay);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }
    private Vector3 CalculateDirectionSpread()
    {
        // 射线检测,从屏幕中心(摄像机)射出一条射线,检测它碰撞了什么东西
        Ray ray = PlayerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            // 射中某个东西
            targetPoint = hit.point;
        }
        else // 射向天空,需要确定射出方向
        {
            targetPoint = ray.GetPoint(100f);
        }
        // 计算子弹运行方向 = 目标位置 - 子弹生成位置
        Vector3 direction = targetPoint - bulletSpawn.position;
        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        return direction + new Vector3(x, y, 0);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
