using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nexus : MonoBehaviour
{
    [SerializeField] public bool spawnXP;
    [SerializeField] public int health;
    [SerializeField] public int maxHealth;
    [SerializeField] public float xpSpawnInterval;
    
    [SerializeField] protected GameObject XP;
    [SerializeField] public Vector3 xpSpawnOffset; 

    private float timeSinceLastSpawn = 0.0f;

    // Start is called before the first frame update
    private void Start()
    {
        GameManager.Instance.Nexus = this.gameObject;
        health = maxHealth;
    }
    public void TakeDamage(int amount = 1)
    {
        health -= amount;
        GameManager.Instance.UIManager.UpdateUI();
        if (health < 0)
        { 
            GameManager.Instance.TriggerGameOver();
            gameObject.SetActive(false);
        }
    }

    public void Update()
    {
        if(spawnXP)
        {
            timeSinceLastSpawn += Time.deltaTime;
            if (timeSinceLastSpawn > xpSpawnInterval)
            {
                timeSinceLastSpawn = 0.0f;
                Instantiate(XP, transform.position + xpSpawnOffset, Quaternion.identity);
            }
        }
    }

}
