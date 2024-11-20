using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperBullet : Bullet
{
    
    protected override void HitEnemy(Collider other)
    {
        other.GetComponent<EnemyHealth>().TakeDamage(damage);
    }

    protected override void HitOre(Collider other)
    {
        other.GetComponent<Ore>().TakeDamage(damage);
    }
    
}
