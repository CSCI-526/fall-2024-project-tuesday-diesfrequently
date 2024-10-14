using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickUpgrade : MonoBehaviour
{   
    [SerializeField] public int goldRequired = 2;
    [SerializeField] public string buildingName;
    [SerializeField] public GameObject indicator;
    private int id;
    private bool upgradeable = true;
    private PlayerLevels playerLevelObject;
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

    public bool PayCostIfPossible(){
        bool haveInventory = GameManager.Instance.InventoryManager.CanPlacebuilding(buildingName);

        if (haveInventory && playerLevelObject.gold >= goldRequired){
            GameManager.Instance.InventoryManager.TryPlaceBuilding(buildingName);
            playerLevelObject.gold -= goldRequired;
            return true;
        }
        return false;
        
    }

    public void upgrade(){
        if(PayCostIfPossible()){
            GameObject indicate = Instantiate(indicator);
            indicate.transform.position = new Vector3(transform.position.x, transform.position.y + 2.3f, transform.position.z);
            gameObject.GetComponent<turretShoot>().upgrade();
            upgradeable = false;

            GameManager.Instance.UIManager.HideUpgradeScreen();
        }else{
            Debug.Log("No enough resources");
            GameManager.Instance.UIManager.HideUpgradeScreen();
        }
        GameManager.Instance.UIManager.UpdateUI();

    }

}
