using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected string enemy_name;
    protected int hp;
    protected int attack;
    protected GameObject bulletPrefab; // 弾のPrefab
    protected float bulletSpeed; // 弾の速度
    protected float fireRate;     // 発射間隔（秒）
    protected float fireCooldown; // 発射タイマー
    protected string text_explaination;
    
    public Vector2 parentObjPos { get; set; } // 親オブジェクトの座標(弾を打つときの方向の計算に必要)
    
    public void Initialize(EnemyData enemyData)
    {
        enemy_name = enemyData.name;
        hp = enemyData.hp;
        attack = enemyData.attack;
        bulletPrefab = enemyData.bulletPrefab;
        bulletSpeed = enemyData.bulletSpeed;
        fireRate = enemyData.fireRate;
        fireCooldown = enemyData.fireCooldown;
        text_explaination = enemyData.text_explaination;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "PlayerBullet")
        {
            this.hp--;
            if (hp == 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
