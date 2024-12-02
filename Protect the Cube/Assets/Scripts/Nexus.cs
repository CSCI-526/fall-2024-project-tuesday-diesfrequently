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
    [SerializeField] private float xpHorizontalOffsetDistance = 1.0f;
    [SerializeField] private float xpVerticalOffsetDistance = 0.0f;

    [Header("UI References")]
    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider localhpBar;

    [Header("Game Over Settings")]
    [SerializeField] private bool triggerGameOver = false;

    private Animator animator;
    private float timeSinceLastSpawn = 0.0f;
    private InventoryManager inventoryManager;
    public event Action<int> Analytics_OnNexusHPLoss;

    public delegate void NexusEvent();
    public event NexusEvent OnTakeDamage;
    public GameObject indicator = null;

    public Animator healthbarAnim;

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

        //Debug.Log(xpPrefab != null ? "XP Prefab is assigned." : "XP Prefab is missing!");
        
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

    public void ResetNexusHealth()
    {
        Debug.Log("[NexusHealth] Resetting Nexus Health");
        currentHealth = NEXUS_MAX_HEALTH;
        UpdateHPBar();
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
        if(healthbarAnim != null)
        {
            healthbarAnim.SetTrigger("DamageBar");
        }

        // reduce currentHP nexus
        currentHealth = Mathf.Max(currentHealth - dmg_amount, 0);

        // Update HP Bar
        UpdateHPBar();
        GameManager.Instance.UIManager.UpdateUI();

        // Update Analytics
        Analytics_OnNexusHPLoss?.Invoke(AnalyticsManager.TYPE_NEXUS_LOSS_HP);
        //GameManager.Instance.AnalyticsManager.UpdateHitpointLossWave(10);

        HandleOffScreenIndicator();

        OnTakeDamage?.Invoke();

        if (currentHealth <= 0)
        {
            if(triggerGameOver)
            {
                GameManager.Instance.TriggerGameOver();
            }
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
        if (spawnXP && (GameManager.Instance.currentPhase == GameManager.GamePhase.HandCraftedWaves 
            || (GameManager.Instance.currentPhase == GameManager.GamePhase.DynamicWaves)))
        {
            timeSinceLastSpawn += Time.deltaTime;
            if (timeSinceLastSpawn >= xpSpawnInterval)
            {
                timeSinceLastSpawn = 0.0f;

                float randomX = UnityEngine.Random.Range(-1.0f, 1.0f);
                float randomZ = UnityEngine.Random.Range(-1.0f, 1.0f);
                
                Vector3 offset = new Vector3(randomX, 0.0f, randomZ);
                offset.Normalize();
                offset *= xpHorizontalOffsetDistance;
                //offset.y = transform.position.y;
                offset.y = xpVerticalOffsetDistance;
                /*
                float randomX = UnityEngine.Random.Range(0, 2) == 0
                    ? UnityEngine.Random.Range(-max_offset, -min_offset)
                    : UnityEngine.Random.Range(min_offset, max_offset);

                float randomZ = UnityEngine.Random.Range(0, 2) == 0
                    ? UnityEngine.Random.Range(-max_offset, -min_offset)
                    : UnityEngine.Random.Range(min_offset, max_offset);
                */
                //Vector3 spawnPosition = transform.position + new Vector3(randomX, transform.position.y, randomZ);

                Vector3 spawnPosition = transform.position + offset;
                Debug.Log("[Nexus] Position: " + spawnPosition);
                Instantiate(xpPrefab, spawnPosition + xpSpawnOffset, Quaternion.identity);
            }
        }
    }

    private void UpdateHPBar()
    {
        if (hpBar != null) hpBar.value = (float)currentHealth / maxHealth;
        if (localhpBar != null) localhpBar.value = (float)currentHealth / maxHealth;
    }
}
