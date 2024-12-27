using System;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPos;
    [SerializeField] private float radius_min = 60f;
    [SerializeField] private float radius_max = 80f; //プレイヤーのパーツから推定するようにする
    [Header("ゴールまでの距離")] [SerializeField] private float max_distance = 1000f; //ゴールまでの距離
    [Header("Wave1で1回のスポーンで湧く敵の数")] [SerializeField] private int baseEnemiesNum = 8;
    [Header("スポーンの間隔")] [SerializeField] private float enemySpawnInterval = 0.5f; // スポーン間隔
    [Header("Wave進行による難易度上昇率")]　[SerializeField] private float difficultyScalingFactor = 0.75f;
    [Header("Waveの数")]　[SerializeField] private int maxWaves = 10;

    private EnemySpawnCaluculateLib _enemySpawnCaluculateLib;
    private int currentWave = 1;
    private float timeSinceLastSpawn = 0f;
    private bool isSpawning = false;
    private float current_distance = 0f; // スタートから現在地までの距離. この値に応じてcurrentWaveを加算
    private Vector3 prev_playerPos;
    private Vector3 current_playerPos;

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

        current_distance = CalDistance();
        if (current_distance >= (max_distance / maxWaves) * (currentWave + 1))
        {
            currentWave++;
        }
    }

    [ContextMenu("StartWave")]
    public void StartWave()
    {
        isSpawning = true;
        prev_playerPos = playerPos.GetComponent<RectTransform>().anchoredPosition;
    }

    private int EnemiesPerWave()
    {
        return Mathf.RoundToInt(baseEnemiesNum * Mathf.Pow(currentWave, difficultyScalingFactor));
    }
    
    [ContextMenu("Spawn")]
    public void Spawn()
    {
        int numberToSpawn = EnemiesPerWave();
        Debug.Log("Spawning " + numberToSpawn + " enemies");
        
        current_playerPos = playerPos.GetComponent<RectTransform>().anchoredPosition;
        radius_min = Vector3.Distance(prev_playerPos, current_playerPos);
        
        for (int i = 0; i < numberToSpawn; i++)
        {
            var enemyData_Normal = EnemyDataScritableObject.Entity.GetEnemyData(EnemyData.EnemyType.NORMAL);
        
            GameObject EnemyObj = Instantiate(enemyData_Normal.enemyPrefab, this.gameObject.transform);
            Vector2 spawnPos = _enemySpawnCaluculateLib.SpawnRandomPos(current_playerPos, radius_min, radius_max);
            EnemyObj.GetComponent<RectTransform>().anchoredPosition = spawnPos - this.gameObject.GetComponent<RectTransform>().anchoredPosition;
            EnemyObj.GetComponent<Enemy>().Initialize(enemyData_Normal);
            
            Debug.Log($"playerPos: {playerPos.GetComponent<RectTransform>().anchoredPosition}");
            Debug.Log($"spawnPos: {spawnPos}");
        }
        
        prev_playerPos = current_playerPos;
    }

    private float CalDistance()
    {
        float dist = Vector3.Distance(this.gameObject.GetComponent<RectTransform>().anchoredPosition, playerPos.GetComponent<RectTransform>().anchoredPosition);
        return dist;
    }

    [ContextMenu("CurrentWave")]
    public void CurrentWave()
    {
        Debug.Log("Current wave: " + currentWave);
    }

}
