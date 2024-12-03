using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class turretShoot : Building
{   
    private float timeSinceLastShot = 0.0f;
    [SerializeField] public float fireRate = 5.0f;
    public float maxRange = 50.0f;
    [SerializeField] float turnSpeed = 15.0f;
    [SerializeField] float upgradeExponentPerLevel = 2;
    [SerializeField] float boostMultiplier = 1.3f;

    [SerializeField] GameObject projectile;
    [SerializeField] GameObject gunBarrel;
    protected GameObject target;

    private float originalFireRate;
    private float originalMaxRange;
    private float originalTurnSpeed;
    public bool canUpgrade = false;
    public float ugpradeCooldown = 0.0f;
    private bool canShoot;

    void Awake()
    {
        canShoot = true;
        originalFireRate = fireRate;
        originalMaxRange = maxRange;
        originalTurnSpeed = turnSpeed;
    }

    public void DisableShooting() { canShoot = false; }
    public void EnableShooting() { canShoot = true; }

    // Update is called once per frame
    void Update()
    {
        if(placed && canShoot)
        {
            FindTarget();
            Aim();
            Shoot();
            ugpradeCooldown += Time.deltaTime;
        }
    }

    public override void OnPlace()
    {
        base.OnPlace();
        CheckForBoost();

        RangeIndicator[] indicators = GetComponents<RangeIndicator>();
        foreach (RangeIndicator i in indicators)
        {
            i.HideIndicator();
        }
    }

    void FixedUpdate()
    {
        timeSinceLastShot += Time.fixedDeltaTime;
    }

    private void Shoot()
    {
        if (target && (timeSinceLastShot > 1 / fireRate) && projectile && gunBarrel)
        {
            Vector3 toTarget = (target.transform.position - transform.position);
            float dist = toTarget.magnitude;
            toTarget.Normalize();
            if (Vector3.Dot(toTarget, transform.forward) > 0.9 || dist < 3.0f)
            {
                if(GameManager.Instance.useBulletPool)
                {
                    var bullet = BulletPool.Instance.GetBullet();

                    if (bullet == null)
                    {
                        Debug.Log("All Bullets are Currently Being Used");
                        return; // return early to indicate "stop shooting"
                    }
                    bullet.transform.position = gunBarrel.transform.position;
                    bullet.transform.rotation = gunBarrel.transform.rotation;
                    timeSinceLastShot = 0;
                }
                else
                {
                    var bullet = Instantiate(projectile, gunBarrel.transform.position, gunBarrel.transform.rotation);
                    bullet.transform.position = gunBarrel.transform.position;
                    bullet.transform.rotation = gunBarrel.transform.rotation;
                    timeSinceLastShot = 0;
                }
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
        foreach (GameObject enemy in GameManager.Instance.WaveManager.AllEnemyEntities)
        {
            if(enemy != null && !enemy.IsDestroyed())
            {
                float dist = (enemy.transform.position - transform.position).magnitude;
                if (dist < minRange)
                {
                    target = enemy;
                    minRange = dist;
                }
            }
        }
    }

    override public void Boost()
    {
        turnSpeed *= boostMultiplier;
        fireRate *= boostMultiplier;
        maxRange *= boostMultiplier;
        originalFireRate *= boostMultiplier;
        originalMaxRange *= boostMultiplier;
        originalTurnSpeed *= boostMultiplier;  

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

    internal void upgrade(int level, string buildingName)
    {
        Debug.Log("[Analytics][Prio] Level: " + level + " and buildingName: " + buildingName);
        int multiplier = (int)Math.Pow(level, upgradeExponentPerLevel);
        maxRange += originalMaxRange/5;
        turnSpeed += originalTurnSpeed/2 * multiplier;
        fireRate += originalFireRate/4 * multiplier;
        Debug.Log($"[Upgrade] Multiplier: {multiplier}");
        Debug.Log($"[Upgrade] Before - Max Range: {originalMaxRange}, Turn Speed: {originalTurnSpeed}, Fire Rate: {originalFireRate}");
        Debug.Log($"[Upgrade] After - Max Range: {maxRange}, Turn Speed: {turnSpeed}, Fire Rate: {fireRate}");
        if (buildingName == "Flamethrower Turret"){
            GameManager.Instance.AnalyticsManager.UpdateTurretLevels(level, 3);
            
        }else if (buildingName == "Gatling Turret"){
            GameManager.Instance.AnalyticsManager.UpdateTurretLevels(level, 2);
           
        }else if (buildingName == "Gun Turret"){
            GameManager.Instance.AnalyticsManager.UpdateTurretLevels(level, 1);
          
        }else if (buildingName == "Sniper Turret"){
            GameManager.Instance.AnalyticsManager.UpdateTurretLevels(level, 4);
        }
        RangeIndicator[] indicators = GetComponents<RangeIndicator>();
        foreach (RangeIndicator i in indicators)
        {
            i.HideIndicator();
        }
    }
}
