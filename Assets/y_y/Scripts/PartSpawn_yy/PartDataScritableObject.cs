using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class PartDataScritableObject : ScriptableObject
{
    public List<PartData> partDatas = new List<PartData>();
    public WeightListPerWave_Parts[] weightListPerWave;

    public PartData GetPartData(PartData.PartType type)
    {
        return partDatas.Find(enemy => enemy.partType == type);
    }

    //MyScriptableObjectが保存してある場所のパス
    public const string PATH = "PartData";

    //MyScriptableObjectの実体
    private static PartDataScritableObject _entity;

    public static PartDataScritableObject Entity
    {
        get
        {
            //初アクセス時にロードする
            if (_entity == null)
            {
                _entity = Resources.Load<PartDataScritableObject>(PATH);

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
public class PartData
{
    public enum PartType
    {
        NORMAL,
        BOSS
    }

    public PartType partType;
    public GameObject partPrefab;
    public string name;
    public int hp;
    [TextArea]
    public string text_explaination;
}

[System.Serializable]
public class WeightListPerWave_Parts
{
    public float[] weights;
}