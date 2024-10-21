using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTower : Building
{

    [SerializeField] public float radius;

    [SerializeField] public float slowRate;

    override public void OnPlace()
    {
        placed = true;
        GetComponent<Collider>().enabled = true;
        HideIndicators();
    }

    private void Update()
    {
        if (placed) {
            SlowEnemies(radius, slowRate);
        }
    }
    void SlowEnemies(float radius = 12.0f, float slowRate = 0.25f)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);//, -1, QueryTriggerInteraction.Collide);
        foreach (var other in hitColliders)
        {
            if (other.gameObject.GetComponent<EnemyMove>() != null)
            {
                //Debug.Log("slowing enemy");
                other.gameObject.GetComponent<EnemyMove>().GetSlowed(slowRate);
                //Debug.Log("Boosting Turret!");
            }
        }
    }
}
