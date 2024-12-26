using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [ContextMenu("Spawn")]
    public void Spawn()
    {
        //適当な位置に試しにスポーン
        
        GameObject EnemyObj = Instantiate(EnemyDataScritableObject.Entity.GetEnemyData(EnemyData.EnemyType.NORMAL).enemyPrefab, new Vector3(0,0,0), Quaternion.identity);
        EnemyObj.GetComponent<Enemy>().Initialize(EnemyDataScritableObject.Entity.GetEnemyData(EnemyData.EnemyType.NORMAL));
    }

}
