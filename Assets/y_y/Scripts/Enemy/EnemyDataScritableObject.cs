using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyDataScritableObject : ScriptableObject
{
    public List<EnemyData> enemyDatas = new List<EnemyData>();
    public WeightListPerWave[] weightListPerWave;

    public EnemyData GetEnemyData(EnemyData.EnemyType type)
    {
        return enemyDatas.Find(enemy => enemy.enemyType == type);
    }

    //MyScriptableObjectが保存してある場所のパス
    public const string PATH = "EnemyData";

    //MyScriptableObjectの実体
    private static EnemyDataScritableObject _entity;

    public static EnemyDataScritableObject Entity
    {
        get
        {
            //初アクセス時にロードする
            if (_entity == null)
            {
                _entity = Resources.Load<EnemyDataScritableObject>(PATH);

                //ロード出来なかった場合はエラーログを表示
                if (_entity == null)
                {
                    Debug.LogError(PATH + " not found");
                }
            }

            return _entity;
        }
    }
}

[System.Serializable]
public class EnemyData
{
    public enum EnemyType
    {
        NORMAL,
        BOSS
    }

    public EnemyType enemyType;
    public GameObject enemyPrefab;
    public string name;
    public int hp;
    public int attack;
    public int score;
    public GameObject bulletPrefab; // 弾のPrefab
    public float bulletSpeed; // 弾の速度
    public float fireRate;     // 発射間隔（秒）
    public float fireCooldown; // 発射タイマー
    [TextArea]
    public string text_explaination;
}

[System.Serializable]
public class WeightListPerWave
{
    public float[] weights;
}