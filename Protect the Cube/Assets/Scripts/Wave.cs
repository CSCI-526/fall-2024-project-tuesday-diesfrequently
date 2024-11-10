using System.Collections.Generic;
using UnityEngine;

// stores all information (enemy types, spawn positions, wave spawn delay) as related to a single wave

public class Wave
{
    public int WaveNumber { get; private set; }
    public float Difficulty { get; private set; }
    public List<SpawnEvent> SpawnSchedule { get; private set; }
    public float WaveDuration { get; private set; }
    public bool IsCustom { get; private set; }

    public Wave(int wave_num, float difficulty, List<SpawnEvent> spawn_schedule, float wave_duration, bool isCustom = false)
    {
        WaveNumber = wave_num;
        Difficulty = difficulty;
        SpawnSchedule = spawn_schedule;
        WaveDuration = wave_duration;
        IsCustom = isCustom;
    }
}