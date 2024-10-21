using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] public int maxHealth = 5;
    [SerializeField] public float invincibilityDuration = 0.01f;
    [SerializeField] private Animator animator;

    public int currentHealth;
    private bool isInvincible = false;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage()
    {
        if (!isInvincible) // Only take damage if not currently invincible
        {
            animator.SetTrigger("Damage");
            currentHealth--;
            GameManager.Instance.UIManager.UpdateUI();
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
        GameManager.Instance.TriggerGameOver();
        gameObject.SetActive(false);
    }
}
