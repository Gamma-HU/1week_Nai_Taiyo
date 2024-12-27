using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class motion : MonoBehaviour
{
    float rz;
    public float speed = 1.5f;
    public float Dash_Speed = 10.0f;
    public float Back_Speed = 2.5f;
    public float stopDistance = 0.5f;
    bool Battle_Mode = false;
    bool isCoroutineRunning = false;
    Vector3 movePosition;
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

        if (player != null && Vector3.Distance(transform.position, player.transform.position) < 4.0f)
        {
            Battle_Mode = true;
        }

        if (Battle_Mode == false)
        {
            Tyorotyoro();
        }
        else
        {
            if (!isCoroutineRunning)
            {
                StartCoroutine(Tosshin(transform.position));
            }
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

    private IEnumerator Tosshin(Vector3 before)
    {
        isCoroutineRunning = true;
        
        // 突進
        while (Vector3.Distance(transform.position, player.transform.position) > stopDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Dash_Speed * Time.deltaTime);
            yield return null;
        }

        // 元の位置に戻る
        while (Vector3.Distance(transform.position, before) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, before, Back_Speed * Time.deltaTime);
            yield return null;
        }

        isCoroutineRunning = false; // コルーチン終了時にフラグをリセット
        Battle_Mode = false; // 突進が終わったら元のモードに戻す
    }
}
