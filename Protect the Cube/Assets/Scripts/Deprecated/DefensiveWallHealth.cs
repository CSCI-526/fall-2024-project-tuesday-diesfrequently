using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensiveWallHealth : MonoBehaviour
{
    [SerializeField] public int maxHealth = 20;
    [SerializeField] public float invincibilityDuration = 0.01f;

    private int currentHealth;
    private bool isInvincible = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage()
    {
        if (!isInvincible) // Only take damage if not currently invincible
        {
            currentHealth--;
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

    private void Die()
    {
        Destroy(gameObject);
    }
}
