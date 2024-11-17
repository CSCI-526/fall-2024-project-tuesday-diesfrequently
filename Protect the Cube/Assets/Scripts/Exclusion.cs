using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exclusion : MonoBehaviour
{
    // want to detect if an object already exist in this position
    // to prevent the player from placing a building on top of another building
    private static List<GameObject> previousColliders = new List<GameObject>();
    private static string[] layers = new string[] { "Building", "Nexus" };

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static bool CheckForExclusion(GameObject placeableObject, Animator top, Animator bottom)
    {
        // always show the range indicator
        if (placeableObject.GetComponent<RangeIndicator>() == null) { Debug.Log("Rangeindicator is Null"); }
        placeableObject.GetComponent<RangeIndicator>().ShowIndicator();

        Vector3 boxSize = placeableObject.transform.localScale;
        Vector3 position = placeableObject.transform.position;

        LayerMask combinedLayerMask = LayerMask.GetMask(layers);
        // // print the layer mask
        // Debug.Log("Layer Mask: " + combinedLayerMask.value);
        Collider[] colliders = Physics.OverlapSphere(position, boxSize.magnitude / 2, combinedLayerMask);
        if (colliders.Length > 1)
        {
            // show exclusion indicator if there are colliding buildings
            placeableObject.GetComponent<RangeIndicator>().ShowIndicator();
            if (top != null){
                top.SetTrigger("Damage");
            } 
            if(bottom != null)
            {
                bottom.SetTrigger("Damage");
            }
            foreach (var collider in colliders)
            {
                if ((collider.gameObject != placeableObject) && (collider.gameObject.GetComponent<RangeIndicator>() != null))
                {
                    collider.gameObject.GetComponent<RangeIndicator>().ShowIndicator();
                    previousColliders.Add(collider.gameObject); 
                }
            } 
            return false;
        } else {
            // hide exclusion indicator if there are no colliding buildings
            placeableObject.GetComponent<RangeIndicator>().HideIndicator();
            // turn off animation for top and bottom
            if (top != null){
                top.enabled = false; // Disable the Animator
                top.Play("New State", -1, 0f); // Reset to the Idle state
                top.Update(0); // Force an update to apply changes
                top.enabled = true; // Re-enable the Animator
            }
            if(bottom != null)
            {
                bottom.enabled = false; // Disable the Animator
                bottom.Play("New State", -1, 0f); // Reset to the Idle state
                bottom.Update(0); // Force an update to apply changes
                bottom.enabled = true; // Re-enable the Animator
            }
            foreach (var obj in previousColliders)
            {
                obj.GetComponent<Building>().HideIndicators();
            }
            
            // Clear the list since there are no more collisions
            previousColliders.Clear();
            return true;
        }
    }
}