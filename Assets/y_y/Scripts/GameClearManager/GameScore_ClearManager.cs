using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening; // DOTweenを使用

public class GameScore_ClearManager : MonoBehaviour
{
    [SerializeField] private float goalDistance = 1000f;
    [SerializeField] private GameObject ClearUI;
    [SerializeField] private TextMeshProUGUI RemainDistanceText;
    private float current_distance = 0f;
    private GameObject playerGameObj;

    [SerializeField] private float duration = 2.0f;
    private float forceMagnitude;
    [Header("スタート地点の座標")][SerializeField] private Vector2 startPosition = new Vector2(0, 0);

    [SerializeField] private TextMeshProUGUI clearText;
    [SerializeField] private TextMeshProUGUI current_scoreText;
    [SerializeField] private TextMeshProUGUI result_scoreText;
    [SerializeField] private float moveDistance = 50f;  // 下方向に動かす距離
    [SerializeField] private float animationDuration = 1f; // アニメーションの長さ
    private bool is_score_anim = false; // スコアのルーレットアニメーション

    [SerializeField] private float Scoretext_animationDuration = 1.5f; // 文字アニメーションの長さ

    [SerializeField] private GameObject buttons;

    private bool is_gameClear = false;
    private Vector2 forceDirection;

    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private PartSpawner partSpawner;
    [SerializeField] private GameObject CanvasUI;

    void Start()
    {
        playerGameObj = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        current_distance = Vector3.Distance(startPosition, playerGameObj.GetComponent<RectTransform>().anchoredPosition);
        float remainDistance = goalDistance - current_distance;
        if (remainDistance <= 0f) remainDistance = 0f;
        RemainDistanceText.text = "GOAL: " + remainDistance.ToString("F0") + "m";
        current_scoreText.text = $"SCORE: {GameManager.instance.score}";
        if (current_distance > goalDistance)
        {
            if (!is_gameClear)
            {
                GameClear();
            }
            
        }

        if (is_score_anim)
        {
            result_scoreText.text = $"SCORE: {Random.Range(100, 1000)}";
        }
    }

    private void FixedUpdate()
    {
        if (is_gameClear)
        {
            // 起点からプレイヤーの方向に力を加え続ける
            //Debug.Log("AddForce");
            playerGameObj.GetComponent<Rigidbody2D>().AddForce(forceDirection.normalized * forceMagnitude);
        }
    }

    [ContextMenu("GameClear")]
    public void GameClear()
    {

        //Time.timeScale = 0f;
        if(enemySpawner == null || partSpawner == null || CanvasUI == null)
        {
            Debug.LogError("スクリプトにenemySpawnerとpartSpawnerとcanvasUIをアタッチしてほしい気持ち(by yy)");
        }
        else
        {
            CanvasUI.SetActive(false);
            enemySpawner.StopSpawn();
            partSpawner.StopSpawn();
            enemySpawner.EliminateAllEnemies();
            partSpawner.EliminateAllParts();
        }

        GameObject[] damageSources = GameObject.FindGameObjectsWithTag("DamageSource");

        // 敵の弾を消す
        foreach (GameObject damageSource in damageSources)
        {
            if (damageSource != null)
            {
                Destroy(damageSource);
            }
        }

        GameObject[] playerBullets = GameObject.FindGameObjectsWithTag("PlayerBullet");

        // 敵の弾を消す
        foreach (GameObject playerBullet in playerBullets)
        {
            if (playerBullets != null)
            {
                Destroy(playerBullet);
            }
        }


        RemainDistanceText.text = "GOAL!";
        current_scoreText.text = $"GOAL!";

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
        clearText.gameObject.SetActive(true);

        AnimateClearText();
    }

    public void PushRestartButton()
    {
        //Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PushTitleButton()
    {
        //Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene");
    }

    [ContextMenu("AnimateClearText")]
    public void AnimateClearText()
    {
        clearText.alpha = 0; // 透明にする
        clearText.rectTransform.anchoredPosition += new Vector2(0, -moveDistance); // 初期位置を下にオフセット

        // アニメーション
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(clearText.DOFade(1f, animationDuration)) // フェードイン
            .Join(clearText.rectTransform.DOAnchorPosY(clearText.rectTransform.anchoredPosition.y + moveDistance, animationDuration)) // 下から上へ移動
            .SetEase(Ease.OutQuad)
            .OnComplete(() => ShowScoreText()); // 緩やかに再生


    }


    private void ShowScoreText()
    {
        result_scoreText.gameObject.SetActive(true);
        StartCoroutine(StartScoreTextAnim());
    }

    IEnumerator StartScoreTextAnim()
    {
        is_score_anim = true;
        yield return new WaitForSeconds(Scoretext_animationDuration);
        is_score_anim = false;

        result_scoreText.text = $"SCORE: {GameManager.instance.score}";

        ShowButtons();
    }

    private void ShowButtons()
    {
        buttons.SetActive(true);
    }
}
