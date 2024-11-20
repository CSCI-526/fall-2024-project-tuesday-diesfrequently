 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class Ore : MonoBehaviour
{
    [Header("PUBLIC Modifiable Constants")]
    [SerializeField] public int BASE_ORE_DROP_XP = 3;
    [SerializeField] public float MAX_HEALTH = 5;

    // Prefabs for Instantiation
    [SerializeField] public GameObject expPrefab;

    [SerializeField] public int xpDropDistanceThreshold = 10;
    [SerializeField] public int xpDropDistanceMultiplyer = 1;
    [SerializeField] public int xpDistanceGroups = 3;
    private int xpDistanceGroup;

    [SerializeField] public float xpDropRatePercent = 1.0f;  


    [SerializeField] public bool showHPBar = true;
    [SerializeField] public GameObject hpCanvas;
    [SerializeField] public Slider hpBar;
    [SerializeField] public Vector3 hpBarOffset = new Vector3(0.0f,3.0f,0.0f);

    // Ore Private Descriptors
    [Header("PRIVATE Ore Descriptors")]
    GameObject _nexus; // private Nexus reference
    [SerializeField] private float currentHealth; // stores current health of ore
    [SerializeField] private int adjustedDropAmount; // stores modified # of dropped reward (Gold or XP)
    [SerializeField] private float distanceFromNexus; // stores distance (units) from nexus per Ore
    [SerializeField] public int oreTier; // what tier the ore is in


    void Start()
    {
        _nexus = GameManager.Instance.Nexus;
        currentHealth = MAX_HEALTH;
        adjustedDropAmount = BASE_ORE_DROP_XP;
        distanceFromNexus = Vector3.Distance(transform.position, _nexus.transform.position);

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
        if(hpCanvas) UpdateHPBarTransform();
    }

    public void SetOreTier(int tier) { oreTier = tier; }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        UpdateHPBar();
        if (currentHealth <= 0) Die();
    }

    public void Die()
    {
        DropExp();
        GameManager.Instance.OreManager.DestroyOreEntity(this.gameObject);
        Destroy(gameObject);
    }

    public void DropExp(){
            if (Random.Range(0.0f, 1.0f) <= xpDropRatePercent)
            {

            adjustedDropAmount *= xpDropDistanceMultiplyer * (int)Mathf.Min(xpDistanceGroup, xpDistanceGroups); // Increase XP drop if the enemy is far from the nexus
                
                for (int i = 0; i < adjustedDropAmount; i++)
                {
                    GameObject xp = Instantiate(expPrefab);
                    xp.transform.position = new Vector3(transform.position.x + Random.Range(-1 * 1, 1), transform.position.y, transform.position.z + Random.Range(-1 * 1, 1)); ;
                }
            }
        
    }

    public void UpdateHPBar() { if (hpBar != null) hpBar.value = currentHealth / MAX_HEALTH; }

    private void UpdateHPBarTransform()
    {
        Vector3 fromCamera = (transform.position - Camera.main.transform.position).normalized;
        hpCanvas.transform.position = transform.position + hpBarOffset + fromCamera * 0.5f; 

        //hpCanvas.transform.LookAt(Camera.main.transform.position);
    }

}
