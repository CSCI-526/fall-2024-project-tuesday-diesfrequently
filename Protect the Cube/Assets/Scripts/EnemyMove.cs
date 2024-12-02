using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyMove : MonoBehaviour
{
    // Start is called before the first frame update
    protected GameObject _target;
    protected Rigidbody _rb;

    protected List<GameObject> targetList = new List<GameObject>();

    public float moveSpeed = 0.1f;

    protected float slowDebufTimer = 0;
    protected float slowAmount = 0;
    [SerializeField] protected int damage = 1;
    [SerializeField] protected bool targetPlayerOnly = false;
    [SerializeField] protected bool targetBuildingsOnly = false;

    private bool isEnemyMovementLocked = false; // controls enemy "movement" lock

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        UpdateTargetList();
    }

    protected void UpdateTargetList()
    {
        targetList.Clear();
        if (!targetPlayerOnly) targetList.Add(GameObject.FindWithTag("Nexus"));
        if (!targetBuildingsOnly) targetList.Add(GameObject.FindWithTag("Player"));
    }

    public void OnlyTargetPlayer()
    {
        targetPlayerOnly = true;
        targetBuildingsOnly = false;
        UpdateTargetList();
    }

    public void OnlyTargetNexus()
    {
        targetPlayerOnly = false;
        targetBuildingsOnly = true;
        UpdateTargetList();
    }

    public void TargetEverything()
    {
        targetPlayerOnly = false;
        targetBuildingsOnly = false;
        UpdateTargetList();
    }

    // Update is called once per frame
    void Update()
    {   
        SetTarget(targetList);
        slowDebufTimer -= Time.deltaTime;
        if (slowDebufTimer <= 0){ slowAmount = 0; } // no longer in slow!
        if (transform.position.y < -5) Destroy(gameObject); // Edge Case: enemy falls off map
    }

    void FixedUpdate()
    {
        if (!_target)
        {
            SetTarget(targetList);
        }
        if (_target.IsDestroyed() || !_target.activeSelf)
        {
            targetList.Remove(_target);
            SetTarget(targetList);
        }

        // does not move if enemy movement is locked
        if (!isEnemyMovementLocked)
        {
            Vector3 dirToTarget = _target.transform.position - _rb.transform.position;
            dirToTarget.y = 0.0f;
            dirToTarget.Normalize();

            _rb.MovePosition(transform.position + dirToTarget * (moveSpeed * (1 - slowAmount)) * Time.fixedDeltaTime);
            transform.rotation = UnityEngine.Quaternion.LookRotation(dirToTarget, Vector3.up);
        }
    }

    protected void SetTarget(List<GameObject> targetList)
    {   
        float closest = Int32.MaxValue; //add your max range here
        GameObject closestObject = null;
        for (int i = 0; i < targetList.Count(); i++)  //list of gameObjects to search through
        {
            if (targetList[i] && !targetList[i].IsDestroyed())
            {
                float dist = Vector3.Distance(targetList[i].transform.position, transform.position);
                if (dist < closest)
                {
                    closest = dist;
                    closestObject = targetList[i];
                }
            }
        }
        // Debug.Log("closest: "+closestObject.name);
        _target = closestObject; 
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.gameObject.CompareTag("Nexus"))
        {
            collision.transform.gameObject.GetComponent<Nexus>().TakeDamage(damage);
            GetComponent<EnemyHealth>().Die();
        }
        else if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage();
        }
    }
    public void GetSlowed(float slowRate){

        slowAmount = Mathf.Max(slowAmount, slowRate);
        slowDebufTimer = 0.1f;
    }

    // lock the enemy movement
    public void LockMovement() {
        //Debug.Log("[EnemyMove] isEnemyMovementLocked = true");
        isEnemyMovementLocked = true; }

    // unlock the enemy movement
    public void UnlockMovement() {
        //Debug.Log("[EnemyMove] isEnemyMovementLocked = false");
        isEnemyMovementLocked = false; }
}
