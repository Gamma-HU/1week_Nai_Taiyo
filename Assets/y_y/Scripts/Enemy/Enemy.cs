using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{  
    private string name;
    private int hp;
    private int attack;
    private GameObject bulletPrefab; // 弾のPrefab
    private float bulletSpeed; // 弾の速度
    private float fireRate;     // 発射間隔（秒）
    private float fireCooldown; // 発射タイマー
    private string text_explaination;
    
    public void Initialize(EnemyData enemyData)
    {
        name = enemyData.name;
        hp = enemyData.hp;
        attack = enemyData.attack;
        bulletPrefab = enemyData.bulletPrefab;
        bulletSpeed = enemyData.bulletSpeed;
        fireRate = enemyData.fireRate;
        fireCooldown = enemyData.fireCooldown;
        text_explaination = enemyData.text_explaination;
    }

    private Transform player;

    void Start()
    {
        // プレイヤーを検索してTransformを取得
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        // プレイヤーが存在する場合のみ動作
        if (player != null)
        {
            // 発射タイマーを更新
            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0f)
            {
                ShootAtPlayer();
                fireCooldown = fireRate; // タイマーをリセット
            }
        }
    }

    void ShootAtPlayer()
    {
        // プレイヤーの方向を計算
        Vector3 direction = (player.localPosition - this.gameObject.transform.localPosition).normalized;

        // 弾を生成
        GameObject bullet = Instantiate(bulletPrefab, this.gameObject.transform.localPosition, Quaternion.identity);

        // 弾に速度を与える
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * bulletSpeed;
        }
    }
}
