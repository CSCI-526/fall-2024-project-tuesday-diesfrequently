//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;

//public class WaveManager : MonoBehaviour
//{

//    [SerializeField] private float waveSpawnInterval = 5.0f;
//    [SerializeField] private int customWaveLimit = 5; // for tutorialization purposes
//    [SerializeField] private float baseDifficulty = 1.2f;

//    [SerializeField] public List<GameObject> enemyPrefabs; // List of enemy prefab options
//    [SerializeField] private GameObject tank; // Specific enemy prefab for tanks
//    [SerializeField] private GameObject spawnerBoss; // Specific enemy prefab for spawner bosses

//    public List<SpawnPoint> spawnPoints = new List<SpawnPoint>(); // List of spawn points now dynamic
//    [SerializeField] private float maxSpawnDelay = 2.0f; // Maximum delay for enemy spawn

//    [SerializeField] private GameObject nexus; // Nexus target reference
//    [SerializeField] private GameObject player; // Player target reference

//    public List<GameObject> enemies = new List<GameObject>();
//    public int enemyCount = 0;  // Track enemy count

//    private List<Wave> waveList = new List<Wave>();
//    public int currentWaveIndex { get; private set; } // represents current wave #
//    private float timeSinceLastWave;
//    public List<GameObject> targetList = new List<GameObject>(); // List of all targets

//    //[SerializeField] protected float waveSpawnInterval = 5.0f;
//    //[SerializeField] protected float maxSpawnDelay = 2.0f;

//    //[SerializeField] public int enemyCount = 0;
//    ////[SerializeField] public int wave = 0;
//    //[SerializeField] public float difficulty = 1.2f;

//    //[SerializeField] public float tankRate = 0.8f;
//    //[SerializeField] public GameObject tank;
//    //[SerializeField] public int tankSpawnStartWave = 5;

//    //[SerializeField] public GameObject spawnerBoss;
//    //[SerializeField] public int spawnerBossStartWave = 10;

//    //[SerializeField] public List<GameObject> enemyPrefabs = new List<GameObject>();
//    //[SerializeField] public List<GameObject> enemies = new List<GameObject>();
//    //[SerializeField] public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
//    //float timeSinceLastWave;


//    // called before first frame update
//    private void Start()
//    {
//        InitializeWaveManager();
//        InitializeCustomWaves();
//        InitializeTargets();
//    }

//    // called every frame update
//    private void Update()
//    {
//        if (ShouldSpawnNextWave())
//        {
//            SpawnWave();
//            timeSinceLastWave = 0.0f;
//        }
//        timeSinceLastWave += Time.deltaTime;
//    }

//    private bool ShouldSpawnNextWave()
//    {
//        return timeSinceLastWave > waveSpawnInterval && AllEnemiesDefeated();
//    }

//    private void InitializeWaveManager()
//    {
//        currentWaveIndex = 0;
//    }

//    private void InitializeCustomWaves()
//    {
//        for (int i = 0; i < customWaveLimit; i++)
//        {
//            var wave = CreateCustomWave(i + 1);
//            waveList.Add(wave);
//        }
//    }

//    private void InitializeTargets()
//    {
//        if (nexus != null) targetList.Add(nexus);
//        if (player != null) targetList.Add(player);
//    }

//    private Wave CreateCustomWave(int waveNumber)
//    {
//        List<EnemySpawnInfo> spawnInfoList = new List<EnemySpawnInfo>();

//        // Define each handcrafted wave with specific enemy types, spawn points, and delays
//        if (waveNumber == 1)
//        {
//            spawnInfoList.Add(new EnemySpawnInfo(enemyPrefabs[0], 1.0f, spawnPoints[0]));
//            spawnInfoList.Add(new EnemySpawnInfo(enemyPrefabs[1], 2.0f, spawnPoints[1]));
//        }
//        // Add custom spawn patterns for each handcrafted wave
//        // Add similar conditions for waveNumber 2, 3, etc.

//        return new Wave(waveNumber, baseDifficulty + waveNumber * 0.2f, spawnInfoList, true);
//    }

//    private void SpawnWave()
//    {
//        Wave wave = GetNextWave();
//        foreach (var spawnInfo in wave.EnemySpawnInfoList)
//        {
//            StartCoroutine(SpawnEnemyWithDelay(spawnInfo));
//        }
//        currentWaveIndex++;
//    }

//    private Wave GetNextWave()
//    {
//        if (currentWaveIndex < customWaveLimit)
//        {
//            return waveList[currentWaveIndex];
//        }
//        else
//        {
//            return GenerateDynamicWave(currentWaveIndex + 1);
//        }
//    }

//    private Wave GenerateDynamicWave(int waveNumber)
//    {
//        float difficulty = baseDifficulty * Mathf.Pow(1.1f, waveNumber - customWaveLimit);

//        List<EnemySpawnInfo> spawnInfoList = new List<EnemySpawnInfo>();
//        int enemyCount = Mathf.CeilToInt(waveNumber * difficulty);

//        for (int i = 0; i < enemyCount; i++)
//        {
//            GameObject enemyType = GetEnemyPrefabForDifficulty(difficulty);
//            float spawnDelay = Random.Range(0.5f, maxSpawnDelay);
//            SpawnPoint spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
//            spawnInfoList.Add(new EnemySpawnInfo(enemyType, spawnDelay, spawnPoint));
//        }

//        return new Wave(waveNumber, difficulty, spawnInfoList);
//    }

//    private GameObject GetEnemyPrefabForDifficulty(float difficulty)
//    {
//        if (difficulty < 2.0f) return enemyPrefabs[0];
//        else if (difficulty < 3.0f) return enemyPrefabs[1];
//        else if (difficulty < 4.0f) return tank;
//        else return spawnerBoss;
//    }

//    private bool AllEnemiesDefeated()
//    {
//        return enemies.Count == 0;
//    }

//    private IEnumerator SpawnEnemyWithDelay(EnemySpawnInfo spawnInfo)
//    {
//        yield return new WaitForSeconds(spawnInfo.SpawnDelay);
//        GameObject enemy = Instantiate(spawnInfo.EnemyPrefab, spawnInfo.SpawnPoint.transform.position, Quaternion.identity);
//        enemies.Add(enemy);
//        enemyCount++;
//    }

//    public List<GameObject> GetTargetList()
//    {
//        return new List<GameObject>(targetList); // Return a copy to avoid accidental modifications
//    }

//    public void OnEnemyDeath(GameObject enemy)
//    {
//        enemies.Remove(enemy);
//        enemyCount--;

//        if (enemyCount <= 0)
//        {
//            Debug.Log("All enemies in the wave are defeated!");
//            HandleWaveCompletion();
//        }
//    }

//    private void HandleWaveCompletion()
//    {
//        // Logic for handling wave completion
//        // Example: start next wave, give rewards, etc.
//    }
//}



using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] public int currentWaveIndex { get; private set; }
    [SerializeField] public float waveInterval { get; private set; } // OLD: 5.0
    [SerializeField] public float maxDelayBetweenSpawnEvent { get; private set; } // OLD: 2.0
    [SerializeField] public int numEnemiesAlive { get; private set; }

    [SerializeField] public List<GameObject> activeEnemyObjects = new List<GameObject>();

    private float _waveDuration;

    [SerializeField] public float difficulty = 1.2f;

    [SerializeField] public float tankRate = 0.8f;
    [SerializeField] public GameObject tank;
    [SerializeField] public int tankSpawnStartWave = 5;

    [SerializeField] public GameObject spawnerBoss;
    [SerializeField] public int spawnerBossStartWave = 10;

    [SerializeField] public List<GameObject> enemyPrefabs = new List<GameObject>();
    [SerializeField] public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
    

    private void Awake()
    {
        InitializeWaveManager();
    }

    private void InitializeWaveManager()
    {
        // initialize balance variables
        waveInterval = 20.0f;
        maxDelayBetweenSpawnEvent = 2.0f;
        numEnemiesAlive = 0;
        _waveDuration = 0;
        activeEnemyObjects = new List<GameObject>();
        spawnPoints = new List<SpawnPoint>();
        currentWaveIndex = 0;
    }


    // Update is called once per frame
    void Update()
    {
        OnWaveCompletion(); // check for wave completion
        _waveDuration += Time.deltaTime; // update waveDuration time
    }

    private void OnWaveCompletion()
    {
        if (AllEnemiesDead() && WaveTimerComplete())
        {
            SpawnWave();
            ResetWaveTimer();
        }
    }

    void SpawnWave()
    {
        UpdateWaveNumber();

        // MATCHA: change to new wave construction logic
        SpawnNormalEnemies();
        SpawnTanks();
        SpawnSpawnerBoss();

        // MATCHA: change to event driven system
        GameManager.Instance.UIManager.UpdateUI();
        Debug.Log("Updating Wave... Wave " + currentWaveIndex + " starting");

        // Send wave number to analytics (use events)
        GameManager.Instance.AnalyticsManager.UpdateWaveNumber(currentWaveIndex);

    }

    private void UpdateWaveNumber()
    {
        ++currentWaveIndex;
    }

    void SpawnNormalEnemies()
    {
        foreach (SpawnPoint sp in spawnPoints)
        {
            sp.SpawnEnemy(Random.Range(0.0f, maxDelayBetweenSpawnEvent), currentWaveIndex, difficulty);
        }
    }

    void SpawnTanks()
    {
        for (int i = currentWaveIndex; i >= tankSpawnStartWave; --i)
        {
            SpawnPoint randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            if (Random.Range(0.0f, 1.0f) <= tankRate)
            {
                GameObject tankEnemy = Instantiate(tank);
                tankEnemy.transform.position = randomSpawnPoint.transform.position;
            }
        }
    }

    void SpawnSpawnerBoss()
    {
        if (currentWaveIndex >= spawnerBossStartWave)
        {
            for (int i = currentWaveIndex / spawnerBossStartWave; i > 0; --i)
            {
                SpawnPoint randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
                GameObject spawnerEnemy = Instantiate(spawnerBoss);
                spawnerEnemy.transform.position = randomSpawnPoint.transform.position;
            }
        }
    }

    public bool AllEnemiesDead()
    {
        if (numEnemiesAlive == 0) return true;
        else if (numEnemiesAlive <= 0) { Debug.Log("[Wave Manager] Negative Enemy Count Detected. "); return false; }
        return false; // return false if enemies still alive
    }

    private bool WaveTimerComplete()
    {
        return (_waveDuration > waveInterval);
    }

    private void ResetWaveTimer()
    {
        _waveDuration = 0.0f;
    } 

    // called by EnemyHealth when Enemy is "instantiated"
    // MATCHA: do we need to move this somewhre else?
    public void OnEnemyCreation(GameObject enemy)
    {
        numEnemiesAlive++;
        activeEnemyObjects.Add(enemy);
        return;
    }

    public void OnEnemyDeath(GameObject enemy)
    {
        // sanity check logic
        if (numEnemiesAlive <= 0) { Debug.Log("[Wave Manager] Negative Amount of Enemies Alive"); return; }

        // if valid, decrease number of enemies alive + stop tracking enemyObject
        numEnemiesAlive--;
        activeEnemyObjects.Remove(enemy);
        return;
    }
}