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

    void OnTriggerEnter (Collider other) {
        if (other.CompareTag("Enemy")){
            other.GetComponent<EnemyHealth>().TakeDamage(damage);
            if (GameManager.Instance.useBulletPool)
            {
                BulletPool.Instance.ReturnBullet(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else if(other.CompareTag("Ore"))
        {
            bool damageTaken = false;

            Tier1Ore ore_t1 = other.GetComponent<Tier1Ore>();
            if (ore_t1 != null) { ore_t1.TakeDamage(damage); damageTaken = true; }

            if (!damageTaken) {
                Tier2Ore ore_t2 = other.GetComponent<Tier2Ore>();
                if (ore_t2 != null) { ore_t2.TakeDamage(damage); damageTaken = true; }
            }

            if (!damageTaken)
            {
                Tier3Ore ore_t3 = other.GetComponent<Tier3Ore>();
                if (ore_t3 != null) { ore_t3.TakeDamage(damage); damageTaken = true; }
            }

            if (GameManager.Instance.useBulletPool) BulletPool.Instance.ReturnBullet(gameObject);
            else Destroy(gameObject);

        }

        if (other.CompareTag("Wall"))
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
    }

    public void ResetBullet()
    {
        lifetime = 0.0f; // Reset lifetime whenever bullet is reused
    }

}
