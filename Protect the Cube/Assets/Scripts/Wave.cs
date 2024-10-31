using System.Collections.Generic;
using UnityEngine;

// stores all information (enemy types, spawn positions, wave spawn delay) as related to a single wave

public class Wave
{
    public int WaveNumber { get; private set; }
    public float Difficulty { get; private set; }
    public List<EnemySpawnInfo> EnemySpawnInfoList { get; private set; }
    public bool IsCustom { get; private set; }

    public Wave(int waveNumber, float difficulty, List<EnemySpawnInfo> enemySpawnInfoList, bool isCustom = false)
    {
        WaveNumber = waveNumber;
        Difficulty = difficulty;
        EnemySpawnInfoList = enemySpawnInfoList;
        IsCustom = isCustom;
    }
}
