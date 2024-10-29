using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] protected bool spawnAtStart = true;
    //[SerializeField] List<GameObject> enemyPrefabs = new List<GameObject>();
    public float spawnRange = 5.0f;

    void Start()
    {
        GameManager.Instance.WaveManager.spawnPoints.Add(this);

        if (spawnAtStart)
        {
            SpawnEnemy();
        }
    }

    public void SpawnEnemy()
    {
        GameObject enemyPrefab = GameManager.Instance.WaveManager.enemyPrefabs[Random.Range(0, GameManager.Instance.WaveManager.enemyPrefabs.Count)];
        SpawnEnemyOfType(enemyPrefab);
    }

    public void SpawnEnemyOfType(GameObject prefab)
    {
        GameObject enemy = Instantiate(prefab);
        enemy.transform.position = new Vector3(
            transform.position.x + Random.Range(-spawnRange, spawnRange),
            transform.position.y,
            transform.position.z + Random.Range(-spawnRange, spawnRange));
    }
}
