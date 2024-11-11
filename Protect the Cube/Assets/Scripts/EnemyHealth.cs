 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{

    [SerializeField] public float maxHealth = 5;
    [SerializeField] public float invincibilityDuration = 0.01f;
    [SerializeField] public GameObject exp;

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

    //[SerializeField] public float MoreXpDropBreakpoints = 10.0f;  
    void Start()
    {
        currentHealth = maxHealth;
        GameManager.Instance.WaveManager.enemyCount++;
        GameManager.Instance.WaveManager.enemies.Add(this.gameObject);
        animator = GetComponent<Animator>();    
        
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
    }

    public void TakeDamage(float damage)
    {
        if (!isInvincible) // Only take damage if not currently invincible
        {
            if (animator != null)
            {
                animator.SetTrigger("Damage");
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
        GameManager.Instance.WaveManager.enemyCount--;
        GameManager.Instance.WaveManager.enemies.Remove(this.gameObject);
        DropExp();
        //Debug.Log(GameManager.Instance.WaveManager.enemyCount);
        Destroy(gameObject);
    }

    public void DropExp(){
        int xpDrop = Random.Range(minXpDrop, maxXpDrop);

        if (Random.Range(0.0f, 1.0f) <= xpDropRatePercent){
            // float distanceFromNexus = Vector3.Distance(transform.position, GameManager.Instance.Nexus.transform.position);
            // if (distanceFromNexus > MoreXpDropBreakpoints)
            // {
            //xpDrop += (int)(distanceFromNexus / MoreXpDropBreakpoints); // Increase XP drop if the enemy is far from the nexus
            // }
            for (int i = 0; i < xpDrop; i++){
                GameObject xp = Instantiate(exp);
                xp.transform.position = new Vector3(transform.position.x+Random.Range(-1*1, 1), transform.position.y, transform.position.z+Random.Range(-1*1, 1));;
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
}
