using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lazer_Shotter : MonoBehaviour
{
    public float speed;
    void Start(){
        
    }

    void Update(){
        transform.Translate(this.speed, 0, 0);
    }

    void OnBecameInvisible(){
        Destroy(this.gameObject);
    }
}
