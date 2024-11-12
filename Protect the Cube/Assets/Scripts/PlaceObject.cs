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
    [SerializeField] protected float rotateIncrement = 10.0f;
    private bool placedFirstTurret = false;
    static private bool isTurretPickedUp = false;
    static private bool isFirstTurretPlaced = false;


    private void Update()
    {

        HandleNewObjectHotkey();

        if (currentPlaceableObject != null)
        {
            isTurretPickedUp = true;
            //rangeIndicator.SetActive(true);
            //rangeIndicator.transform.localScale = new Vector3(currentPlaceableObject.GetComponent<turretShoot>().maxRange, rangeIndicator.transform.localScale.y, currentPlaceableObject.GetComponent<turretShoot>().maxRange);
            MoveCurrentObjectToMouse();
            RotateFromMouseWheel();
            RotateFromQE();
            ReleaseIfClicked();
        }

    }


    public void CancelPlace()
    {
        isTurretPickedUp = false;
        if (currentPlaceableObject == null) return;
        Destroy(currentPlaceableObject);
        GameManager.Instance.UIManager.SetCursorCrosshair();

    }
    private void HandleNewObjectHotkey()
    {
        for (int i = 0; i < placeableObjectPrefabs.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + 1 + i))
            {
                // don't interrupt another placing
                if (currentPlaceableObject != null) Destroy(currentPlaceableObject);

                string bName = placeableObjectPrefabs[i].GetComponent<Building>().buildingName;
                StartPlacingIfPossible(placeableObjectPrefabs[i], bName);
                break;
            }
        }
    }

    public void StartPlacingIfPossible(GameObject turret, string bName)
    {
        if (CanStartPlacing(bName))
        {
            SelectTurretPlace(turret);
        }
    }

    private bool CanStartPlacing(string bName)
    {
        bool canPlace = GameManager.Instance.InventoryManager.isInventoryAvailable(bName);
        canPlace &= GameManager.Instance.UIManager.rewardMenu.activeSelf == false;
        canPlace &= GameManager.Instance.UIManager.pauseUI.activeSelf == false;
        return canPlace; // if true, returns 1 and removed item from inventory
    }

    private bool SelectTurretPlace(GameObject turret)
    {
        GameManager.Instance.UIManager.SetCursorHand();
        currentPlaceableObject = Instantiate(turret);

        Debug.Log("[PlaceObject] turret has been created");
        if (placedFirstTurret == false)
        {
            placedFirstTurret = true;
            GameManager.Instance.UIManager.HideSelectGunTutorial();
        }
        return true;
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

        if (currentPlaceableObject.GetComponent<turretShoot>() != null) //make exclusion only apply for turrets
        {
            canPlace &= Exclusion.CheckForExclusion(currentPlaceableObject);
        }

        if (Input.GetMouseButtonDown(0) && canPlace)
        {
            GameManager.Instance.InventoryManager.TryUseInventoryItem(b.buildingName);
            b.OnPlace();
            isFirstTurretPlaced = true;
            currentPlaceableObject = null;
            GameManager.Instance.UIManager.SetCursorCrosshair();

            //rangeIndicator.SetActive(false);
        }
        else if (Input.GetMouseButtonDown(1))
        {

            CancelPlace();
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
        RotateBuilding();
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

    static public bool turretPickedUp() { return isTurretPickedUp; }
    static public bool firstTurretPlaced() { return isFirstTurretPlaced; }

}
