using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEnemy : Enemy
{
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
        // 親オブジェクトの座標は引いておく
        Vector2 direction = (player.GetComponent<RectTransform>().anchoredPosition - (this.gameObject.GetComponent<RectTransform>().anchoredPosition)).normalized;
        // 弾を生成
        GameObject bullet = Instantiate(bulletPrefab, this.gameObject.GetComponent<RectTransform>().anchoredPosition, Quaternion.identity);

        // 弾に速度を与える
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * bulletSpeed;
        }
    }
}
