using System.Collections.Generic;
using UnityEngine;

// stores all information (enemy types, spawn positions, wave spawn delay) as related to a single wave

public class Wave
{
    public float Difficulty { get; private set; }
    [SerializeField] public List<SpawnEvent> SpawnSchedule { get; private set; }

    [SerializeField] public List<GameObject> SpawnEvents;
    public float WaveDuration { get; private set; }
    public bool IsCustom { get; private set; }

    public Wave(float difficulty, List<SpawnEvent> spawn_schedule, float wave_duration, bool isCustom = false)
    {
        Difficulty = difficulty;
        SpawnSchedule = spawn_schedule;
        WaveDuration = wave_duration;
        IsCustom = isCustom;
    }

    // adjust as needed
    public void UpdateConfiguration(float newDifficulty, float newWaveDuration)
    {
        Difficulty = newDifficulty;
        WaveDuration = newWaveDuration;
    }
}