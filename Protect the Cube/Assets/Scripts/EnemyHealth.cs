 using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{

    [SerializeField] public float maxHealth = 5;
    [SerializeField] public float invincibilityDuration = 0.01f;
    [SerializeField] public GameObject exp;
    [SerializeField] public GameObject magnet;

    [SerializeField] public int maxXpDrop = 3;
    [SerializeField] public int minXpDrop = 5;
    [SerializeField] public float currentHealth;
    [SerializeField] public float xpDropRatePercent = 1.0f;  

    private bool isInvincible = false;
    private Animator animator;

    [SerializeField] public bool showHPBar = true;
    [SerializeField] public GameObject hpCanvas;
    [SerializeField] public Slider hpBar;
    [SerializeField] public Vector3 hpBarOffset = new Vector3(0.0f,3.0f,0.0f);
    [SerializeField] public bool countInWave = true;
    [SerializeField] public string enemyName = "None";

    [SerializeField] public float upgradeChance = 0.01f;
    [SerializeField] public float upgradeHealth = 1.0f;
    [SerializeField] public Material upgradeColorMaterial;
    
    [SerializeField] public bool needToUpgrade = false;

    private bool isUpgraded = false;

    
    private bool isTutorialEXPDrop = false;
    private int tutorialEXPAmt = 0; 

    //[SerializeField] public float MoreXpDropBreakpoints = 10.0f;  
    void Start()
    {
        if(needToUpgrade){
            LevelUp();
        }
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();  
        if(isUpgraded && animator != null)
        {
            animator.SetTrigger("UpgradedNormalState");

        }
        if(hpCanvas)
        {
            hpCanvas.SetActive(showHPBar);

            ConstraintSource cs = new ConstraintSource();
            cs.weight = 1.0f;
            cs.sourceTransform = Camera.main.transform;

            hpCanvas.GetComponent<RotationConstraint>().AddSource(cs);
            UpdateHPBar();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(hpCanvas)
        {
            UpdateHPBarTransform();
        }

        
        // if (animator.GetCurrentAnimatorStateInfo(0).IsName("DamageUpgradedEnemy") && animator.enabled && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f){
        //     animator.enabled = false;
        // }

    }

    public void LevelUp()
    {
        maxHealth = upgradeHealth;
        currentHealth = upgradeHealth;
        //GetComponent<MeshRenderer>().material = upgradeColor;
        //GetComponent<Material>().color = upgradeColor;
        GetComponent<Renderer>().material = upgradeColorMaterial;
        isUpgraded = true;
        //Debug.Log("Upgraded Enemy!");
        if (animator != null)
        {
            animator.SetTrigger("UpgradedNormalState");
        }
        this.gameObject.transform.localScale = new Vector3(1.5f * this.gameObject.transform.localScale.x, this.gameObject.transform.localScale.y, 1.5f * this.gameObject.transform.localScale.z);
        
        
    }

    public void TakeDamage(float damage)
    {
        if (!isInvincible) // Only take damage if not currently invincible
        {
            if (animator != null)
            {
                // animator.enabled = true;
                if (isUpgraded){
                    animator.SetTrigger("UpgradedDamage");
                }
                else{
                    animator.SetTrigger("Damage");

                }
            }
            currentHealth -= damage;
            UpdateHPBar();
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
        if(countInWave) GameManager.Instance.WaveManager.KillEnemyEntity(this.gameObject, GameManager.Instance.WaveManager.GetEnemyIDX(enemyName));
        if (enemyName == "SpawnerBossEnemy") { DropMagnet(); }
        else { DropExp(); }
        
        Destroy(gameObject);
    }

    public void DropMagnet() {
        int wave = GameManager.Instance.WaveManager.wave_count;
        int lastWaveMagnetSpawned = GameManager.Instance.WaveManager.lastWaveMagnetSpawned;
        int wavesBetweenMagnetSpawns = GameManager.Instance.WaveManager.wavesBetweenMagnetSpawns;
        if (wave >= lastWaveMagnetSpawned + wavesBetweenMagnetSpawns)
        {
            GameManager.Instance.WaveManager.lastWaveMagnetSpawned = wave;
            GameObject magnetPowerUp = Instantiate(magnet);
            magnetPowerUp.transform.position = new Vector3(transform.position.x + Random.Range(-1 * 1, 1), transform.position.y, transform.position.z + Random.Range(-1 * 1, 1)); ;
        }
    }

    public void DropExp(){

        if (isTutorialEXPDrop)
        {
            for (int i = 0; i < tutorialEXPAmt; i++)
            {
                GameObject xp = Instantiate(exp);
                xp.transform.position = new Vector3(transform.position.x + Random.Range(-1 * 1, 1), transform.position.y, transform.position.z + Random.Range(-1 * 1, 1)); ;
            }
        }
        else
        {
            int xpDrop = Random.Range(minXpDrop, maxXpDrop);

            if (Random.Range(0.0f, 1.0f) <= xpDropRatePercent)
            {
                // float distanceFromNexus = Vector3.Distance(transform.position, GameManager.Instance.Nexus.transform.position);
                // if (distanceFromNexus > MoreXpDropBreakpoints)
                // {
                //xpDrop += (int)(distanceFromNexus / MoreXpDropBreakpoints); // Increase XP drop if the enemy is far from the nexus
                // }
                for (int i = 0; i < xpDrop; i++)
                {
                    GameObject xp = Instantiate(exp);
                    xp.transform.position = new Vector3(transform.position.x + Random.Range(-1 * 1, 1), transform.position.y, transform.position.z + Random.Range(-1 * 1, 1)); ;
                }
            }
        }
    }

    public void UpdateHPBar()
    {
        if(hpBar)
        {
            hpBar.value = currentHealth / maxHealth;
        }
    }
    private void UpdateHPBarTransform()
    {
        Vector3 fromCamera = (transform.position - Camera.main.transform.position).normalized;
        hpCanvas.transform.position = transform.position + hpBarOffset + fromCamera * 0.5f; 

        //hpCanvas.transform.LookAt(Camera.main.transform.position);
    }

    public void ActivateTutorialEXPDrop(int exp_amount) {
        isTutorialEXPDrop = true;
        tutorialEXPAmt = exp_amount;
    }
    public void DeactivateTutorialEXPDrop() { isTutorialEXPDrop = false; }
}
