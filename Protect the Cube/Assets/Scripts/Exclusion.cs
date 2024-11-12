using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exclusion : MonoBehaviour
{
    // want to detect if an object already exist in this position
    // to prevent the player from placing a building on top of another building

    // make the box size the size of the game object
    public Vector3 boxSize = new Vector3(1, 1, 1);
    // layer to check for existing objects 
    public LayerMask objectLayer;

    private static List<GameObject> previousColliders = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static bool CheckForExclusion(GameObject placeableObject)
    {
        // always show the range indicator
        if (placeableObject.GetComponent<RangeIndicator>() == null) { Debug.Log("Rangeindicator is Null"); }
        placeableObject.GetComponent<RangeIndicator>().ShowIndicator();

        Vector3 boxSize = placeableObject.transform.localScale;
        Vector3 position = placeableObject.transform.position;
        LayerMask objectLayer = 1 << placeableObject.layer;
        Collider[] colliders = Physics.OverlapSphere(position, boxSize.magnitude/2, objectLayer);
        if (colliders.Length > 1)
        {
            // show exclusion indicator if there are colliding buildings
            placeableObject.GetComponent<RangeIndicator>().ShowIndicator();
            Renderer r = placeableObject.gameObject.GetComponent<Renderer>();
            Debug.Log("current color: " + r.material.color);
            r.material.color = Color.green;
            Debug.Log("new color: " + r.material.color);
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