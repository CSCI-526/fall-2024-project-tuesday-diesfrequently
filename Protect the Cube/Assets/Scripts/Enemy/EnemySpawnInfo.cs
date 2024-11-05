using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// each wave has a list of EnemySpawnInfo objects (specifies enemy types, spawn delay, spawn points)

public class EnemySpawnInfo
{
    public GameObject EnemyPrefab { get; set; }
    public float SpawnDelay { get; set; }
    public SpawnPoint SpawnPoint { get; set; }

    public EnemySpawnInfo(GameObject enemyPrefab, float spawnDelay, SpawnPoint spawnPoint)
    {
        EnemyPrefab = enemyPrefab;
        SpawnDelay = spawnDelay;
        SpawnPoint = spawnPoint;
    }
}

