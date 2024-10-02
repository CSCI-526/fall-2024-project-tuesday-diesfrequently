using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class turretShoot : Building
{   
    private float timeSinceLastShot = 0.0f;
    [SerializeField] float fireRate = 5.0f;
    [SerializeField] float maxRange = 50.0f;
    [SerializeField] float turnSpeed = 15.0f;

    [SerializeField] GameObject projectile;
    [SerializeField] GameObject gunBarrel;
    [SerializeField] GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(placed)
        {
            FindTarget();
            Aim();
            Shoot();
        }
    }
    public override void OnPlace()
    {
        placed = true;
        GetComponent<Collider>().enabled = true;
        CheckForBoost();
    }

    void FixedUpdate()
    {
        timeSinceLastShot += Time.fixedDeltaTime;
    }

    private void Shoot()
    {
        if (target && (timeSinceLastShot > 1 / fireRate) && projectile && gunBarrel)
        {
            Vector3 toTarget = (target.transform.position - transform.position).normalized;
            if(Vector3.Dot(toTarget,transform.forward) > 0.9)
            {
                var bullet = Instantiate(projectile, gunBarrel.transform.position, gunBarrel.transform.rotation);
                timeSinceLastShot = 0;
            }
        }
    }

    private void Aim()
    {
        if(target != null)
        {
            Vector3 toTarget = (target.transform.position - transform.position).normalized;
            toTarget.y = 0.0f;
           
            Debug.DrawRay(transform.position, toTarget, Color.red);
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, toTarget, turnSpeed * Time.deltaTime, 0.0f);

            Debug.DrawRay(transform.position, newDirection, Color.red);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    private void FindTarget()
    {
        target = null;
        float minRange = maxRange;
        foreach (GameObject enemy in GameManager.Instance.WaveManager.enemies)
        {
            float dist = (enemy.transform.position - transform.position).magnitude;
            if (dist < minRange)
            {
                target = enemy;
                minRange = dist;
            }
        }
    }

    override public void Boost()
    {
        ++turnSpeed;
        ++fireRate;
        ++maxRange;
    }
    void CheckForBoost(float radius = 3.0f)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);// -1, QueryTriggerInteraction.Collide);
        foreach (var other in hitColliders)
        {
            if (other.gameObject.GetComponent<TurretBooster>() != null)
            {
                Boost();
                //Debug.Log("Boosted by other Turret!");
            }
        }
    }

}
