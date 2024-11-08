using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickAndDrop : MonoBehaviour, IPointerClickHandler
{
    public GameObject turretPrefab;
    public String bName;

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject player = GameManager.Instance.Player;
        PlaceObject placeObject = player.GetComponent<PlaceObject>();
        placeObject.StartPlacingIfPossible(turretPrefab, bName);

    }
}