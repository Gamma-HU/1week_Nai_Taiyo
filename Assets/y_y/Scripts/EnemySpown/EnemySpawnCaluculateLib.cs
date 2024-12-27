using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnCaluculateLib
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
}
