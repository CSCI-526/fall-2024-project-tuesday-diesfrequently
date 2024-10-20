using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvester : Building
{
    [SerializeField] public float radius = 6.0f;
    bool IsValidPlacement(float radius = 6.0f)
    {
        Debug.Log("Checking for harvestor exclusion");
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);//, -1, QueryTriggerInteraction.Collide);
        foreach (var other in hitColliders)
        {
            if (other.gameObject.GetComponent<Harvester>() != null)
            {
                Debug.Log("Invalid Placement: too close to existing harvester");
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