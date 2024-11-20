using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    // NEW VARIABLES
    [Header("PUBLIC Modifiable Constants")]
    public const float MIN_WAVE_COMPLETION_LENGTH = 2.0f;
    public const float MAX_DISTANCE_FROM_NEXUS = 40.0f;
    public const float MIDDLE_DISTANCE_FROM_NEXUS = 20.0f;
    public const float WAVE_DIFFICULTY = 1.0f;
    [SerializeField] public List<GameObject> EnemyPrefabs = new List<GameObject>();
    [SerializeField] public List<WaveInfo> Waves = new List<WaveInfo>();
    [SerializeField] public WaveInfo dynamicWaveInfo = null;

    [SerializeField] public List<int> EnemyCounter = new List<int>();
    [SerializeField] public List<GameObject> AllEnemyEntities;
    [SerializeField] public int wave_index;
    [SerializeField] public int wave_count;

    private List<List<Vector3>> spawnConfigs = new List<List<Vector3>>(); //stores the spawn point configuration
    private float _currentWaveLength;

    [Header("PRIVATE Members")]
    [SerializeField] private Transform nexus; // reference to Nexus position
    [SerializeField] private Vector3 centralPosition; // central location of the nexus

    private void Awake()
    {
        // size of the potential number of dropped inventory items
        EnemyCounter = new List<int>();
        AllEnemyEntities = new List<GameObject>();
        while (EnemyCounter.Count < EnemyPrefabs.Count) //initialize enemy counter
        {
            EnemyCounter.Add(0);
        }

        _currentWaveLength = 0;
        spawnConfigs = new List<List<Vector3>>();
        wave_index = 0;
        wave_count = 0;
    }

    // Start is called before the first frame update
    void Start() {
        SetupSpawnConfigs();
    }

    private void SetupSpawnConfigs()
    {
        // Index spawnConfigs[0]
        List<Vector3> opposite_2_config = InitializeSpawnPoints(2, MAX_DISTANCE_FROM_NEXUS, "opposite"); // creates 2 spawn points opposite from each other

        // spawnConfigs [1]
        List<Vector3> circle_8_config = InitializeSpawnPoints(8, MAX_DISTANCE_FROM_NEXUS, "circle");

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

    public void SpawnSingleEnemy()
    {
        Vector3 spawnPosition = spawnConfigs[0][UnityEngine.Random.Range(0, spawnConfigs[0].Count)];

        // Instantiate the enemy at the selected spawn point
        string enemyName = "Enemy";
        GameObject enemyPrefab = GetEnemyPrefab(enemyName);
        GameObject enemyEntity = Instantiate(enemyPrefab);
        AddEnemyEntity(enemyEntity, GetEnemyIDX(enemyName));

        // MATCHA: Hard Coded
        int configID = 0;
        float spawnRange = 2.0f;
        Vector3 location = spawnConfigs[configID][UnityEngine.Random.Range(0, spawnConfigs[configID].Count - 1)];
        enemyEntity.transform.position = location + new Vector3(UnityEngine.Random.Range(-spawnRange, spawnRange), 0, UnityEngine.Random.Range(-spawnRange, spawnRange));
        if (GameManager.Instance.DEBUG_WAVE_MANAGER) Debug.Log("[Update] EnemyCount: [" + string.Join(", ", EnemyCounter) + "]");
    }

    public void LockAllEnemiesMovement()
    {
        if (GameManager.Instance.DEBUG_WAVE_MANAGER) Debug.Log("[Wave Manager] Locking All Enemies Movement");
        foreach (var enemy in AllEnemyEntities)
        {
            if (enemy != null)
            {
                EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
                if (enemyMove != null) enemyMove.LockMovement();
            }
        }
    }

    public void UnlockAllEnemiesMovement()
    {
        if (GameManager.Instance.DEBUG_WAVE_MANAGER) Debug.Log("[Wave Manager] Unlocking All Enemies Movement");
        foreach (var enemy in AllEnemyEntities)
        {
            if (enemy != null)
            {
                EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
                if (enemyMove != null) enemyMove.UnlockMovement();
            }
        }
    }

    public void SetConstantXPDrops(int exp_amount)
    {
        if (GameManager.Instance.DEBUG_WAVE_MANAGER) Debug.Log("[Wave Manager] Setting All XP Drops to Constant");
        foreach (var enemy in AllEnemyEntities)
        {
            if (enemy != null)
            {
                EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                if (enemyHealth != null) enemyHealth.ActivateTutorialEXPDrop(exp_amount);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.CurrentPhase == GameManager.GamePhase.HandCraftedWaves || 
            GameManager.Instance.CurrentPhase == GameManager.GamePhase.DynamicWaves)
        {
            UpdateGlobalWaveTimer();
            if (AllEnemiesKilled() && WaveTimerComplete())
            {
                SpawnNextWave();
            }
        }
        
    }

    private void UpdateGlobalWaveTimer() { _currentWaveLength += Time.deltaTime; }

    private void SpawnNextWave()
    {
        if(GameManager.Instance.CurrentPhase == GameManager.GamePhase.DynamicWaves)
        {
            SpawnDynamicWave(dynamicWaveInfo); //generates custom wave
        }
        else //normal behavior (uses custom waves)
        {
            WaveInfo waveInfo = Waves[wave_index];
            Debug.Log("[Starting Wave " + wave_index + " ]...");

            // output each wave event per wave
            foreach (EnemyInfo enemy in waveInfo.enemyList)
            {
                for (int i = 0; i < enemy.amount; ++i)
                {
                    if (waveInfo.randomLocations)
                    {
                        StartCoroutine(DelayedSpawn(waveInfo, enemy));
                    }
                    else
                    {
                        StartCoroutine(DelayedSpawn(waveInfo, enemy, i));
                    }
                }
            }

            wave_index++; // incrementing wave number
            wave_count++;
            _currentWaveLength = 0;
            if (wave_index >= Waves.Count) //swap to dynamic wave system
            {
                GameManager.Instance.SetGamePhase(GameManager.GamePhase.DynamicWaves);
                //wave_index = Waves.Count - 1;
            }

            GameManager.Instance.UIManager.UpdateUI();
            GameManager.Instance.AnalyticsManager.UpdateWaveNumber(wave_index);// Send wave number to analytics
        }
    }

    private void SpawnDynamicWave(WaveInfo waveInfo = null)
    {
        if(waveInfo == null)
        {
            waveInfo = Waves[Waves.Count - 1]; //use last wave as base stats if missing
        }
        Debug.Log("[Starting Dynamic Wave " + wave_index + " ]...");

        // output each wave event per wave
        foreach (EnemyInfo enemy in waveInfo.enemyList)
        {
            for (int i = 0; i < wave_count; ++i) //make spawns scale with wave length
            {
                if(UnityEngine.Random.Range(0, 100) <= enemy.spawnRate * 100.0f)
                {
                    if (waveInfo.randomLocations)
                    {
                        StartCoroutine(DelayedSpawn(waveInfo, enemy));
                    }
                    else
                    {
                        StartCoroutine(DelayedSpawn(waveInfo, enemy, i));
                    }
                }
            }
        }

        wave_index++; // incrementing wave number
        wave_count++;
        _currentWaveLength = 0;

        GameManager.Instance.UIManager.UpdateUI();
        GameManager.Instance.AnalyticsManager.UpdateWaveNumber(wave_index);// Send wave number to analytics

    }

    private IEnumerator DelayedSpawn(WaveInfo wave, EnemyInfo enemy, int spawnLocationID = -1)// Vector3 location, float delay)
    {
        float delay = UnityEngine.Random.Range(0, wave.spawnDelay) + enemy.spawnDelay;
        float spawnRange = wave.spawnRange;
        yield return new WaitForSeconds(delay);

        String enemyName = GetEnemyName(enemy);
        GameObject enemyPrefab = GetEnemyPrefab(enemyName);

        GameObject enemyEntity = Instantiate(enemyPrefab);
        AddEnemyEntity(enemyEntity, GetEnemyIDX(enemyName));

        int configID = (int)wave.spawnConfig;
        Vector3 location;
        if(spawnLocationID == -1) //choose spawn location randomly
        {
            location = spawnConfigs[configID][UnityEngine.Random.Range(0, spawnConfigs[configID].Count - 1)];
        }
        else //choose locations sequentially
        {
            location = spawnConfigs[configID][spawnLocationID % (spawnConfigs[configID].Count - 1)];
        }
        enemyEntity.transform.position = location + new Vector3(UnityEngine.Random.Range(-spawnRange,spawnRange), 0, UnityEngine.Random.Range(-spawnRange, spawnRange));
        if (GameManager.Instance.DEBUG_WAVE_MANAGER) Debug.Log("[Update] EnemyCount: [" + string.Join(", ", EnemyCounter) + "]");
    }

    //Used to get enemy names using the EnemyType enum
    public String GetEnemyName(EnemyInfo enemyInfo)
    {
        EnemyType type = enemyInfo.type;
        switch (type)
        {
            case EnemyType.Enemy: return "Enemy";
            case EnemyType.FastEnemy: return "FastEnemy";
            case EnemyType.ShieldEnemy: return "ShieldEnemy";
            case EnemyType.SpawnerBossEnemy: return "SpawnerBossEnemy";
            case EnemyType.TankEnemy: return "TankEnemy";
            case EnemyType.RangedEnemy: return "Ranged Enemy";
            default: return "Unknown";
        }
    }

    // get IDX of enemy given an enemyName
    public int GetEnemyIDX(string enemyName)
    {
        for(int i = 0; i < EnemyPrefabs.Count; ++i)
        {
            if (EnemyPrefabs[i].name == enemyName) return i;
        }
        Debug.Log("Warning: " + enemyName + " was not found in the list of enemy prefabs!");
        return -1;
    }

    // get prefab of an enemy given an enemyName
    public GameObject GetEnemyPrefab(string enemyName)
    {
        for (int i = 0; i < EnemyPrefabs.Count; ++i)
        {
            if (EnemyPrefabs[i].name == enemyName) return EnemyPrefabs[i];
        }
        Debug.Log("Warning: " + enemyName + " was not found in the list of enemy prefabs!");
        return null;
    }
    private bool WaveTimerComplete()
    {
        return _currentWaveLength > MIN_WAVE_COMPLETION_LENGTH;
    }

    public bool AllEnemiesKilled()
    {
        return AllEnemyEntities.Count == 0;
    }

    public void AddEnemyEntity(GameObject enemy_entity, int enemyIDX)
    {
        if (GameManager.Instance.DEBUG_WAVE_MANAGER) Debug.Log("Adding Enemy Entity ... ");
        AllEnemyEntities.Add(enemy_entity);
        EnemyCounter[enemyIDX] += 1;
    }

    public void KillEnemyEntity(GameObject enemy_entity, int enemyIDX)
    {
        if (GameManager.Instance.DEBUG_WAVE_MANAGER) Debug.Log("Removing Enemy Entity ... ");
        AllEnemyEntities.Remove(enemy_entity);
        EnemyCounter[enemyIDX] -= 1;
    }
}