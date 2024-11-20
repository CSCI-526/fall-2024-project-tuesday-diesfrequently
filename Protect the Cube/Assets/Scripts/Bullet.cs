using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] public float maxLifetime;
    [SerializeField] public float damage;

    float lifetime = 0.0f;

    private void Start()
    {
        lifetime = 0.0f;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += transform.right * speed * Time.fixedDeltaTime;

        lifetime += Time.fixedDeltaTime;
        if (lifetime > maxLifetime)
        {
            //lifetime = 0.0f;
            if(GameManager.Instance.useBulletPool)
            {
                BulletPool.Instance.ReturnBullet(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

    }
    public void KillBullet()
    {
        if (GameManager.Instance.useBulletPool)
        {
            BulletPool.Instance.ReturnBullet(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    protected virtual void  HitEnemy(Collider other) 
    {
        other.GetComponent<EnemyHealth>().TakeDamage(damage);
        KillBullet();
    }

    protected virtual void HitOre(Collider other) 
    {
        other.GetComponent<Ore>().TakeDamage(damage);
        KillBullet();
    }

    protected virtual void OnTriggerEnter (Collider other) {
        if (other.CompareTag("Enemy")){
            HitEnemy(other);
        }
        else if(other.CompareTag("Ore"))
        {
            
            HitOre(other);
        }


    }

    public void ResetBullet()
    {
        lifetime = 0.0f; // Reset lifetime whenever bullet is reused
    }

}
