using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaveManager : MonoBehaviour
{

    [SerializeField] protected int maxEnemies = 10;
    [SerializeField] protected float waveInterval = 5.0f;
    [SerializeField] protected float maxSpawnDelay = 2.0f;

    [SerializeField] public int enemyCount = 0;
    [SerializeField] public int wave = 0;
    [SerializeField] public float difficulty = 1.2f;

    [SerializeField] public float tankRate = 0.8f;
    [SerializeField] GameObject tank;
    [SerializeField] public int tankSpawnStartWave = 5;

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
        GameManager.Instance.UIManager.UpdateUI();
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
        for (int i = wave; i > tankSpawnStartWave; --i)
        {
            SpawnPoint randomSpawnPoint = spawnPoints[Random.Range(0, 4)];
            if (Random.Range(0.0f, 1.0f) <= tankRate)
            {
                GameObject tankEnemy = Instantiate(tank);
                tankEnemy.transform.position = randomSpawnPoint.transform.position;
            }
        }
    }
}