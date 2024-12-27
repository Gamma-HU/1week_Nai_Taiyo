using System;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerGameObj;
    [Header("ゴールまでの距離")] [SerializeField] private float max_distance = 1000f; //ゴールまでの距離
    [Header("Wave1で1回のスポーンで湧く敵の数")] [SerializeField] private int baseEnemiesNum = 8;
    [Header("スポーンの間隔")] [SerializeField] private float enemySpawnInterval = 0.5f; // スポーン間隔
    [Header("Wave進行による難易度上昇率")]　[SerializeField] private float difficultyScalingFactor = 0.75f;
    [Header("Waveの数")]　[SerializeField] private int maxWaves = 10;

    private EnemySpawnCaluculateLib _enemySpawnCaluculateLib;
    public int currentWave = 1;
    private float timeSinceLastSpawn = 0f;
    private bool isSpawning = false;
    private float current_distance = 0f; // スタートから現在地までの距離. この値に応じてcurrentWaveを加算
    private float radius_min;
    private float radius_max;
    
    private float[] current_weights;

    private void Awake()
    {
        _enemySpawnCaluculateLib = new EnemySpawnCaluculateLib();
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

        current_distance = Vector3.Distance(this.gameObject.GetComponent<RectTransform>().anchoredPosition, playerGameObj.GetComponent<RectTransform>().anchoredPosition);;
        if (current_distance >= (max_distance / maxWaves) * (currentWave + 1))
        {
            currentWave++;
        }
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
        
        // カメラの範囲外に敵をスポーンする
        Vector3 camera_bottomLeft = Camera.main.ViewportToWorldPoint(Vector2.zero);
        Vector3 camera_topRight = Camera.main.ViewportToWorldPoint(Vector2.one);
        radius_min = Vector3.Distance(camera_bottomLeft, camera_topRight);
        radius_max = radius_min * 1.5f;

        current_weights = EnemyDataScritableObject.Entity.weightListPerWave[currentWave - 1].weights;
        
        for (int i = 0; i < numberToSpawn; i++)
        {
            int enemyIndex_to_spawn = _enemySpawnCaluculateLib.Choose(current_weights);
            //Debug.Log(enemyIndex_to_spawn);
            var enemyData_to_spawn = EnemyDataScritableObject.Entity.enemyDatas[enemyIndex_to_spawn];
            Vector2 current_playerPos = playerGameObj.GetComponent<RectTransform>().anchoredPosition;
        
            GameObject EnemyObj = Instantiate(enemyData_to_spawn.enemyPrefab, this.gameObject.transform);
            Vector2 spawnPos = _enemySpawnCaluculateLib.SpawnRandomPos(current_playerPos, radius_min, radius_max);
            EnemyObj.GetComponent<RectTransform>().anchoredPosition = spawnPos - this.gameObject.GetComponent<RectTransform>().anchoredPosition;
            EnemyObj.GetComponent<Enemy>().Initialize(enemyData_to_spawn);
            
            //Debug.Log($"playerPos: {playerGameObj.GetComponent<RectTransform>().anchoredPosition}");
            //Debug.Log($"spawnPos: {spawnPos}");
        }
    }

}
