using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Part_Power : Part
{
    [SerializeField] public float power;
    [SerializeField] public Vector2 powerVec;
    [SerializeField] public float inputEffectRatio;

    public void AddPower(Rigidbody2D rb, Vector2 inputVector)
    {
        //ワールド座標のRotationを取得
        float angle = transform.rotation.eulerAngles.z;

        //部品固有の出力方向とプレイヤーの入力方向の合成
        //Vector2 forceVec = (Vector2)(Quaternion.Euler(0, 0, angle) * powerVec).normalized * (1 - inputEffectRatio) + inputVector * inputEffectRatio;
        //Vector2 forcePos = transform.position;
        //Debug.Log(("1", rb.worldCenterOfMass - forcePos));
        //forcePosを0.1単位で丸める
        //forcePos = new Vector2(Mathf.Round(forcePos.x * 10) / 10, Mathf.Round(forcePos.y * 10) / 10);
        //Debug.Log((gameObject.name, rb.worldCenterOfMass - forcePos));

        Vector2 forceVec = (Vector2)(Quaternion.Euler(0, 0, angle) * powerVec).normalized;

        rb.AddForceAtPosition(forceVec * power, transform.position);


    }
}
