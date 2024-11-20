using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class Tier3Ore : MonoBehaviour
{
    // Prefabs for Instantiation
    [Header("Prefabs")]
    [SerializeField] public GameObject ExpPrefab;
    [SerializeField] public GameObject GoldPrefab;

    private int oreT3ExpDropAmt; // stores # of exp orbs dropped on die()
    private int oreT3GoldDropAmt;// stores # of gold orbs dropped on die()
    private float oreDropZoneRadius; // stores how wide a zone to drop resources on die()


    [Header("HP Bar Debug")]
    [SerializeField] public bool showHPBar = true;
    [SerializeField] public GameObject hpCanvas;
    [SerializeField] public Slider hpBar;
    [SerializeField] public float hpBarVerticalOffset;

    // Ore Private Descriptors
    [Header("Ore Debug")]
    GameObject _nexus; // private Nexus reference
    private Vector3 hpBarOffset;
    [SerializeField] private float currentHealth; // stores current health of ore
    [SerializeField] private float distanceFromNexus; // stores distance (units) from nexus per Ore

    void Start()
    {
        _nexus = GameManager.Instance.Nexus;
        currentHealth = GameManager.Instance.OreManager.ORE_T3_MAX_HEALTH;
        oreT3ExpDropAmt = GameManager.Instance.OreManager.ORE_T3_DROP_XP;
        oreT3GoldDropAmt = GameManager.Instance.OreManager.ORE_T3_DROP_GOLD;
        oreDropZoneRadius = GameManager.Instance.OreManager.ORE_RESOURCE_DROP_ZONE_RADIUS;

        if (hpCanvas)
        {
            hpCanvas.SetActive(showHPBar);
            ConstraintSource cs = new ConstraintSource();
            cs.weight = 1.0f;
            cs.sourceTransform = Camera.main.transform;
            hpCanvas.GetComponent<RotationConstraint>().AddSource(cs);
            UpdateHPBar(); // update HP bar for the first time
        }


        hpBarVerticalOffset = 2.0f;
        hpBarOffset = new Vector3(0.0f, hpBarVerticalOffset, 0.0f);

        Transform capsuleTransform = transform.Find("Capsule");

        // removes "bulbs / eyes" on the Ore Capsule based on distance
        if (capsuleTransform != null)
        {
            int num_eyes_deactivated = 0;
            int total_num_eyes = capsuleTransform.childCount;

            foreach (Transform eye_transform in capsuleTransform)
            {
                GameObject eye_entity = eye_transform.gameObject;
                eye_entity.SetActive(false);
                if (num_eyes_deactivated > (total_num_eyes - GameManager.Instance.OreManager.ORE_T3_NUM_EYES)) { break; }
                num_eyes_deactivated += 1;
            }
        }
    }

    void Update() { if (hpCanvas != null) UpdateHPBarTransform(); }

    public void TakeDamage(float dmg_amount)
    {
        currentHealth = Mathf.Max(currentHealth - dmg_amount, 0);
        UpdateHPBar();
        if (currentHealth <= 0) Die();
    }

    public void Die()
    {
        DropExp();
        DropGold();
        GameManager.Instance.OreManager.DestroyOreEntity(this.gameObject);
        Destroy(gameObject);
    }

    public void DropExp()
    {
        for (int i = 0; i < oreT3ExpDropAmt; i++)
        {
            GameObject exp_entity = Instantiate(ExpPrefab);
            exp_entity.transform.position = new Vector3(transform.position.x + Random.Range(-oreDropZoneRadius, oreDropZoneRadius), transform.position.y, transform.position.z + Random.Range(-oreDropZoneRadius, oreDropZoneRadius)); ;
        }
    }

    public void DropGold()
    {
        for (int i = 0; i < oreT3GoldDropAmt; i++)
        {
            GameObject gold_entity = Instantiate(GoldPrefab);
            gold_entity.transform.position = new Vector3(transform.position.x + Random.Range(-oreDropZoneRadius, oreDropZoneRadius), transform.position.y, transform.position.z + Random.Range(-oreDropZoneRadius, oreDropZoneRadius)); ;
        }
    }

    public void UpdateHPBar() { if (hpBar != null) hpBar.value = currentHealth / GameManager.Instance.OreManager.ORE_T3_MAX_HEALTH; }

    private void UpdateHPBarTransform()
    {
        Vector3 fromCamera = (transform.position - Camera.main.transform.position).normalized;
        hpCanvas.transform.position = transform.position + hpBarOffset + fromCamera * 0.5f;
    }

}
