using System.Collections;
using UnityEngine;

public class Kurukuru_Man : MonoBehaviour
{
    public float rotSpeed;    // 初期の回転速度
    public float rotMinus;  // 回転速度の減衰
    public float speed;        // 移動速度
    public float distance;      // 移動距離

    // ↑インスペクター上から調整してください

    private bool isCoroutineRunning = false;

    void Update()
    {
        // 回転処理
        transform.Rotate(0, 0, rotSpeed * Time.deltaTime);

        // 回転速度の減衰(念の為付けたけどいらないかも？)
        if (rotSpeed > 0)
        {
            rotSpeed -= rotMinus * Time.deltaTime;
        }
        else
        {
            rotSpeed = 0;
        }

        // コルーチンの管理
        if (!isCoroutineRunning)
        {
            StartCoroutine(Kietyau());
        }
    }

    private IEnumerator Kietyau()
    {
        isCoroutineRunning = true;

        // 現在の位置と回転から移動先を計算
        Vector3 currentPosition = transform.position;
        float angleZ = transform.rotation.eulerAngles.z * Mathf.Deg2Rad; // 回転をラジアンに変換
        Vector3 movePosition = new Vector3(
            currentPosition.x + distance * Mathf.Cos(angleZ),
            currentPosition.y + distance * Mathf.Sin(angleZ),
            currentPosition.z
        );

        // 移動処理
        while (Vector3.Distance(transform.position, movePosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePosition, speed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        // オブジェクトの削除
        Destroy(gameObject);
    }
}
