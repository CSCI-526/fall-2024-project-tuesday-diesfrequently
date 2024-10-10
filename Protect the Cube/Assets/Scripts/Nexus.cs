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

    [SerializeField] public bool triggerGameOver = false;

    private float timeSinceLastSpawn = 0.0f;
    public delegate void NexusEvent();
    public event NexusEvent OnTakeDamage;

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
        if(OnTakeDamage != null)
        {
            OnTakeDamage();
        }
        if (health <= 0)
        { 
            if(triggerGameOver)
            {
                GameManager.Instance.TriggerGameOver();
            }
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
