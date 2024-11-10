using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class WaveManager : MonoBehaviour
{
    [SerializeField] protected float waveInterval = 5.0f;
    [SerializeField] protected float maxSpawnDelay = 2.0f;

    [SerializeField] public int wave = 0;
    [SerializeField] public float difficulty = 1.2f;

    [SerializeField] public float tankRate = 0.8f;
    [SerializeField] public GameObject tank;
    [SerializeField] public int tankSpawnStartWave = 5;

    [SerializeField] public GameObject spawnerBoss;
    [SerializeField] public int spawnerBossStartWave = 10;

    [SerializeField] public GameObject shieldBoss;

    [SerializeField] public List<GameObject> enemies = new List<GameObject>();
    [SerializeField] public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
    float timeSinceLastWave;

    // new variables
    private Dictionary<string, int> _enemyMapping; // Dictionary for Enemy Types
    public const int TOTAL_NUM_ENEMY_TYPES = 5;
    private List<GameObject> enemyPrefabs;
    private List<int> enemyCount;
    private List<Wave> tutorialWaves;
    private float _globalWaveTimer;
    private Wave _currentWave;
    private List<GameObject> spawnPointList = new List<GameObject>();

    private void Awake()
    {
        _enemyMapping = new Dictionary<string, int>()
        {
            { "Enemy", 0 },
            { "FastEnemy", 1 },
            { "ShieldEnemy", 2 },
            { "SpawnerBossEnemy", 3 },
            { "TankEnemy", 4 }
        };

        spawnPointList = new List<GameObject>();

        // size of the potential number of dropped inventory items
        enemyPrefabs = new List<GameObject>();
        enemyCount = new List<int>();
        _globalWaveTimer = 0; 

        enemies = new List<GameObject>();
        spawnPoints = new List<SpawnPoint>();
        wave = 0;
    }

    // Start is called before the first frame update
    void Start() { InitializeWavePrefabs(); CreateTutorialWaves(); }

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

            if (prefabMultiTag.HasTag("Enemy")) // handle enemy prefabs
            {
                int enemyIDX = getItemIDX(prefab.name); // Use prefab name or another identifier
                if (enemyIDX >= 0 && enemyIDX < TOTAL_NUM_ENEMY_TYPES)
                {
                    enemyPrefabs[enemyIDX] = prefab;
                    enemyCount[enemyIDX] = 0;
                }
                Debug.Log($"Initialized {enemyPrefabs.Count} enemy prefabs with the 'Enemy' tag.");
            }
            else if (prefabMultiTag.HasTag("SpawnPoint"))
            {
                spawnPointList.Add(prefab);
                Debug.Log($"Initialized {spawnPointList.Count} spawn_point prefabs with the 'SpawnPoint' tag.");
            }
        }  
    }


    private void InitializeSpawnPoints()
    {

    }

    private void CreateTutorialWaves()
    {
        List<SpawnEvent> spawn_schedule = new List<SpawnEvent>();
        SpawnEvent wave_event = new SpawnEvent(Instantiate(enemyPrefabs[getItemIDX("Enemy")]), 0.0f, spawnPointList[0]);
        spawn_schedule.Add(wave_event);

        Wave wave = new Wave(1, 1.0f, spawn_schedule, 10.0f, true);
    }

    // Update is called once per frame
    void Update()
    {

        if(AllEnemiesKilled() && WaveTimerComplete(_currentWave)) SpawnNextWave();
        UpdateGlobalWaveTimer();
    }

    private bool WaveTimerComplete(Wave wave)
    {
        return _globalWaveTimer > wave.WaveDuration;
    }

    private bool AllEnemiesKilled()
    {
        int numEnemiesLeft = 0;
        foreach (int enemy_count in enemyCount)
        {
            if (enemy_count < 0) throw new ArgumentException("there is negative amount of enemimes (illegal)");
            numEnemiesLeft += enemy_count;
        }
        return numEnemiesLeft > 0; 
    }

    private void UpdateGlobalWaveTimer()
    {
        _globalWaveTimer += Time.deltaTime;
    }

    void SpawnNextWave()
    {
        timeSinceLastWave = 0.0f; // reset 
        ++wave;
        SpawnNormalEnemies();
        SpawnTanks();
        SpawnSpawnerBoss();
        SpawnShieldEnemy();
        GameManager.Instance.UIManager.UpdateUI();
        Debug.Log("Updating Wave... Wave " + wave + " starting");
        GameManager.Instance.AnalyticsManager.UpdateWaveNumber(wave);// Send wave number to analytics

    }

    void SpawnNormalEnemies()
    {
        foreach (SpawnPoint sp in spawnPoints)
        {
            sp.SpawnEnemy(UnityEngine.Random.Range(0.0f, maxSpawnDelay), wave, difficulty);
        }
    }

    void SpawnTanks()
    {
        for (int i = wave; i >= tankSpawnStartWave; --i)
        {
            SpawnPoint randomSpawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];
            if (UnityEngine.Random.Range(0.0f, 1.0f) <= tankRate)
            {
                GameObject tankEnemy = Instantiate(tank);
                tankEnemy.transform.position = randomSpawnPoint.transform.position;
            }
        }
    }

    void SpawnSpawnerBoss()
    {
        if(wave >= spawnerBossStartWave)
        {
            for(int i = wave / spawnerBossStartWave; i > 0; --i)
            {
                SpawnPoint randomSpawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];
                GameObject spawnerEnemy = Instantiate(spawnerBoss);
                spawnerEnemy.transform.position = randomSpawnPoint.transform.position;
            }
        }
    }

    void SpawnShieldEnemy()
    {
        for (int i = wave; i >= tankSpawnStartWave; --i)
        {
            SpawnPoint randomSpawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];
            if (UnityEngine.Random.Range(0.0f, 1.0f) <= tankRate)
            {
                GameObject shieldEnemy = Instantiate(shieldBoss);
                shieldEnemy.transform.position = randomSpawnPoint.transform.position;
            }
        }
    }

    // get IDX of enemy given a enemyName
    public int getItemIDX(string enemyName) => _enemyMapping.TryGetValue(enemyName, out int index) ? index : -1;
}