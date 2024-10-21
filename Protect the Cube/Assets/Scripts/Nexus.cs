using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Nexus : MonoBehaviour
{
    [SerializeField] public bool spawnXP;
    [SerializeField] public int health;
    [SerializeField] public int maxHealth;
    [SerializeField] public float xpSpawnInterval;
    
    [SerializeField] protected GameObject XP;
    [SerializeField] public Vector3 xpSpawnOffset;

    [SerializeField] public bool triggerGameOver = false;
    [SerializeField] private Animator animator;

    private float timeSinceLastSpawn = 0.0f;
    public delegate void NexusEvent();
    public event NexusEvent OnTakeDamage;

    [SerializeField] public Slider hpBar;

    public GameObject indicator = null;

    // Start is called before the first frame update
    private void Start()
    {
        GameManager.Instance.Nexus = this.gameObject;
        health = maxHealth;
        animator = GetComponent<Animator>();

        if(hpBar)
        {
            //hpBar.value = 1;
        }
    }
    public void TakeDamage(int amount = 1)
    {
        animator.SetTrigger("Damage");
        health -= amount;
        GameManager.Instance.AnalyticsManager.UpdateHitpointLossWave(10);
        GameManager.Instance.UIManager.UpdateUI();
        if (indicator == null)
        {
            Vector3 screenpos = Camera.main.WorldToScreenPoint(transform.position);

            if (!(screenpos.z > 0 && screenpos.x > 0 && screenpos.y > 0 && screenpos.x < Screen.width && screenpos.y < Screen.height)) // on screen
            {
                OffScreenIndicator.Instance.GetIndicator(gameObject);
                Debug.Log("Indicator Assigned");
            }
        }
        else
        {
            Indicator ind = indicator.GetComponent<Indicator>();
            ind.timeLeft += ind.revealTime;
            Debug.Log("Indicator Already Assigned");
        }

        if (OnTakeDamage != null)
        {
            OnTakeDamage();
        }
        if (health <= 0)
        { 
            if(triggerGameOver)
            {
                GameManager.Instance.TriggerGameOver();
            }
            //gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    public void Update()
    {
        if(spawnXP)
        {
            timeSinceLastSpawn += Time.deltaTime;
            if (timeSinceLastSpawn > xpSpawnInterval)
            {
                timeSinceLastSpawn = 0.0f;
                Instantiate(XP, transform.position + xpSpawnOffset, Quaternion.identity);
            }
        }
        if (hpBar)
        {
            hpBar.value = (float)health / maxHealth;
        }
    }
}
