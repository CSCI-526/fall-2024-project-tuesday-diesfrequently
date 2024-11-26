using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBullet : Bullet
{

    [SerializeField] private int maxBounces = 3;
    [SerializeField]private int maxBounceRange = 5;
    private int bounces = 0;

    private GameObject FindClosestEnemy(GameObject currentEnemy)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            if (enemy != currentEnemy && enemy.GetComponent<EnemyHealth>() != null)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < minDistance && distance < maxBounceRange)
                {
                    minDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }

        return closestEnemy;
    }


    protected override void HitEnemy(Collider other)
    {
        //Physics.IgnoreCollision(other, GetComponent<Collider>());
        other.GetComponent<EnemyHealth>().TakeDamage(damage);
        if (bounces < maxBounces)
        {
            GameObject closestEnemy = FindClosestEnemy(other.gameObject);
            if (closestEnemy != null)
            {
                Debug.Log("Matched closest enemy: " + closestEnemy.name);

                Vector3 directionToEnemy = (closestEnemy.transform.position - transform.position).normalized;
                transform.right = directionToEnemy;
                bounces++;
            }
            else
            {
                KillBullet();
            }
        }
        else{
            KillBullet();
        }
    }

    //protected override void HitOre(Collider other)
    //{
    //    other.GetComponent<Ore>().TakeDamage(damage);
    //}

    protected virtual void OnTriggerEnter (Collider other) {
        if (other.CompareTag("Enemy")){
            HitEnemy(other);
        }
        //else if(other.CompareTag("Ore"))
        //{
            
        //    HitOre(other);
        //}


    }
}
