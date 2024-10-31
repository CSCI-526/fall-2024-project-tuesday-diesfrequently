using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceObject : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject[] placeableObjectPrefabs;
    [SerializeField] private GameObject rangeIndicator;
    public GameObject currentPlaceableObject;
    private float buildingRotation;
    private int currentPrefabIndex = -1;
    [SerializeField] protected float rotateIncrement = 10.0f;

    private void Update()
    {

        HandleNewObjectHotkey();

        if (currentPlaceableObject != null)
        {
            //rangeIndicator.SetActive(true);
            //rangeIndicator.transform.localScale = new Vector3(currentPlaceableObject.GetComponent<turretShoot>().maxRange, rangeIndicator.transform.localScale.y, currentPlaceableObject.GetComponent<turretShoot>().maxRange);
            MoveCurrentObjectToMouse();
            RotateFromMouseWheel();
            RotateFromQE();
            ReleaseIfClicked();
        }

    }
    
    

    private void HandleNewObjectHotkey()
    {
        for (int i = 0; i < placeableObjectPrefabs.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + 1 + i))
            {
                if (PressedKeyOfCurrentPrefab(i))
                {
                    Destroy(currentPlaceableObject);
                    currentPrefabIndex = -1;
                }
                else
                {
                    if (currentPlaceableObject != null)
                    {
                        Destroy(currentPlaceableObject);
                    }

                    string bName = placeableObjectPrefabs[i].GetComponent<Building>().buildingName;
                    bool canPlace = GameManager.Instance.InventoryManager.isInventoryAvailable(bName);
            
                    if (canPlace)
                    {
                        currentPlaceableObject = Instantiate(placeableObjectPrefabs[i]);
                        currentPrefabIndex = i;
                    }
                }

                break;
            }
        }
    }

    private bool PressedKeyOfCurrentPrefab(int i)
    {
        return currentPlaceableObject != null && currentPrefabIndex == i;
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

    private void RotateFromMouseWheel()
    {
        //Debug.Log(Input.mouseScrollDelta);
        buildingRotation += Input.mouseScrollDelta.y;
        RotateBuilding();
    }

    private void ReleaseIfClicked()
    {
        Building b = currentPlaceableObject.GetComponent<Building>();
        bool canPlace = GameManager.Instance.InventoryManager.isInventoryAvailable(b.buildingName);
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
            GameManager.Instance.InventoryManager.TryUseInventoryItem(b.buildingName);
            b.OnPlace();
            currentPlaceableObject = null;
            //rangeIndicator.SetActive(false);
        }

    }

    private void RotateFromQE()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            buildingRotation -= 1;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            buildingRotation += 1;
        }
        RotateBuilding() ;
    }

    private void RotateBuilding()
    {
        if (currentPlaceableObject.GetComponent<Building>().rotatable)
        {
            currentPlaceableObject.transform.Rotate(Vector3.up, buildingRotation * rotateIncrement);
        }
        else
        {
            currentPlaceableObject.transform.rotation = Quaternion.identity;
        }
    }

}
