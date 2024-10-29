using System.Collections.Generic;
using UnityEngine;

// stores all information as related to a single wave

public class Wave
{
    public int WaveNumber { get; private set; }
    public float Difficulty { get; private set; }
    public List<EnemySpawnInfo> EnemySpawnInfoList { get; private set; }
    public bool IsHandcrafted { get; private set; }

    public Wave(int waveNumber, float difficulty, List<EnemySpawnInfo> enemySpawnInfoList, bool isHandcrafted = false)
    {
        WaveNumber = waveNumber;
        Difficulty = difficulty;
        EnemySpawnInfoList = enemySpawnInfoList;
        IsHandcrafted = isHandcrafted;
    }
}
