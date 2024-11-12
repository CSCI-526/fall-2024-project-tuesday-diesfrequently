using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Enemy, FastEnemy, ShieldEnemy, SpawnerBossEnemy, TankEnemy, RangedEnemy, None
}

public enum SpawnConfig
{
    Opposite, Circle, Spiral, None,
}

[System.Serializable]
public class EnemyInfo
{
    public EnemyType type = EnemyType.Enemy;
    public int amount = 0;
    public float spawnRate = 0.0f;
    public float spawnDelay = 0.0f;
}

[CreateAssetMenu(fileName = "WaveInfo", menuName = "ScriptableObjects/WaveInfo", order = 1)]
public class WaveInfo : ScriptableObject
{
    [SerializeField] public SpawnConfig spawnConfig = SpawnConfig.Circle;
    [SerializeField] public List<EnemyInfo> enemyList;

    [SerializeField] public float spawnDelay = 1.0f;
    [SerializeField] public float spawnRange = 5.0f;
    [SerializeField] public bool randomLocations = true;
}
