 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class GoldVein : MonoBehaviour
{

    [SerializeField] public float maxHealth = 5;
    [SerializeField] public GameObject exp;

    [SerializeField] public int xpDrop = 3;
    [SerializeField] public int xpDropDistanceThreshold = 10;
    [SerializeField] public int xpDropDistanceMultiplyer = 1;
    [SerializeField] public int xpDistanceGroups = 3;
    private int xpDistanceGroup;


    [SerializeField] public float currentHealth;
    [SerializeField] public float xpDropRatePercent = 1.0f;  


    [SerializeField] public bool showHPBar = true;
    [SerializeField] public GameObject hpCanvas;
    [SerializeField] public Slider hpBar;
    [SerializeField] public Vector3 hpBarOffset = new Vector3(0.0f,3.0f,0.0f);


    void Start()
    {
        currentHealth = maxHealth;

        float distanceFromNexus = Vector3.Distance(transform.position, GameManager.Instance.Nexus.transform.position);
        xpDistanceGroup = (int)((distanceFromNexus+xpDropDistanceThreshold) / xpDropDistanceThreshold);

        
        if(hpCanvas)
        {
            hpCanvas.SetActive(showHPBar);

            ConstraintSource cs = new ConstraintSource();
            cs.weight = 1.0f;
            cs.sourceTransform = Camera.main.transform;

            hpCanvas.GetComponent<RotationConstraint>().AddSource(cs);
            UpdateHPBar();
        }

        Transform capsuleTransform = transform.Find("Capsule");
        
        if (capsuleTransform != null)
        {
            int i = 0;
            foreach (Transform grandChild in capsuleTransform)
            {
                int numberOfChildren = capsuleTransform.childCount;
                GameObject grandChildObject = grandChild.gameObject;
                grandChildObject.SetActive(false);
                if (i > numberOfChildren - xpDistanceGroup*(numberOfChildren/xpDistanceGroups))
                {
                    break;
                }
                i += 1;
                
            }
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
        
        currentHealth -= damage;
        UpdateHPBar();
        if (currentHealth <= 0)
        {
            Die();
        }
        
        
    }



    public void Die()
    {
        
        DropExp();
        Destroy(gameObject);
    }

    public void DropExp(){

        
    
        

            if (Random.Range(0.0f, 1.0f) <= xpDropRatePercent)
            {
                
                xpDrop *= xpDropDistanceMultiplyer * (int)Mathf.Min(xpDistanceGroup, xpDistanceGroups); // Increase XP drop if the enemy is far from the nexus
                
                for (int i = 0; i < xpDrop; i++)
                {
                    GameObject xp = Instantiate(exp);
                    xp.transform.position = new Vector3(transform.position.x + Random.Range(-1 * 1, 1), transform.position.y, transform.position.z + Random.Range(-1 * 1, 1)); ;
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
