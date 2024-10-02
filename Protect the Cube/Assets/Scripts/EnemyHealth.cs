 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    [SerializeField] public float maxHealth = 5;
    [SerializeField] public float invincibilityDuration = 0.01f;
    [SerializeField] public GameObject exp;

    [SerializeField] public float xpDropRate = 0.5f;  
    [SerializeField] public int maxXpDrop = 3;
    [SerializeField] public int minXpDrop = 5;
    private float currentHealth;
    private bool isInvincible = false;


    void Start()
    {
        currentHealth = maxHealth;
        GameManager.Instance.WaveManager.enemyCount++;
        GameManager.Instance.WaveManager.enemies.Add(this.gameObject);
        //Debug.Log(GameManager.Instance.WaveManager.enemyCount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        if (!isInvincible) // Only take damage if not currently invincible
        {
            currentHealth-= damage;
            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                StartCoroutine(InvincibilityCoroutine());
            }
        }
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    public void Die()
    {
        GameManager.Instance.WaveManager.enemyCount--;
        GameManager.Instance.WaveManager.enemies.Remove(this.gameObject);
        DropExp();
        //Debug.Log(GameManager.Instance.WaveManager.enemyCount);
        Destroy(gameObject);
    }

    public void DropExp(){
        int xpDrop = Random.Range(minXpDrop, maxXpDrop);
        if (Random.Range(0.0f, 1.0f) <= xpDropRate){
            for (int i = 0; i < xpDrop; i++){
                GameObject xp = Instantiate(exp);
                xp.transform.position = new Vector3(transform.position.x+Random.Range(-1*1, 1), transform.position.y, transform.position.z+Random.Range(-1*1, 1));;
            }
        }
    }
}
