using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] public int maxHealth = 5;
    [SerializeField] public float invincibilityDuration = 0.01f;
    [SerializeField] private Animator animator;
    [SerializeField] public Slider hpBar;
    [SerializeField] private Canvas hpCanvas;

    public int currentHealth;
    private bool isInvincible = false;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        if (hpCanvas)
        {
            ConstraintSource cs = new ConstraintSource();
            cs.weight = 1.0f;
            cs.sourceTransform = Camera.main.transform;

            hpCanvas.GetComponent<RotationConstraint>().AddSource(cs);
            UpdateHPBar();
        }
    }

    public void TakeDamage()
    {
        if (!isInvincible) // Only take damage if not currently invincible
        {
            animator.SetTrigger("Damage");
            currentHealth--;
            UpdateHPBar();
            GameManager.Instance.UIManager.UpdateUI();
            GameManager.Instance.AnalyticsManager.UpdateHitpointLossWave();
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
    public void UpdateHPBar()
    {
        if (hpBar)
        {
            hpBar.value = (float)currentHealth / maxHealth;
        }
    }
}
