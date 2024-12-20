using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] public int PLAYER_MAX_HEALTH = 5;
    [SerializeField] private float invincibilityDuration = 0.01f;

    [Header("UI References")]
    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider playerHpBar;

    public event Action<GameManager.GamePhase> Tutorial_OnDeath;

    private Animator animator;
    public int maxHealth { get; private set; }

    //public int currentHealth { get; private set; }
    [SerializeField] private int CurrentHealth;

    public int currentHealth
    {
        get { return CurrentHealth; }
        private set { CurrentHealth = value; }
    }


    private bool isInvincible = false;

    private InventoryManager inventoryManager;
    public event Action<int> Analytics_OnPlayerHPLoss;

    public Animator healthbarAnim;

    private void Awake()
    {
        // updateMaxHealth
        maxHealth = PLAYER_MAX_HEALTH;
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    void Start()
    {
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

    private IEnumerator DelayedSubscribeToEvents()
    {
        // Wait until GameManager instance and InventoryManager are initialized
        yield return new WaitUntil(() => GameManager.Instance?.InventoryManager != null);

        inventoryManager = GameManager.Instance.InventoryManager;
        inventoryManager.PlayerHealth_OnPlayerHealthUpdate += AddPlayerHealth;
    }

    public void ResetPlayerHealth()
    {
        Debug.Log("[PlayerHealth] Resetting Player Death");
        currentHealth = PLAYER_MAX_HEALTH;
        UpdateHPBar();
    }

    public void TakeDamage(int amount = 1)
    {
        if (isInvincible) return; // can't take dmg if Invincibile

        // Update Animation 
        animator.SetTrigger("Damage");
        healthbarAnim.SetTrigger("DamageBar");

        // Decrease Current Health (by dmg)
        currentHealth = Mathf.Max(currentHealth - amount, 0);

        // Update HP Bar
        UpdateHPBar();
        GameManager.Instance.UIManager.DamageEffect(currentHealth);
        // Die() if below 0 hp, Momentary Invincibility otherwise
        if (currentHealth <= 0)
        {
            Debug.Log("[PlayerHealth] Player has DIED (currentHealth <= 0)");
            if (GameManager.Instance.currentPhase == GameManager.GamePhase.P1_Setup_Tutorial ||
                GameManager.Instance.currentPhase == GameManager.GamePhase.P1_TakeDamage_Tutorial ||
                GameManager.Instance.currentPhase == GameManager.GamePhase.P1_Movement_Tutorial ||
                GameManager.Instance.currentPhase == GameManager.GamePhase.P1_Dodging_Tutorial ||
                GameManager.Instance.currentPhase == GameManager.GamePhase.P1_Shooting_Tutorial ||
                GameManager.Instance.currentPhase == GameManager.GamePhase.P1_XP_Collection_Tutorial ||
                GameManager.Instance.currentPhase == GameManager.GamePhase.P2_Setup_Tutorial ||
                GameManager.Instance.currentPhase == GameManager.GamePhase.BasicTutorial_Reward ||
                GameManager.Instance.currentPhase == GameManager.GamePhase.BasicTutorial_Placement
                )
            {
                StartCoroutine(InvincibilityCoroutine());
                
                if (!GameManager.Instance.inTutorialDeath) {
                    Debug.Log("[PlayerHealth] Invoking Tutorial Death");
                    Tutorial_OnDeath?.Invoke(GameManager.Instance.currentPhase);
                }
                else { }

            }
            else { Die(); }

        }
        else
        {
            StartCoroutine(InvincibilityCoroutine());
        }

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

    public void AddPlayerHealth()
    {
        currentHealth = Mathf.Min(currentHealth + InventoryManager.PLAYER_HEALTH_INCREASE, maxHealth);
        UpdateHPBar();
        GameManager.Instance.UIManager.DamageEffect(currentHealth);
    }

    public void AddPlayerHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHPBar();
        //GameManager.Instance.UIManager.DamageEffect(currentHealth);
    }

    private void UpdateHPBar()
    {
        if (hpBar) hpBar.value = (float)currentHealth/maxHealth;
        if (playerHpBar) playerHpBar.value = (float)currentHealth/maxHealth;
    }
}
