using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("ゴールまでの距離")] [SerializeField] private float max_distance = 1000f; //ゴールまでの距離
    [Header("Wave1で1回のスポーンで湧く敵の数")] [SerializeField] private int baseEnemiesNum = 8;
    [Header("スポーンの間隔")] [SerializeField] private float enemySpawnInterval = 0.5f; // スポーン間隔
    [Header("Wave進行による難易度上昇率")]　[SerializeField] private float difficultyScalingFactor = 0.75f;
    [Header("Waveの数")]　[SerializeField] private int maxWaves = 10;
    [Header("スタート地点の座標")]　[SerializeField] private Vector3 startPosition = new Vector3(0, 0, 0);
    [Header("Enemyの湧き上限数")]　[SerializeField] private int maxEnemiesNum = 50;
    
    private SpawnCaluculateLib _spawnCaluculateLib;
    private GameObject playerGameObj;
    public int currentWave = 1;
    private float timeSinceLastSpawn = 0f;
    private bool isSpawning = false;
    private float current_distance = 0f; // スタートから現在地までの距離. この値に応じてcurrentWaveを加算

    private Camera _camera;
    private float diagonalDistance_Camera; // カメラの対角線の長さ. カメラの範囲外に敵をスポーンするために必要
    private float radius_min;
    private float radius_max;
    
    private float[] current_weights;
    public List<GameObject> spawnedEnemies = new List<GameObject>();

    private void Awake()
    {
        _spawnCaluculateLib = new SpawnCaluculateLib();
    }

    private void Start()
    {
        playerGameObj = GameObject.FindGameObjectWithTag("Player");
        _camera = Camera.main;
        Vector3 camera_bottomLeft = _camera.ViewportToWorldPoint(Vector2.zero);
        Vector3 camera_topRight = _camera.ViewportToWorldPoint(Vector2.one);
        diagonalDistance_Camera = Vector3.Distance(camera_bottomLeft, camera_topRight);
        radius_min = diagonalDistance_Camera / 2f;
        radius_max = radius_min * 1.2f;
    }

    private void Update()
    {
        if (!isSpawning) return;
        
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= enemySpawnInterval)
        {
            Spawn();
            timeSinceLastSpawn = 0f;
        }

        current_distance = Vector3.Distance(startPosition, playerGameObj.GetComponent<RectTransform>().anchoredPosition);;
        if (current_distance >= (max_distance / maxWaves) * (currentWave + 1))
        {
            currentWave++;
        }

        EliminateOutOfRangeEnemies();
    }

    [ContextMenu("StartWave")]
    public void StartWave()
    {
        isSpawning = true;
    }

    private int EnemiesPerWave()
    {
        return Mathf.RoundToInt(baseEnemiesNum * Mathf.Pow(currentWave, difficultyScalingFactor));
    }
    
    [ContextMenu("Spawn")]
    public void Spawn()
    {
        int numberToSpawn = EnemiesPerWave();
        //Debug.Log("Spawning " + numberToSpawn + " enemies");
        
        if (spawnedEnemies.Count + + numberToSpawn >= maxEnemiesNum) return; //上限数を超えていたらスポーンしない
        
        // カメラの範囲外に敵をスポーンする
        Vector3 camera_bottomLeft = _camera.ViewportToWorldPoint(Vector2.zero);
        Vector3 camera_topRight = _camera.ViewportToWorldPoint(Vector2.one);
        diagonalDistance_Camera = Vector3.Distance(camera_bottomLeft, camera_topRight);
        radius_min = diagonalDistance_Camera / 2f;
        radius_max = radius_min * 1.2f;

        current_weights = EnemyDataScritableObject.Entity.weightListPerWave[currentWave - 1].weights;
        
        for (int i = 0; i < numberToSpawn; i++)
        {
            int enemyIndex_to_spawn = _spawnCaluculateLib.Choose(current_weights);
            //Debug.Log(enemyIndex_to_spawn);
            var enemyData_to_spawn = EnemyDataScritableObject.Entity.enemyDatas[enemyIndex_to_spawn];
            Vector2 current_playerPos = playerGameObj.GetComponent<RectTransform>().anchoredPosition;
        
            GameObject EnemyObj = Instantiate(enemyData_to_spawn.enemyPrefab, this.gameObject.transform);
            
            Vector2 spawnPos = _spawnCaluculateLib.SpawnRandomPos(current_playerPos, radius_min, radius_max);
            EnemyObj.GetComponent<RectTransform>().anchoredPosition = spawnPos;
            EnemyObj.GetComponent<Enemy>().Initialize(enemyData_to_spawn);


            AddEnemiesList(EnemyObj);
            //Debug.Log($"playerPos: {playerGameObj.GetComponent<RectTransform>().anchoredPosition}");
            //Debug.Log($"spawnPos: {spawnPos}");
        }
    }


    private void AddEnemiesList(GameObject enemy)
    {
        spawnedEnemies.Add(enemy);
    }

    private void RemoveEnemiesList(GameObject enemy)
    {
        spawnedEnemies.Remove(enemy);
    }
    
    // スポーンした敵とプレイヤーの距離を計算し、一定距離(適当にカメラの対角線の距離とする.)離れているものに関してはDestroyし、リストから取り除く
    private void EliminateOutOfRangeEnemies()
    {
        List<GameObject> tempList = new List<GameObject>(spawnedEnemies);
        
        float distance = 0f;
        foreach (GameObject enemy in tempList)
        {
            distance = Vector3.Distance(enemy.GetComponent<RectTransform>().anchoredPosition, playerGameObj.GetComponent<RectTransform>().anchoredPosition);
            if (distance > radius_max)
            {
                RemoveEnemiesList(enemy);
                Destroy(enemy);
            }
        }
    }

    public void EliminateAllEnemies()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            Destroy(enemy);
        }
        
        spawnedEnemies.Clear();
    }
}
