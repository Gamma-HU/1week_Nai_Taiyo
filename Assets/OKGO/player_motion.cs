using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_motion : MonoBehaviour
{
    float W_speed = 2.5f;
    void Start()
    {
        Time.timeScale = 1;
    }

    void Update()
    {    
        Vector3 position = transform.position;

        if(Input.GetKey(KeyCode.RightArrow)){
            position.x += this.W_speed * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.LeftArrow)){
            position.x -= this.W_speed * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.UpArrow)){
            position.y += this.W_speed * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.DownArrow)){
            position.y -= this.W_speed * Time.deltaTime;
        }
        transform.position = position;

    }
}
