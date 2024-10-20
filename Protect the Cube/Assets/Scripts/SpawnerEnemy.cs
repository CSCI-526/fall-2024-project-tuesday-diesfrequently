using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnerEnemy : MonoBehaviour
{
    [SerializeField] public GameObject enemyPrefab;
    [SerializeField] public float spawnInterval = 5.0f;
    [SerializeField] public int spawnQuantity = 3;
    [SerializeField] protected Transform spawnPos;

    protected float spawnTimer = 0.0f;

    // Update is called once per frame
    void Update()
    {
        spawnTimer += Time.deltaTime;

        if(spawnTimer > spawnInterval && enemyPrefab)
        {
            for(int i = 0; i < spawnQuantity; i++)
            {
                Instantiate(enemyPrefab, spawnPos.position, transform.rotation);
            }
            spawnTimer = 0.0f;
        }
    }
}
