using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvester : Building
{
    public GameObject currentPlaceableObject;
    [SerializeField] public float radius = 4.0f;
    bool IsValidPlacement(float radius = 4.0f)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);//, -1, QueryTriggerInteraction.Collide);
        foreach (var other in hitColliders)
        {
            if(other.gameObject == this.gameObject)
            {
                continue;
            }
            GetComponent<RangeIndicator>().ShowIndicator();
            if (other.gameObject.GetComponent<Harvester>() != null)
            {
                // other.gameObject.GetComponent<RangeIndicator>().ShowIndicator();
                Debug.Log("Invalid Placement: too close to existing harvester");
                return false;
            } else if (other.gameObject.GetComponent<turretShoot>() != null)
            {
                // other.gameObject.GetComponent<RangeIndicator>().ShowIndicator();
                Debug.Log("Invalid Placement: too close to turret");
                return false;
            } 
        }
        return true;
    }

    public override bool CanPlace()
    {
        return IsValidPlacement(radius);
    }

    public override void OnPlace()
    {
        base.OnPlace();
        GetComponent<Collider>().enabled = true;
        GetComponent<RangeIndicator>().HideIndicator();
    }
}