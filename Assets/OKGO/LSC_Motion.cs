using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class LSC_Motion : MonoBehaviour
{
    public float speed;
    public LightSaber LS;
    public float Cos_Man, Sin_Man;
    GameObject LSE;
    void Start()
    {
        //this.LSE = transform.parent.gameObject;
    }

    void Update()
    {
        transform.Translate(this.speed * Cos_Man, this.speed * Sin_Man, 0);
    }

    void OnBecameInvisible(){
        Destroy(this.gameObject);
    }
}
