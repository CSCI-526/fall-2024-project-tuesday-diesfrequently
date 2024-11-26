using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class Harvester : Building
{
    public GameObject currentPlaceableObject;
    [SerializeField] public float radius = 4.0f;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
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

            if (other.gameObject.GetComponent<Harvester>() != null || other.transform.root.gameObject.GetComponent<Harvester>() != null)
            {
                // other.gameObject.GetComponent<RangeIndicator>().ShowIndicator();
                Debug.Log("Invalid Placement: too close to existing harvester");
                anim.SetTrigger("Damage");
                return false;
            } else if (other.gameObject.GetComponent<turretShoot>() != null)
            {
                // other.gameObject.GetComponent<RangeIndicator>().ShowIndicator();
                anim.SetTrigger("Damage");
                Debug.Log("Invalid Placement: too close to turret");
                return false;
            } 
        }

        // turn off animation for top and bottom
        if (anim != null)
        {
            anim.enabled = false; // Disable the Animator
            anim.Play("New State", -1, 0f); // Reset to the Idle state
            anim.Update(0); // Force an update to apply changes
            anim.enabled = true; // Re-enable the Animator
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