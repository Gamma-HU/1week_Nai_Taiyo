using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine; // Cinemachineを使用
using DG.Tweening; // DOTweenを使用

public class CameraController : MonoBehaviour
{
    public GameObject player; // プレイヤーオブジェクト
    public GameObject cockPit; // コックピットオブジェクト
    public CinemachineVirtualCamera virtualCamera; // 仮想カメラ
    public float zoomDuration = 0.5f; // ズームアニメーションの時間
    public float maxCameraSize = 50f; // カメラサイズの最大値

    private float minCameraSize; // 初期サイズを最小サイズとして使用
    private float targetCameraSize; // 目標のカメラサイズ

    // Start is called before the first frame update
    void Start()
    {
        if (virtualCamera != null)
        {
            // 現在のカメラサイズを最小サイズとして設定
            minCameraSize = virtualCamera.m_Lens.OrthographicSize;
            targetCameraSize = minCameraSize; // 初期値を設定
        }
    }

    // Update is called once per frame
    void Update()
    {
        Transform[] childTransforms = player.GetComponentsInChildren<Transform>();
        List<Transform> parts = new List<Transform>();

        // Tagが"Part"のオブジェクトを収集
        foreach (Transform child in childTransforms)
        {
            if (child.CompareTag("Part"))
            {
                parts.Add(child);
            }
        }

        Transform farthestPart = null;
        float maxDistance = 0f;

        // 最も遠いPartを探す
        foreach (Transform part in parts)
        {
            Part partComponent = part.GetComponent<Part>();
            if (partComponent != null && !partComponent.isCockpit)
            {
                float distance = Vector3.Distance(partComponent.transform.position, cockPit.transform.position);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    farthestPart = part;
                }
            }
        }

        // 目標カメラサイズを計算
        if (farthestPart != null)
        {
            targetCameraSize = Mathf.Clamp(maxDistance * 1.2f, minCameraSize, maxCameraSize);
        }

        // DOTweenで滑らかにカメラサイズをアニメーション
        if (virtualCamera != null)
        {
            DOTween.To(
                () => virtualCamera.m_Lens.OrthographicSize,
                size => virtualCamera.m_Lens.OrthographicSize = size,
                targetCameraSize,
                zoomDuration
            );
        }
    }
}