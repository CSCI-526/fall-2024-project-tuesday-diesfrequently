using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// each wave has a list of EnemySpawnInfo objects (specifies enemy types, spawn delay, spawn points)

public class SpawnEvent
{
    public GameObject SpawnedEnemyPrefab { get; private set; }
    public float SpawnDelay { get; private set; }
    public SpawnPoint SpawnPoint { get; private set; }

    public SpawnEvent(GameObject prefab, float spawn_delay, SpawnPoint spawn_point)
    {
        SpawnedEnemyPrefab = prefab;
        SpawnDelay = spawn_delay;
        SpawnPoint = spawn_point;
    }
}

