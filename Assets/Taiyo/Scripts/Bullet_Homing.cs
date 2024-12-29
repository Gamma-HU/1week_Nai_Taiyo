using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet_Homing : Bullet
{
    [SerializeField] float distanceMax;
    [SerializeField] float attractionForce;
    [SerializeField] GameObject targetMarkerPfb;
    GameObject target;
    GameObject targetMarker;
    Rigidbody2D rb;
    float speed;


    void Start()
    {
        List<GameObject> enemies = GameManager.instance.enemySpawner.spawnedEnemies;
        float nearestDistance = distanceMax;
        foreach (GameObject enemy in enemies)
        {
            if (enemy == null)
            {
                continue;
            }
            float distance = (enemy.transform.position - transform.position).magnitude;
            if (distance < nearestDistance)
            {
                target = enemy;
                nearestDistance = distance;
            }
        }
        if (target)
        {
            targetMarker = Instantiate(targetMarkerPfb, target.transform);
            targetMarker.transform.localScale = Vector3.one / targetMarker.transform.parent.localScale.x;
        }
        rb = GetComponent<Rigidbody2D>();
        speed = rb.velocity.magnitude;
    }

    void FixedUpdate()
    {
        if (target)
        {
            rb.AddForce((target.transform.position - transform.position).normalized * attractionForce * speed);
            rb.velocity = rb.velocity.normalized * speed;
        }

        transform.up = rb.velocity.normalized;
    }

    void OnDestroy()
    {
        if (targetMarker)
        {
            Destroy(targetMarker);
        }
    }
}
