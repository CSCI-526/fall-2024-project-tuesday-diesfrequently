using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class PlayerHealth : MonoBehaviour
{
    public const int PLAYER_MAX_HEALTH = 5;

    [SerializeField] private float invincibilityDuration = 0.01f;

    [Header("UI References")]
    [SerializeField] private Slider hpBar;
    [SerializeField] private Canvas hpCanvas;

    private Animator animator;
    public int maxHealth { get; private set; }
    public int currentHealth { get; private set; }
    private bool isInvincible = false;

    private InventoryManager inventoryManager;
    public event Action<int> Analytics_OnPlayerHPLoss;

    private void Awake()
    {
        // updateMaxHealth
        maxHealth = PLAYER_MAX_HEALTH;
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        SetupHealthUI();
        UpdateHPBar();
    }

    private void OnEnable()
    {
        StartCoroutine(DelayedSubscribeToEvents());
    }

    private void OnDisable()
    {
        inventoryManager.PlayerHealth_OnPlayerHealthUpdate -= AddPlayerHealth;
    }

    private void SetupHealthUI()
    {
        if (hpCanvas != null)
        {
            var rotationConstraint = hpCanvas.GetComponent<RotationConstraint>();
            if (rotationConstraint != null)
            {
                ConstraintSource cameraSource = new ConstraintSource
                {
                    sourceTransform = Camera.main.transform,
                    weight = 1.0f
                };
                rotationConstraint.AddSource(cameraSource);
            }
        }
    }

    private IEnumerator DelayedSubscribeToEvents()
    {
        // Wait until GameManager instance and InventoryManager are initialized
        // yield return new WaitUntil(() => GameManager.Instance != null && GameManager.Instance.InventoryManager != null);
        yield return new WaitUntil(() => GameManager.Instance?.InventoryManager != null);

        inventoryManager = GameManager.Instance.InventoryManager;
        inventoryManager.PlayerHealth_OnPlayerHealthUpdate += AddPlayerHealth;
    }

    public void TakeDamage()
    {
        if (isInvincible) return; // can't take dmg if Invincibile

        // Update Animation 
        animator.SetTrigger("Damage");

        // Decrease Current Health (by dmg)
        currentHealth = Mathf.Max(currentHealth - 1, 0);

        // Update HP Bar
        UpdateHPBar();

        // Die() if below 0 hp, Momentary Invincibility otherwise
        if (currentHealth <= 0) Die();
        else StartCoroutine(InvincibilityCoroutine());

        // Update Analytics
        Analytics_OnPlayerHPLoss?.Invoke(AnalyticsManager.TYPE_PLAYER_LOSS_HP);
        // GameManager.Instance.AnalyticsManager.UpdateHitpointLossWave(20);
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

    private void AddPlayerHealth()
    {
        currentHealth = Mathf.Min(currentHealth + InventoryManager.PLAYER_HEALTH_INCREASE, maxHealth);
        UpdateHPBar();
    }

    private void UpdateHPBar()
    {
        if (hpBar) hpBar.value = (float)currentHealth/maxHealth;
    }
}
