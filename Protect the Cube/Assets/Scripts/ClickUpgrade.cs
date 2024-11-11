using System.Collections;
using System.Collections.Generic;
// using UnityEditor.PackageManager;
using UnityEngine;

public class ClickUpgrade : MonoBehaviour
{   
    [SerializeField] public int goldRequired = 4;
    [SerializeField] public string buildingName;
    [SerializeField] public GameObject indicator;
    [SerializeField] public GameObject upgradeArrow;
    private int id;
    private bool upgradeable = true;
    private PlayerLevels playerLevelObject;
    public List<GameObject> turrets;

    private int level = 0;
    // Start is called before the first frame update
    void Start()
    {
        id  = gameObject.GetInstanceID();
        playerLevelObject = GameObject.FindWithTag("Player").GetComponent<PlayerLevels>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown(){
        if(upgradeable && PayCostIfPossible(goldRequired)){
            // GameManager.Instance.UIManager.ShowUpgradeScreen();
            upgrade();
            GameManager.Instance.UIManager.updateUpgradeUI(buildingName, goldRequired, id);
            Debug.Log("upgrade");
        }else{
            Debug.Log("no enough resources");
        }
    }

    public bool PayCostIfPossible(int gold){
        if (playerLevelObject.currentGold >= goldRequired){
            playerLevelObject.currentGold -= goldRequired;
            GameManager.Instance.AnalyticsManager.UpdatePlayerSpentGold(goldRequired);
            return true;
        }
        return false;
        
    }

    public void upgrade(){
        // if(PayCostIfPossible(goldRequired)){
            level++;
            goldRequired += level*3;
            GameObject indicate = Instantiate(indicator);
            indicate.transform.position = new Vector3(transform.position.x, transform.position.y + 2.0f + level/5.0f, transform.position.z);
            
            gameObject.GetComponent<turretShoot>().upgrade(level, buildingName);
            if(level == 3){
                upgradeable = false;
            }
            // GameManager.Instance.UIManager.HideUpgradeScreen();
        // }else{
        //     Debug.Log("No enough resources");
        //     GameManager.Instance.UIManager.HideUpgradeScreen();
        // }
        GameManager.Instance.UIManager.UpdateUI();
        CheckForUpgradeableTurrets();

    }


    public void CheckForUpgradeableTurrets()
    {
        // update the list of turrets
        turrets = new List<GameObject>(GameObject.FindGameObjectsWithTag("Turret"));
        foreach (GameObject turret in turrets)
        {
            Transform existingArrow = turret.transform.Find("UpgradeArrow");
            // Check if the turret can be upgraded based on player's gold
            if (goldRequired <= playerLevelObject.currentGold)
            {
                if(existingArrow == null){
                    ShowUpgradeArrow(turret); // Show the upgrade arrow if affordable
                }
            }
            else
            {
                if(existingArrow != null){
                    HideUpgradeArrow(turret); // Hide the upgrade arrow if not affordable
                }
            }
        }
    }

    void ShowUpgradeArrow(GameObject turret)
    {
        Debug.Log("Showing upgrade arrow");
        // create as child of turret
        GameObject arrow = Instantiate(upgradeArrow, turret.transform);
        arrow.name = "UpgradeArrow"; // Give it a name to identify it later
        arrow.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        // Set the position above the turret
        arrow.transform.localPosition = Vector3.up * 2.2f;

    }

    void HideUpgradeArrow(GameObject turret)
    {
        Transform existingArrow = turret.transform.Find("UpgradeArrow");
        Destroy(existingArrow.gameObject);
    }

}
