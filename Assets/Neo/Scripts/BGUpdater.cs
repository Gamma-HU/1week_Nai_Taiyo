using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGUpdater : MonoBehaviour
{
    public RectTransform background;   // 背景のImage (RectTransform)
    public Transform player;           // 自機のTransform
    public Camera mainCamera;          // メインカメラ

    public Vector2 backgroundSize;     // 背景画像の幅と高さ
    public float margin = 0.1f;        // 背景のエッジを超える際の余裕距離

    private Vector2 cameraHalfSize;    // カメラの視野半分のサイズ

    void Start()
    {
        // カメラの半分のサイズを計算（画面の上下左右に必要）
        cameraHalfSize = new Vector2(
            mainCamera.orthographicSize * mainCamera.aspect,
            mainCamera.orthographicSize
        );
        backgroundSize = background.sizeDelta;
    }

    void LateUpdate()
    {
        Vector2 cameraPosition = player.position;

        // カメラ端の範囲
        Vector2 cameraMin = cameraPosition - cameraHalfSize;
        Vector2 cameraMax = cameraPosition + cameraHalfSize;

        // 背景端の範囲
        Vector2 backgroundMin = background.anchoredPosition - (backgroundSize / 2);
        Vector2 backgroundMax = background.anchoredPosition + (backgroundSize / 2);

        // 背景をカメラの範囲にあわせて移動
        if (cameraMin.x < backgroundMin.x || cameraMax.x > backgroundMax.x)
        {
            background.anchoredPosition = new Vector2(player.position.x, background.anchoredPosition.y);
        }
        if (cameraMin.y < backgroundMin.y || cameraMax.y > backgroundMax.y)
        {
            background.anchoredPosition = new Vector2(background.anchoredPosition.x, player.position.y);
        }
    }
}
