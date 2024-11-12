using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Nexus : MonoBehaviour
{
    [SerializeField] public  int NEXUS_MAX_HEALTH = 20;

    public int maxHealth { get; private set; }
    public int currentHealth { get; private set; }

    [Header("XP Settings")]
    [SerializeField] private bool spawnXP = true;
    [SerializeField] private float xpSpawnInterval = 5f;
    [SerializeField] private GameObject xpPrefab;
    [SerializeField] private Vector3 xpSpawnOffset;

    [Header("UI References")]
    [SerializeField] private Slider hpBar;

    [Header("Game Over Settings")]
    [SerializeField] private bool triggerGameOver = false;

    private Animator animator;
    private float timeSinceLastSpawn = 0.0f;
    private InventoryManager inventoryManager;
    public event Action<int> Analytics_OnNexusHPLoss;

    public delegate void NexusEvent();
    public event NexusEvent OnTakeDamage;
    public GameObject indicator = null;

    private void Awake()
    {
        maxHealth = NEXUS_MAX_HEALTH;
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // set Nexus GameObject to this scripts object (same reference)
        GameManager.Instance.Nexus = this.gameObject;

        Debug.Log(xpPrefab != null ? "XP Prefab is assigned." : "XP Prefab is missing!");

        // initialize HP Bar
        UpdateHPBar();
    }

    private void OnEnable() { StartCoroutine(DelayedSubscribeToEvents()); }

    private void OnDisable()
    {
        inventoryManager.Nexus_OnNexusHealthUpdate -= AddNexusHealth;
    }

    private IEnumerator DelayedSubscribeToEvents()
    {
        // Wait until GameManager instance and InventoryManager are initialized
        // yield return new WaitUntil(() => GameManager.Instance != null && GameManager.Instance.InventoryManager != null);
        yield return new WaitUntil(() => GameManager.Instance?.InventoryManager != null);

        inventoryManager = GameManager.Instance.InventoryManager;
        inventoryManager.Nexus_OnNexusHealthUpdate += AddNexusHealth;
    }

    private void Update() {
         HandleXPSpawn();
    }

    private void AddNexusHealth()
    {
        currentHealth = Mathf.Min(currentHealth + InventoryManager.NEXUS_HEALTH_INCREASE, maxHealth);
        Debug.Log("[Nexus] Updated currentHealth to: " + currentHealth);
        UpdateHPBar();
    }

    public void TakeDamage(int dmg_amount = 1)
    {
        // activate the animator
        animator.SetTrigger("Damage");

        // reduce currentHP nexus
        currentHealth = Mathf.Max(currentHealth - dmg_amount, 0);

        // Update HP Bar
        UpdateHPBar();

        // Update Analytics
        Analytics_OnNexusHPLoss?.Invoke(AnalyticsManager.TYPE_NEXUS_LOSS_HP);
        //GameManager.Instance.AnalyticsManager.UpdateHitpointLossWave(10);

        HandleOffScreenIndicator();

        OnTakeDamage?.Invoke();

        if (currentHealth <= 0 && triggerGameOver)
        {
            GameManager.Instance.TriggerGameOver();
            Destroy(gameObject);
        }

        GameManager.Instance.UIManager.UpdateUI();
    }

    private void HandleOffScreenIndicator()
    {
        if (indicator == null)
        {
            if (!IsOnScreen())
            {
                indicator = OffScreenIndicator.Instance.GetIndicator(gameObject);
                Debug.Log("Indicator Assigned");
            }
        }
        else
        {
            var ind = indicator.GetComponent<Indicator>();
            ind.timeLeft += ind.revealTime;
            Debug.Log("Indicator Already Assigned");
        }
    }

    private bool IsOnScreen()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        return screenPos.z > 0 && screenPos.x > 0 && screenPos.y > 0 && screenPos.x < Screen.width && screenPos.y < Screen.height;
    }

    private void HandleXPSpawn()
    {
        if (spawnXP && GameManager.Instance.CurrentPhase == GameManager.GamePhase.HandCraftedWaves)
        {
            timeSinceLastSpawn += Time.deltaTime;
            if (timeSinceLastSpawn >= xpSpawnInterval)
            {
                timeSinceLastSpawn = 0.0f;
                Instantiate(xpPrefab, transform.position + xpSpawnOffset, Quaternion.identity);
            }
        }
    }

    private void UpdateHPBar()
    {
        if (hpBar != null) hpBar.value = (float)currentHealth / maxHealth;
    }
}
