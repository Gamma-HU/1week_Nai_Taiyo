using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCaluculateLib
{
    public Vector2 SpawnRandomPos(Vector2 playerPos, float radius_min, float radius_max)
    {
        Vector2 spwanPos = new Vector2();
        float angel = Random.Range(0, 360f);
        float radius = Random.Range(radius_min, radius_max);
        spwanPos.x = playerPos.x + (radius * Mathf.Sin(angel));
        spwanPos.y = playerPos.y + (radius * Mathf.Cos(angel));
        return spwanPos;
    }
    
    /// <summary>
    /// 重み付き乱択
    /// </summary>
    /// <returns>選択された要素のインデックス</returns>
    public int Choose(float[] weights)
    {
        float _totalWeight = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            _totalWeight += weights[i];
        }
        
        // 0～重みの総和の範囲の乱数値取得
        var randomPoint = UnityEngine.Random.Range(0, _totalWeight);

        // 乱数値が属する要素を先頭から順に選択
        var currentWeight = 0f;
        for (var i = 0; i < weights.Length; i++)
        {
            // 現在要素までの重みの総和を求める
            currentWeight += weights[i];

            // 乱数値が現在要素の範囲内かチェック
            if (randomPoint < currentWeight)
            {
                return i;
            }
        }

        // 乱数値が重みの総和以上なら末尾要素とする
        return weights.Length - 1;
    }
}
