using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickAndDrop : MonoBehaviour, IPointerClickHandler
{
    public GameObject turretPrefab;
    public GameObject currentPlaceableObject;
    public String bName;
    protected bool canPlace;
    public void OnPointerClick(PointerEventData eventData)
    {   
        Debug.Log("CLICKED");
        bool canPlace = GameManager.Instance.InventoryManager.HasBuilding(bName);
        if (canPlace){
            Debug.Log("INIT");
            currentPlaceableObject = Instantiate(turretPrefab);
        } 
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPlaceableObject != null)
        {
            MoveCurrentObjectToMouse();
            ReleaseIfClicked();
        }
    }

    private void MoveCurrentObjectToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layer_mask = LayerMask.GetMask("Ground");
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 10000, layer_mask))
        {
            currentPlaceableObject.transform.position = hitInfo.point;
            currentPlaceableObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            //rangeIndicator.transform.position = currentPlaceableObject.transform.root.position;
        }
    }

    private void ReleaseIfClicked()
    {
        Building b = currentPlaceableObject.GetComponent<Building>();
        bool canPlace = GameManager.Instance.InventoryManager.HasBuilding(b.buildingName);
        if (b.gameObject.GetComponent<Harvester>() != null)
        {
            canPlace &= currentPlaceableObject.GetComponent<Harvester>().CanPlace();
        }

        canPlace &= Exclusion.CheckForExclusion(currentPlaceableObject);

        if(currentPlaceableObject.GetComponent<turretShoot>() != null) //make exclusion only apply for turrets
        {
            canPlace &= Exclusion.CheckForExclusion(currentPlaceableObject);
        }
        
        if (Input.GetMouseButtonDown(0) && canPlace)
        {   
            Debug.Log("droped");
            GameManager.Instance.InventoryManager.TryPlaceBuilding(b.buildingName);
            b.OnPlace();
            currentPlaceableObject = null;
            Destroy(currentPlaceableObject);
            //rangeIndicator.SetActive(false);
        }
    }
}
