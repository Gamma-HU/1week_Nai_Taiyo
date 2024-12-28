using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PartSpawner : MonoBehaviour
{
    public static PartSpawner instance;

    [SerializeField] Transform floatingPartFolder;
    [SerializeField] public List<Part> floatingPartList = new List<Part>();
    
    [Header("ゴールまでの距離")] [SerializeField] private float max_distance = 1000f; //ゴールまでの距離
    [Header("Wave1で1回のスポーンで湧くパーツの数")] [SerializeField] private int basePartsNum = 8;
    [Header("スポーンの間隔")] [SerializeField] private float partsSpawnInterval = 0.5f; // スポーン間隔
    [Header("Wave進行による難易度上昇率")]　[SerializeField] private float difficultyScalingFactor = 0.75f;
    [Header("Waveの数")]　[SerializeField] private int maxWaves = 10;

    private SpawnCaluculateLib _spawnCaluculateLib;
    private GameObject playerGameObj;
    public int currentWave = 1;
    private float timeSinceLastSpawn = 0f;
    private bool isSpawning = false;
    private float current_distance = 0f; // スタートから現在地までの距離. この値に応じてcurrentWaveを加算
    private float radius_min;
    private float radius_max;
    
    private float[] current_weights;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        _spawnCaluculateLib = new SpawnCaluculateLib();
    }

    void Start()
    {
        foreach (Part part in floatingPartFolder.GetComponentsInChildren<Part>())
        {
            floatingPartList.Add(part);
        }
        playerGameObj = GameObject.FindGameObjectWithTag("Player");
    }
    
    private void Update()
    {
        if (!isSpawning) return;
        
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= partsSpawnInterval)
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
        return Mathf.RoundToInt(basePartsNum * Mathf.Pow(currentWave, difficultyScalingFactor));
    }
    
    [ContextMenu("Spawn")]
    public void Spawn()
    {
        int numberToSpawn = EnemiesPerWave();
        //Debug.Log("Spawning " + numberToSpawn + " enemies");
        
        // カメラの範囲外に敵をスポーンする
        Vector3 camera_bottomLeft = Camera.main.ViewportToWorldPoint(Vector2.zero);
        Vector3 camera_topRight = Camera.main.ViewportToWorldPoint(Vector2.one);
        radius_min = Vector3.Distance(camera_bottomLeft, camera_topRight) / 2f;
        radius_max = radius_min * 1.2f;

        current_weights = PartDataScritableObject.Entity.weightListPerWave[currentWave - 1].weights;
        
        for (int i = 0; i < numberToSpawn; i++)
        {
            int partIndex_to_spawn = _spawnCaluculateLib.Choose(current_weights);
            Debug.Log(partIndex_to_spawn);
            var partData_to_spawn = PartDataScritableObject.Entity.partDatas[partIndex_to_spawn];
            Vector2 current_playerPos = playerGameObj.GetComponent<RectTransform>().anchoredPosition;
        
            GameObject PartObj = Instantiate(partData_to_spawn.partPrefab, this.gameObject.transform);
            
            Vector2 spawnPos = _spawnCaluculateLib.SpawnRandomPos(current_playerPos, radius_min, radius_max);
            PartObj.GetComponent<RectTransform>().anchoredPosition = spawnPos - this.gameObject.GetComponent<RectTransform>().anchoredPosition;
            PartObj.GetComponent<Part>().Initialize(partData_to_spawn);
            PartObj.GetComponent<Part>().parentObjPos = this.gameObject.GetComponent<RectTransform>().anchoredPosition;
            
            AddFloatingPart(PartObj.GetComponent<Part>());
            //Debug.Log($"playerPos: {playerGameObj.GetComponent<RectTransform>().anchoredPosition}");
            //Debug.Log($"spawnPos: {spawnPos}");
        }
    }

    public void AddFloatingPart(Part part)
    {
        if (floatingPartList.Contains(part))
        {
            return;
        }
        floatingPartList.Add(part);
        part.transform.SetParent(floatingPartFolder);
    }

    public void RemoveFloatingPart(Part part)
    {
        floatingPartList.Remove(part);
    }
}
