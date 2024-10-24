using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] protected float waveInterval = 5.0f;
    [SerializeField] protected float maxSpawnDelay = 2.0f;

    [SerializeField] public int enemyCount = 0;
    [SerializeField] public int wave = 0;
    [SerializeField] public float difficulty = 1.2f;

    [SerializeField] public float tankRate = 0.8f;
    [SerializeField] public GameObject tank;
    [SerializeField] public int tankSpawnStartWave = 5;

    [SerializeField] public GameObject spawnerBoss;
    [SerializeField] public int spawnerBossStartWave = 10;

    [SerializeField] public List<GameObject> enemyPrefabs = new List<GameObject>();
    [SerializeField] public List<GameObject> enemies = new List<GameObject>();
    [SerializeField] public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
    float timeSinceLastWave;
    private void Awake()
    {
        enemies = new List<GameObject>();
        spawnPoints = new List<SpawnPoint>();
        enemyCount = 0;
        wave = 0;
    }


    // Start is called before the first frame update
    void Start()
    {
        //SpawnWave();
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyCount <= 0 && timeSinceLastWave > waveInterval)
        {
            SpawnWave();
            timeSinceLastWave = 0.0f;
        }
        timeSinceLastWave += Time.deltaTime;
    }

    void SpawnWave()
    {
        ++wave;
        SpawnNormalEnemies();
        SpawnTanks();
        SpawnSpawnerBoss();
        GameManager.Instance.UIManager.UpdateUI();
        Debug.Log("Updating Wave... Wave " + wave + " starting");
        GameManager.Instance.AnalyticsManager.UpdateWaveNumber(wave);// Send wave number to analytics

    }

    void SpawnNormalEnemies()
    {
        foreach (SpawnPoint sp in spawnPoints)
        {
            sp.SpawnEnemy(Random.Range(0.0f, maxSpawnDelay), wave, difficulty);
        }
    }

    void SpawnTanks()
    {
        for (int i = wave; i >= tankSpawnStartWave; --i)
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
        if(wave >= spawnerBossStartWave)
        {
            for(int i = wave / spawnerBossStartWave; i > 0; --i)
            {
                SpawnPoint randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
                GameObject spawnerEnemy = Instantiate(spawnerBoss);
                spawnerEnemy.transform.position = randomSpawnPoint.transform.position;
            }
        }
    }
}