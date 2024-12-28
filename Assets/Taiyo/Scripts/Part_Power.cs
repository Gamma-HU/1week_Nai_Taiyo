using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Part_Power : Part
{
    [SerializeField] public float power;
    [SerializeField] public Vector2 powerVec;
    [SerializeField] public float fuelConsumption; //燃料消費量

    public void AddPower(Rigidbody2D rb, Vector2 inputVector)
    {
        //ワールド座標のRotationを取得
        float angle = transform.rotation.eulerAngles.z;

        Vector2 forceVec = (Vector2)(Quaternion.Euler(0, 0, angle) * powerVec).normalized;

        rb.AddForceAtPosition(forceVec * power, transform.position);


    }
}
