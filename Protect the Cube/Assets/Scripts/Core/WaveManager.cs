using System.Collections.Generic;
using UnityEngine;
using System;

public class WaveManager : MonoBehaviour
{
    // NEW VARIABLES
    public const int TOTAL_NUM_ENEMY_TYPES = 5;
    public const float MIN_WAVE_COMPLETION_LENGTH = 2.0f;
    public const float MAX_DISTANCE_FROM_NEXUS = 40.0f;
    public const float MIDDLE_DISTANCE_FROM_NEXUS = 20.0f;
    public const float WAVE_DIFFICULTY = 1.0f;
    public const int STARTING_WAVE_IDX = 1;
    public const int MAX_WAVE_IDX = 10;

    public List<GameObject> EnemyPrefabs { get; private set; }
    public List<int> EnemyCount { get; private set; }
    public List<GameObject> AllEnemyEntities { get; private set; }

    public int wave_index { get; private set; }

    private Dictionary<string, int> _enemyMapping; // Dictionary for Enemy Types
    private List<List<Vector3>> spawnConfigs = new List<List<Vector3>>(); //stores the spawn point configuration
    [SerializeField] private List<Wave> TutorialWaves = new List<Wave>(); //contains waves of spawnevents

    private float _currentWaveLength;
    private Wave _currentWave;

    [SerializeField] private Transform nexus; // reference to Nexus position
    [SerializeField] private Vector3 centralPosition; // central location of the nexus

    private void Awake()
    {
        _enemyMapping = new Dictionary<string, int>()
        {
            { "Enemy", 0 },
            { "FastEnemy", 1 },
            { "ShieldEnemy", 2 },
            { "SpawnerBossEnemy", 3 },
            { "TankEnemy", 4 },
            { "Ranged Enemy", 5 }
        };

        // size of the potential number of dropped inventory items
        EnemyPrefabs = new List<GameObject>();
        EnemyCount = new List<int>();
        AllEnemyEntities = new List<GameObject>();

        // setup inventory list && count
        for (int idx = 0; idx < TOTAL_NUM_ENEMY_TYPES; idx++)
        {
            // add placeholders for upcoming inventory storage
            EnemyPrefabs.Add(null);
            EnemyCount.Add(0);
        }

        _currentWaveLength = 0;
        spawnConfigs = new List<List<Vector3>>();
        wave_index = 0;
    }

    // Start is called before the first frame update
    void Start() {
        InitializeWavePrefabs();
        SetupSpawnConfigs();
        SetupTutorialWaves();
    }

    private void InitializeWavePrefabs()
    {
        // Load all prefabs from Resources/Prefabs/ into GameObject Array
        // MATCHA: should be able to specify deeper into folders in the prefabs, do this once we organize prefabs into folders properly
        GameObject[] allPrefabs = Resources.LoadAll<GameObject>("Prefabs");
        Helper.checkArrayLengthSafe(allPrefabs, "No prefabs found in the Resources / Prefabs / directory.");

        foreach (var prefab in allPrefabs)
        {
            if (prefab == null) continue; // skip if prefab is invalid

            MultiTag prefabMultiTag = prefab.GetComponent<MultiTag>(); // get prefab MultiTag

            if (prefabMultiTag == null) continue; // if the prefab is not valid, go to next prefab
            if (!prefabMultiTag.HasTag("Enemy")) continue; // if the prefab is not enemy, go to next prefab

            int enemyIDX = getEnemyIDX(prefab.name); // Use prefab name or another identifier
            if (enemyIDX >= 0 && enemyIDX < TOTAL_NUM_ENEMY_TYPES)
            {
                EnemyPrefabs[enemyIDX] = prefab;
                EnemyCount[enemyIDX] = 0;
            }

        }
        Debug.Log($"Initialized {EnemyPrefabs.Count} enemy prefabs with the 'Enemy' tag.");
    }

    private void SetupSpawnConfigs()
    {
        // Index spawnConfigs[0]
        List<Vector3> opposite_2_config = InitializeSpawnPoints(2, MAX_DISTANCE_FROM_NEXUS, "opposite"); // creates 2 spawn points opposite from each other

        // spawnConfigs [1]
        List<Vector3> circle_8_config = InitializeSpawnPoints(8, MIDDLE_DISTANCE_FROM_NEXUS, "circle");

        // spawnConfigs [2]
        List<Vector3> spiral_6_config = InitializeSpawnPoints(6, MIDDLE_DISTANCE_FROM_NEXUS, "spiral");

        // Add Vector3 Spawn Positions as Spawn Configs
        spawnConfigs.Add(opposite_2_config);
        spawnConfigs.Add(circle_8_config);
        spawnConfigs.Add(spiral_6_config);

    }

    private List<Vector3> InitializeSpawnPoints(int num_spawn_points, float radius, string pattern)
    {
        List<Vector3> temp_spawn_points = new List<Vector3>();
        for (int i = 0; i < num_spawn_points; i++)
        {
            Vector3 spawnPosition;
            if (pattern == "circle")
            {
                float angle = i * Mathf.PI * 2 / num_spawn_points;
                spawnPosition = centralPosition + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            }
            else if (pattern == "spiral")
            {
                float angle = i * Mathf.PI * 2 / num_spawn_points;
                float distance = radius * i / num_spawn_points;
                spawnPosition = centralPosition + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * distance;
            }
            else if (pattern == "opposite")
            {
                if (num_spawn_points != 2) { Debug.LogWarning("The 'opposite' pattern requires exactly 2 spawn points."); continue;  }
                float angle = i * Mathf.PI;  // 0 and 180 degrees for opposite positions
                spawnPosition = centralPosition + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            }
            else spawnPosition = centralPosition + new Vector3(i * 2.0f, 0, 0); // default line pattern

            // add all spawn points together as a pattern
            temp_spawn_points.Add(spawnPosition);
        }
        return temp_spawn_points;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGlobalWaveTimer();
        if (AllEnemiesKilled() && WaveTimerComplete(_currentWave)) {
            SpawnNextWave();
        }
    }

    private bool WaveTimerComplete(Wave wave) { return _currentWaveLength > MIN_WAVE_COMPLETION_LENGTH; }

    private bool AllEnemiesKilled()
    {
        //foreach (int enemy_count in EnemyCount)
        //{
        //    if (enemy_count < 0) throw new ArgumentException("there is negative amount of enemimes (illegal)");
        //    else if (enemy_count > 0) return false;
        //}
        //return true;

        return AllEnemyEntities.Count == 0;
    }

    public void AddEnemyEntity(GameObject enemy_entity, int enemyIDX)
    {
        Debug.Log("Adding Enemy Entity ... ");
        AllEnemyEntities.Add(enemy_entity);
        EnemyCount[enemyIDX] += 1;
    }

    public void KillEnemyEntity(GameObject enemy_entity, int enemyIDX)
    {
        Debug.Log("Removing Enemy Entity ... ");
        AllEnemyEntities.Remove(enemy_entity);
        EnemyCount[enemyIDX] -= 1;
    }

    private void UpdateGlobalWaveTimer() { _currentWaveLength += Time.deltaTime; }

    private void SpawnNextWave()
    {
        wave_index++; // incrementing wave number
        if (wave_index == MAX_WAVE_IDX) { wave_index = STARTING_WAVE_IDX; } // loop tutorial waves
        Debug.Log("[Starting Wave " + wave_index + " ]...");

        _currentWaveLength = 0.0f; // reset
        Debug.Log("Size of TutorialWaves: :" + TutorialWaves.Count);
        _currentWave = TutorialWaves[wave_index - 1];

        // output each wave event per wave
        foreach (SpawnEvent x in _currentWave.SpawnSchedule) StartCoroutine(x.DelayedSpawn());

        GameManager.Instance.UIManager.UpdateUI();
        GameManager.Instance.AnalyticsManager.UpdateWaveNumber(wave_index);// Send wave number to analytics
        Debug.Log("[Update] EnemyCount: [" + string.Join(", ", EnemyCount) + "]");
    }

    private void SetupTutorialWaves()
    {
        TutorialWaves.Add(SetupOppositeWave());
        TutorialWaves.Add(SetupCircleWave());
        TutorialWaves.Add(SetupSpiralWave());
        TutorialWaves.Add(SetupEscalatingWave());
        TutorialWaves.Add(SetupBossWave());
        TutorialWaves.Add(SetupMultiTypeWave());
        TutorialWaves.Add(SetupTimedWave());
        TutorialWaves.Add(SetupClusterWave());
        TutorialWaves.Add(SetupSpiralCircleWave());
        TutorialWaves.Add(SetupEnduranceWave());
    }

    private Wave SetupOppositeWave()
    {
        var spawnSchedule = new List<SpawnEvent>
        {
            new SpawnEvent("Enemy", 0.0f, spawnConfigs[0][0]),
            new SpawnEvent("Enemy", 3.0f, spawnConfigs[0][1])
        };
        return new Wave(WAVE_DIFFICULTY, spawnSchedule, MIN_WAVE_COMPLETION_LENGTH, true);
    }

    private Wave SetupCircleWave()
    {
        var spawnSchedule = new List<SpawnEvent>();
        for (int i = 0; i < 8; i++)
        {
            spawnSchedule.Add(new SpawnEvent("Enemy", 0.5f * i, spawnConfigs[1][i]));
        }
        return new Wave(WAVE_DIFFICULTY, spawnSchedule, MIN_WAVE_COMPLETION_LENGTH, true);
    }

    private Wave SetupSpiralWave()
    {
        var spawnSchedule = new List<SpawnEvent>();
        for (int i = 0; i < 6; i++)
        {
            spawnSchedule.Add(new SpawnEvent("FastEnemy", 0.3f * i, spawnConfigs[2][i]));
        }
        return new Wave(WAVE_DIFFICULTY, spawnSchedule, MIN_WAVE_COMPLETION_LENGTH, true);
    }

    private Wave SetupEscalatingWave()
    {
        var spawnSchedule = new List<SpawnEvent>();
        for (int i = 0; i < 5; i++)
        {
            spawnSchedule.Add(new SpawnEvent("Enemy", i, spawnConfigs[1][i]));
            spawnSchedule.Add(new SpawnEvent("FastEnemy", i + 1.5f, spawnConfigs[1][(i + 2) % 5]));
        }
        return new Wave(WAVE_DIFFICULTY * 1.5f, spawnSchedule, MIN_WAVE_COMPLETION_LENGTH + 1, true);
    }

    private Wave SetupBossWave()
    {
        var spawnSchedule = new List<SpawnEvent>
        {
            new SpawnEvent("SpawnerBossEnemy", 0.0f, spawnConfigs[0][0])
        };
        return new Wave(WAVE_DIFFICULTY * 2, spawnSchedule, MIN_WAVE_COMPLETION_LENGTH * 2, true);
    }

    private Wave SetupMultiTypeWave()
    {
        var spawnSchedule = new List<SpawnEvent>();
        for (int i = 0; i < 4; i++)
        {
            spawnSchedule.Add(new SpawnEvent("Enemy", i, spawnConfigs[1][i]));
            spawnSchedule.Add(new SpawnEvent("ShieldEnemy", i + 1, spawnConfigs[1][(i + 2) % 4]));
        }
        return new Wave(WAVE_DIFFICULTY * 1.8f, spawnSchedule, MIN_WAVE_COMPLETION_LENGTH + 2, true);
    }

    private Wave SetupTimedWave()
    {
        var spawnSchedule = new List<SpawnEvent>();
        for (int i = 0; i < 5; i++)
        {
            spawnSchedule.Add(new SpawnEvent("FastEnemy", i * 0.5f, spawnConfigs[2][i]));
        }
        return new Wave(WAVE_DIFFICULTY * 1.5f, spawnSchedule, MIN_WAVE_COMPLETION_LENGTH + 1, true);
    }

    private Wave SetupClusterWave()
    {
        var spawnSchedule = new List<SpawnEvent>
        {
            new SpawnEvent("Enemy", 0.0f, spawnConfigs[1][0]),
            new SpawnEvent("Enemy", 0.0f, spawnConfigs[1][1]),
            new SpawnEvent("FastEnemy", 0.5f, spawnConfigs[1][2]),
            new SpawnEvent("FastEnemy", 0.5f, spawnConfigs[1][3])
        };
        return new Wave(WAVE_DIFFICULTY * 1.7f, spawnSchedule, MIN_WAVE_COMPLETION_LENGTH + 1.5f, true);
    }

    private Wave SetupSpiralCircleWave()
    {
        var spawnSchedule = new List<SpawnEvent>();
        for (int i = 0; i < 4; i++)
        {
            spawnSchedule.Add(new SpawnEvent("Enemy", 0.4f * i, spawnConfigs[2][i]));
            spawnSchedule.Add(new SpawnEvent("ShieldEnemy", 0.5f * i, spawnConfigs[1][i]));
        }
        return new Wave(WAVE_DIFFICULTY * 2.0f, spawnSchedule, MIN_WAVE_COMPLETION_LENGTH + 2, true);
    }

    private Wave SetupEnduranceWave()
    {
        var spawnSchedule = new List<SpawnEvent>();
        for (int i = 0; i < 5; i++)
        {
            spawnSchedule.Add(new SpawnEvent("TankEnemy", 1.5f * i, spawnConfigs[1][i]));
        }
        return new Wave(WAVE_DIFFICULTY * 2.5f, spawnSchedule, MIN_WAVE_COMPLETION_LENGTH * 2.5f, true);
    }

    // get IDX of enemy given a enemyName
    public int getEnemyIDX(string enemyName) => _enemyMapping.TryGetValue(enemyName, out int index) ? index : -1;
}