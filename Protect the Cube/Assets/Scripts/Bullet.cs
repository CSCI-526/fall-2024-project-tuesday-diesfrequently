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
            BulletPool.Instance.ReturnBullet(gameObject);
            //Destroy(gameObject);
        }

    }

    void OnTriggerEnter (Collider other) {
        if (other.CompareTag("Enemy")){
            other.GetComponent<EnemyHealth>().TakeDamage(damage);
            BulletPool.Instance.ReturnBullet(gameObject);
             //Destroy(gameObject);   
        }

        if (other.CompareTag("Wall"))
        {
            BulletPool.Instance.ReturnBullet(gameObject);
            //Destroy(gameObject); // when bullet hits wall, destroy bullet
        }

    }

    public void ResetBullet()
    {
        lifetime = 0.0f; // Reset lifetime whenever bullet is reused
    }

}
