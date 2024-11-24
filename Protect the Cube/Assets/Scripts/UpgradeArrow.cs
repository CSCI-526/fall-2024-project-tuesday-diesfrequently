using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeArrow : MonoBehaviour
{
    private GameObject parentTurret;
    [SerializeField] public GameObject clickableObject;

    public void SetParentTurret(GameObject turret)
    {
        parentTurret = turret;
    }

    public void UpgradeTurret()
    {
        Debug.Log("upgrade attempted!");
        if (parentTurret != null)
        {
            ClickUpgrade clickUpgrade = parentTurret.GetComponent<ClickUpgrade>();
            if(clickUpgrade != null)
            {
                clickUpgrade.AttemptUpgrade();
            }
        }
        else
        {
            Debug.Log("upgrade failed: parent turret not assigned");
        }
    }

    private void Update()
    {
        ShowRangeWhenHovered();
        UpgradeWhenClicked();
    }

    private void UpgradeWhenClicked()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);
            for (int i = 0; i < results.Count; ++i)
            {
                if (clickableObject != null && results[i].gameObject == clickableObject)
                {
                    UpgradeTurret();
                }
            }
        }
    }

    private void ShowRangeWhenHovered()
    {
        RangeIndicator[] rangeIndicators = parentTurret.GetComponents<RangeIndicator>();
        for (int j = 0; j < rangeIndicators.Length; ++j)
        {
            if (rangeIndicators[j].useTurretRange)
            {
                rangeIndicators[j].HideIndicator();
            }
        }

        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);
        for (int i = 0; i < results.Count; ++i)
        {
            if (clickableObject != null && results[i].gameObject == clickableObject)
            {
                for(int j = 0; j < rangeIndicators.Length; ++j)
                {
                    if(rangeIndicators[j].useTurretRange)
                    {
                        rangeIndicators[j].ShowIndicator();
                    }
                }
            }
        }
    }
}
