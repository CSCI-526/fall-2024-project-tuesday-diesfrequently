using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class OreManager : MonoBehaviour
{
    [Header("PUBLIC Modifiable Constants")]
    [SerializeField] public GameObject OrePrefabTier1;
    [SerializeField] public GameObject OrePrefabTier2;
    [SerializeField] public GameObject OrePrefabTier3;
    [SerializeField] public float OreHeightAboveGround = 1.2f; 
    [SerializeField] public float OreSpawnRateSeconds = 5.0f;
    [SerializeField] public float MaxOreSpawnDistance = 45.0f;
    [SerializeField] public float InitialOreCount = 5;
    [SerializeField] public int OreSpawnStartLevelThreshold = 5;
    [SerializeField] public int NumOreSpawnedPerLevel = 2;

    // Ore Tier Variables
    [SerializeField] public float MIN_DISTANCE_THRESHOLD = 10;
    [SerializeField] public float TIER_1_DISTANCE_THRESHOLD = 20;
    [SerializeField] public float TIER_2_DISTANCE_THRESHOLD = 30;
    [SerializeField] public float TIER_3_DISTANCE_THRESHOLD = 40;
    public const int ORE_TIER_1 = 1;
    public const int ORE_TIER_2 = 2;
    public const int ORE_TIER_3 = 3;

    // Private References
    private GameObject _nexus;
    private PlayerLevels _playerLevel;
    [Header("PRIVATE OreManager Members")]
    [SerializeField] private int _playerLevelSnapshot;
    [SerializeField] private bool _canOreSpawn;
    [SerializeField] private int _oreCount;
    [SerializeField] private List<GameObject> _OreEntityList;

    void Start()
    {
        _playerLevel = GameManager.Instance.Player.GetComponent<PlayerLevels>();
        _nexus = GameManager.Instance.Nexus;
        _playerLevelSnapshot = 0;
        _canOreSpawn = false; 
        _oreCount = 0;
        _OreEntityList = new List<GameObject>();
    }

    void Update()
    {
        // if PlayerCurrentLevel is updated + above threshold, spawnOre()
        if (UpdatePlayerCurrentLevel())
        {
            if (_playerLevelSnapshot == OreSpawnStartLevelThreshold)
            {
                _canOreSpawn = true;
                InitializeSpawnedOre();
            }
            else if (_playerLevelSnapshot > OreSpawnStartLevelThreshold && _canOreSpawn) { SpawnOrePerNewLevel(); }
        }
    }

    private bool UpdatePlayerCurrentLevel() // checks and updates player level, returns bool if level was updated
    {
        if (_playerLevelSnapshot != _playerLevel.currentLevel) {
            _playerLevelSnapshot = _playerLevel.currentLevel;
            return true; // if the level is updated
        }
        return false; // if level is not updated
    }

    void InitializeSpawnedOre()
    {
        // Populate Map Initially w/ InitialOreCount
        for (int oreSpawned = 0; oreSpawned < InitialOreCount; oreSpawned++) { SpawnOre(ORE_TIER_1); }
    }

    void SpawnOrePerNewLevel()
    {
        for (int oreSpawned = 0; oreSpawned < NumOreSpawnedPerLevel; oreSpawned++) { SpawnOre(ORE_TIER_1); }
    }

    void SpawnOre(int ore_tier) // Spawns an Ore of type ore_tier
    {
        Vector3 spawnPosition;
        GameObject oreEntity;

        if (ore_tier == 1) {

            float randomX = Random.Range(0, 2) == 0
                ? Random.Range(-TIER_1_DISTANCE_THRESHOLD, -MIN_DISTANCE_THRESHOLD)
                : Random.Range(MIN_DISTANCE_THRESHOLD, TIER_1_DISTANCE_THRESHOLD);

            float randomZ = Random.Range(0, 2) == 0
                ? Random.Range(-TIER_1_DISTANCE_THRESHOLD, -MIN_DISTANCE_THRESHOLD)
                : Random.Range(MIN_DISTANCE_THRESHOLD, TIER_1_DISTANCE_THRESHOLD);

            spawnPosition = _nexus.transform.position + new Vector3(randomX, OreHeightAboveGround, randomZ);
            oreEntity = Instantiate(OrePrefabTier1, spawnPosition, Quaternion.identity);
        }
        else if (ore_tier == 2) {
            float randomX = Random.Range(0, 2) == 0
                ? Random.Range(-TIER_2_DISTANCE_THRESHOLD, -TIER_1_DISTANCE_THRESHOLD)
                : Random.Range(TIER_1_DISTANCE_THRESHOLD, TIER_2_DISTANCE_THRESHOLD);

            float randomZ = Random.Range(0, 2) == 0
                ? Random.Range(-TIER_3_DISTANCE_THRESHOLD, -TIER_2_DISTANCE_THRESHOLD)
                : Random.Range(TIER_2_DISTANCE_THRESHOLD, TIER_3_DISTANCE_THRESHOLD);

            spawnPosition = _nexus.transform.position + new Vector3(randomX, OreHeightAboveGround, randomZ);
            oreEntity = Instantiate(OrePrefabTier2, spawnPosition, Quaternion.identity);
        } else {
            float randomX = Random.Range(0, 2) == 0
                ? Random.Range(-TIER_1_DISTANCE_THRESHOLD, -MIN_DISTANCE_THRESHOLD)
                : Random.Range(MIN_DISTANCE_THRESHOLD, TIER_1_DISTANCE_THRESHOLD);

            float randomZ = Random.Range(0, 2) == 0
                ? Random.Range(-TIER_1_DISTANCE_THRESHOLD, -MIN_DISTANCE_THRESHOLD)
                : Random.Range(MIN_DISTANCE_THRESHOLD, TIER_1_DISTANCE_THRESHOLD);

            spawnPosition = _nexus.transform.position + new Vector3(randomX, OreHeightAboveGround, randomZ);
            oreEntity = Instantiate(OrePrefabTier3, spawnPosition, Quaternion.identity);
        }

        // keep track of ore entities
        AddOreEntity(oreEntity, ore_tier);
    }

    public void AddOreEntity(GameObject ore_entity, int ore_tier)
    {
        _OreEntityList.Add(ore_entity);
        ore_entity.GetComponent<Ore>().SetOreTier(ore_tier);
        if (GameManager.Instance.DEBUG_ORE_MANAGER) Debug.Log("[Ore Manager] Adding Ore Entity of Tier " + ore_tier);
        _oreCount++;
    }

    public void DestroyOreEntity(GameObject ore_entity)
    {
        if (GameManager.Instance.DEBUG_ORE_MANAGER) Debug.Log("[Ore Manager] Removing Ore Entity ... ");
        _OreEntityList.Remove(ore_entity);
        _oreCount--;
    }


}
