using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] public bool placed = false;
    [SerializeField] public string buildingName = "missing name";
    [SerializeField] public string buildingDesc = "missing description";

    public virtual void OnPlace()
    {
        placed = true;
        foreach(Collider c in GetComponentsInChildren<Collider>())
        {
            c.enabled = true;
        }
    }

    public virtual void Boost()
    {

    }
}
