using System;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPos;
    [SerializeField] private float radius_min = 60f;
    [SerializeField] private float radius_max = 80f;
    [SerializeField] private float max_distance = 1000f;
    [SerializeField] private int baseEnemiesNum = 8;
    [SerializeField] private float enemiesPerSecond = 0.5f;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float difficultyScalingFactor = 0.75f;

    [Header("Events")] public static UnityEvent onEnemyDestroy = new UnityEvent();

    private EnemySpawnCaluculateLib _enemySpawnCaluculateLib;
    private int currentWave = 0;
    private float timeSinceLastSpawn = 0f;
    private int enemyAlive;
    //private int enemiesLeftToSpawn;
    private bool isSpawning = false;

    private void Awake()
    {
        _enemySpawnCaluculateLib = new EnemySpawnCaluculateLib();
        onEnemyDestroy.AddListener(EnemyDestroyed);
    }

    private void Update()
    {
        if (!isSpawning) return;
        
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= (1f / enemiesPerSecond))
        {
            Spawn();
            enemyAlive++;
            timeSinceLastSpawn = 0f;
        }
    }

    private void StartWave()
    {
        isSpawning = true;
        enemiesLeftToSpawn = EnemiesPerWave();
    }

    private int EnemiesPerWave()
    {
        return Mathf.RoundToInt(baseEnemiesNum * Mathf.Pow(currentWave, difficultyScalingFactor));
    }

    private void EnemyDestroyed()
    {
        enemyAlive--;
    }
    
    [ContextMenu("Spawn")]
    public void Spawn()
    {
        var enemyData_Normal = EnemyDataScritableObject.Entity.GetEnemyData(EnemyData.EnemyType.NORMAL);
        
        GameObject EnemyObj = Instantiate(enemyData_Normal.enemyPrefab, this.gameObject.transform);
        Vector2 spawnPos = _enemySpawnCaluculateLib.SpawnRandomPos(playerPos.transform, radius_min, radius_max);
        Debug.Log(spawnPos);
        EnemyObj.GetComponent<RectTransform>().anchoredPosition = spawnPos;
        
        EnemyObj.GetComponent<Enemy>().Initialize(enemyData_Normal);
    }

    [ContextMenu("TestCalDistance")]
    public void TestCalDistance()
    {
        float dist = Vector3.Distance(this.gameObject.GetComponent<RectTransform>().anchoredPosition, playerPos.GetComponent<RectTransform>().anchoredPosition);
        Debug.Log(dist);
    }

}
