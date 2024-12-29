using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPsystem : MonoBehaviour
{
    public float HP;
    public float LazerDamage;
    void Start(){
    }

    void Update(){
        if(HP <= 0){
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.tag == "Lazer"){
            HP -= LazerDamage;
        }
        Debug.Log("kkk");
    }
}
