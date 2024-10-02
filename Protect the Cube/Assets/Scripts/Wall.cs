using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Building
{
    override public void OnPlace()
    {
        placed = true;
        foreach (Collider c in GetComponentsInChildren<Collider>())
        {
            c.enabled = true;
        }
    }
}
