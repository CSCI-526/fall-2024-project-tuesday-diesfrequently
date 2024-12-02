using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : Bullet
{
    // Start is called before the first frame update
   
    protected override void OnTriggerEnter(Collider other){
        if(other.tag == "Enemy"){
            HitEnemy(other);
            KillBullet();
        }
        else if(other.CompareTag("Ore")){
            HitOre(other);
            KillBullet();
        }
        
    }

}
