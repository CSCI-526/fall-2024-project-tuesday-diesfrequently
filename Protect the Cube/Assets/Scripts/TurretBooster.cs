using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBooster : Building
{
    [SerializeField] protected GameObject antenna;
    //[SerializeField] protected Collider aoeCollider;
    [SerializeField] public float rotationSpeed;
    [SerializeField] public float radius;

    override public void OnPlace()
    {
        GetComponent<Collider>().enabled = true;
        BoostNeighbors(radius);
    }

    private void Update()
    {
        antenna.transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
    void BoostNeighbors(float radius = 3.0f)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);//, -1, QueryTriggerInteraction.Collide);
        foreach (var other in hitColliders)
        {
            if (other.gameObject.GetComponent<Building>() != null)
            {
                other.gameObject.GetComponent<Building>().Boost();
                //Debug.Log("Boosting Turret!");
            }
        }
    }
}
