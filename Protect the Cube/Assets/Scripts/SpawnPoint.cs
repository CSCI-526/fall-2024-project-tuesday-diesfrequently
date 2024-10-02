using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] protected bool spawnAtStart = true;
    [SerializeField] List<GameObject> enemyPrefabs = new List<GameObject>();
    public float spawnRange = 1.0f;

    void Start()
    {
        GameManager.Instance.WaveManager.spawnPoints.Add(this);
        if(spawnAtStart)
        {
            SpawnEnemy();
        }
    }

    public void SpawnEnemy(float delay = 0f, int wave = 1, float difficulty = 1.0f)
    {   
        
        int enemyNum = Random.Range(Math.Max(1,wave-3), Math.Max((int)Math.Round(difficulty * wave), 2));
        //UnityEngine.Debug.Log(enemyNum);
        if(enemyPrefabs.Count > 0)
        {   
            for(int i = 0; i < enemyNum; i++){
                StartCoroutine(DelayedSpawn(delay));
            }
        }
        else
        {
            UnityEngine.Debug.Log("Error: spawning pool not populated");
        }
    }

    public void SpawnEnemyOfType(GameObject prefab)
    {
        GameObject enemy = Instantiate(prefab);
        enemy.transform.position = transform.position;
        // enemy.GetComponent<EnemyMove>().SetTarget(GameManager.Instance.Player);
    }

    IEnumerator DelayedSpawn(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        GameObject enemy = Instantiate(enemyToSpawn); 
        enemy.transform.position = new Vector3(transform.position.x+Random.Range(-1*spawnRange, spawnRange), transform.position.y, transform.position.z+Random.Range(-1*spawnRange, spawnRange));
    }

    public static implicit operator int(SpawnPoint v)
    {
        throw new NotImplementedException();
    }
}
