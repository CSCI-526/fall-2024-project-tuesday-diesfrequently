using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickUpgrade : MonoBehaviour
{   
    [SerializeField] public int materialNum = 2;
    [SerializeField] public string buildingName;
    [SerializeField] public GameObject indicator;
    private int id;
    private bool upgradeable = true;
    // Start is called before the first frame update
    void Start()
    {
        id  = gameObject.GetInstanceID();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown(){
        if(upgradeable){
            GameManager.Instance.UIManager.ShowUpgradeScreen();
            GameManager.Instance.UIManager.updateUpgradeUI(buildingName, materialNum, id);
        }else{
            Debug.Log("Upgraded");
        }
    }

    public void upgrade(){
        bool haveInventory = GameManager.Instance.InventoryManager.CanPlacebuilding(buildingName);
        if(haveInventory){
            if (GameManager.Instance.InventoryManager.TryPlaceBuilding(buildingName)){
                GameObject indicate = Instantiate(indicator);
                indicate.transform.position = new Vector3(transform.position.x, transform.position.y + 2.3f, transform.position.z);
                gameObject.GetComponent<turretShoot>().upgrade();
                upgradeable = false;

            }
        }else{
            Debug.Log("No enough resources");
        }
    }

}
