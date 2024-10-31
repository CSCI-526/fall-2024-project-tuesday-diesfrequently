 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

// tracks enemy health (current, max)
// handles dmg taken for an enemy
// implements invincibility logic to prevent enemy from taking dmg right after
// triggers Animation (when dmg taken) - visual UI hp bar
// calls die() on death ... dropping XP, updating WaveManager
// handles all logic with enemies dropping XP (randomness, xp instantiating)
// manages Health Bar UI (updates based on enemy health), visual bar displayed on top of enemy

// EnemyController, EnemyStats, HealthManager

public class EnemyHealth : MonoBehaviour
{

    [SerializeField] public float maxHealth = 5;
    [SerializeField] public float invincibilityDuration = 0.01f;
    [SerializeField] public GameObject exp;

    [SerializeField] public float xpDropRate = 0.5f;  
    [SerializeField] public int maxXpDrop = 3;
    [SerializeField] public int minXpDrop = 5;
    [SerializeField] public float currentHealth;
    private bool isInvincible = false;
    private Animator animator;

    [SerializeField] public bool showHPBar = true;
    [SerializeField] public GameObject hpCanvas;
    [SerializeField] public Slider hpBar;
    [SerializeField] public Vector3 hpBarOffset = new Vector3(0.0f,3.0f,0.0f);

    void Start()
    {
        currentHealth = maxHealth;
        //GameManager.Instance.WaveManager.enemyCount++;
        //GameManager.Instance.WaveManager.enemies.Add(this.gameObject);
        animator = GetComponent<Animator>();    
        
        if(hpCanvas)
        {
            hpCanvas.SetActive(showHPBar);

            ConstraintSource cs = new ConstraintSource { weight = 1.0f, sourceTransform = Camera.main.transform };
            //ConstraintSource cs = new ConstraintSource();
            //cs.weight = 1.0f;
            //cs.sourceTransform = Camera.main.transform;

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
    }

    public void TakeDamage(float damage)
    {
        if (!isInvincible) // Only take damage if not currently invincible
        {
            animator?.SetTrigger("Damage");
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
        GameManager.Instance.WaveManager.OnEnemyDeath(this.gameObject);
        //GameManager.Instance.WaveManager.enemyCount--;
        //GameManager.Instance.WaveManager.enemies.Remove(this.gameObject);
        DropExp();
        //Debug.Log(GameManager.Instance.WaveManager.enemyCount);
        Destroy(gameObject);
    }

    public void DropExp(){
        int xpDrop = Random.Range(minXpDrop, maxXpDrop);
        if (Random.Range(0.0f, 1.0f) <= xpDropRate){
            for (int i = 0; i < xpDrop; i++){
                GameObject xp = Instantiate(exp);
                xp.transform.position = new Vector3(
                    transform.position.x + Random.Range(-1*1, 1),
                    transform.position.y,
                    transform.position.z + Random.Range(-1*1, 1));;
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
    }
}
