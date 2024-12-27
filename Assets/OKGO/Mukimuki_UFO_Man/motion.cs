using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class motion : MonoBehaviour
{
    float rz;
    public float speed = 1.5f;
    bool Battle_Mode = false;
    Vector3 movePosition;
    bool isMoving = false;
    GameObject player;

    void Start()
    {
        SetRandomTarget();
        player = GameObject.Find("Nise_Player");
    }

    void Update()
    {
        rz = 20 * Mathf.Sin(Time.time * 6);
        transform.rotation = Quaternion.Euler(0, 0, rz);

        if(Vector3.Distance(transform.position, player.transform.position) < 4.0f){
            Battle_Mode = true;
        }

        if (Battle_Mode == false){
            Tyorotyoro();
        }else{
            
        }
    }

    void Tyorotyoro()
    {
        Vector3 position = transform.position;
        if (Vector3.Distance(position, movePosition) < 0.1f)
        {
            SetRandomTarget();
        }

        transform.position = Vector3.MoveTowards(position, movePosition, speed * Time.deltaTime);
    }

    void SetRandomTarget()
    {
        float plus_x = Random.Range(-2.0f, 2.0f);
        float plus_y = Random.Range(-2.0f, 2.0f);
        movePosition = new Vector3(transform.position.x + plus_x, transform.position.y + plus_y, transform.position.z);
    }
}
