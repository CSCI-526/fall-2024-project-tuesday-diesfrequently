using System.Collections;
using UnityEngine;

public class SpawnEvent
{
    public string EnemyType { get; private set; }
    public float SpawnDelay { get; private set; }
    public Vector3 SpawnPoint { get; private set; }
    public float SpawnRange { get; private set; }
    private WaveManager _waveManagerReference;

    public SpawnEvent(string enemy_type, float spawn_delay, Vector3 spawn_point, float spawn_range = 5.0f)
    {
        EnemyType = enemy_type;
        SpawnDelay = spawn_delay;
        SpawnPoint = spawn_point;
        SpawnRange = spawn_range;
        _waveManagerReference = GameManager.Instance.WaveManager;
    }

    // adjust as needed
    public void UpdateConfiguration(float newSpawnDelay, float newSpawnRange)
    {
        SpawnDelay = newSpawnDelay;
        SpawnRange = newSpawnRange;
    }

    public IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(SpawnDelay);
        int enemyIDX = _waveManagerReference.getEnemyIDX(EnemyType);
        GameObject enemyPrefab = _waveManagerReference.EnemyPrefabs[enemyIDX];
        GameObject enemyEntity = Object.Instantiate(enemyPrefab);

        _waveManagerReference.AddEnemyEntity(enemyEntity, enemyIDX);
        enemyEntity.transform.position = SpawnPoint + new Vector3(Random.Range(-SpawnRange, SpawnRange), 0, Random.Range(-SpawnRange, SpawnRange));
    }


}
