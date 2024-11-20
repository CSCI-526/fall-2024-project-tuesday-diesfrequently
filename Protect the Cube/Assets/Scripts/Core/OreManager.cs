using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class OreManager : MonoBehaviour
{
    // MATCHA: change from player levels to wave number (otherwise is positive feedback loop)
    [Header("Prefabs")]
    [SerializeField] public GameObject OrePrefabTier1;
    [SerializeField] public GameObject OrePrefabTier2;
    [SerializeField] public GameObject OrePrefabTier3;

    [Header("Ore Level Thresholds")]
    /* THESE ARE DEBUG STARTING SPAWN LEVELS, change higher for actual game */
    [SerializeField] public int OreTier1StartingSpawnLevel = 2;
    [SerializeField] public int OreTier2StartingSpawnLevel = 4;
    [SerializeField] public int OreTier3StartingSpawnLevel = 6;

    [Header("Ore Distance Thresholds")]
    [SerializeField] public float ORE_MIN_DIST_THRESHOLD = 10.0f;
    [SerializeField] public float ORE_T1_DIST_THRESHOLD = 20.0f;
    [SerializeField] public float ORE_T2_DIST_THRESHOLD = 28.0f;
    [SerializeField] public float ORE_T3_DIST_THRESHOLD = 35.0f;
    [SerializeField] public float ORE_MAX_DIST_THRESHOLD = 45.0f;

    [Header("Ore Visuals Balancing")]
    [SerializeField] public float ORE_HEIGHT_ABOVE_GROUND = 1.2f;
    [SerializeField] public float ORE_RESOURCE_DROP_ZONE_RADIUS = 2.0f;
    [SerializeField] public int ORE_T1_NUM_EYES = 5;
    [SerializeField] public int ORE_T2_NUM_EYES = 10;
    [SerializeField] public int ORE_T3_NUM_EYES = 15;

    [Header("Ore Number Balancing")]
    [SerializeField] public int InitialOreCount = 4;
    [SerializeField] public int ORE_INCREASE_PER_LVL = 1;

    [Header("Ore Resource Drop Balancing")]
    [SerializeField] public int ORE_T1_DROP_XP = 4;
    [SerializeField] public int ORE_T2_DROP_XP = 6;
    [SerializeField] public int ORE_T2_DROP_GOLD = 3;
    [SerializeField] public int ORE_T3_DROP_XP = 8;
    [SerializeField] public int ORE_T3_DROP_GOLD = 5;

    [Header("Ore Max Health Balancing")]
    [SerializeField] public float ORE_T1_MAX_HEALTH = 15;
    [SerializeField] public float ORE_T2_MAX_HEALTH = 25;
    [SerializeField] public float ORE_T3_MAX_HEALTH = 35;

    // Ore Tier Variables
    public const int ORE_TIER_1 = 1;
    public const int ORE_TIER_2 = 2;
    public const int ORE_TIER_3 = 3;

    // Private References
    private GameObject _nexus;
    private PlayerLevels _playerLevel;

    [Header("Ore Debugging")]
    [SerializeField] private int _playerLevelSnapshot;
    [SerializeField] private List<bool> _canOreSpawn; // each index represents a tier
    [SerializeField] private int _oreCount;
    [SerializeField] private List<GameObject> _OreEntityList;

    void Start()
    {
        _playerLevel = GameManager.Instance.Player.GetComponent<PlayerLevels>();
        _nexus = GameManager.Instance.Nexus;
        _playerLevelSnapshot = 0;

        _canOreSpawn = new List<bool>();
        _canOreSpawn.Add(false); // tier 1
        _canOreSpawn.Add(false); // tier 2
        _canOreSpawn.Add(false); // tier 3

        _oreCount = 0;
        _OreEntityList = new List<GameObject>();
    }

    void Update()
    {
        // if PlayerCurrentLevel is updated + above threshold, spawnOre()
        if (UpdatePlayerCurrentLevel())
        {
            // intialize ore spawns on level thresholds
            if (_playerLevelSnapshot == OreTier1StartingSpawnLevel)
            {
                _canOreSpawn[ORE_TIER_1 - 1] = true;
                InitializeSpawnedOre(ORE_TIER_1);
            }
            else if (_playerLevelSnapshot == OreTier2StartingSpawnLevel)
            {
                _canOreSpawn[ORE_TIER_2 - 1] = true;
                InitializeSpawnedOre(ORE_TIER_2);
            }
            else if (_playerLevelSnapshot == OreTier3StartingSpawnLevel)
            {
                _canOreSpawn[ORE_TIER_3 - 1] = true;
                InitializeSpawnedOre(ORE_TIER_3);
            }

            // spawn consistent ore per level
            if ((_playerLevelSnapshot > OreTier1StartingSpawnLevel) && _canOreSpawn[ORE_TIER_1 - 1])
            {
                SpawnOrePerNewLevel(ORE_TIER_1);
            }

            if ((_playerLevelSnapshot > OreTier2StartingSpawnLevel) && _canOreSpawn[ORE_TIER_2 - 1])
            {
                SpawnOrePerNewLevel(ORE_TIER_2);
            }

            if ((_playerLevelSnapshot > OreTier3StartingSpawnLevel) && _canOreSpawn[ORE_TIER_3 - 1])
            {
                SpawnOrePerNewLevel(ORE_TIER_3);
            }
        }
    }

    private bool UpdatePlayerCurrentLevel() // checks and updates player level, returns bool if level was updated
    {
        if (_playerLevelSnapshot != _playerLevel.currentLevel)
        {
            _playerLevelSnapshot = _playerLevel.currentLevel;
            return true; // if the level is updated
        }
        return false; // if level is not updated
    }

    private void InitializeSpawnedOre(int ore_tier)
    {
        // Populate Map Initially w/ InitialOreCount
        for (int oreSpawned = 0; oreSpawned < InitialOreCount; oreSpawned++) { SpawnOre(ore_tier); }
    }

    void SpawnOrePerNewLevel(int ore_tier)
    {
        for (int oreSpawned = 0; oreSpawned < ORE_INCREASE_PER_LVL; oreSpawned++) { SpawnOre(ore_tier); }
    }

    void SpawnOre(int ore_tier) // Spawns an Ore of type ore_tier
    {
        Vector3 spawnPosition;
        GameObject oreEntity;

        if (ore_tier == 1)
        {
            float randomX = Random.Range(0, 2) == 0
                ? Random.Range(-ORE_T1_DIST_THRESHOLD, -ORE_MIN_DIST_THRESHOLD)
                : Random.Range(ORE_MIN_DIST_THRESHOLD, ORE_T1_DIST_THRESHOLD);

            float randomZ = Random.Range(0, 2) == 0
                ? Random.Range(-ORE_T1_DIST_THRESHOLD, -ORE_MIN_DIST_THRESHOLD)
                : Random.Range(ORE_MIN_DIST_THRESHOLD, ORE_T1_DIST_THRESHOLD);

            spawnPosition = _nexus.transform.position + new Vector3(randomX, ORE_HEIGHT_ABOVE_GROUND, randomZ);
            oreEntity = Instantiate(OrePrefabTier1, spawnPosition, Quaternion.identity);
        }
        else if (ore_tier == 2)
        {
            float randomX = Random.Range(0, 2) == 0
                ? Random.Range(-ORE_T2_DIST_THRESHOLD, -ORE_T1_DIST_THRESHOLD)
                : Random.Range(ORE_T1_DIST_THRESHOLD, ORE_T2_DIST_THRESHOLD);

            float randomZ = Random.Range(0, 2) == 0
                ? Random.Range(-ORE_T2_DIST_THRESHOLD, -ORE_T1_DIST_THRESHOLD)
                : Random.Range(ORE_T1_DIST_THRESHOLD, ORE_T2_DIST_THRESHOLD);

            spawnPosition = _nexus.transform.position + new Vector3(randomX, ORE_HEIGHT_ABOVE_GROUND, randomZ);
            oreEntity = Instantiate(OrePrefabTier2, spawnPosition, Quaternion.identity);
        }
        else
        {
            float randomX = Random.Range(0, 2) == 0
                ? Random.Range(-ORE_T3_DIST_THRESHOLD, -ORE_T2_DIST_THRESHOLD)
                : Random.Range(ORE_T2_DIST_THRESHOLD, ORE_T3_DIST_THRESHOLD);

            float randomZ = Random.Range(0, 2) == 0
                ? Random.Range(-ORE_T3_DIST_THRESHOLD, -ORE_T2_DIST_THRESHOLD)
                : Random.Range(ORE_T2_DIST_THRESHOLD, ORE_T3_DIST_THRESHOLD);

            spawnPosition = _nexus.transform.position + new Vector3(randomX, ORE_HEIGHT_ABOVE_GROUND, randomZ);
            oreEntity = Instantiate(OrePrefabTier3, spawnPosition, Quaternion.identity);
        }

        AddOreEntity(oreEntity, ore_tier); // keep track of ore entities
    }

    public void AddOreEntity(GameObject ore_entity, int ore_tier)
    {
        _OreEntityList.Add(ore_entity);
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
