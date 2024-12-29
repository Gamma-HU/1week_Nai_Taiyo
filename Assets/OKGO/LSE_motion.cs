using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSE_motion : MonoBehaviour
{
    public float speed = 1.5f;
    public float rotationSpeed = 5.0f; // 回転速度
    public float rotationBackSpeed = 2.0f;
    public float detectionRange = 4.0f;
    public LightSaber lightSaber; // インスペクターで設定
    public LS_collisionDirector LSCD;
    private Vector3 movePosition;
    private GameObject player;
    GameObject square;

    void Start()
    {
        SetRandomTarget();
        player = GameObject.FindWithTag("Player");

        if (lightSaber == null)
        {
            lightSaber = transform.GetComponentInChildren<LightSaber>();
        }
        this.square = transform.Find("LightSaber/Square").gameObject;
        square.SetActive(false);
    }

    void Update()
    {
        if (player != null && Vector3.Distance(transform.position, player.transform.position) < detectionRange)
        {
            if (lightSaber != null)
            {
                lightSaber.Play_Man();
                square.SetActive(true);
                //LSCD.CC_Generator();
            }
            SmoothLookAt(player.transform.position);
        }
        else
        {
            if (lightSaber != null)
            {
                lightSaber.Stop_Man();
                square.SetActive(false);
            }
            RandomMove();
            Quaternion targetRotation = Quaternion.Euler(0, 0, 0); // 目標の回転
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationBackSpeed * Time.deltaTime);
        }
    }

    void RandomMove()
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
        float range = 5.0f; // 移動範囲
        float x = Random.Range(-range, range);
        float y = Random.Range(-range, range);
        movePosition = new Vector3(x, y, transform.position.z);
    }

    void SmoothLookAt(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, direction);
        Quaternion adjustedRotation = lookRotation * Quaternion.Euler(0, 0, -90);
        transform.rotation = Quaternion.Slerp(transform.rotation, adjustedRotation, rotationSpeed * Time.deltaTime);
    }
}

