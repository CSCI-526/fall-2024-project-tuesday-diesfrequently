using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class OreManager : MonoBehaviour
{

    [SerializeField] public GameObject _orePrefab;
    private GameObject _nexus;
    WaveManager _waveManager;
    private PlayerLevels _playerLevel;
    [SerializeField] public float _oreSpawnRateSeconds = 5.0f;
    [SerializeField] public float maxOreDistance = 40.0f;
    private float _oreSpawnTimer = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        _waveManager = GetComponent<WaveManager>();
        _playerLevel = GameManager.Instance.Player.GetComponent<PlayerLevels>();
        _nexus = GameManager.Instance.Nexus;
    }

    // Update is called once per frame
    void Update()
    {
        int playerLevelSnapshot = _playerLevel.currentLevel; // get player current XP level
        
        if (playerLevelSnapshot >= 5){
            if (_oreSpawnTimer >= _oreSpawnRateSeconds)
            {
                SpawnOre();
                _oreSpawnTimer = 0.0f;
            }
            else
            {
                _oreSpawnTimer += Time.deltaTime;
            }
        }
    }

    void SpawnOre()
    {
        Vector3 spawnPosition = _nexus.transform.position + new Vector3(Random.Range(-maxOreDistance, maxOreDistance), 1.2f, Random.Range(-maxOreDistance, maxOreDistance));
        Instantiate(_orePrefab, spawnPosition, Quaternion.identity);
    }


}
