using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEnemy : Enemy
{
    [SerializeField] private float attack_range = 10f;
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
        Debug.Log(fireCooldown);
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

        Vector2 diff = player.GetComponent<RectTransform>().anchoredPosition -
                       (this.gameObject.GetComponent<RectTransform>().anchoredPosition);

        Debug.Log(diff.magnitude > attack_range);
        if (diff.magnitude > attack_range) return;
        
        // プレイヤーの方向を計算
        // 親オブジェクトの座標は引いておく
        Vector2 direction = diff.normalized;
        // 弾を生成
        GameObject bullet = Instantiate(bulletPrefab, this.gameObject.GetComponent<RectTransform>().anchoredPosition, Quaternion.identity);
        bullet.GetComponent<DamageParameter>().damage = this.attack;
        
        // 弾に速度を与える
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * bulletSpeed;
        }
    }
}
