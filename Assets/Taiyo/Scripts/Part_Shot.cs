using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part_Shot : Part_Action
{
    [SerializeField] Vector2 shotVec;
    [SerializeField] float shotPower;
    [SerializeField] float shotSpan;
    [SerializeField] GameObject bulletPfb_Push;
    [SerializeField] GameObject bulletPfb_Hold;

    float shotTimer = 0;


    //アクションボタン短押しで発動
    public override void PushAction()
    {
        if (bulletPfb_Push == null)
        {
            return;
        }
        //ワールド座標のRotationを取得
        float angle = transform.rotation.eulerAngles.z;

        GameObject bullet = Instantiate(bulletPfb_Push, transform.position, transform.rotation);
        bullet.GetComponent<Rigidbody2D>().AddForce(Quaternion.Euler(0, 0, angle) * shotVec.normalized * shotPower, ForceMode2D.Impulse);
    }

    //アクションボタン長押しで発動
    public override void HoldAction()
    {
        if (bulletPfb_Hold == null)
        {
            return;
        }

        if (shotTimer <= 0)
        {//ワールド座標のRotationを取得
            float angle = transform.rotation.eulerAngles.z;

            GameObject bullet = Instantiate(bulletPfb_Hold, transform.position, transform.rotation);
            bullet.GetComponent<Rigidbody2D>().velocity = GameManager.instance.player.GetComponent<Rigidbody2D>().velocity;
            bullet.GetComponent<Rigidbody2D>().AddForce(Quaternion.Euler(0, 0, angle) * shotVec.normalized * shotPower, ForceMode2D.Impulse);

            shotTimer = shotSpan;

            AudioSetting.instance.PlaySE(4);
        }


    }

    protected override void Update()
    {
        base.Update();
        if (shotTimer > 0)
        {
            shotTimer -= Time.deltaTime;
        }
    }
}
