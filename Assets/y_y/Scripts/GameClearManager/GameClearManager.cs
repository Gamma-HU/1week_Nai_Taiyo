using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening; // DOTweenを使用

public class GameClearManager : MonoBehaviour
{
    [SerializeField] private float goalDistance = 1000f;
    [SerializeField] private GameObject ClearUI;
    [SerializeField] private TextMeshProUGUI RemainDistanceText;
    private float current_distance = 0f;
    private GameObject playerGameObj;
    
    [SerializeField] private float duration = 2.0f;
    private float forceMagnitude;
    [Header("スタート地点の座標")]　[SerializeField] private Vector2 startPosition = new Vector2(0, 0);

    private bool is_gameClear = false;
    private Vector2 forceDirection;

    void Start()
    {
        playerGameObj = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        current_distance = Vector3.Distance(startPosition, playerGameObj.GetComponent<RectTransform>().anchoredPosition);
        float remainDistance = goalDistance - current_distance;
        if(remainDistance <= 0f) remainDistance = 0f;
        RemainDistanceText.text = $"残り" + remainDistance.ToString("F0") + "m";
        if (current_distance > goalDistance)
        {
            GameClear();
        }
    }

    private void FixedUpdate()
    {
        if (is_gameClear)
        {
            // 起点からプレイヤーの方向に力を加え続ける
            Debug.Log("AddForce");
            playerGameObj.GetComponent<Rigidbody2D>().AddForce(forceDirection.normalized * forceMagnitude);
        }
    }

    private void GameClear()
    {
        //Time.timeScale = 0f;
        
        // プレイヤーの操作を受け付けないように
        playerGameObj.GetComponent<Player>().is_Gameclear = true;
        
        is_gameClear = true;
        
        forceDirection = playerGameObj.GetComponent<RectTransform>().anchoredPosition - startPosition;
        forceMagnitude = playerGameObj.GetComponent<Rigidbody2D>().mass;
        forceMagnitude *= 10f;
        // カメラを回転してプレイヤーの向きを正面にする
        float playerZRotation = playerGameObj.GetComponent<RectTransform>().localRotation.eulerAngles.z;
        Vector3 cameraRotation = Camera.main.transform.rotation.eulerAngles;
        cameraRotation.z = playerZRotation;
        
        //Camera.main.transform.DORotate(cameraRotation, duration, RotateMode.FastBeyond360);  
        ClearUI.SetActive(true);
    }

    public void PushRestartButton()
    {
        //Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
