using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperBullet : Bullet
{
    [SerializeField] public int piercing = 2;
    protected override void HitEnemy(Collider other)
    {
        other.GetComponent<EnemyHealth>().TakeDamage(damage);
        --piercing;
        if (piercing <= 0)
        {
            KillBullet();
        }
    }

    protected override void HitWall(Collider other)
    {
        --piercing;
        if (piercing <= 0)
        {
            KillBullet();
        }
    }

    //protected override void HitOre(Collider other)
    //{
    //    other.GetComponent<Ore>().TakeDamage(damage);
    //}

}
