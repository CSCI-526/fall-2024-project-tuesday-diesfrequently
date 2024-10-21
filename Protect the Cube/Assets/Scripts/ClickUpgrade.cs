using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class ClickUpgrade : MonoBehaviour
{   
    [SerializeField] public int goldRequired = 4;
    [SerializeField] public string buildingName;
    [SerializeField] public GameObject indicator;
    private int id;
    private bool upgradeable = true;
    private PlayerLevels playerLevelObject;

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
        if(upgradeable){
            GameManager.Instance.UIManager.ShowUpgradeScreen();
            GameManager.Instance.UIManager.updateUpgradeUI(buildingName, goldRequired, id);
        }else{
            Debug.Log("Upgraded");
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
        if(PayCostIfPossible(goldRequired)){
            level++;
            goldRequired += level*3;
            GameObject indicate = Instantiate(indicator);
            indicate.transform.position = new Vector3(transform.position.x, transform.position.y + 2.0f + level/5.0f, transform.position.z);
            
            gameObject.GetComponent<turretShoot>().upgrade(level, buildingName);
            if(level == 3){
                upgradeable = false;
            }
            GameManager.Instance.UIManager.HideUpgradeScreen();
        }else{
            Debug.Log("No enough resources");
            GameManager.Instance.UIManager.HideUpgradeScreen();
        }
        GameManager.Instance.UIManager.UpdateUI();

    }

}
