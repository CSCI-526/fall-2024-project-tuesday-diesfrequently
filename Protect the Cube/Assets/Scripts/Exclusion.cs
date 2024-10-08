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
        Vector3 boxSize = placeableObject.transform.localScale;
        Vector3 position = placeableObject.transform.position;
        LayerMask objectLayer = 1 << placeableObject.layer;

        //Collider[] colliders = Physics.OverlapBox(position, boxSize, Quaternion.identity, objectLayer);
        Collider[] colliders = Physics.OverlapSphere(position, boxSize.magnitude, objectLayer);
        if (colliders.Length > 1)
        {
            foreach (var other in colliders)
            {
                if (other.gameObject.GetComponent<turretShoot>() != null)
                {
                    other.gameObject.GetComponent<Building>().ShowIndicators(1.0f);
                    Debug.Log("Invalid Placement: too close to existing building");
                    return false;
                }
            }
        }
        return true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }
}
